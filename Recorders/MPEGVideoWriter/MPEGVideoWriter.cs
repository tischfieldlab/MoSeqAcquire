using System;
using System.ComponentModel;
using System.Drawing;
using Accord.Video.FFMPEG;
using MoSeqAcquire.Models.Acquisition;
using MoSeqAcquire.Models.Attributes;
using MoSeqAcquire.Models.Utility;

namespace MoSeqAcquire.Models.Recording.MPEGVideoWriter
{
    [KnownType(typeof(MPEGVideoWriterSettings))]
    [DisplayName("MPEG Video Writer")]
    [SettingsImplementation(typeof(MPEGVideoWriterSettings))]
    [SupportedChannelType(MediaType.Video, ChannelCapacity.Single)]
    [SupportedChannelType(MediaType.Audio, ChannelCapacity.Single)]
    public class MPEGVideoWriter : MediaWriter
    {
        private readonly object lockobject = new object();
        protected VideoFileWriter writer;
        protected TimestampCoWriter tsWriter;

        protected MediaWriterPin videoPin;
        protected MediaWriterPin audioPin;

        public MPEGVideoWriter() : base()
        {
            this.videoPin = new MediaWriterPin(MediaType.Video, ChannelCapacity.Single, this.GetVideoActionBlock);
            this.RegisterPin(this.videoPin);

            this.audioPin = new MediaWriterPin(MediaType.Audio, ChannelCapacity.Single, this.GetAudioActionBlock);
            this.RegisterPin(this.audioPin);
        }
        protected override string Ext { get => "mp4"; }

        public override void Start()
        {
            var vChanMeta = this.videoPin.Channel.Metadata as VideoChannelMetadata;
            var conf = this.Settings as MPEGVideoWriterSettings;

            this.writer = new VideoFileWriter();
            this.writer.Open(this.FilePath, vChanMeta.Width, vChanMeta.Height, new Accord.Math.Rational(30), conf.VideoCodec, conf.VideoBitrate);
            //conf.AudioCodec, conf.AudioBitrate, 16000, 1);

            if (conf.WriteTimestamps)
            {
                this.tsWriter = new TimestampCoWriter(this.FormatFilePath("{0}_ts.txt"));
                this.tsWriter.Open();
            }

            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
            this.writer.Flush();
            this.writer.Close();

            if (this.tsWriter != null)
            {
                this.tsWriter.Close();
                this.tsWriter = null;
            }
        }
        




        protected void GetAudioActionBlock(ChannelFrame frame)
        {
            if (this.IsRecording)
            {
                lock (this.lockobject)
                {
                    //this.writer.WriteAudioFrame((byte[])frame.FrameData);
                }
            }
        }
        protected void GetVideoActionBlock(ChannelFrame frame)
        {
            if (this.IsRecording)
            {
                unsafe
                {
                    byte[] data = (byte[])frame.FrameData;
                    var meta = frame.Metadata as VideoChannelFrameMetadata;
                    fixed (byte* first = &data[0])
                    {
                        Bitmap bmp = new Bitmap(meta.Width,
                                                meta.Height,
                                                meta.BytesPerPixel * meta.Width,
                                                meta.PixelFormat.ToDrawingPixelFormat(),
                                                (IntPtr)first);
                        lock (this.lockobject)
                        {
                            try
                            {
                                if (this.Performance.TotalFrames > 0)
                                {
                                    this.writer.WriteVideoFrame(bmp, TimeSpan.FromMilliseconds(meta.AbsoluteTime.Subtract(this.Epoch).TotalMilliseconds));
                                }
                                else
                                {
                                    this.writer.WriteVideoFrame(bmp);
                                }

                                if (this.tsWriter != null)
                                {
                                    this.tsWriter.Write(frame.Metadata.AbsoluteTime);
                                }
                            }
                            catch
                            {
                                Console.WriteLine("missed frame");
                            }
                            this.Performance.Increment();
                        }
                    }
                }
            }
        }
    }
}
