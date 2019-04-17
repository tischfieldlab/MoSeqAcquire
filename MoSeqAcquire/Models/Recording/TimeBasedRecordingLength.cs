using MoSeqAcquire.Models.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace MoSeqAcquire.Models.Recording
{
    public class TimeBasedRecordingLength : ObservableObject, IRecordingLengthStrategy
    {
        protected Timer timer;
        protected TimeSpan targetLength;
        protected DateTime startTime;
        protected DateTime endTime;
        protected bool hasFired;

        public event EventHandler TriggerStop;

        public TimeBasedRecordingLength(TimeSpan recordingLength)
        {
            this.hasFired = false;
            this.targetLength = recordingLength;
            this.timer = new Timer()
            {
                Interval = 10, //10 milliseconds
                AutoReset = true
            };
            this.timer.Elapsed += this.Check_condition;
        }
        public string Name { get => "Time-based Recording Length"; }
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

        private void Check_condition(object sender, ElapsedEventArgs e)
        {
            if (this.endTime <= DateTime.UtcNow && !this.hasFired)
            {
                this.hasFired = true; //prevent multiple firings!
                this.TriggerStop?.Invoke(this, e);
            }
            this.NotifyPropertyChanged(null);
        }

        public TimeSpan Duration { get => DateTime.UtcNow - this.startTime; }

        public double? Progress { get => this.Duration.TotalSeconds / this.targetLength.TotalSeconds; }

        public TimeSpan? TimeRemaining { get => this.targetLength - this.Duration; }
    }
}
