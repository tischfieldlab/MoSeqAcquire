using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Windows;
using System.Windows.Media.Imaging;
using MoSeqAcquire.Models.Acquisition;

namespace MoSeqAcquire.ViewModels
{
    public class VideoChannelViewModel : ChannelViewModel
    {
        public VideoChannelViewModel(Channel channel) : base(channel)
        {
        }
        public override void BindChannel()
        {
            MediaBus.Instance.Subscribe(
                bc => bc.Channel == this.channel,
                new ActionBlock<ChannelFrame>(frame =>
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        var meta = frame.Metadata as VideoChannelFrameMetadata;
                        if (this.Stream == null || !this.CheckBitmapOk(frame))
                        {
                            this.Stream = new WriteableBitmap(meta.Width, meta.Height, 96, 96, meta.PixelFormat, null);
                        }
                        this.Stream.WritePixels(
                            new Int32Rect(0, 0, meta.Width, meta.Height),
                            frame.FrameData,
                            meta.Stride,
                            0);

                        this.FrameRate.Increment();
                    }));
                })
            );
        }
        protected bool CheckBitmapOk(ChannelFrame frame)
        {
            var meta = frame.Metadata as VideoChannelFrameMetadata;
            if (this.Stream.PixelHeight != meta.Height) return false;
            if (this.Stream.PixelWidth != meta.Width) return false;
            if (this.Stream.Format != meta.PixelFormat) return false;
            return true;
        }
        private WriteableBitmap _stream;
        public WriteableBitmap Stream { get => _stream; set => SetField(ref _stream, value); }

        protected bool showCrosshairs;
        public bool ShowCrosshairs
        {
            get => this.showCrosshairs;
            set => this.SetField(ref this.showCrosshairs, value);
        }
    }
}
