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
            return pcol;
            
        }
        public void ApplyProtocol(Protocol protocol)
        {
            foreach(var s in protocol.Configurations)
            {
                Task.Run(() =>
                {
                    var provider = protocol.CreateProvider(s.GetProviderType());
                    while (!provider.Initalize())
                    {
                        Thread.Sleep(500);
                    }
                    provider.Config.ApplySnapshot(s.Config);
                    this.__mediaBus.Publish(provider);
                    provider.Start();
                    var pvm = new MediaSourceViewModel(provider);
                    this.mediaSources.Add(pvm);
                });
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
