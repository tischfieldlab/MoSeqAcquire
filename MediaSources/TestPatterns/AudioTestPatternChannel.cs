using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MoSeqAcquire.Models.Acquisition;
using MoSeqAcquire.Models.Utility;
using NAudio.Wave.SampleProviders;

namespace TestPatterns
{
    class AudioTestPatternChannel : Channel
    {
        private SignalGenerator signalGenerator;



        public AudioTestPatternChannel()
        {
            this.Name = "Audio Test";
            this.MediaType = MediaType.Audio;
            this.DataType = typeof(float);
            this.Enabled = true;


            this.signalGenerator = new SignalGenerator(44100, 1);

            this.__timer = new MultimediaTimer()
            {
                Interval = 1000 / 30,
                Resolution = 0
            };
            this.__timer.Elapsed += (s, e) => this.ProduceFrame();
            this.__timer.Start();
        }
        public override ChannelMetadata Metadata => new AudioChannelMetadata()
        {
            TargetFramesPerSecond = 30,
            
        };

        private int numSamples = 1000;
        private MultimediaTimer __timer;
        private int currentFrameId;
        private float[] _copyBuffer;
        private void ProduceFrame()
        {
            if (!this.Enabled)
                return;

            var meta = new AudioChannelFrameMetadata()
            {
                FrameId = this.currentFrameId++,
                AbsoluteTime = PreciseDatetime.Now,
                TotalBytes = this.numSamples * 4,
            };

            // Declare an array to hold the bytes
            if (this._copyBuffer == null || this._copyBuffer.Length != this.numSamples)
            {
                this._copyBuffer = new float[meta.TotalBytes];
            }
            this.signalGenerator.Read(this._copyBuffer, 0, this.numSamples);

            this.PostFrame(new ChannelFrame(this._copyBuffer, meta));
        }
    }

    
}
