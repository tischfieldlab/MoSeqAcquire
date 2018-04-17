using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using MoSeqAcquire.Models.Acquisition;
using MoSeqAcquire.Models.Recording;

namespace MoSeqAcquire.ViewModels.Recording
{
    public abstract class RecorderPinViewModel : BaseViewModel
    {
        protected MediaWriterPin pin;

        public RecorderPinViewModel(MediaWriterPin Pin, ObservableCollection<SelectableChannelViewModel> AvailableChannels)
        {
            this.pin = Pin;
            this.AvailableChannels = new CollectionViewSource { Source = AvailableChannels }.View;
            this.AvailableChannels.Filter = this.FilterAvailableChannels;
        }
        public MediaType MediaType { get => this.pin.MediaType; }
        public string PinName { get => this.pin.Name; }
        public ICollectionView AvailableChannels { get; protected set; }

        public abstract IEnumerable<ChannelViewModel> SelectedChannels { get; }
        protected virtual bool FilterAvailableChannels(object potentiallyAvailableChannel)
        {
            if (this.MediaType == MediaType.Any)
            {
                return true;
            }
            var scvm = potentiallyAvailableChannel as SelectableChannelViewModel;
            return scvm.Channel.Channel.MediaType == this.MediaType;
        }

        public static RecorderPinViewModel Factory(MediaWriterPin Pin, ObservableCollection<SelectableChannelViewModel> AvailableChannels)
        {
            RecorderPinViewModel pinvm = null;
            switch (Pin.Capacity)
            {
                case ChannelCapacity.Single:
                    pinvm = new SingleCapacityRecorderPin(Pin, AvailableChannels);
                    break;
                case ChannelCapacity.Multiple:
                    pinvm = new MultipleCapacityRecorderPin(Pin, AvailableChannels);
                    break;
            }
            return pinvm;
        }
    }
    public class SingleCapacityRecorderPin : RecorderPinViewModel
    {
        private ChannelViewModel selectedChannel;

        public SingleCapacityRecorderPin(MediaWriterPin Pin, ObservableCollection<SelectableChannelViewModel> AvailableChannels) : base(Pin, AvailableChannels)
        {
            this.SelectedChannel = AvailableChannels.Where(scvm => scvm.Channel.Channel.Equals(pin.Channel)).Select(scvm => scvm.Channel).FirstOrDefault();
        }

        public ChannelViewModel SelectedChannel
        {
            get => this.selectedChannel;
            set
            {
                this.SetField(ref this.selectedChannel, value, () => { this.pin.Channel = value.Channel; });
                this.NotifyPropertyChanged("SelectedChannels");
            }
        }
        public override IEnumerable<ChannelViewModel> SelectedChannels
        {
            get => new List<ChannelViewModel>() { this.SelectedChannel };
        }
    }
    public class MultipleCapacityRecorderPin : RecorderPinViewModel
    {
        public MultipleCapacityRecorderPin(MediaWriterPin Pin, ObservableCollection<SelectableChannelViewModel> AvailableChannels) : base(Pin, AvailableChannels)
        {
            AvailableChannels.ForEach(scvm => scvm.PropertyChanged += Scvm_PropertyChanged);
        }

        private void Scvm_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if ("IsSelected".Equals(e.PropertyName))
            {
                this.NotifyPropertyChanged("SelectedChannels");
            }
        }

        public override IEnumerable<ChannelViewModel> SelectedChannels
        {
            get
            {
                return this.AvailableChannels
                           .Cast<SelectableChannelViewModel>()
                           .Where(scvm => scvm.IsSelected)
                           .Select(scvm => scvm.Channel);
            }
        }
    }
}
