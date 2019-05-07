using MoSeqAcquire.Models.Acquisition;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using MoSeqAcquire.ViewModels.MediaSources;

namespace MoSeqAcquire.ViewModels.Performance
{
    public class PerformanceMonitorPreviewAdapter : PerformanceMonitorAdapter
    {
        public PerformanceMonitorPreviewAdapter(PerformanceMonitorViewModel Owner) : base(Owner)
        {
            Owner.Root.MediaSources.Items.CollectionChanged += MediaSources_CollectionChanged;
            Owner.Root.MediaSources.Items
                 .SelectMany(msvm => msvm.Channels)
                 .Select(cvm => this.ItemFactory(cvm))
                 .ForEach(p => this.Owner.Performances.Add(p));
        }

        private void MediaSources_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach(var item in e.NewItems)
                {
                    (item as MediaSourceViewModel).InitTask.ContinueWith((initTask) =>
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            (item as MediaSourceViewModel).Channels
                            .Select(cvm => this.ItemFactory(cvm))
                            .ForEach(p => this.Owner.Performances.Add(p));
                        });
                    });
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var item in e.OldItems)
                {
                    (item as MediaSourceViewModel).Channels
                        .SelectMany(c => this.Owner.Performances.Where(p => p.Performance == c.Performance))
                        .ToList()
                        .ForEach(p => this.Owner.Performances.Remove(p));
                }
            }
        }

        private PerformanceItemViewModel ItemFactory(ChannelViewModel channel)
        {
            return new PerformanceItemViewModel()
            {
                Category = "Preview",
                Name = channel.FullName,
                Performance = channel.Performance
            };
        }
    }
}
