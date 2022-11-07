using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using MoSeqAcquire.Models.Acquisition;
using MoSeqAcquire.Views.MediaSources.Visualization;

namespace MoSeqAcquire.ViewModels.MediaSources.Visualization.Video
{
    public class ColorVideoVisualization : IVideoVisualizationPlugin
    {
        private readonly VideoStreamControl videoStreamControl = new VideoStreamControl();
        public string Name => "Color Video";
        public object Content => videoStreamControl;

        private Int32Rect _rect;
        public void OnNewFrame(ChannelFrame frame)
        {
            var meta = frame.Metadata as VideoChannelFrameMetadata;
            if (this.videoStreamControl.Stream == null || !this.CheckBitmapOk(frame))
            {
                this._rect = new Int32Rect(0, 0, meta.Width, meta.Height);
                this.videoStreamControl.Stream = new WriteableBitmap(meta.Width, meta.Height, 96, 96, meta.PixelFormat, null);
            }
            this.videoStreamControl.Stream.WritePixels(this._rect, frame.FrameData, meta.Stride, 0);
        }
        protected bool CheckBitmapOk(ChannelFrame frame)
        {
            var meta = frame.Metadata as VideoChannelFrameMetadata;
            if (this.videoStreamControl.Stream.PixelHeight != meta.Height) return false;
            if (this.videoStreamControl.Stream.PixelWidth != meta.Width) return false;
            if (this.videoStreamControl.Stream.Format != meta.PixelFormat) return false;
            return true;
        }
    }
}
