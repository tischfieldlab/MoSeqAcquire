using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.Models
{
    public static class DrawingExtensions
    {
        public static System.Windows.Media.Color ToWinMediaColor(this System.Drawing.Color originalColor)
        {
            return System.Windows.Media.Color.FromArgb(originalColor.A, originalColor.R, originalColor.G, originalColor.B);
        }

        public static System.Drawing.Color ToDrawingColor(this System.Windows.Media.Color originalColor)
        {
            return System.Drawing.Color.FromArgb(originalColor.A, originalColor.R, originalColor.G, originalColor.B);
        }

        public static System.Drawing.Imaging.PixelFormat ToDrawingPixelFormat(this System.Windows.Media.PixelFormat pixelFormat)
        {
            if(pixelFormat == System.Windows.Media.PixelFormats.Bgr24)
                return System.Drawing.Imaging.PixelFormat.Format24bppRgb;
            if (pixelFormat == System.Windows.Media.PixelFormats.Bgr32)
                return System.Drawing.Imaging.PixelFormat.Format32bppRgb;

            throw new NotSupportedException("Could not convert pixel format "+pixelFormat.ToString());
        }
    }
}
