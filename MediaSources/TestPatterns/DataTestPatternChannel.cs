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
using MoSeqAcquire.Models.Utility.Random;

namespace TestPatterns
{
    public enum TestDataMode
    {
        Constant,
        Random
    }
    class DataTestPatternChannel : Channel
    {
        public DataTestPatternChannel(TestPatternSource source) : base()
        {
            this.Device = source;
            this.Name = "Data Test";
            this.MediaType = MediaType.Data;
            this.Enabled = true;

            this._config = source.Settings as TestPatternConfig;
           
            this.__timer = new MultimediaTimer()
            {
                Interval = 1000 / 30,
                Resolution = 0
            };
            this.__timer.Elapsed += (s, e) => this.ProduceFrame();
            this.__timer.Start();
        }
        public TestPatternSource Device { get; protected set; }
        public override ChannelMetadata Metadata => new ChannelMetadata()
        {
            DataType = typeof(float),
            TargetFramesPerSecond = 30,
        };

        
        private MultimediaTimer __timer;
        private int _currentFrameId;
        private Distribution _random;
        private TestPatternConfig _config;
        private void ProduceFrame()
        {
            if (!this.Enabled)
                return;


            var meta = new DataChannelFrameMetadata()
            {
                FrameId = this._currentFrameId++,
                AbsoluteTime = PreciseDatetime.Now
            };

            float[] samples = new float[this._config.SampleSize];
            switch (this._config.DataMode)
            {
                case TestDataMode.Constant:
                    for (int i=0; i < samples.Length; i++)
                    {
                        samples[i] = i;
                    }
                    break;

                case TestDataMode.Random:
                    for (int i = 0; i < samples.Length; i++)
                    {
                        samples[i] = (float)this.NextSample();
                    }
                    break;
            }
            
            this.PostFrame(new ChannelFrame(samples, meta));
        }
        private double NextSample()
        {
            if(this._random == null || !this._random.GetType().Name.Replace("Distribution", "").Equals(this._config.Distribution.ToString()))
            {
                string dname = $"MoSeqAcquire.Models.Utility.Random.{this._config.Distribution}Distribution, MoSeqAcquire";
                this._random = (Distribution)Activator.CreateInstance(Type.GetType(dname));
            }
            return this._random.NextDouble();
        }
    }

    public enum DataDistribution
    {
        Weibull,
        Beta,
        BetaPrime,
        Cauchy,
        Chi,
        ChiSquare,
        ContinuousUniform,
        Erlang,
        Exponential,
        FisherSnedecor,
        FisherTippett,
        Gamma,
        Laplace,
        Lognormal,
        Normal,
        Pareto,
        Power,
        Rayleigh,
        StudentsT,
        Triangular

    }


}
