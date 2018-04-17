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
