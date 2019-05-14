using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using MoSeqAcquire.Models.Acquisition;
using MoSeqAcquire.Models.Recording;
using MoSeqAcquire.ViewModels.MediaSources;
using MvvmValidation;

namespace MoSeqAcquire.ViewModels.Recording
{
    public abstract class RecorderPinViewModel : BaseViewModel, INotifyDataErrorInfo
    {
        protected MediaWriterPin pin;
        protected ObservableCollection<ChannelViewModel> selectedChannels;

        public RecorderPinViewModel(MediaWriterPin Pin, AvailableRecordingChannelsProvider AvailableChannelsProvider)
        {
            this.pin = Pin;
            this.selectedChannels = new ObservableCollection<ChannelViewModel>();
            this.SelectedChannels = new ReadOnlyObservableCollection<ChannelViewModel>(this.selectedChannels);
            this.selectedChannels.CollectionChanged += SelectedChannels_CollectionChanged;
            this.AvailableChannels = new CollectionViewSource { Source = AvailableChannelsProvider }.View;
            this.AvailableChannels.Filter = this.FilterAvailableChannels;
            AvailableChannelsProvider.CollectionChanged += (s, e) => { this.AvailableChannels.Refresh(); };
            this.RegisterRules();
        }

        private void SelectedChannels_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            e.NewItems?.Cast<ChannelViewModel>().ForEach(cvm => cvm.PropertyChanged += SelectedChannel_PropertyChanged);
            e.OldItems?.Cast<ChannelViewModel>().ForEach(cvm => cvm.PropertyChanged -= SelectedChannel_PropertyChanged);
            this.NotifyPropertyChanged();
        }

        private void SelectedChannel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.NotifyPropertyChanged();
        }

        protected void RegisterRules()
        {
            NotifyDataErrorInfoAdapter = new NotifyDataErrorInfoAdapter(this.Validator);
            this.PropertyChanged += (s, e) => { Validator.ValidateAll(); };
            Validator.AddRule(() =>
            {
                foreach (var sc in this.SelectedChannels) {
                    if (!sc.Enabled)
                        return RuleResult.Invalid($"Channel {sc.FullName} is not enabled.");
                }
                return RuleResult.Valid();
            });
        }
        public MediaType MediaType { get => this.pin.MediaType; }
        public string PinName { get => this.pin.Name; }
        public ICollectionView AvailableChannels { get; protected set; }

        public ReadOnlyObservableCollection<ChannelViewModel> SelectedChannels
        {
            get;
            protected set;
        }
        protected virtual bool FilterAvailableChannels(object potentiallyAvailableChannel)
        {
            if (this.MediaType == MediaType.Any)
            {
                return true;
            }
            var scvm = potentiallyAvailableChannel as SelectableChannelViewModel;
            return scvm.Channel.Channel.MediaType == this.MediaType;
        }

        private NotifyDataErrorInfoAdapter NotifyDataErrorInfoAdapter { get; set; }
        public IEnumerable GetErrors(string propertyName)
        {
            if (propertyName == null)
            {
                ValidationResult result = this.Validator.GetResult();
                if (result.IsValid)
                    return (IEnumerable)Enumerable.Empty<string>();
                return (IEnumerable)new string[1]
                {
                    result.ToString()
                };
            }
            return NotifyDataErrorInfoAdapter.GetErrors(propertyName);
        }

        public bool HasErrors
        {
            get { return NotifyDataErrorInfoAdapter.HasErrors; }
        }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged
        {
            add { NotifyDataErrorInfoAdapter.ErrorsChanged += value; }
            remove { NotifyDataErrorInfoAdapter.ErrorsChanged -= value; }
        }

        public static RecorderPinViewModel Factory(MediaWriterPin Pin, AvailableRecordingChannelsProvider AvailableChannels)
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

        public SingleCapacityRecorderPin(MediaWriterPin Pin, AvailableRecordingChannelsProvider AvailableChannels) : base(Pin, AvailableChannels)
        {
            this.SelectedChannel = AvailableChannels.Where(scvm => scvm.Channel.Channel.Equals(pin.Channel))
                                                    .Select(scvm => scvm.Channel)
                                                    .FirstOrDefault();
        }

        public ChannelViewModel SelectedChannel
        {
            get => this.selectedChannel;
            set
            {
                this.SetField(ref this.selectedChannel, value, () => { this.pin.Channel = value?.Channel; });
                this.selectedChannels.Clear();
                if (this.selectedChannel != null)
                {
                    this.selectedChannels.Add(this.selectedChannel);
                }
            }
        }
    }
    public class MultipleCapacityRecorderPin : RecorderPinViewModel
    {
        public MultipleCapacityRecorderPin(MediaWriterPin Pin, AvailableRecordingChannelsProvider AvailableChannels) : base(Pin, AvailableChannels)
        {
            AvailableChannels.ForEach(scvm => scvm.PropertyChanged += Scvm_PropertyChanged);
        }

        private void Scvm_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if ("IsSelected".Equals(e.PropertyName))
            {
                var scvm = sender as SelectableChannelViewModel;
                if (scvm.IsSelected)
                {
                    this.selectedChannels.Add(scvm.Channel);
                }
                else
                {
                    this.selectedChannels.Remove(scvm.Channel);
                }
            }
        }
    }
}
