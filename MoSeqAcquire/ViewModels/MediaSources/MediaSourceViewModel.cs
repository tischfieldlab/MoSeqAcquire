using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using GongSolutions.Wpf.DragDrop;
using MoSeqAcquire.Models.Acquisition;
using MoSeqAcquire.Models.Configuration;
using MoSeqAcquire.Models.Management;

namespace MoSeqAcquire.ViewModels.MediaSources
{
    public class MediaSourceViewModel : BaseViewModel, IDropTarget
    {
        //protected bool isConfigOpen;
        protected ObservableCollection<ChannelViewModel> _channels;
        protected ReadOnlyObservableCollection<ChannelViewModel> _ro_Channels;

        protected bool isReady;

        protected MediaSourceViewModel()
        {
            this.isReady = false;
            this._channels = new ObservableCollection<ChannelViewModel>();
            this._ro_Channels = new ReadOnlyObservableCollection<ChannelViewModel>(this._channels);
        }
        public MediaSourceViewModel(ProtocolSource mediaSource) : this()
        {
            this.MediaSource = (MediaSource)mediaSource.Create();
            Task.Run(() =>
            {
                while (!this.isReady)
                {
                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        this.NotifyPropertyChanged(nameof(this.CurrentStatus));
                    }));
                    Thread.Sleep(100);
                }
            });
            this.InitTask = Task.Run(() =>
            {
                while (!this.MediaSource.Initialize(mediaSource.DeviceId))
                {
                    Thread.Sleep(100);
                }
                this.MediaSource.Settings.ApplySnapshot(mediaSource.Config);
                this.MediaSource.Start();
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    this.RetrieveChannels();
                    this.IsReady = true;
                    this.NotifyPropertyChanged(null);
                }));
            });
        }

        public void Shutdown()
        {
            this.MediaSource.Stop();
            while (this.Channels.Count > 0)
            {
                this.Channels.RemoveAt(this.Channels.Count - 1);
            }
        }
        public Task InitTask { get; protected set; }
        public string Name => this.MediaSource.Name;
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
                this._channels.Add(ChannelViewModel.FromChannel(c));
            }
        }

        void IDropTarget.DragOver(IDropInfo dropInfo)
        {
            GongSolutions.Wpf.DragDrop.DragDrop.DefaultDropHandler.DragOver(dropInfo);
            if (!this.Channels.Contains(dropInfo.Data as ChannelViewModel))
            {
                dropInfo.Effects = System.Windows.DragDropEffects.None;
            }
        }

        void IDropTarget.Drop(IDropInfo dropInfo)
        {
            if (this.Channels.Contains(dropInfo.Data as ChannelViewModel))
            {
                GongSolutions.Wpf.DragDrop.DragDrop.DefaultDropHandler.Drop(dropInfo);
            }
        }

        protected MediaSource MediaSource { get; set; }
        public BaseConfiguration Config { get => this.isReady ? MediaSource.Settings : null; }
        public ObservableCollection<ChannelViewModel> Channels { get => this._channels; }

        public ProtocolSource GetMediaSourceDefinition()
        {
            return new ProtocolSource()
            {
                Provider = this.MediaSource.GetType().AssemblyQualifiedName,
                DeviceId = this.MediaSource.DeviceId,
                Config = this.Config.GetSnapshot()
            };
        }

        public void DragEnter(IDropInfo dropInfo)
        {
            throw new NotImplementedException();
        }

        public void DragLeave(IDropInfo dropInfo)
        {
            throw new NotImplementedException();
        }
    }
}
