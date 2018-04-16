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

namespace MoSeqAcquire.ViewModels
{
    public abstract class ChannelViewModel : BaseViewModel
    {
        protected Channel channel;
        public ChannelViewModel(Channel channel)
        {
            this.channel = channel;
            this.FrameRate = new FrameRateCounter();
            this.BindChannel();
        }
        public abstract void BindChannel();
        
        public Channel Channel { get => this.channel; }
        public string Name { get => this.channel.Name; }
        public string DeviceName { get => this.channel.DeviceName; }
        public string FullName { get => this.channel.FullName; }
        public bool Enabled { get => this.channel.Enabled; set => this.channel.Enabled = value; }
        public FrameRateCounter FrameRate { get; protected set; }
    }
}
