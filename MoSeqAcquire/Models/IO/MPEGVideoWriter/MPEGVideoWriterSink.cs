using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Accord.Video.FFMPEG;
using MoSeqAcquire.Models.Acquisition;

namespace MoSeqAcquire.Models.IO.MPEGVideoWriter
{
    /*public class MPEGVideoWriterSink : MediaWriterSink
    {
        protected VideoFileWriter writer;
        public MPEGVideoWriterSink(RecorderSettings settings, Channel channel) : base(settings, channel)
        {

        }
        public string FilePath
        {
            get
            {
                return "";//Path.Combine(this.settings.Directory, this.settings.Basename + "." + this.channel.Name + ".mp4");
            }
        }
        public override void Close()
        {
            this.sink.Complete();
            this.sink.Completion.Wait();
            this.writer.Close();
        }
        public override void Open()
        {
            this.writer = new VideoFileWriter();
            this.AttachSink(channel);
            
        }

        protected override ActionBlock<ChannelFrame> GetActionBlock(Type type)
        {
            return new ActionBlock<ChannelFrame>(frame =>
            {
                if (!this.writer.IsOpen)
                {
                    this.writer.Open(this.FilePath, frame.Metadata.Width, frame.Metadata.Height);
                }
                unsafe
                {
                    byte[] data = (byte[])frame.FrameData;
                    fixed (byte* first = &data[0])
                    {
                        Bitmap bmp = new Bitmap(frame.Metadata.Width,
                                                frame.Metadata.Height,
                                                frame.Metadata.BytesPerPixel * frame.Metadata.Width,
                                                frame.Metadata.PixelFormat.ToDrawingPixelFormat(),
                                                (IntPtr)first);
                        this.writer.WriteVideoFrame(bmp);
                    }
                }
            });
        }
    }*/
}
