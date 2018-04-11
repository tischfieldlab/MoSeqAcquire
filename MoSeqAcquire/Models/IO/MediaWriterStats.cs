using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using MoSeqAcquire.Models.Performance;

namespace MoSeqAcquire.Models.Recording
{
    public class MediaWriterStats : ObservableObject, IFrameRateProvider, ITotalFrameCountProvider, IDurationProvider
    {
        private DateTime __started;
        private DateTime __lastTime;
        private Timer __timer;
        private long __totalCount;
        private long __countSinceLast;
        private static readonly object lockobject = new object();

        public MediaWriterStats(double Interval=1000)
        {            
            this.__timer = new Timer()
            {
                Interval = Interval,
                AutoReset = true
            };
            this.__timer.Elapsed += this.compute_framerate;
        }
        public void Start()
        {
            this.__started = this.__lastTime = DateTime.Now;
            this.__timer.Enabled = true;
        }
        public void Stop()
        {
            this.__timer.Enabled = false;
        }

        private void compute_framerate(object sender, ElapsedEventArgs e)
        {
            lock (lockobject)
            {
                var seconds = (e.SignalTime - this.__lastTime).TotalSeconds;
                this.FrameRate = this.__countSinceLast / seconds;
                this.__totalCount += this.__countSinceLast;
                this.__countSinceLast = 0;
                this.__lastTime = e.SignalTime;
                this.NotifyPropertyChanged(null);
            }
        }

        public void Increment()
        {
            lock (lockobject)
            {
                this.__countSinceLast++;
            }
        }
        public double FrameRate { get; protected set; }
        public long TotalFrames { get => this.__totalCount; }
        public TimeSpan Duration { get => DateTime.Now - this.__started; }
    }
}
