using MoSeqAcquire.Models.IO;
using MoSeqAcquire.Models.Management;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.ViewModels.Recording
{
    public class RecorderViewModel : BaseViewModel
    {
        protected MoSeqAcquireViewModel rootViewModel;
        protected string name;
        protected string recorderType;
        protected ObservableCollection<SelectableChannelViewModel> channels;
        protected RecorderSettingsViewModel settings;

        public RecorderViewModel(MoSeqAcquireViewModel RootViewModel, string RecorderType, BaseRecordingSettingsViewModel settings)
        {
            this.rootViewModel = RootViewModel;
            this.loadChannels();
            this.RecorderType = RecorderType;
            this.settings = new RecorderSettingsViewModel(settings);
        }
        protected void loadChannels()
        {
            var channels = this.rootViewModel.MediaSources.SelectMany(s => s.Channels.Select(c => new SelectableChannelViewModel(c)));
            this.channels = new ObservableCollection<SelectableChannelViewModel>(channels);
            this.AvailableChannels = new ReadOnlyObservableCollection<SelectableChannelViewModel>(this.channels);
            this.SelectedChannels = new ObservableCollection<SelectableChannelViewModel>();

            this.SelectedChannels.CollectionChanged += (s, e) => { this.NotifyPropertyChanged("DisplayName"); };
            this.PropertyChanged += (s, e) => { if (e.PropertyName == "Name") { this.NotifyPropertyChanged("DisplayName"); } };
        }
        public MoSeqAcquireViewModel Root { get => this.rootViewModel; }
        public string DisplayName { get => this.name + " (" + this.SelectedChannels.Count + " Channels)"; }
        
        public string Name
        {
            get => this.name;
            set => this.SetField(ref this.name, value);
        }
        public string RecorderType
        {
            get => this.recorderType;
            protected set => this.SetField(ref this.recorderType, value);
        }

        public RecorderSettingsViewModel Settings { get => this.settings; }
        public ReadOnlyObservableCollection<SelectableChannelViewModel> AvailableChannels { get; protected set; }
        public ObservableCollection<SelectableChannelViewModel> SelectedChannels { get; protected set; }


        public IMediaWriter MakeMediaWriter()
        {
            var writer = (IMediaWriter)Activator.CreateInstance(Type.GetType(this.recorderType));
            writer.ApplySettings((RecorderSettings)this.settings.GetSnapshot());
            foreach(var c in this.SelectedChannels)
            {
                writer.ConnectChannel(c.Channel.Channel);
            }
            return writer;
        }
        public ProtocolRecorder GetRecorderDefinition()
        {
            return new ProtocolRecorder()
            {
                Name = this.Name,
                Provider = this.RecorderType,
                Config = this.Settings.GetSnapshot()
            };
        }
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
    }
}
