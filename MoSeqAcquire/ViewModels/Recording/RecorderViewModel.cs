using MoSeqAcquire.Models.Attributes;
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
        protected Type recorderType;
        protected ObservableCollection<SelectableChannelViewModel> channels;
        protected RecorderSettings settings;

        public RecorderViewModel(MoSeqAcquireViewModel RootViewModel, Type RecorderType)
        {
            this.rootViewModel = RootViewModel;
            this.loadChannels();
            this.RecorderType = RecorderType;

            this.PrepareSettings();
        }
        public RecorderViewModel(MoSeqAcquireViewModel RootViewModel, ProtocolRecorder Recorder) : this(RootViewModel, Recorder.GetProviderType())
        {
            this.Name = Recorder.Name;
            this.Settings = Recorder.Config;
            foreach(var c in this.AvailableChannels)
            {
                if (Recorder.Channels.Contains(c.Channel.Name))
                {
                    this.SelectedChannels.Add(c);
                    //c.IsSelected = true;
                }
            }
        }
        protected void PrepareSettings()
        {
            var sit = this.recorderType.GetCustomAttribute<SettingsImplementationAttribute>();
            this.settings = Activator.CreateInstance(sit.SettingsImplementation) as RecorderSettings;
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
        public string DisplayName
        {
            get => this.name + " (" + this.SelectedChannels.Count + " Channels)";
        }
        
        public string Name
        {
            get => this.name;
            set => this.SetField(ref this.name, value);
        }
        public Type RecorderType
        {
            get => this.recorderType;
            protected set => this.SetField(ref this.recorderType, value);
        }

        public RecorderSettings Settings
        {
            get => this.settings;
            set => this.SetField(ref this.settings, value);
        }
        public ReadOnlyObservableCollection<SelectableChannelViewModel> AvailableChannels { get; protected set; }
        public ObservableCollection<SelectableChannelViewModel> SelectedChannels { get; protected set; }
        public MediaWriterStats Stats { get; protected set; }

        public IMediaWriter MakeMediaWriter()
        {
            var writer = (MediaWriter)Activator.CreateInstance(this.recorderType);
            writer.Name = this.Name;
            writer.Settings = this.settings;
            foreach(var c in this.SelectedChannels)
            {
                writer.ConnectChannel(c.Channel.Channel);
            }
            this.Stats = writer.Stats;
            this.NotifyPropertyChanged("Stats");
            return writer;
        }
        public ProtocolRecorder GetRecorderDefinition()
        {
            return new ProtocolRecorder()
            {
                Name = this.Name,
                Provider = this.RecorderType.FullName,
                Config = this.Settings,
                Channels = this.SelectedChannels.Select(c => c.Channel.Name).ToList()
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
