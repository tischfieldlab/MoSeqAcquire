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

namespace MoSeqAcquire.ViewModels
{
    public class MoSeqAcquireViewModel : BaseViewModel
    {
        protected bool isProtocolLocked;
        protected int forceProtocolLock;
        

        public MoSeqAcquireViewModel()
        {
            this.RecentlyUsedProtocols = new ObservableCollection<string>(Settings.Default.RecentProtocols.Cast<string>());

            this.Initialize();
        }

        protected void Initialize()
        {
            App.SetCurrentStatus("Loading Theme....");
            this.Theme = new ThemeViewModel();
            App.SetCurrentStatus("Loading Components...");
            ProtocolHelpers.FindComponents(); //preload assemblies here...

            App.SetCurrentStatus("Initializing Views....");
            this.TriggerBus = new TriggerBus();
            this.MediaSources = new MediaSourceCollectionViewModel();
            this.RecordingMetadata = new MetadataViewModel(this);
            this.Recorder = new RecordingManagerViewModel(this);
            this.Triggers = new TriggerManagerViewModel(this);
            this.Commands = new CommandLibrary(this);
            this.TaskbarItemInfo = new TaskbarItemInfoViewModel(this);

            this.SubsystemComponents = new ObservableCollection<BaseViewModel>()
            {
                this.Recorder,
                this.Triggers,
                this.RecordingMetadata
            };

            App.SetCurrentStatus("Loading default protocol....");
            this.Commands.LoadProtocol.Execute(ProtocolExtensions.GetDefaultProtocol());
        }

        public void PushRecentProtocol(string path)
        {
            if (this.RecentlyUsedProtocols.Contains(path))
            {
                this.RecentlyUsedProtocols.Move(this.RecentlyUsedProtocols.IndexOf(path), 0);
            }
            else
            {
                this.RecentlyUsedProtocols.Insert(0, path);
            }

            if (this.RecentlyUsedProtocols.Count > Settings.Default.MaxRecentProtocolsToRemember)
            {
                this.RecentlyUsedProtocols
                    .Skip(Settings.Default.MaxRecentProtocolsToRemember)
                    .ToList()
                    .ForEach(rup => this.RecentlyUsedProtocols.Remove(rup));
            }

            Settings.Default.RecentProtocols.Clear();
            Settings.Default.RecentProtocols.AddRange(this.RecentlyUsedProtocols.ToArray());
            Settings.Default.Save();
        }
        public ObservableCollection<BaseViewModel> SubsystemComponents { get; protected set; }
        public TaskbarItemInfoViewModel TaskbarItemInfo { get; protected set; }
        public CommandLibrary Commands { get; protected set; }
        public ThemeViewModel Theme { get; protected set; }
        public TriggerBus TriggerBus { get; protected set; }
        public MediaSourceCollectionViewModel MediaSources { get; protected set; }
        public MetadataViewModel RecordingMetadata { get; private set; }
        public RecordingManagerViewModel Recorder { get; protected set; }
        public TriggerManagerViewModel Triggers { get; protected set; }
        public ObservableCollection<string> RecentlyUsedProtocols { get; protected set; }

        public void ForceProtocolLocked()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                this.forceProtocolLock++;
                this.NotifyPropertyChanged(nameof(this.IsProtocolLocked));
                this.NotifyPropertyChanged(nameof(this.IsProtocolForceLocked));
            });
        }
        public void UndoForceProtocolLocked()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                this.forceProtocolLock--;
                if (this.forceProtocolLock < 0)
                {
                    this.forceProtocolLock = 0;
                }
                this.NotifyPropertyChanged(nameof(this.IsProtocolLocked));

                this.NotifyPropertyChanged(nameof(this.IsProtocolForceLocked));
            });
        }

        public bool IsProtocolForceLocked
        {
            get => this.forceProtocolLock > 0;
        }
        public bool IsProtocolLocked
        {
            get => this.isProtocolLocked || this.forceProtocolLock > 0;
            set => this.SetField(ref this.isProtocolLocked, value);
        }

        public string MainWindowTitle
        {
            get
            {
                return "MoSeq Acquire - " + this.CurrentProtocol.Name;
            }
        }

        public Protocol CurrentProtocol { get; protected set; }


        public Protocol GenerateProtocol()
        {
            this.ForceProtocolLocked();
            var pcol = new Protocol("basic");
            foreach (var ms in this.MediaSources.Items)
            {
                pcol.Sources.Add(ms.GetMediaSourceDefinition());
            }
            foreach(var mw in this.Recorder.Recorders)
            {
                pcol.Recordings.Recorders.Add(mw.GetRecorderDefinition());
            }
            foreach(var tvm in this.Triggers.Triggers)
            {
                pcol.Triggers.Add(tvm.GetTriggerDefinition());
            }

            pcol.Metadata = this.RecordingMetadata.Items;
            pcol.Recordings.GeneralSettings = this.Recorder.GeneralSettings.GetSnapshot();
            pcol.Locked = this.isProtocolLocked;
            this.CurrentProtocol = pcol;
            this.UndoForceProtocolLocked();
            return pcol;
        }
        public void UnloadProtocol()
        {
            this.ForceProtocolLocked();
            //prepare
            this.MediaSources.Items.ForEach(s => s.Shutdown());
            this.MediaSources.Items.Clear();
            this.Recorder.ClearRecorders();
            this.Triggers.RemoveTriggers();
            this.RecordingMetadata.Items.Clear();
            this.isProtocolLocked = false;
            this.UndoForceProtocolLocked();
        }
        public void ApplyProtocol(Protocol protocol)
        {
            this.ForceProtocolLocked();   
            //prepare
            this.UnloadProtocol();

            this.CurrentProtocol = protocol;

            //add media sources
            var tasks = new List<Task>();
            foreach(var s in protocol.Sources)
            {
                var msvm = new MediaSourceViewModel(s);
                this.MediaSources.Items.Add(msvm);
                tasks.Add(msvm.InitTask);
            }

            //necessary to wait for all hardware to load prior to applying recorders
            //otherwise the channel does not exist!
            Task.WhenAll(tasks).ContinueWith((t) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (protocol.Recordings != null)
                    {
                        this.Recorder.GeneralSettings.ApplySnapshot(protocol.Recordings.GeneralSettings);
                        foreach (var r in protocol.Recordings.Recorders)
                        {
                            this.Recorder.AddRecorder(new RecorderViewModel(this, r));
                        }
                    }
                    if(protocol.Triggers != null)
                    {
                        foreach(var trigger in protocol.Triggers)
                        {
                            this.Triggers.AddTrigger(trigger);
                        }
                    }

                    if (protocol.Metadata != null)
                    {
                        foreach (var item in protocol.Metadata)
                        {
                            this.RecordingMetadata.Items.Add(item);
                        }
                    }
                    this.RecordingMetadata.Items.ResetValuesToDefaults();
                    this.isProtocolLocked = protocol.Locked;
                    this.NotifyPropertyChanged();
                    this.UndoForceProtocolLocked();
                });
            }, TaskScheduler.Default);
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
