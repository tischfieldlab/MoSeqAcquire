using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MoSeqAcquire.Models.Acquisition;

namespace MoSeqAcquire.ViewModels
{
    public class DataChannelViewModel : ChannelViewModel
    {
        public DataChannelViewModel(Channel channel) : base(channel)
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
                            this.Stream = new WriteableBitmap(100, 100, 96, 96, PixelFormats.Gray16, null);
                        }
                        float[] data = frame.FrameData as float[];
                        var max = data.Max();
                        int width = (int)(this.Stream.Width / frame.FrameData.Length);

                        int stride = (int)(this.Stream.Width * this.Stream.Format.BitsPerPixel + 7) / 8;
                        this.Stream.WritePixels(
                                new Int32Rect(0, 0, 100, 100),
                                new short[100 * 100],
                                stride,
                                0);
                        try
                        {
                            for (int i = 0; i < data.Length; i++)
                            {
                                stride = (int)(width * this.Stream.Format.BitsPerPixel + 7) / 8;
                                var height = (int)((data[i] / max) * 100);
                                this.Stream.WritePixels(
                                    new Int32Rect(i * width, 0, width, stride),
                                    Enumerable.Repeat(short.MaxValue, width * height).ToArray(),
                                    stride,
                                    0);
                            }
                        }
                        catch { }
                        
                        this.FrameRate.Increment();
                    }));
                })
            );
        }
        protected bool CheckBitmapOk(ChannelFrame frame)
        {
            /*var meta = frame.Metadata as VideoChannelFrameMetadata;
            if (this.Stream.PixelHeight != meta.Height) return false;
            if (this.Stream.PixelWidth != meta.Width) return false;
            if (this.Stream.Format != meta.PixelFormat) return false;*/
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
