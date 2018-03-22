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
        //protected ReadOnlyObservableCollection<MediaSourceViewModel> ro_mediaSources;

        protected RecordingManagerViewModel recorderManager;

        public MoSeqAcquireViewModel()
        {
            this.__mediaBus = MediaBus.Instance;
            this.mediaSources = new ObservableCollection<MediaSourceViewModel>();
            this.recorderManager = new RecordingManagerViewModel(this);
            


            this.Commands = new CommandLibrary(this);
            this.Commands.LoadProtocol.Execute(null);
        }
        public CommandLibrary Commands { get; protected set; }
        public Protocol GenerateProtocol()
        {
            var pcol = new Protocol("basic");
            foreach (var ms in this.mediaSources)
            {
                pcol.RegisterProvider(ms.MediaSource.GetType(), ms.Config.GetSnapshot());
            }
            foreach(var mw in this.recorderManager.Recorders)
            {
                pcol.RegisterProvider(Type.GetType(mw.RecorderType), mw.Settings.GetSnapshot());
            }
            return pcol;
            
        }
        public void ApplyProtocol(Protocol protocol)
        {
            foreach(var s in protocol.Sources)
            {
                Task.Run(() =>
                {
                    var provider = (MediaSource)protocol.CreateProvider(s.GetProviderType());
                    while (!provider.Initalize())
                    {
                        Thread.Sleep(500);
                    }
                    provider.Config.ApplySnapshot(s.Config);
                    this.__mediaBus.Publish(provider);
                    provider.Start();
                    
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        this.mediaSources.Add(new MediaSourceViewModel(provider));
                    });
                });
            }

            this.recorderManager.Recorders.Clear();
            foreach(var r in protocol.Recorders)
            {
                var recorder = new RecorderViewModel(this, this.recorderManager.Settings)
                {
                    RecorderType = r.Provider
                };
                recorder.Settings.ApplySnapshot(r.Config);
                this.recorderManager.Recorders.Add(recorder);
            }
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
            pcol.RegisterProvider(typeof(KinectManager), KinectConfigSnapshot.GetDefault());
            return pcol;
        }
    }
}
