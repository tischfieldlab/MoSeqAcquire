using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using MoSeqAcquire.Models.Utility;

namespace MoSeqAcquire.Models.Acquisition.DirectShow
{
    public class DirectSoundChannel : Channel
    {
        public DirectSoundChannel(IAudioProvider Source)
        {
            this.Device = Source;

            this.Name = "Audio Channel";
            this.DeviceName = Source.Name;
            this.Device.AudioDevice.NewFrame += this.Device_NewFrame;
            this.MediaType = MediaType.Audio;
            this.DataType = typeof(byte);
            this.Enabled = true;
        }

        public IAudioProvider Device { get; protected set; }
        public override bool Enabled { get; set; }

        public override ChannelMetadata Metadata
        {
            get
            {
                return new AudioChannelMetadata()
                {
                    TargetFramesPerSecond = this.Device.AudioDevice.,
                    Channels = this.Device.AudioDevice.Channels
                    /*Width = this.Device.Device.VideoResolution.FrameSize.Width,
                    Height = this.Device.Device.VideoResolution.FrameSize.Height,
                    TargetFramesPerSecond = this.Device.Device.VideoResolution.MaximumFrameRate,
                    BytesPerPixel = this.Device.Device.VideoResolution.BitCount / 8,
                    */
                };
            }
        }

        private int currentFrameId;
        private void Device_NewFrame(object sender, Accord.Audio.NewFrameEventArgs e)
        {
            if (!this.Enabled)
                return;

            if (e.Signal != null)
            {
                var frame = e.Signal;
                var meta = new AudioChannelFrameMetadata()
                {
                    AbsoluteTime = PreciseDatetime.Now,
                    FrameId = this.currentFrameId++,
                    TotalBytes = e.Signal.RawData.Length,
                    
                };

                this.PostFrame(new ChannelFrame(e.Signal.RawData, meta));
            }
        }
    }
}
