using MoSeqAcquire.Models.IO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.ViewModels
{
    public class RecorderViewModel : BaseViewModel
    {
        protected MoSeqAcquireViewModel rootViewModel;
        protected string name;
        protected string recorderType;
        protected ObservableCollection<SelectableChannelViewModel> channels;

        public RecorderViewModel(MoSeqAcquireViewModel RootViewModel)
        {
            this.rootViewModel = RootViewModel;
            this.AvailableRecorderTypes = new ReadOnlyObservableCollection<string>(new ObservableCollection<string>(this.FindRecorderTypes()));
            this.loadChannels();
        }
        protected void loadChannels()
        {
            var channels = this.rootViewModel.MediaSources.SelectMany(s => s.Channels.Select(c => new SelectableChannelViewModel(c)));
            this.channels = new ObservableCollection<SelectableChannelViewModel>(channels);
            this.AvailableChannels = new ReadOnlyObservableCollection<SelectableChannelViewModel>(this.channels);
            this.SelectedChannels = new ObservableCollection<SelectableChannelViewModel>();

            this.SelectedChannels.CollectionChanged += (s, e) => { this.NotifyPropertyChanged("DisplayName"); };
        }
        public MoSeqAcquireViewModel Root { get => this.rootViewModel; }
        public string DisplayName
        {
            get
            {
                return this.name + " (" + this.SelectedChannels.Count + " Channels)";
            }
        }
        
        public string Name
        {
            get => this.name;
            set => this.SetField(ref this.name, value);
        }
        public string RecorderType
        {
            get => this.recorderType;
            set => this.SetField(ref this.recorderType, value);
        }
        public ReadOnlyObservableCollection<String> AvailableRecorderTypes { get; protected set; }

        public ReadOnlyObservableCollection<SelectableChannelViewModel> AvailableChannels { get; protected set; }
        public ObservableCollection<SelectableChannelViewModel> SelectedChannels { get; protected set; }


        protected IEnumerable<string> FindRecorderTypes()
        {
            return Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => !t.IsAbstract && typeof(MediaWriter).IsAssignableFrom(t))
                .Select(t => t.Name);
        }
        public MediaWriter MakeMediaWriter()
        {
            var writer = (MediaWriter)Activator.CreateInstance(Type.GetType(this.recorderType));

            return writer;
        }
    }

    public class SelectableChannelViewModel : BaseViewModel
    {
        protected bool isSelected;   
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
    }
}
