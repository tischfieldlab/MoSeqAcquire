using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.Models.Acquisition
{
    public enum SampleFormat
    {
        PCM8,
        PCM16,
        PCM32,
        IeeeFloat32,
        IeeeFloat64
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
        public int SampleCount { get; set; }
    }
}
