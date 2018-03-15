using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoSeqAcquire.Models.Acquisition;

namespace MoSeqAcquire.ViewModels
{
    public class MediaSourceViewModel : BaseViewModel
    {
        protected ObservableCollection<ChannelViewModel> _channels;
        protected ReadOnlyObservableCollection<ChannelViewModel> _ro_Channels;
        public MediaSourceViewModel(MediaSource mediaSource)
        {
            this.MediaSource = mediaSource;
            this._channels = new ObservableCollection<ChannelViewModel>();
            this._ro_Channels = new ReadOnlyObservableCollection<ChannelViewModel>(this._channels);

            foreach (var c in mediaSource.Channels)
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
        public ReadOnlyObservableCollection<ChannelViewModel> Channels { get => this._ro_Channels; }
    }
}
