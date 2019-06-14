using System;
using System.Collections.Generic;
using System.Diagnostics;
using MoSeqAcquire.Models.Acquisition;
using NAudio.Dsp;
using NAudio.Wave;

namespace MoSeqAcquire.ViewModels.MediaSources.Visualization.Audio
{
    public class SampleAggregatorDataFlow
    {
        // volume
        //public event EventHandler<MaxSampleEventArgs> MaximumCalculated;
        private float maxValue;
        private float minValue;
        public int NotificationCount { get; set; }
        int count;

        // FFT
        //public event EventHandler<FftEventArgs> FftCalculated;
        public bool PerformFFT { get; set; }
        private readonly Complex[] fftBuffer;
        private readonly FftEventArgs fftArgs;
        private int fftPos;
        private readonly int fftLength;
        private readonly int m;
        //private readonly ISampleProvider source;

        private readonly int channels;

        public SampleAggregatorDataFlow(int fftLength = 1024)
        {
            if (!IsPowerOfTwo(fftLength))
            {
                throw new ArgumentException("FFT Length must be a power of two");
            }
            m = (int)Math.Log(fftLength, 2.0);
            this.fftLength = fftLength;
            fftBuffer = new Complex[fftLength];
            fftArgs = new FftEventArgs(fftBuffer);
        }

        static bool IsPowerOfTwo(int x)
        {
            return (x & (x - 1)) == 0;
        }


        public void Reset()
        {
            count = 0;
            maxValue = minValue = 0;
        }

        private SampleData Add(float value)
        {
            fftBuffer[fftPos].X = (float)(value * FastFourierTransform.HammingWindow(fftPos, fftLength));
            fftBuffer[fftPos].Y = 0;
            fftPos++;
            if (fftPos >= fftBuffer.Length)
            {
                fftPos = 0;
                // 1024 = 2^10
                FastFourierTransform.FFT(true, m, fftBuffer);
            }

            maxValue = Math.Max(maxValue, value);
            minValue = Math.Min(minValue, value);

            var sample = new SampleData()
            {
                MinSample = minValue,
                MaxSample = maxValue,
                Fft = fftBuffer
            };
            Reset();
            return sample;
        }

        public IEnumerable<SampleData> ProduceSample(ChannelFrame frame)
        {
            var meta = frame.Metadata as AudioChannelFrameMetadata;
            var samples = new List<SampleData>();
            for (int n = 0; n < frame.FrameData.Length; n += 1)
            {
                samples.Add(Add(Convert.ToSingle(((byte[])frame.FrameData)[n])));
            }
            return samples;
        }
    }

    public class SampleData
    {
        public float MaxSample { get; set; }
        public float MinSample { get; set; }
        public Complex[] Fft { get; set; }
    }
}
