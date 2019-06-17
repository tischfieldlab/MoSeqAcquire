using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.Models.Acquisition
{
    public enum SampleFormat
    {

    }

    public class AudioChannelMetadata : ChannelMetadata
    {
        public SampleFormat SampleFormat { get; set; }
        public int Channels { get; set; }
        public int SampleRate { get; set; }
    }


    public class AudioChannelFrameMetadata : ChannelFrameMetadata
    {
        public int Channels { get; set; }
        public int SampleRate { get; set; }
        public int BitsPerSample { get; set; }
    }
}
