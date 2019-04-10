using MoSeqAcquire.Models.Management;
using MoSeqAcquire.Models.Recording;
using MoSeqAcquire.Models.Triggers;
using MoSeqAcquire.ViewModels.Commands;
using MoSeqAcquire.ViewModels.Recording;
using MoSeqAcquire.ViewModels.Triggers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MoSeqAcquire.ViewModels
{
    public class MoSeqAcquireViewModel : BaseViewModel
    {

        public MoSeqAcquireViewModel()
        {
            this.Theme = new ThemeViewModel();
            this.TriggerBus = new TriggerBus();
            this.MediaSources = new ObservableCollection<MediaSourceViewModel>();
            this.Recorder = new RecordingManagerViewModel(this);
            this.Triggers = new TriggerManagerViewModel(this);
            
            this.Commands = new CommandLibrary(this);
            this.Commands.LoadProtocol.Execute(ProtocolExtensions.GetDefaultProtocol());
        }
        public CommandLibrary Commands { get; protected set; }
        public ThemeViewModel Theme { get; protected set; }
        public TriggerBus TriggerBus { get; protected set; }
        public ObservableCollection<MediaSourceViewModel> MediaSources { get; protected set; }
        public RecordingManagerViewModel Recorder { get; protected set; }
        public TriggerManagerViewModel Triggers { get; protected set; }

        public bool IsProtocolLocked { get => true; }

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
            pcol.Recordings.GeneralSettings = this.Recorder.GeneralSettings.GetSnapshot();
            this.CurrentProtocol = pcol;
            return pcol;
        }
        public void UnloadProtocol()
        {
            //prepare
            this.MediaSources.ForEach(s => s.MediaSource.Stop());
            this.MediaSources.Clear();
            this.Recorder.Recorders.Clear();
        }
        public void ApplyProtocol(Protocol protocol)
        {
            
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
                    this.NotifyPropertyChanged();
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
