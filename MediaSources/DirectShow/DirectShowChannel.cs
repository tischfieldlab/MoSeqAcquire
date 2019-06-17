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
    public class DirectShowChannel : Channel
    {
        public DirectShowChannel(IVideoProvider Source)
        {
            this.Device = Source;

            this.Name = "Video Channel";
            this.DeviceName = Source.Name;
            this.Device.VideoDevice.NewFrame += this.Device_NewFrame;
            this.MediaType = MediaType.Video;
            this.DataType = typeof(byte);
            this.Enabled = true;
        }
        public override bool Enabled { get; set; }
        public IVideoProvider Device { get; protected set; }
        public override ChannelMetadata Metadata
        {
            get
            {
                return new VideoChannelMetadata()
                {
                    Width = this.Device.VideoDevice.VideoResolution.FrameSize.Width,
                    Height = this.Device.VideoDevice.VideoResolution.FrameSize.Height,
                    TargetFramesPerSecond = this.Device.VideoDevice.VideoResolution.MaximumFrameRate,
                    BytesPerPixel = this.Device.VideoDevice.VideoResolution.BitCount / 8,
                };
            }
        }

        



        private byte[] _copyBuffer;
        private int currentFrameId;
        private Rectangle rect;
        private BitmapData bmpData;
        private void Device_NewFrame(object sender, Accord.Video.NewFrameEventArgs e)
        {
            if (!this.Enabled)
                return;

            if (e.Frame != null)
            {
                var meta = new VideoChannelFrameMetadata()
                {
                    FrameId = this.currentFrameId++,
                    //Timestamp = imageFrame.Timestamp,
                    Width = e.Frame.Width,
                    Height = e.Frame.Height,
                    BytesPerPixel = Image.GetPixelFormatSize(e.Frame.PixelFormat) / 8,
                    PixelFormat = e.Frame.PixelFormat.ToMediaPixelFormat(),
                    AbsoluteTime = PreciseDatetime.Now
                };

                if (this.rect.Width != e.Frame.Width || this.rect.Height != e.Frame.Height)
                {
                    this.rect = new Rectangle(0, 0, e.Frame.Width, e.Frame.Height);
                }
                
                // Declare an array to hold the bytes of the bitmap.
                if (this._copyBuffer == null || this._copyBuffer.Length != meta.TotalBytes)
                {
                    this._copyBuffer = new byte[meta.TotalBytes];
                }
                bmpData = e.Frame.LockBits(rect, ImageLockMode.ReadOnly, e.Frame.PixelFormat);
                Marshal.Copy(bmpData.Scan0, this._copyBuffer, 0, meta.TotalBytes);

                this.PostFrame(new ChannelFrame(this._copyBuffer, meta));
            }
        }
    }
}
