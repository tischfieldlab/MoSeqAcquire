using MoSeqAcquire.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace MoSeqAcquire.Models.Performance
{
    public interface IFrameRateProvider
    {
        FrameRateCounter FrameRate { get; }
    }
    public class FrameRateCounter : BaseViewModel
    {
        private DateTime __lastTime;
        private Timer __timer;
        private long __totalCount;
        private static readonly object lockobject = new object();

        public FrameRateCounter()
        {
            this.__lastTime = DateTime.Now;
            this.__timer = new Timer();
            this.__timer.Interval = 1000;
            this.__timer.Elapsed += compute_framerate;
            this.__timer.AutoReset = true;
            this.__timer.Enabled = true;
        }

        private void compute_framerate(object sender, ElapsedEventArgs e)
        {
            var seconds = (e.SignalTime - this.__lastTime).TotalSeconds;
            this.FrameRate = this.__totalCount / seconds;
            this.__totalCount = 0;
            this.__lastTime = e.SignalTime;
            this.NotifyPropertyChanged("FrameRate");
        }

        public void Increment()
        {
            lock (lockobject)
            {
                this.__totalCount++;
            }
        }
        public double FrameRate { get; protected set; }

    }
}
