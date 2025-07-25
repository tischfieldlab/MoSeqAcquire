﻿using MoSeqAcquire.Models.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace MoSeqAcquire.Models.Recording
{
    public class IndeterminantRecordingLength : ObservableObject, IRecordingLengthStrategy
    {
        protected Timer timer;
        protected DateTime startTime;

        
        public event EventHandler TriggerStop
        {
            //do this to suppress the "event never used" warning
            //in this implementation, the TriggerStop event is NEVER triggered!
            add { } 
            remove { }
        }

        public IndeterminantRecordingLength()
        {
            this.timer = new Timer()
            {
                Interval = 100, //100 milliseconds
                AutoReset = true
            };
            this.timer.Elapsed += this.Check_condition;
        }
        public string Name { get => "Indeterminate Recording Length"; }
        public void Start()
        {
            this.startTime = DateTime.UtcNow;
            this.timer.Enabled = true;
        }
        public void Stop()
        {
            this.timer.Enabled = false;
        }

        private void Check_condition(object sender, ElapsedEventArgs e)
        {
            this.NotifyPropertyChanged(null);
        }

        public TimeSpan Duration { get => DateTime.UtcNow - this.startTime; }

        public double? Progress { get => null; }

        public TimeSpan? TimeRemaining { get => null; }
    }
}
