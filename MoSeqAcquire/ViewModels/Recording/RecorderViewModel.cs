using MoSeqAcquire.Models.Attributes;
using MoSeqAcquire.Models.Recording;
using MoSeqAcquire.Models.Management;
using MoSeqAcquire.ViewModels.PropertyManagement;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MoSeqAcquire.ViewModels.Recording
{
    public class RecorderViewModel : BaseViewModel
    {
        protected MoSeqAcquireViewModel rootViewModel;
        protected string name;
        protected Type recorderType;
        protected MediaWriter writer; //not always set!!!
        //protected ObservableCollection<SelectableChannelViewModel> channels;
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
                    //this.SelectedChannels.Add(c);
                    c.IsSelected = true;
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
            this.AvailableChannels = new ObservableCollection<SelectableChannelViewModel>(channels);

            this.SelectedChannels = new CollectionView(this.AvailableChannels);
            //this.SelectedChannels =(CollectionView)CollectionViewSource.GetDefaultView(this.AvailableChannels);
            this.SelectedChannels.Filter = (e) => (e as SelectableChannelViewModel).IsSelected;

            //this.SelectedChannels.CollectionChanged += (s, e) => { this.NotifyPropertyChanged("DisplayName"); };
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
        public PropertyCollection SettingsProps
        {
            get => new PropertyCollection(this.settings);
        }
        //public ReadOnlyObservableCollection<SelectableChannelViewModel> AvailableChannels { get; protected set; }
        public ObservableCollection<SelectableChannelViewModel> AvailableChannels
        {
            get;
            protected set;
        }
        public CollectionView SelectedChannels { get; protected set; }
        public MediaWriterStats Stats { get; protected set; }

        public IEnumerable<RecorderProduct> Products
        {
            get
            {
                var products = new List<RecorderProduct>();
                //if(this.writer != null)
                //{
                    products.AddRange(
                        this.MakeMediaWriter()
                            .GetChannelFileMap()
                            .Select(kvp => new RecorderProduct()
                            {
                                Name = kvp.Key,
                                Channels = this.SelectedChannels.OfType<SelectableChannelViewModel>().Select(scvm => scvm.Channel)
                            })
                    );
                //}
                return products;
            }
        }

        public IMediaWriter MakeMediaWriter()
        {
            this.writer = (MediaWriter)Activator.CreateInstance(this.recorderType);
            this.writer.Name = this.Name;
            this.writer.Settings = this.settings;
            foreach(var c in this.SelectedChannels)
            {
                this.writer.ConnectChannel((c as SelectableChannelViewModel).Channel.Channel);
            }
            this.Stats = this.writer.Stats;
            this.NotifyPropertyChanged("Stats");
            return this.writer;
        }
        public ProtocolRecorder GetRecorderDefinition()
        {
            return new ProtocolRecorder()
            {
                Name = this.Name,
                Provider = this.RecorderType.FullName,
                Config = this.Settings,
                Channels = this.AvailableChannels.Where(c => c.IsSelected).Select(c => c.Channel.Name).ToList()
            };
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
