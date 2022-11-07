using Microsoft.Azure.Kinect.Sensor;
using System.Windows.Media;

namespace MoSeqAcquire.Models.Acquisition.KinectAzure
{
    public abstract class KinectChannel : Channel
    {
        public KinectManager Kinect { get; protected set; }

        public KinectChannel(KinectManager Kinect)
        {
            this.Kinect = Kinect;
        }

        public abstract void Dispose();

        internal abstract void BindConfig();

        internal abstract void RecieveFrame(Capture capture);

        protected static int ImageFormatToBytesPerPixel(ImageFormat format)
        {
            switch (format)
            {
                case ImageFormat.ColorBGRA32:
                    return 4;
                
                case ImageFormat.IR16:
                case ImageFormat.Depth16:
                case ImageFormat.Custom16:
                    return 2;

                case ImageFormat.Custom8:
                    return 1;

                case ImageFormat.ColorMJPG:
                case ImageFormat.ColorNV12:
                case ImageFormat.ColorYUY2:
                default:
                    return 0;
            }
        }
        protected static PixelFormat ImageFormatToPixelFormat(ImageFormat format)
        {
            switch (format)
            {
                case ImageFormat.ColorBGRA32:
                    return PixelFormats.Bgra32;

                case ImageFormat.IR16:
                case ImageFormat.Depth16:
                case ImageFormat.Custom16:
                    return PixelFormats.Gray16;

                case ImageFormat.Custom8:
                    return PixelFormats.Gray8;

                case ImageFormat.ColorMJPG:
                case ImageFormat.ColorNV12:
                case ImageFormat.ColorYUY2:
                default:
                    return PixelFormats.Default;
            }
        }
        protected static int FPStoFPS(FPS fps)
        {
            switch (fps)
            {
                case FPS.FPS5:
                    return 5;
                case FPS.FPS15:
                    return 15;
                case FPS.FPS30:
                    return 30;
                default:
                    return 0;
            }
        }
    }
}