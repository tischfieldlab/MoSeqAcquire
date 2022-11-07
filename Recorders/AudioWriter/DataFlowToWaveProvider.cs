using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoSeqAcquire.Models.Acquisition;
using NAudio.Wave;

namespace AudioWriter
{
    class DataFlowToWaveProvider : IWaveProvider
    {
        private readonly BufferedWaveProvider _buffer;

        public DataFlowToWaveProvider(int sampleRate, int channels)
        {
            this._buffer = new BufferedWaveProvider(WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channels));
        }

        public WaveFormat WaveFormat => this._buffer.WaveFormat;

        public void Write(ChannelFrame frame)
        {
            byte[] data = new byte[frame.Metadata.TotalBytes];
            Buffer.BlockCopy(frame.FrameData, 0, data, 0, data.Length);
            this._buffer.AddSamples(data, 0, data.Length);
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            return this._buffer.Read(buffer, offset, count);
        }
    }
}
