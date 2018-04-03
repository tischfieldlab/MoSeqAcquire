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
        private MemoryStream _ms;
        private void Device_NewFrame(object sender, Accord.Video.NewFrameEventArgs eventArgs)
        {
            if (eventArgs.Frame != null)
            {
                var meta = new VideoChannelFrameMetadata()
                {
                    //FrameId = imageFrame.FrameNumber,
                    //Timestamp = imageFrame.Timestamp,
                    Width = eventArgs.Frame.Width,
                    Height = eventArgs.Frame.Height,
                    BytesPerPixel = Image.GetPixelFormatSize(eventArgs.Frame.PixelFormat) / 8,
                    PixelFormat = eventArgs.Frame.PixelFormat.ToMediaPixelFormat(),
                    //TotalBytes = imageFrame.PixelDataLength * imageFrame.BytesPerPixel
                };
                meta.TotalBytes = meta.Width * meta.Height * meta.BytesPerPixel;

                if(this._ms == null)
                {
                    this._ms = new MemoryStream();
                }
                eventArgs.Frame.RotateFlip(RotateFlipType.Rotate180FlipNone);
                eventArgs.Frame.Save(this._ms, ImageFormat.Bmp);
                
                this.Buffer.Post(new ChannelFrame(this._ms.ToArray(), meta));
                this._ms.SetLength(0); //reset the memorystream buffer;
            }
        }
    }
}
