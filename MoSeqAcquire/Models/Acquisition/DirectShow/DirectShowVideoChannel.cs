using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Windows.Media;
using Microsoft.Kinect;

namespace MoSeqAcquire.Models.Acquisition.DirectShow
{
    public class DirectShowVideoChannel : DirectShowChannel
    {
        public DirectShowVideoChannel(DirectShowSource Source) : base(Source)
        {
            this.Name = "Direct Show Video Channel";
            this.Device.Device.NewFrame += this.Device_NewFrame;
            this.MediaType = MediaType.Video;
            this.DataType = typeof(byte);
            //this.Kinect.Config.PropertyChanged += (s,e) => this.RecomputeMetadata();
            //this.RecomputeMetadata();
        }

        

        //public ColorImageStream InnerStream { get { return Kinect.Sensor.ColorStream; } }
        public override bool Enabled
        { get; set;
            
        }
        private byte[] _copyBuffer;
        private void Device_NewFrame(object sender, Accord.Video.NewFrameEventArgs e)
        {
            if (e.Frame != null)
            {
                var frame = e.Frame;
                var meta = new VideoChannelFrameMetadata()
                {
                    //FrameId = imageFrame.FrameNumber,
                    //Timestamp = imageFrame.Timestamp,
                    Width = e.Frame.Width,
                    Height = e.Frame.Height,
                    BytesPerPixel = Image.GetPixelFormatSize(e.Frame.PixelFormat) / 8,
                    PixelFormat = e.Frame.PixelFormat.ToMediaPixelFormat(),
                    //TotalBytes = imageFrame.PixelDataLength * imageFrame.BytesPerPixel
                };
                meta.TotalBytes = meta.Width * meta.Height * meta.BytesPerPixel;

                Rectangle rect = new Rectangle(0, 0, e.Frame.Width, e.Frame.Height);
                System.Drawing.Imaging.BitmapData bmpData = e.Frame.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly, e.Frame.PixelFormat);

                // Declare an array to hold the bytes of the bitmap.
                int bytes = Math.Abs(bmpData.Stride) * e.Frame.Height;
                if(this._copyBuffer == null || this._copyBuffer.Length != bytes)
                {
                    this._copyBuffer = new byte[bytes];
                }
                System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, this._copyBuffer, 0, bytes);


                //e.Frame.RotateFlip(RotateFlipType.Rotate180FlipNone);
                //e.Frame.Save(this._ms, ImageFormat.Bmp);
                
                this.Buffer.Post(new ChannelFrame(this._copyBuffer, meta));
                //this._ms.SetLength(0); //reset the memorystream buffer;
            }
        }
    }
}
