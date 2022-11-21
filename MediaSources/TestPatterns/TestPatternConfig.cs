using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoSeqAcquire.Models.Acquisition;
using NAudio.Wave.SampleProviders;

namespace TestPatterns
{
    class TestPatternConfig : MediaSourceConfig
    {
        [DisplayName("Frame Rate")]
        [DefaultValue(30)]
        [Category("Video")]
        public int FrameRate
        {
            get => this._frameRate;
            set => this.SetField(ref this._frameRate, value);
        }
        private int _frameRate;

        [DisplayName("Sample Rate")]
        [DefaultValue(44100)]
        [Category("Audio")]
        public int SampleRate
        {
            get => this._sampleRate;
            set => this.SetField(ref this._sampleRate, value);
        }
        private int _sampleRate;

        [DisplayName("Number of Channels")]
        [DefaultValue(1)]
        [Range(1, 100)]
        [Category("Audio")]
        public int NumChannels
        {
            get => this._numChannels;
            set => this.SetField(ref this._numChannels, value);
        }
        private int _numChannels;

        [DisplayName("Type of Signal")]
        [DefaultValue(SignalGeneratorType.Sweep)]
        [Category("Audio")]
        public SignalGeneratorType SignalType
        {
            get => this._signalType;
            set => this.SetField(ref this._signalType, value);
        }
        private SignalGeneratorType _signalType;

        [DisplayName("Gain")]
        [DefaultValue(0.5)]
        [Range(0, 1)]
        [Category("Audio")]
        public double Gain
        {
            get => this._gain;
            set => this.SetField(ref this._gain, value);
        }
        private double _gain;

        [DisplayName("Start Frequency")]
        [DefaultValue(440.0)]
        [Range(20.0, 20000.0)]
        [Category("Audio")]
        public double FrequencyStart
        {
            get => this._startFrequency;
            set => this.SetField(ref this._startFrequency, value);
        }
        private double _startFrequency;

        [DisplayName("End Frequency")]
        [DefaultValue(10000.0)]
        [Range(20.0, 20000.0)]
        [Category("Audio")]
        public double FrequencyEnd
        {
            get => this._endFrequency;
            set => this.SetField(ref this._endFrequency, value);
        }
        private double _endFrequency;

        [DisplayName("Sweep Duration")]
        [DefaultValue(2.0)]
        [Range(0.1, 10.0)]
        [Category("Audio")]
        public double SweepDuration
        {
            get => this._sweepDuration;
            set => this.SetField(ref this._sweepDuration, value);
        }
        private double _sweepDuration;


        [DisplayName("Data Mode")]
        [DefaultValue(TestDataMode.Constant)]
        [Category("Data")]
        public TestDataMode DataMode
        {
            get => this._dataMode;
            set => this.SetField(ref this._dataMode, value);
        }
        private TestDataMode _dataMode;

        [DisplayName("Sample Size")]
        [DefaultValue(16)]
        [Range(1, 512)]
        [Category("Data")]
        public int SampleSize
        {
            get => this._sampleSize;
            set => this.SetField(ref this._sampleSize, value);
        }
        private int _sampleSize;

        [DisplayName("Sample Distribution")]
        [DefaultValue(DataDistribution.ContinuousUniform)]
        [Category("Data")]
        public DataDistribution Distribution
        {
            get => this._distribution;
            set => this.SetField(ref this._distribution, value);
        }
        private DataDistribution _distribution;
    }
}
