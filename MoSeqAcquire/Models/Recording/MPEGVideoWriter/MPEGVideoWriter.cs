using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks.Dataflow;
using Accord.Video.FFMPEG;
using MoSeqAcquire.Models.Acquisition;
using MoSeqAcquire.Models.Attributes;

namespace MoSeqAcquire.Models.Recording.MPEGVideoWriter
{
    [KnownType(typeof(MPEGVideoWriterSettings))]
    [DisplayName("MPEG Video Writer")]
    [SettingsImplementation(typeof(MPEGVideoWriterSettings))]
    [SupportedChannelType(MediaType.Video, ChannelCapacity.Single)]
    [SupportedChannelType(MediaType.Audio, ChannelCapacity.Single)]
    public class MPEGVideoWriter : MediaWriter
    {
        private object lockobject = new object();
        protected VideoFileWriter writer;

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
            this.writer = new VideoFileWriter();
            var vChanMeta = this.videoPin.Channel.Metadata as VideoChannelMetadata;
            var conf = this.Settings as MPEGVideoWriterSettings;
            this.writer.Open(this.FilePath);
            this.writer.Width = vChanMeta.Width;
            this.writer.Height = vChanMeta.Height;
            this.writer.FrameRate = new Accord.Math.Rational(30);
            this.writer.VideoCodec = conf.VideoCodec;
            this.writer.BitRate = conf.VideoBitrate;
            //conf.AudioCodec, conf.AudioBitrate, 16000, 1);

            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
            this.writer.Flush();
            this.writer.Close();
        }
        




        protected ActionBlock<ChannelFrame> GetAudioActionBlock()
        {
            return new ActionBlock<ChannelFrame>(frame =>
            {
                if (this.IsRecording)
                {
                    lock (this.lockobject)
                    {
                        //this.writer.WriteAudioFrame((byte[])frame.FrameData);
                    }
                }
            });
        }
        protected ActionBlock<ChannelFrame> GetVideoActionBlock()
        {
            return new ActionBlock<ChannelFrame>(frame =>
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
                                this.writer.WriteVideoFrame(bmp);
                            }
                        }
                    }
                    this.Stats.Increment();
                }
            });
        }
    }
}
