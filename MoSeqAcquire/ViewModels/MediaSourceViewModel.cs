using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using MoSeqAcquire.Models.Acquisition;
using MoSeqAcquire.Models.Management;
using MoSeqAcquire.ViewModels.PropertyManagement;

namespace MoSeqAcquire.ViewModels
{
    public class MediaSourceViewModel : BaseViewModel
    {
        protected ObservableCollection<ChannelViewModel> _channels;
        protected ReadOnlyObservableCollection<ChannelViewModel> _ro_Channels;

        protected bool isReady;

        protected MediaSourceViewModel()
        {
            this.isReady = false;
            this._channels = new ObservableCollection<ChannelViewModel>();
            this._ro_Channels = new ReadOnlyObservableCollection<ChannelViewModel>(this._channels);
        }

        public MediaSourceViewModel(MediaSource mediaSource) : this()
        {
            this.MediaSource = mediaSource;
            this.RetrieveChannels();
        }
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
                    Thread.Sleep(500);
                }
                this.MediaSource.Config.ApplySnapshot(mediaSource.Config);
                this.MediaSource.Start();
                Application.Current.Dispatcher.Invoke(() =>
                {
                    this.RetrieveChannels();
                    this.IsReady = true;
                });
            });
        }
        public Task InitTask { get; protected set; }
        public bool IsReady
        {
            get => this.isReady;
            set => this.SetField(ref this.isReady, value);
        }
        public string CurrentStatus
        {
            get => this.MediaSource.Status;
        }

        protected void RetrieveChannels()
        {
            foreach (var c in this.MediaSource.Channels)
            {
                switch (c.MediaType)
                {
                    case MediaType.Video:
                        this._channels.Add(new VideoChannelViewModel(c));
                        break;
                    case MediaType.Audio:
                        this._channels.Add(new AudioChannelViewModel(c));
                        break;
                }
            }
        }

        public MediaSource MediaSource { get; protected set; }
        public MediaSourceConfig Config { get => MediaSource.Config; }
        public PropertyCollection ConfigItems { get => new PropertyCollection(this.Config); }
        public ReadOnlyObservableCollection<ChannelViewModel> Channels { get => this._ro_Channels; }
    }
}
