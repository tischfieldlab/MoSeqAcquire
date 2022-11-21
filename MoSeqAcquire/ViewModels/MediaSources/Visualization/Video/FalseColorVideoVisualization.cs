using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MoSeqAcquire.Models.Acquisition;
using MoSeqAcquire.Views.MediaSources.Visualization;
using SciColorMaps;

namespace MoSeqAcquire.ViewModels.MediaSources.Visualization.Video
{
    public class FalseColorVideoVisualization : IVideoVisualizationPlugin
    {
        private readonly VideoStreamControl videoStreamControl = new VideoStreamControl();
        public string Name => "False Color Video";
        public object Content => videoStreamControl;

        private Int32Rect _rect;
        private ColorMap _cmap;
        public void OnNewFrame(ChannelFrame frame)
        {
            var meta = frame.Metadata as VideoChannelFrameMetadata;
            if (this.videoStreamControl.Stream == null || !this.CheckBitmapOk(frame))
            {
                this._rect = new Int32Rect(0, 0, meta.Width, meta.Height);
                this._cmap = new ColorMap();
                this.videoStreamControl.Stream = new WriteableBitmap(meta.Width, meta.Height, 96, 96, PixelFormats.Rgb24, null);
            }
            this.videoStreamControl.Stream.WritePixels(this._rect, this.ConvertColor(frame.FrameData, this._cmap, 0, 1000), meta.Stride * 3, 0);
        }
        protected bool CheckBitmapOk(ChannelFrame frame)
        {
            var meta = frame.Metadata as VideoChannelFrameMetadata;
            if (this.videoStreamControl.Stream.PixelHeight != meta.Height) return false;
            if (this.videoStreamControl.Stream.PixelWidth != meta.Width) return false;
            if (this.videoStreamControl.Stream.Format != meta.PixelFormat) return false;
            return true;
        }

        protected Array ConvertColor(Array data, ColorMap cmap, int vmin, int vmax)
        {
            Array newData = Array.CreateInstance(typeof(byte), data.Length);
            for (int i = 0; i < data.Length; i++)
            {
                int j = i * 3;
                double v = ((double)data.GetValue(i) - vmin) / vmax;
                System.Drawing.Color c = cmap.GetColor(v);
                newData.SetValue(c.R, j + 0);
                newData.SetValue(c.G, j + 1);
                newData.SetValue(c.B, j + 2);
            }
            return newData;
        }
    }
}
