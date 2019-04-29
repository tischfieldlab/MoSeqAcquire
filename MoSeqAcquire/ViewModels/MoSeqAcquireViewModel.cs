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
            ProtocolHelpers.FindComponents();
            App.SetCurrentStatus("Initializing Trigger Bus....");
            this.TriggerBus = new TriggerBus();
            App.SetCurrentStatus("Loading Media Sources....");
            this.MediaSources = new ObservableCollection<MediaSourceViewModel>();
            App.SetCurrentStatus("Loading Recording Console....");
            this.Recorder = new RecordingManagerViewModel(this);
            App.SetCurrentStatus("Loading Triggers....");
            this.Triggers = new TriggerManagerViewModel(this);

            App.SetCurrentStatus("Initializing Commands....");
            this.Commands = new CommandLibrary(this);
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

        public CommandLibrary Commands { get; protected set; }
        public ThemeViewModel Theme { get; protected set; }
        public TriggerBus TriggerBus { get; protected set; }
        public ObservableCollection<MediaSourceViewModel> MediaSources { get; protected set; }
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
        public void UndoForceProtoclLocked()
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
            foreach (var ms in this.MediaSources)
            {
                pcol.Sources.Add(ms.MediaSource.GetType(), ms.MediaSource.DeviceId, ms.Config.GetSnapshot());
            }
            foreach(var mw in this.Recorder.Recorders)
            {
                pcol.Recordings.Recorders.Add(mw.GetRecorderDefinition());
            }
            foreach(var tvm in this.Triggers.Triggers)
            {
                pcol.Triggers.Add(tvm.GetTriggerDefinition());
            }

            pcol.Metadata = this.Recorder.RecordingMetadata.Items;
            pcol.Recordings.GeneralSettings = this.Recorder.GeneralSettings.GetSnapshot();
            pcol.Locked = this.isProtocolLocked;
            this.CurrentProtocol = pcol;
            this.UndoForceProtoclLocked();
            return pcol;
        }
        public void UnloadProtocol()
        {
            this.ForceProtocolLocked();
            //prepare
            this.MediaSources.ForEach(s => s.MediaSource.Stop());
            this.MediaSources.Clear();
            this.Recorder.ClearRecorders();
            this.Triggers.RemoveTriggers();
            this.Recorder.RecordingMetadata.Items.Clear();
            this.isProtocolLocked = false;
            this.UndoForceProtoclLocked();
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
                tasks.Add(msvm.InitTask);
                this.MediaSources.Add(msvm);
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
                            this.Recorder.RecordingMetadata.Items.Add(item);
                        }
                    }
                    this.Recorder.RecordingMetadata.Items.ResetValuesToDefaults();
                    this.isProtocolLocked = protocol.Locked;
                    this.NotifyPropertyChanged();
                    this.UndoForceProtoclLocked();
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
