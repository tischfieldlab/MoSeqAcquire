using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using MoSeqAcquire.ViewModels.MediaSources;

namespace MoSeqAcquire.ViewModels.Recording
{
    public class AvailableRecordingChannelsProvider : ObservableCollection<SelectableChannelViewModel>
    {
        public AvailableRecordingChannelsProvider()
        {
            MoSeqAcquireViewModel.Instance.MediaSources.Items.CollectionChanged += Items_CollectionChanged;
            MoSeqAcquireViewModel.Instance
                .MediaSources
                .Items
                .SelectMany(s => s.Channels.Select(c => new SelectableChannelViewModel(c)))
                .ForEach(scvm => this.Add(scvm));

            this.View = CollectionViewSource.GetDefaultView(this);
            this.CollectionChanged += (s, e) => { this.View.Refresh(); };
        }

        public ICollectionView View
        {
            get; protected set;
        }

        private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            e.NewItems?.Cast<MediaSourceViewModel>().ForEach(msvm => msvm.Channels.CollectionChanged += Channels_CollectionChanged);
            e.OldItems?.Cast<MediaSourceViewModel>().ForEach(msvm => msvm.Channels.CollectionChanged -= Channels_CollectionChanged);
        }

        private void Channels_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            e.NewItems?.Cast<ChannelViewModel>().Select(cvm => new SelectableChannelViewModel(cvm)).ForEach(scvm => this.Add(scvm));

            if (e.OldItems != null)
            {
                var toRemove = this.Where(scvm => e.OldItems.Cast<ChannelViewModel>().Contains(scvm.Channel)).ToList();
                toRemove.ForEach(scvm => this.Remove(scvm));
            }
        }
    }
}
