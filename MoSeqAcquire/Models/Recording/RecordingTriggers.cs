using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoSeqAcquire.Models.Triggers;
using Microsoft.Extensions.DependencyInjection;

namespace MoSeqAcquire.Models.Recording
{
    public abstract class RecordingTriggerEvent : TriggerEvent
    {
        protected virtual void FireTrigger(object sender, EventArgs e)
        {
            this.Fire();
        }
    }
    [DisplayName("Before Recording Started")]
    public class BeforeRecordingStartedTrigger : RecordingTriggerEvent
    {
        public override void Start()
        {
            var recorder = App.Current.Services.GetService<RecordingManager>();
            recorder.BeforeStartRecording += this.FireTrigger;
        }

        public override void Stop()
        {
            var recorder = App.Current.Services.GetService<RecordingManager>();
            recorder.BeforeStartRecording -= this.FireTrigger;
        }
    }
    [DisplayName("After Recording Started")]
    public class AfterRecordingStartedTrigger : RecordingTriggerEvent
    {
        public override void Start()
        {
            var recorder = App.Current.Services.GetService<RecordingManager>();
            recorder.RecordingStarted += this.FireTrigger;
        }

        public override void Stop()
        {
            var recorder = App.Current.Services.GetService<RecordingManager>();
            recorder.RecordingStarted -= this.FireTrigger;
        }
    }
    [DisplayName("Before Recording Finished")]
    public class BeforeRecordingFinishedTrigger : RecordingTriggerEvent
    {
        public override void Start()
        {
            var recorder = App.Current.Services.GetService<RecordingManager>();
            recorder.BeforeRecordingEnd += this.FireTrigger;
        }

        public override void Stop()
        {
            var recorder = App.Current.Services.GetService<RecordingManager>();
            recorder.BeforeRecordingEnd -= this.FireTrigger;
        }
    }
    [DisplayName("After Recording Finished")]
    public class AfterRecordingFinishedTrigger : RecordingTriggerEvent
    {
        public AfterRecordingFinishedTrigger() : base() { }
        public AfterRecordingFinishedTrigger(bool aborted) : base()
        {
            this.Aborted = aborted;
        }
        public bool Aborted { get; protected set; }

        public override void Start()
        {
            var recorder = App.Current.Services.GetService<RecordingManager>();
            recorder.BeforeStartRecording += this.FireTrigger;
        }

        public override void Stop()
        {
            var recorder = App.Current.Services.GetService<RecordingManager>();
            recorder.BeforeStartRecording -= this.FireTrigger;
        }

        protected override void FireTrigger(object sender, EventArgs e)
        {
            this.Aborted = (sender as RecordingManager).IsAbortRequested;
            this.Fire();
        }
    }
}
