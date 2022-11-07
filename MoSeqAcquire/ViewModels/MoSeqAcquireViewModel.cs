using MoSeqAcquire.Models.Management;
using MoSeqAcquire.Models.Recording;
using MoSeqAcquire.Models.Triggers;
using MoSeqAcquire.ViewModels.Commands;
using MoSeqAcquire.ViewModels.Recording;
using MoSeqAcquire.ViewModels.Triggers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using MoSeqAcquire.Properties;
using MoSeqAcquire.ViewModels.MediaSources;
using MoSeqAcquire.ViewModels.Metadata;
using Microsoft.Extensions.DependencyInjection;

namespace MoSeqAcquire.ViewModels
{
    public class MoSeqAcquireViewModel : BaseViewModel
    {

        public MoSeqAcquireViewModel(TaskbarItemInfoViewModel taskbarViewModel, 
                                     MediaSourceCollectionViewModel mediaSourceViewModel,
                                     RecordingManagerViewModel recordingViewModel,
                                     TriggerManagerViewModel triggerManagerViewModel,
                                     ProtocolManagerViewModel protocolViewModel)
        {
            this.TaskbarItemInfo = taskbarViewModel;
            this.MediaSources = mediaSourceViewModel;
            this.Recorder = recordingViewModel;
            this.Triggers = triggerManagerViewModel;
            this.Protocol = protocolViewModel;

            this.Initialize();
        }

        private static MoSeqAcquireViewModel _instance;
        public static MoSeqAcquireViewModel Instance => _instance ?? (_instance = new MoSeqAcquireViewModel());

        protected void Initialize()
        {
            App.SetCurrentStatus("Loading Theme....");
            this.Theme = new ThemeViewModel();
            App.SetCurrentStatus("Loading Components...");
            ProtocolHelpers.FindComponents(); //preload assemblies here...

            App.SetCurrentStatus("Initializing Views....");
        }


        public TaskbarItemInfoViewModel TaskbarItemInfo { get; protected set; }
        public CommandLibrary Commands { get => App.Current.Services.GetService<CommandLibrary>(); }
        public ThemeViewModel Theme { get; protected set; }
        public MediaSourceCollectionViewModel MediaSources { get; protected set; }
        public MetadataViewModel RecordingMetadata { get; private set; }
        public RecordingManagerViewModel Recorder { get; protected set; }
        public TriggerManagerViewModel Triggers { get; protected set; }
        public ProtocolManagerViewModel Protocol { get; protected set; }


        public string MainWindowTitle
        {
            get
            {
                return "MoSeq Acquire - " + this.Protocol.CurrentProtocol.Name;
            }
        }
    }


    public static class ProtocolExtensions
    {
        public static Protocol GetDefaultProtocol()
        {
            return new Protocol("Default");
        }
    }
}
