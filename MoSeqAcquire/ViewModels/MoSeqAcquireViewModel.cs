using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using MoSeqAcquire.Models;
using MoSeqAcquire.Models.Acquisition;
using MoSeqAcquire.Models.Acquisition.Kinect;
using MoSeqAcquire.Models.IO;
using MoSeqAcquire.Models.Management;
using MoSeqAcquire.ViewModels.Commands;
using MoSeqAcquire.ViewModels.Recording;

namespace MoSeqAcquire.ViewModels
{
    public class MoSeqAcquireViewModel : BaseViewModel
    {
        protected MediaBus __mediaBus;
        protected ObservableCollection<MediaSourceViewModel> mediaSources;

        protected RecordingManagerViewModel recorderManager;

        public MoSeqAcquireViewModel()
        {
            this.Theme = new ThemeViewModel();
            this.__mediaBus = MediaBus.Instance;
            this.mediaSources = new ObservableCollection<MediaSourceViewModel>();
            this.recorderManager = new RecordingManagerViewModel(this);
            


            this.Commands = new CommandLibrary(this);
            this.Commands.LoadProtocol.Execute(ProtocolExtensions.GetDefaultProtocol());
        }
        public CommandLibrary Commands { get; protected set; }
        public ThemeViewModel Theme { get; protected set; }
        public Protocol GenerateProtocol()
        {
            var pcol = new Protocol("basic");
            foreach (var ms in this.mediaSources)
            {
                pcol.Sources.Add(ms.MediaSource.GetType(), ms.Config.GetSnapshot());
            }
            foreach(var mw in this.recorderManager.Recorders)
            {
                pcol.Recordings.Recorders.Add(mw.GetRecorderDefinition());
            }
            pcol.Recordings.GeneralSettings = this.recorderManager.GeneralSettings;
            return pcol;
            
        }
        public void ApplyProtocol(Protocol protocol)
        {
            //prepare
            this.mediaSources.ForEach(s => s.MediaSource.Stop());
            this.mediaSources.Clear();
            //this.__mediaBus.Clear();
            this.recorderManager.Recorders.Clear();

            //add media sources
            var tasks = new List<Task>();
            foreach(var s in protocol.Sources)
            {
                var msvm = new MediaSourceViewModel(s);
                tasks.Add(msvm.InitTask);
                this.mediaSources.Add(msvm);
                
                /*tasks.Add(Task.Run(() =>
                {
                    var provider = (MediaSource)s.Create();
                    while (!provider.Initalize())
                    {
                        Thread.Sleep(500);
                    }
                    provider.Config.ApplySnapshot(s.Config);
                    this.__mediaBus.Publish(provider);
                    
                    
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        
                    });
                    provider.Start();
                }));*/
            }

            Task.WhenAll(tasks).ContinueWith((t) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    this.recorderManager.GeneralSettings = protocol.Recordings.GeneralSettings;
                    foreach (var r in protocol.Recordings.Recorders)
                    {
                        this.recorderManager.Recorders.Add(new RecorderViewModel(this, r));
                    }
                });
            });
        }

        public ObservableCollection<MediaSourceViewModel> MediaSources { get => mediaSources; }
        public RecordingManagerViewModel Recorder { get => this.recorderManager; }

    }

    

    

    public static class ProtocolExtensions
    {
        public static Protocol GetDefaultProtocol()
        {
            var pcol = new Protocol("Default");
            //default protocol contains the Kinect sensor
            pcol.Sources.Add(typeof(KinectManager), KinectConfigSnapshot.GetDefault());
            pcol.Recordings.GeneralSettings = new GeneralRecordingSettings();
            return pcol;
        }
    }
}
