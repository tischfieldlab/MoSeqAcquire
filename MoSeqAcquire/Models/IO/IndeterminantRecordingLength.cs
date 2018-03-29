using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace MoSeqAcquire.Models.IO
{
    public class IndeterminantRecordingLength : ObservableObject, IRecordingLengthStrategy
    {
        protected Timer timer;
        protected DateTime startTime;

        public event EventHandler TriggerStop;

        public IndeterminantRecordingLength()
        {
            this.timer = new Timer()
            {
                Interval = 100, //100 milliseconds
                AutoReset = true
            };
            this.timer.Elapsed += this.check_condition;
        }
        public void Start()
        {
            this.startTime = DateTime.UtcNow;
            this.timer.Enabled = true;
        }
        public void Stop()
        {
            this.timer.Enabled = false;
        }

        private void check_condition(object sender, ElapsedEventArgs e)
        {
            this.NotifyPropertyChanged(null);
        }

        public TimeSpan Duration { get => DateTime.UtcNow - this.startTime; }

        public double? Progress { get => null; }

        public TimeSpan? TimeRemaining { get => null; }
    }
}
