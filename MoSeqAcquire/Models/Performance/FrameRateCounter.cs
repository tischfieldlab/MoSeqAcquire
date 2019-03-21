using MoSeqAcquire.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace MoSeqAcquire.Models.Performance
{
    public class FrameRateCounter : BaseViewModel, IFrameRateProvider
    {
        private DateTime __lastTime;
        private readonly Timer __timer;
        private long __countSinceLast;
        private readonly object lockobject = new object();

        public FrameRateCounter(double Interval = 1000)
        {
            this.__lastTime = DateTime.Now;
            this.__timer = new Timer()
            {
                Interval = Interval,
                AutoReset = true,
                Enabled = true
            };
            this.__timer.Elapsed += this.Compute_framerate;
        }
        private void Compute_framerate(object sender, ElapsedEventArgs e)
        {
            this.FrameRate = this.__countSinceLast / (e.SignalTime - this.__lastTime).TotalSeconds;
            this.__countSinceLast = 0;
            this.__lastTime = e.SignalTime;
            this.NotifyPropertyChanged("FrameRate");
        }

        public void Increment()
        {
            lock (lockobject)
            {
                this.__countSinceLast++;
            }
        }
        public double FrameRate { get; protected set; }
    }
}
