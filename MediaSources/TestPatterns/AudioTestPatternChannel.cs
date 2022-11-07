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



        public AudioTestPatternChannel(TestPatternSource source)
        {
            this.Device = source;
            this.Name = "Audio Test";
            this.MediaType = MediaType.Audio;
            this.Enabled = true;

            this.Device.Settings.PropertyChanged += Settings_PropertyChanged;

            this._timer = new MultimediaTimer()
            {
                Interval = 10,
                Resolution = 0
            };
            this._timer.Elapsed += (s, e) => this.ProduceFrame();

            this.PrepareSignalGenerator();
            this._timer.Start();
        }

        //private readonly List<string> _audioSettings = new List<string>() { "SampleRate", "NumChannels", "SignalType", "Gain" };
        private void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == null || this.Device.Settings.GetPropertyCategory(e.PropertyName).Equals("Audio"))
            {
                this.PrepareSignalGenerator();
            }
        }

        public TestPatternSource Device { get; protected set; }
        public override ChannelMetadata Metadata => new AudioChannelMetadata()
        {
            TargetFramesPerSecond = this._timer.Interval,
            SampleRate = this.signalGenerator.WaveFormat.SampleRate,
            Channels = this.signalGenerator.WaveFormat.Channels,
            DataType = typeof(float),
            SampleFormat = SampleFormat.IeeeFloat32
        };

        private void PrepareSignalGenerator()
        {
            var config = this.Device.Settings as TestPatternConfig;
            this.signalGenerator = new SignalGenerator(config.SampleRate, config.NumChannels)
            {
                Type = config.SignalType,
                Gain = config.Gain,
                Frequency = config.FrequencyStart,
                FrequencyEnd = config.FrequencyEnd,
                SweepLengthSecs = config.SweepDuration,

            };
            this._numSamples = (this.signalGenerator.WaveFormat.ConvertLatencyToByteSize(this._timer.Interval) * 8) / this.signalGenerator.WaveFormat.BitsPerSample;
        }

        private int _numSamples;
        private readonly MultimediaTimer _timer;
        private int _currentFrameId;
        private float[] _copyBuffer;
        private void ProduceFrame()
        {
            if (!this.Enabled)
                return;

            var meta = new AudioChannelFrameMetadata()
            {
                FrameId = this._currentFrameId++,
                AbsoluteTime = PreciseDatetime.Now,
                TotalBytes = this._numSamples * 4,
                Channels = this.signalGenerator.WaveFormat.Channels,
                SampleRate = this.signalGenerator.WaveFormat.SampleRate,
                SampleCount = this._numSamples
            };

            // Declare an array to hold the bytes
            if (this._copyBuffer == null || this._copyBuffer.Length != this._numSamples)
            {
                this._copyBuffer = new float[this._numSamples];
            }
            var numRead = this.signalGenerator.Read(this._copyBuffer, 0, this._numSamples);

            this.PostFrame(new ChannelFrame(this._copyBuffer, meta));
        }
    }

    
}
