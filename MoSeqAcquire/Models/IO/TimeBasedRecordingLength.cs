using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace MoSeqAcquire.Models.IO
{
    public class TimeBasedRecordingLength : ObservableObject, IRecordingLengthStrategy
    {
        protected Timer timer;
        protected TimeSpan targetLength;
        protected DateTime startTime;
        protected DateTime endTime;

        public event EventHandler TriggerStop;

        public TimeBasedRecordingLength(TimeSpan recordingLength)
        {
            this.targetLength = recordingLength;
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
            this.endTime = this.startTime.Add(this.targetLength);
            this.timer.Enabled = true;
        }
        public void Stop()
        {
            this.timer.Enabled = false;
        }

        private void check_condition(object sender, ElapsedEventArgs e)
        {
            if (this.endTime <= DateTime.UtcNow)
            {
                this.TriggerStop?.Invoke(this, e);
            }
            this.NotifyPropertyChanged(null);
        }

        public TimeSpan Duration { get => DateTime.UtcNow - this.startTime; }

        public double? Progress { get => this.Duration.TotalSeconds / this.targetLength.TotalSeconds; }

        public TimeSpan? TimeRemaining { get => this.targetLength - this.Duration; }
    }
}
