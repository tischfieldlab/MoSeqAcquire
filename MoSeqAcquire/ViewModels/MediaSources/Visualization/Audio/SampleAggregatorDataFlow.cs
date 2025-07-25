﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using MoSeqAcquire.Models.Acquisition;
using NAudio.Dsp;
using NAudio.Wave;

namespace MoSeqAcquire.ViewModels.MediaSources.Visualization.Audio
{
    public class SampleAggregatorDataFlow
    {
        // FFT
        private readonly Complex[] fftBuffer;
        private int fftPos;
        private readonly int fftLength;
        private readonly int m;

        private float maxValue;
        private float minValue;


        public SampleAggregatorDataFlow(int fftLength = 1024)
        {
            if (!IsPowerOfTwo(fftLength))
            {
                throw new ArgumentException("FFT Length must be a power of two");
            }
            this.m = (int)Math.Log(fftLength, 2.0);
            this.fftLength = fftLength;
            this.fftBuffer = new Complex[fftLength];
        }

        static bool IsPowerOfTwo(int x)
        {
            return (x & (x - 1)) == 0;
        }


        public void Reset()
        {
            //count = 0;
            maxValue = minValue = 0;
        }

        private SampleData? Compute(float value)
        {
            fftBuffer[fftPos].X = (float)(value * FastFourierTransform.HammingWindow(fftPos, fftLength));
            fftBuffer[fftPos].Y = 0;
            fftPos++;

            maxValue = Math.Max(maxValue, value);
            minValue = Math.Min(minValue, value);

            if (fftPos >= fftBuffer.Length)
            {
                fftPos = 0;
                // 1024 = 2^10
                FastFourierTransform.FFT(true, m, fftBuffer);

                var sample = new SampleData()
                {
                    MinSample = minValue,
                    MaxSample = maxValue,
                    Fft = (Complex[])fftBuffer.Clone()
                };
                Reset();
                return sample;
            }

            return null;
        }

        public IEnumerable<SampleData> ProduceSample(ChannelFrame frame)
        {
            var meta = frame.Metadata as AudioChannelFrameMetadata;
            for (int n = 0; n < frame.FrameData.Length; n += meta.Channels)
            {
                var result = Compute(((float[]) frame.FrameData)[n]);
                if (result.HasValue)
                {
                    yield return result.Value;
                }
            }
        }
    }

    public struct SampleData
    {
        public float MaxSample { get; set; }
        public float MinSample { get; set; }
        public Complex[] Fft { get; set; }
    }
}
