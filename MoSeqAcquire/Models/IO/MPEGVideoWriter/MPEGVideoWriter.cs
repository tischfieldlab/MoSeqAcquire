using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Accord.Video;
using Accord.Video.FFMPEG;
using MoSeqAcquire.Models.Acquisition;
using MoSeqAcquire.Models.Attributes;

namespace MoSeqAcquire.Models.Recording.MPEGVideoWriter
{
    [KnownType(typeof(MPEGVideoWriterSettings))]
    [DisplayName("MPEG Video Writer")]
    [SettingsImplementation(typeof(MPEGVideoWriterSettings))]
    [SupportedChannelType(MediaType.Video, 1)]
    [SupportedChannelType(MediaType.Audio, 1)]
    public class MPEGVideoWriter : MediaWriter
    {
        private object lockobject = new object();
        protected VideoFileWriter writer;

        protected Channel video_channel;
        protected BufferBlock<ChannelFrame> video_back_buffer;
        protected ActionBlock<ChannelFrame> video_sink;

        protected Channel audio_channel;
        protected BufferBlock<ChannelFrame> audio_back_buffer;
        protected ActionBlock<ChannelFrame> audio_sink;


        public override void ConnectChannel(Channel Channel)
        {
            if (Channel.MediaType == MediaType.Video)
            {
                if(this.video_channel != null)
                {
                    throw new InvalidOperationException("Video channel was already connected!");
                }
                this.video_channel = Channel;
                this.video_back_buffer = new BufferBlock<ChannelFrame>(new DataflowBlockOptions() { EnsureOrdered = true });
                this.video_sink = this.GetVideoActionBlock(Channel.DataType);
                MediaBus.Instance.Subscribe(bc => bc.Channel == Channel, this.video_back_buffer);
                this.video_back_buffer.LinkTo(this.video_sink, new DataflowLinkOptions() { PropagateCompletion = true });
            }
            else if(Channel.MediaType == MediaType.Audio)
            {
                if (this.audio_channel != null)
                {
                    throw new InvalidOperationException("Audio channel was already connected!");
                }
                this.audio_channel = Channel;
                this.audio_back_buffer = new BufferBlock<ChannelFrame>(new DataflowBlockOptions() { EnsureOrdered = true });
                this.audio_sink = this.GetAudioActionBlock(Channel.DataType);
                MediaBus.Instance.Subscribe(bc => bc.Channel == Channel, this.audio_back_buffer);
                this.audio_back_buffer.LinkTo(this.audio_sink, new DataflowLinkOptions() { PropagateCompletion = true });
            }
        }
        protected override string Ext
        {
            get => "mp4";
        }

        public override void Start()
        {
            this.writer = new VideoFileWriter();
            var vChanMeta = this.video_channel.Metadata as VideoChannelMetadata;
            var conf = this.Settings as MPEGVideoWriterSettings;
            this.writer.Open(this.FilePath, vChanMeta.Width, vChanMeta.Height, 
                             new Accord.Math.Rational(30), conf.VideoCodec, conf.VideoBitrate);
                            //conf.AudioCodec, conf.AudioBitrate, 16000, 1);
            this.IsRecording = true;
            this.Stats.Start();
        }

        public override void Stop()
        {
            this.Stats.Stop();
            this.video_sink.Complete();
            this.audio_sink.Complete();
            this.video_sink.Completion.Wait();
            this.audio_sink.Completion.Wait();
            this.IsRecording = false;
            this.writer.Close();
        }
        public override IDictionary<string, IList<Channel>> GetChannelFileMap()
        {
            return new Dictionary<string, IList<Channel>>()
            {
                { this.FilePath, new List<Channel>(){ this.video_channel, this.audio_channel } }
            };
        }




        protected ActionBlock<ChannelFrame> GetAudioActionBlock(Type type)
        {
            return new ActionBlock<ChannelFrame>(frame =>
            {
                if (this.IsRecording)
                {
                    lock (this.lockobject)
                    {
                        this.writer.WriteAudioFrame((byte[])frame.FrameData);
                    }
                }
            });
        }
        protected ActionBlock<ChannelFrame> GetVideoActionBlock(Type type)
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
