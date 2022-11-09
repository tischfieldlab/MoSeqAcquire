using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MoSeqAcquire.Models.Acquisition;
using MoSeqAcquire.Models.Utility;

namespace TestPatterns
{
    class ImageTestPatternChannel : Channel
    {
        public ImageTestPatternChannel(TestPatternSource source)
        {
            this.Device = source;
            this.Name = "Video Test";
            this.MediaType = MediaType.Video;
            this.Enabled = true;

            this.PrepareImage();
           
            this.__timer = new MultimediaTimer()
            {
                Interval = 1000 / 30,
                Resolution = 0
            };
            this.__timer.Elapsed += (s, e) => this.ProduceFrame();
            this.__timer.Start();
        }
        public TestPatternSource Device { get; protected set; }
        public override ChannelMetadata Metadata => new VideoChannelMetadata()
        {
            DataType = typeof(byte),
            TargetFramesPerSecond = 30,
            Width = this.bitmap.PixelWidth,
            Height = this.bitmap.PixelHeight,
            PixelFormat = this.bitmap.Format,
            BytesPerPixel = this.bitmap.Format.BitsPerPixel / 8
        };

        private void PrepareImage()
        {
            this.bitmap = new BitmapImage(new Uri("pack://application:,,,/TestPatterns;component/Patterns/PM5544_with_non-PAL_signals.png"));
            this.bitmap.Freeze();
        }

        private BitmapImage bitmap;
        private MultimediaTimer __timer;
        private int currentFrameId;
        private byte[] _copyBuffer;
        private void ProduceFrame()
        {
            if (!this.Enabled)
                return;


            var meta = new VideoChannelFrameMetadata()
            {
                FrameId = this.currentFrameId++,
                Width = bitmap.PixelWidth,
                Height = bitmap.PixelHeight,
                BytesPerPixel = bitmap.Format.BitsPerPixel / 8,
                PixelFormat = bitmap.Format,
                AbsoluteTime = PreciseDatetime.Now
            };

            // Declare an array to hold the bytes of the bitmap.
            if (this._copyBuffer == null || this._copyBuffer.Length != meta.TotalBytes)
            {
                this._copyBuffer = new byte[meta.TotalBytes];
                bitmap.CopyPixels(this._copyBuffer, (meta.Width * meta.BytesPerPixel), 0);
            }
            
            this.PostFrame(new ChannelFrame(this._copyBuffer, meta));
        }
    }

    
}
