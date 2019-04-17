using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using MoSeqAcquire.Models.Acquisition;
using MoSeqAcquire.Models.Configuration;
using MoSeqAcquire.Models.Management;

namespace MoSeqAcquire.ViewModels
{
    public class MediaSourceViewModel : BaseViewModel
    {
        protected bool isConfigOpen;
        protected ObservableCollection<ChannelViewModel> _channels;
        protected ReadOnlyObservableCollection<ChannelViewModel> _ro_Channels;

        protected bool isReady;

        protected MediaSourceViewModel()
        {
            this.isReady = false;
            this._channels = new ObservableCollection<ChannelViewModel>();
            this._ro_Channels = new ReadOnlyObservableCollection<ChannelViewModel>(this._channels);
        }

        /*public MediaSourceViewModel(MediaSource mediaSource) : this()
        {
            this.MediaSource = mediaSource;
            this.RetrieveChannels();
        }*/
        public MediaSourceViewModel(ProtocolSource mediaSource) : this()
        {
            this.MediaSource = (MediaSource)mediaSource.Create();
            this.InitTask = Task.Run(() =>
            {
                while (!this.MediaSource.Initalize(mediaSource.DeviceId))
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        this.NotifyPropertyChanged("CurrentStatus");
                    });
                    Thread.Sleep(100);
                }
                this.MediaSource.Settings.ApplySnapshot(mediaSource.Config);
                this.MediaSource.Start();
                Application.Current.Dispatcher.Invoke(() =>
                {
                    this.RetrieveChannels();
                    this.IsReady = true;
                    this.NotifyPropertyChanged(null);
                    /*if(this.MediaSource is DirectShowSource)
                    {
                        (this.MediaSource as DirectShowSource).Device.DisplayPropertyPage(new WindowInteropHelper(Application.Current.MainWindow).Handle);
                    }*/
                });
            });
        }
        public Task InitTask { get; protected set; }
        public bool IsReady
        {
            get => this.isReady;
            set => this.SetField(ref this.isReady, value);
        }
        public bool IsConfigOpen
        {
            get => this.isConfigOpen;
            set => this.SetField(ref this.isConfigOpen, value);
        }
        public string CurrentStatus
        {
            get => this.MediaSource.Status;
        }

        protected void RetrieveChannels()
        {
            foreach (var c in this.MediaSource.Channels)
            {
                this._channels.Add(ChannelViewModel.FromChannel(c));
            }
        }

        public MediaSource MediaSource { get; protected set; }
        public BaseConfiguration Config { get => this.isReady ? MediaSource.Settings : null; }
        public ReadOnlyObservableCollection<ChannelViewModel> Channels { get => this._ro_Channels; }
    }
}
