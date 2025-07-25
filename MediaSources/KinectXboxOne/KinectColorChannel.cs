﻿using Microsoft.Kinect;
using MoSeqAcquire.Models.Utility;
using System.Windows.Media;

namespace MoSeqAcquire.Models.Acquisition.KinectXboxOne
{
    public class KinectColorChannel : KinectChannel
    {
        private ColorFrameReader colorFrameReader;

        public override bool Enabled
        {
            get => !this.colorFrameReader.IsPaused;
            set
            {
                this.colorFrameReader.IsPaused = !value;
                //this.Kinect.Settings.ReadState();
            }
        }

        public KinectColorChannel(KinectManager Kinect) : base(Kinect)
        {
            this.Name = "Color Channel";
            this.DeviceName = "Microsoft Kinect XBOX One";
            this.MediaType = MediaType.Video;

            this.colorFrameReader = this.Kinect.Sensor.ColorFrameSource.OpenReader();
            this.colorFrameReader.FrameArrived += ColorFrameReader_FrameArrivedHandler;
        }

        public override ChannelMetadata Metadata
        {
            get
            {
                return new VideoChannelMetadata()
                {
                    DataType = typeof(byte),
                    Width = colorFrameReader.ColorFrameSource.FrameDescription.Width,
                    Height = colorFrameReader.ColorFrameSource.FrameDescription.Height,
                    TargetFramesPerSecond = 30.0,
                    BytesPerPixel = 4,
                    PixelFormat = PixelFormats.Bgra32
                };
            }
        }

        private byte[] _pixelData;
        private void ColorFrameReader_FrameArrivedHandler(object sender, ColorFrameArrivedEventArgs e)
        {
            using (ColorFrame colorFrame = e.FrameReference.AcquireFrame())
            {
                if (colorFrame != null)
                {
                    var meta = new VideoChannelFrameMetadata()
                    {
                        //Timestamp = colorFrame.RelativeTime.Ticks,
                        AbsoluteTime = PreciseDatetime.Now,
                        Width = colorFrame.FrameDescription.Width,
                        Height = colorFrame.FrameDescription.Height,
                        BytesPerPixel = 4,
                        PixelFormat = PixelFormats.Bgra32,
                        TotalBytes = (int)(4 * colorFrame.FrameDescription.LengthInPixels)
                    };

                    if (this._pixelData == null || this._pixelData.Length != colorFrame.FrameDescription.LengthInPixels)
                    {
                        this._pixelData = new byte[meta.Width * meta.Height * 4];
                    }

                    colorFrame.CopyConvertedFrameDataToArray(_pixelData, ColorImageFormat.Bgra);
                    //colorFrame.CopyRawFrameDataToArray(_pixelData);

                    this.PostFrame(new ChannelFrame(_pixelData, meta));
                }
            }
        }

        public override void Dispose()
        {
            this.colorFrameReader.Dispose();
        }

        internal override void BindConfig()
        {
            KinectConfig cfg = this.Kinect.Settings as KinectConfig;
        }
    }
}