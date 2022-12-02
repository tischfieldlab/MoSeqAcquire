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
        protected int fps;
        public ImageTestPatternChannel(TestPatternSource source)
        {
            this.Device = source;
            var config = this.Device.Settings as TestPatternConfig;

            this.Name = "Video Test";
            this.MediaType = MediaType.Video;
            this.Enabled = true;
            
            this.PrepareImage(config.FrameSource);

            this.fps = config.FrameRate;
            this.__timer = new MultimediaTimer()
            {
                Interval = 1000 / this.fps,
                Resolution = 0
            };
            this.__timer.Elapsed += (s, e) => this.ProduceFrame();
            this.__timer.Start();
            this.Device.Settings.PropertyChanged += Settings_PropertyChanged;
        }

        private void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var config = this.Device.Settings as TestPatternConfig;

            if (e.PropertyName == null || e.PropertyName.Equals(nameof(config.FrameRate)))
            {
                this.UpdateFrameRate(config.FrameRate);
            }

            if (e.PropertyName == null || e.PropertyName.Equals(nameof(config.FrameSource)))
            {
                this.PrepareImage(config.FrameSource);
            }
        }

        public TestPatternSource Device { get; protected set; }
        public override ChannelMetadata Metadata => new VideoChannelMetadata()
        {
            DataType = typeof(byte),
            TargetFramesPerSecond = this.fps,
            Width = this.bitmap.PixelWidth,
            Height = this.bitmap.PixelHeight,
            PixelFormat = this.bitmap.Format,
            BytesPerPixel = this.bitmap.Format.BitsPerPixel / 8
        };

        private void PrepareImage(string imageName)
        {
            this.bitmap = new BitmapImage(new Uri($"pack://application:,,,/TestPatterns;component/Patterns/{imageName}"));
            this.bitmap.Freeze();
        }

        private void UpdateFrameRate(int frameRate)
        {
            if (frameRate != this.fps)
            {
                this.fps = frameRate;
                this.__timer.Stop();
                this.__timer.Interval = 1000 / this.fps;
                this.__timer.Start();
            }
        }

        private BitmapImage bitmap;
        private MultimediaTimer __timer;
        private long currentFrameId;
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
