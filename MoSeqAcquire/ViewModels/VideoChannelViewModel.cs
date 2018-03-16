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
                        if (this.Stream == null || !this.CheckBitmapOk(frame))
                        {
                            this.Stream = new WriteableBitmap(frame.Metadata.Width, frame.Metadata.Height, 96, 96, frame.Metadata.PixelFormat, null);
                        }

                        this.Stream.WritePixels(
                            new Int32Rect(0, 0, frame.Metadata.Width, frame.Metadata.Height),
                            frame.FrameData,
                            frame.Metadata.Width * frame.Metadata.BytesPerPixel,
                            0);
                        this.FrameRate.Increment();
                    }));
                })
            );
        }
        protected bool CheckBitmapOk(ChannelFrame frame)
        {
            if (this.Stream.PixelHeight != frame.Metadata.Height) return false;
            if (this.Stream.PixelWidth != frame.Metadata.Width) return false;
            if (this.Stream.Format != frame.Metadata.PixelFormat) return false;
            return true;
        }
        private WriteableBitmap _stream;
        public WriteableBitmap Stream { get => _stream; set => SetField(ref _stream, value); }

    }
}
