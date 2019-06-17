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
        public ImageTestPatternChannel()
        {
            this.Name = "Video Test";
            this.MediaType = MediaType.Video;
            this.DataType = typeof(byte);
            this.Enabled = true;

           
            this.__timer = new MultimediaTimer()
            {
                Interval = 1000 / 30,
                Resolution = 0
            };
            this.__timer.Elapsed += (s, e) => this.ProduceFrame();
            this.__timer.Start();
        }
        public override ChannelMetadata Metadata => new VideoChannelMetadata()
        {
            TargetFramesPerSecond = 30,
            
        };

        private BitmapImage bitmap;
        private MultimediaTimer __timer;
        private int currentFrameId;
        private byte[] _copyBuffer;
        private Rectangle rect;
        private void ProduceFrame()
        {
            if (!this.Enabled)
                return;

            if (this.bitmap == null)
            {
                this.bitmap = new BitmapImage(new Uri( "pack://application:,,,/TestPatterns;component/Patterns/PM5544_with_non-PAL_signals.png"));
                this.bitmap.Freeze();
            }


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
