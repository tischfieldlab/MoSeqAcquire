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

namespace MoSeqAcquire.ViewModels
{
    public class MoSeqAcquireViewModel : BaseViewModel
    {
        protected MediaBus __mediaBus;
        protected ObservableCollection<MediaSourceViewModel> mediaSources;
        protected ReadOnlyObservableCollection<MediaSourceViewModel> ro_mediaSources;

        protected ObservableCollection<ChannelViewModel> channelBitmaps;
        protected ReadOnlyObservableCollection<ChannelViewModel> ro_channelBitmaps;


        public MoSeqAcquireViewModel()
        {
            this.__mediaBus = MediaBus.Instance;
            this.mediaSources = new ObservableCollection<MediaSourceViewModel>();
            this.ro_mediaSources = new ReadOnlyObservableCollection<MediaSourceViewModel>(this.mediaSources);

            this.loadAndApplyProtocol("basic.xml");
            //this.generateAndSaveProtocol("basic.xml");
        }
        protected Protocol generateProtocol()
        {
            var pcol = new Protocol("basic");
            foreach (var ms in this.mediaSources)
            {
                pcol.RegisterProvider(ms.MediaSource.GetType(), ms.Config.GetSnapshot());
            }
            return pcol;
            
        }
        protected void generateAndSaveProtocol(string filename)
        {
            MediaSettingsWriter.WriteProtocol("basic.xml", this.generateProtocol());
        }
        protected void loadAndApplyProtocol(string filename)
        {
            Protocol pcol = null;
            try
            {
                pcol = MediaSettingsWriter.ReadProtocol(filename);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            if(pcol == null)
            {
                pcol = ProtocolExtensions.GetDefaultProtocol();
            }
            this.applyProtocol(pcol);
        }
        protected void applyProtocol(Protocol protocol)
        {
            foreach(var s in protocol.Configurations)
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
            }
        }

        protected void startRecording()
        {
            var h5 = new HDF5FileWriter("test.h5");
            foreach (var c in this.__mediaBus.Channels)
            {
                h5.ConnectChannel(c, c.Channel.Name);
            }
        }

        public ReadOnlyObservableCollection<MediaSourceViewModel> MediaSources { get => ro_mediaSources; }
        public ReadOnlyObservableCollection<ChannelViewModel> ChannelStreams { get => ro_channelBitmaps; }
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
