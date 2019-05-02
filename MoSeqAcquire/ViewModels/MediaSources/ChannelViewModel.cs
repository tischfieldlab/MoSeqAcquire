using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Windows;
using System.Windows.Media.Imaging;
using MoSeqAcquire.Models.Acquisition;
using MoSeqAcquire.Models.Performance;
using MoSeqAcquire.Models.Recording;
using MoSeqAcquire.ViewModels.Core;

namespace MoSeqAcquire.ViewModels.MediaSources
{
    public abstract class ChannelViewModel : BaseViewModel, IPerformanceProvider
    {
        protected Channel channel;
        protected SizeHelper sizeHelper;

        


        public ChannelViewModel(Channel channel)
        {
            this.channel = channel;
            this.BindChannel();
            this.Performance = new TotalFrameCounter();
            this.Performance.Start();

            this.sizeHelper = new SizeHelper(150, 500, 100, 500, 200, 150);

        }
        public abstract void BindChannel();
        
        public Channel Channel { get => this.channel; }
        public string Name { get => this.channel.Name; }
        public string DeviceName { get => this.channel.DeviceName; }
        public string FullName { get => this.channel.FullName; }
        public bool Enabled { get => this.channel.Enabled; set => this.channel.Enabled = value; }
        public TotalFrameCounter Performance { get; protected set; }

        public SizeHelper DisplaySize { get => this.sizeHelper; }

        



        public static ChannelViewModel FromChannel(Channel channel)
        {
            switch (channel.MediaType)
            {
                case MediaType.Video:
                    return new VideoChannelViewModel(channel);

                case MediaType.Audio:
                    return new AudioChannelViewModel(channel);

                case MediaType.Data:
                    return new DataChannelViewModel(channel);
            }
            throw new InvalidOperationException("Unable to determine correct implementation for channel!");
        }
    }
}
