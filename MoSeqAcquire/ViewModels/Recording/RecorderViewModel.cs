using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using MoSeqAcquire.Models.Acquisition;
using MoSeqAcquire.Models.Management;
using MoSeqAcquire.Models.Recording;
using MoSeqAcquire.ViewModels.PropertyManagement;

namespace MoSeqAcquire.ViewModels.Recording
{
    public class RecorderViewModel : BaseViewModel
    {
        protected MoSeqAcquireViewModel rootViewModel;

        protected MediaWriter writer;
        protected PropertyCollection settings;

        protected ObservableCollection<SelectableChannelViewModel> availableChannels;
        protected ObservableCollection<ChannelViewModel> selectedChannels;
        
        public RecorderViewModel(MoSeqAcquireViewModel RootViewModel, Type RecorderType)
        {
            this.rootViewModel = RootViewModel;
            var spec = new RecorderSpecification(RecorderType);
            this.writer = spec.Factory();
            this.Initialize();
        }
        public RecorderViewModel(MoSeqAcquireViewModel RootViewModel, ProtocolRecorder Recorder)
        {
            this.rootViewModel = RootViewModel;
            this.writer = MediaWriter.FromProtocolRecorder(Recorder);
            this.Initialize();
        }
        protected void Initialize()
        {
            this.loadChannels();
            this.settings = new PropertyCollection(this.writer.Settings);
            var pins = new List<RecorderPinViewModel>();
            foreach (var wp in this.writer.Pins.Values)
            {
                RecorderPinViewModel pin = RecorderPinViewModel.Factory(wp, AvailableChannels);
                if (pin != null)
                {
                    pin.PropertyChanged += this.UpdateSelectedChannels;
                    pins.Add(pin);
                }
            }
            this.RecorderPins = pins;
        }
        
        protected void loadChannels()
        {
            var channels = this.rootViewModel.MediaSources.SelectMany(s => s.Channels.Select(c => new SelectableChannelViewModel(c)));
            this.availableChannels = new ObservableCollection<SelectableChannelViewModel>(channels);
            this.selectedChannels = new ObservableCollection<ChannelViewModel>();

            this.SelectedChannels.CollectionChanged += (s, e) => {
                this.NotifyPropertyChanged("Products");
            };
        }


        private void UpdateSelectedChannels(object sender, PropertyChangedEventArgs e)
        {
            this.SelectedChannels.Clear();
            this.RecorderPins
                .SelectMany(rp => rp.SelectedChannels)
                .Where((cvm) => { return cvm != null; })
                .ForEach(cvm => this.SelectedChannels.Add(cvm));
        }

        
        
        
        public string Name
        {
            get => this.writer.Name;
            set
            {
                this.writer.Name = value;
                this.NotifyPropertyChanged("Name");
            }
        }
        public MediaWriter Writer { get => this.writer; }
        public MoSeqAcquireViewModel Root { get => this.rootViewModel; }
        public RecorderSpecification Specification { get => this.writer.Specification; }
        public IEnumerable<RecorderPinViewModel> RecorderPins { get; protected set; }
        public PropertyCollection Settings { get => this.settings; }
        public ObservableCollection<SelectableChannelViewModel> AvailableChannels { get => this.availableChannels; }
        public ObservableCollection<ChannelViewModel> SelectedChannels { get => this.selectedChannels; }
        public MediaWriterStats Stats { get; protected set; }

        public IEnumerable<RecorderProduct> Products
        {
            get
            {
                return this.writer
                           .GetChannelFileMap()
                           .Select(kvp => new RecorderProduct()
                           {
                               Name = kvp.Key,
                               Channels = this.SelectedChannels
                           });
            }
        }
        public ProtocolRecorder GetRecorderDefinition()
        {
            return this.writer.GetProtocolRecorder();
        }
    }

    public abstract class RecorderPinViewModel : BaseViewModel
    {
        protected MediaWriterPin pin;

        public RecorderPinViewModel(MediaWriterPin Pin, ObservableCollection<SelectableChannelViewModel> AvailableChannels) {
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
            if(this.MediaType == MediaType.Any)
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

    public class RecorderProduct
    {
        public string Name { get; set; }
        public IEnumerable<ChannelViewModel> Channels { get; set; }
    }

    public class SelectableChannelViewModel : BaseViewModel
    {
        protected bool isSelected;
        protected bool isEnabled;
        public SelectableChannelViewModel(ChannelViewModel Channel, bool IsSelected = false)
        {
            this.Channel = Channel;
            this.IsSelected = IsSelected;
        }
        public ChannelViewModel Channel { get; protected set; }
        public bool IsSelected
        {
            get => this.isSelected;
            set => this.SetField(ref isSelected, value);
        }
        public bool IsEnabled
        {
            get => this.isEnabled;
            set => this.SetField(ref isEnabled, value);
        }
        public override bool Equals(Object obj)
        {
            // Check for null values and compare run-time types.
            if (obj == null || GetType() != obj.GetType())
                return false;

            SelectableChannelViewModel other = (SelectableChannelViewModel)obj;
            return this.Channel.Equals(other.Channel);
        }
        public override int GetHashCode()
        {
            return this.Channel.GetHashCode();
        }
    }
}
