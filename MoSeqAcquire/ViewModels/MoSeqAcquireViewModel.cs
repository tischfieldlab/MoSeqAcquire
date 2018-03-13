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

            this.channelBitmaps = new ObservableCollection<ChannelViewModel>();
            this.ro_channelBitmaps = new ReadOnlyObservableCollection<ChannelViewModel>(this.channelBitmaps);

            //this.LoadMediaSources();
            this.loadAndApplyProtocol("basic.xml");
            


        }
        protected void generateProtocol()
        {
            var pcol = new Protocol("basic");
            foreach (var ms in this.mediaSources)
            {
                pcol.RegisterProvider(ms.MediaSource.GetType(), ms.Config.GetSnapshot());
            }
            MediaSettingsWriter.WriteProtocol("basic.xml", pcol);
        }
        protected void loadAndApplyProtocol(string filename)
        {
            var pcol = MediaSettingsWriter.ReadProtocol(filename);
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
                provider.Initalize();
                while (!provider.Initalize())
                {
                    Thread.Sleep(500);
                }
                this.__mediaBus.Publish(provider);
                provider.Start();
                var pvm = new MediaSourceViewModel(provider);
                this.mediaSources.Add(pvm);
                foreach (var c in pvm.Channels) { this.channelBitmaps.Add(c); }
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

        /*protected void LoadMediaSources()
        {
            var kinect = new KinectManager();
            while (!kinect.Initalize())
            {
                Thread.Sleep(500);
            }
            this.__mediaBus.Publish(kinect);
            kinect.Start();
            var kvm = new MediaSourceViewModel(kinect);
            this.mediaSources.Add(kvm);
            foreach(var c in kvm.Channels) { this.channelBitmaps.Add(c); }
        }*/


    }

    public class MediaSourceViewModel : BaseViewModel
    {
        protected ObservableCollection<ChannelViewModel> _channels;
        protected ReadOnlyObservableCollection<ChannelViewModel> _ro_Channels;
        public MediaSourceViewModel(MediaSource mediaSource)
        {
            this.MediaSource = mediaSource;
            this._channels = new ObservableCollection<ChannelViewModel>();
            this._ro_Channels = new ReadOnlyObservableCollection<ChannelViewModel>(this._channels);

            foreach(var c in mediaSource.Channels)
            {
                this._channels.Add(new ChannelViewModel(c));
            }
        }

        public MediaSource MediaSource { get; protected set; }
        public MediaSourceConfig Config { get => MediaSource.Config; }
        public ReadOnlyObservableCollection<ChannelViewModel> Channels { get => this._ro_Channels; }
    }

    public class ChannelViewModel : BaseViewModel
    {
        private Channel channel;
        public ChannelViewModel(Channel channel)
        {
            this.channel = channel;
            this.BindChannel();
        }
        public void BindChannel()
        {
            MediaBus.Instance.Subscribe(
                bc => bc.Channel == this.channel,
                new ActionBlock<ChannelFrame>(frame =>
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        if (this.Stream == null || !this.CheckBitmapOk(frame))
                        {
                            this.Stream = new WriteableBitmap(frame.Metadata.Width, frame.Metadata.Height, 96, 96, frame.Metadata.PixelFormat, null);
                        }

                        this.Stream.WritePixels(
                            new Int32Rect(0, 0, frame.Metadata.Width, frame.Metadata.Height),
                            frame.FrameData,
                            frame.Metadata.Width * frame.Metadata.BytesPerPixel,
                            0);
                    }));
                }));
        }
        protected bool CheckBitmapOk(ChannelFrame frame)
        {
            if (this.Stream.PixelHeight != frame.Metadata.Height) return false;
            if (this.Stream.PixelWidth != frame.Metadata.Width) return false;
            if (this.Stream.Format != frame.Metadata.PixelFormat) return false;
            return true;
        }
        private WriteableBitmap _stream;
        public WriteableBitmap Stream { get => _stream; set => SetField(ref _stream, value); }

        public string Name { get => this.channel.Name; }
        public bool Enabled { get => this.channel.Enabled; set => this.channel.Enabled = value; }
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
