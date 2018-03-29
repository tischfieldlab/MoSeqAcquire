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
        private Timer __timer;
        private long __countSinceLast;
        private object lockobject = new object();

        public FrameRateCounter()
        {
            this.__lastTime = DateTime.Now;
            this.__timer = new Timer()
            {
                Interval = 1000,
                AutoReset = true,
                Enabled = true
            };
            this.__timer.Elapsed += this.compute_framerate;
        }

        private void compute_framerate(object sender, ElapsedEventArgs e)
        {
            var seconds = (e.SignalTime - this.__lastTime).TotalSeconds;
            this.FrameRate = this.__countSinceLast / seconds;
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
