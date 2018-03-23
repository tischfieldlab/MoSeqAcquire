using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using MoSeqAcquire.Models.Acquisition;

namespace MoSeqAcquire.Models.IO.RawDataWriter
{
    public class RawDataSink : MediaWriterSink
    {

        protected FileStream file;
        protected BinaryWriter writer;

        public RawDataSink(RecorderSettings settings, Channel channel) : base(settings, channel)
        {

        }
        public string FilePath
        {
            get
            {
                return Path.Combine(this.settings.Directory, this.settings.Basename + "." + this.channel.Name + "." + this.Ext);
            }
        }
        public string Ext
        {
            get
            {
                if (this.channel.DataType == typeof(short))
                {
                    return "short";
                }
                else if (this.channel.DataType == typeof(byte))
                {
                    return "byte";
                }
                else
                {
                    return "unknown";
                }
            }
        }
        public override void Close()
        {
            this.back_buffer.Complete();
            this.writer.Close();
        }

        public override void Open()
        {
            this.file = File.Open(this.FilePath, FileMode.Create);
            this.writer = new BinaryWriter(this.file);
            this.AttachSink(channel);
        }
        protected override ActionBlock<ChannelFrame> GetActionBlock(Type type)
        {
            if (type == typeof(short))
            {
                return new ActionBlock<ChannelFrame>(frame =>
                {
                    if (!this.IsRecording) { return; }
                    var d = frame.FrameData as short[];
                    for (var i = 0; i < d.Length; i++)
                    {
                        this.writer.Write(d[i]);
                    }
                });
            }
            else if (type == typeof(byte))
            {
                return new ActionBlock<ChannelFrame>(frame => { if (!this.IsRecording) { return; } this.writer.Write(frame.FrameData as byte[]); });
            }
            return null;
        }
    }
}
