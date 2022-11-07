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
using MoSeqAcquire.ViewModels.MediaSources.Visualization.Data;

namespace MoSeqAcquire.ViewModels.MediaSources
{
    public class DataChannelViewModel : ChannelViewModel
    {
        public DataChannelViewModel(Channel channel) : base(channel)
        {
            this.RegisterViewPlugin(new BarGraphVisualization());
            this.RegisterViewPlugin(new LineGraphVisualization());
            this.SetChannelViewCommand.Execute(this.AvailableViews.First());
        }

        public override void BindChannel()
        {
            MediaBus.Instance.Subscribe(
                bc => bc.Channel == this.channel,
                new ActionBlock<ChannelFrame>(frame =>
                    {
                        var meta = frame.Metadata as DataChannelFrameMetadata;
                        (this.SelectedView.VisualizationPlugin as IDataVisualizationPlugin).OnNewFrame(frame);
                        this.Performance.Increment();
                    },
                    new ExecutionDataflowBlockOptions() { TaskScheduler = TaskScheduler.FromCurrentSynchronizationContext() }
                ));
        }

        /*protected ushort[] blank_array = new ushort[100 * 100];
        public override void BindChannel()
        {
            MediaBus.Instance.Subscribe(
                bc => bc.Channel == this.channel,
                new ActionBlock<ChannelFrame>(frame =>
                {
                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        var meta = frame.Metadata as VideoChannelFrameMetadata;
                        if (this.Stream == null || !this.CheckBitmapOk(frame))
                        {
                            this.Stream = new WriteableBitmap(100, 100, 96, 96, PixelFormats.Gray16, null);
                        }
                        float[] data = frame.FrameData as float[];
                        var max = data.Select(x => Math.Abs(x)).Max();
                        int width = (int)(this.Stream.PixelWidth / frame.FrameData.Length);
                        int half = (this.Stream.PixelHeight / 2);
                        int wholestride = (int)(this.Stream.PixelWidth * this.Stream.Format.BitsPerPixel + 7) / 8;
                        int widthstride = (int)(width * this.Stream.Format.BitsPerPixel + 7) / 8;
                        Int32Rect rect = new Int32Rect(0, 0, 100, 100);

                        this.Stream.WritePixels(
                                rect,
                                this.blank_array,
                                wholestride,
                                0);
                        
                        int height;
                        for (int i = 0; i < data.Length; i++)
                        {
                            height = (int)((data[i] / max) * half);
                            if (height < 0)
                            {
                                try
                                {
                                    rect.X = i * width;
                                    rect.Y = half;
                                    rect.Width = width;
                                    rect.Height = -height;
                                    this.Stream.WritePixels(
                                        rect,
                                        Enumerable.Repeat(ushort.MaxValue, width * -height).ToArray(),
                                        widthstride,
                                        0);
                                }
                                catch (Exception) { }
                            }
                            else
                            {
                                try
                                {
                                    rect.X = i * width;
                                    rect.Y = half - height;
                                    rect.Width = width;
                                    rect.Height = height;
                                    this.Stream.WritePixels(
                                        rect,
                                        Enumerable.Repeat(ushort.MaxValue, width * height).ToArray(),
                                        widthstride,
                                        0);
                                }
                                catch (Exception) { }

                            }
                        }
                        
                        this.Performance.Increment();
                    }));
                })
            );
        }
        protected bool CheckBitmapOk(ChannelFrame frame)
        {
            /*var meta = frame.Metadata as VideoChannelFrameMetadata;
            if (this.Stream.PixelHeight != meta.Height) return false;
            if (this.Stream.PixelWidth != meta.Width) return false;
            if (this.Stream.Format != meta.PixelFormat) return false;*//*
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
        */
    }
}
