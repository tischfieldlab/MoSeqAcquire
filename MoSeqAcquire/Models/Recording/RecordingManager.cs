using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;
using MoSeqAcquire.Models.Core;
using MoSeqAcquire.Models.Management;
using MoSeqAcquire.Models.Triggers;

namespace MoSeqAcquire.Models.Recording
{
    public enum RecordingManagerState
    {
        Idle,
        Starting,
        Recording,
        Completing
    }

    public static class RecordingManagerTasks
    {
        public static readonly string None = "";
        public static readonly string RunningBeforeStartTriggers = "Running Before Recording Start Triggers";
        public static readonly string RunningAfterStartTriggers = "Running After Recording Start Triggers";
        public static readonly string RunningBeforeCompleteTriggers = "Running Before Recording Finish Triggers";
        public static readonly string RunningAfterCompleteTriggers = "Running After Recording Finish Triggers";
        public static readonly string WritingRecordingInfo = "Writing Recording Information";
        public static readonly string AbortingRecording = "Aborting Recording Session";
        public static readonly string StartingRecorders = "Starting Recorders";
        public static readonly string StoppingRecorders = "Stopping Recorders";
        public static readonly string ActivelyRecording = "Actively Recording";
    }

    public class RecordingManager : ObservableObject
    {
        protected bool abortRequested;
        protected IRecordingLengthStrategy terminator;
        private readonly TriggerBus triggerBus;
        protected List<MediaWriter> writers;
        protected RecordingManagerState state;
        protected string currentTask;

        public event EventHandler RecordingStarted;
        public event EventHandler RecordingFinished;
        public event EventHandler RecordingAborted;

        public RecordingManager(TriggerBus triggerBus)
        {
            this.triggerBus = triggerBus;
            this.writers = new List<MediaWriter>();
            this.GeneralSettings = new GeneralRecordingSettings();
        }

        public void AddRecorder(MediaWriter Writer)
        {
            this.EnsureState(RecordingManagerState.Idle, "Cannot add recorder after initialization!");
            
            Writer.RequestDestinationBase += this.ReplyToDestinationRequest;
            this.writers.Add(Writer);
        }
        public void RemoveRecorder(MediaWriter Writer)
        {
            this.EnsureState(RecordingManagerState.Idle, "Cannot remove recorder after initialization!");

            Writer.RequestDestinationBase -= this.ReplyToDestinationRequest;
            this.writers.Remove(Writer);
        }
        public void ClearRecorders()
        {
            this.EnsureState(RecordingManagerState.Idle, "Cannot clear recorders after initialization!");
            
            this.writers.ForEach(w => w.RequestDestinationBase -= this.ReplyToDestinationRequest);
            this.writers.Clear();
        }
        protected string ReplyToDestinationRequest()
        {
            if (!string.IsNullOrWhiteSpace(this.GeneralSettings.ComputedBasePath))
            {
                Directory.CreateDirectory(this.GeneralSettings.ComputedBasePath);
            }
            return this.GeneralSettings.ComputedBasePath ?? string.Empty;
        }

        public RecordingManagerState State
        {
            get => this.state;
            protected set => this.SetField(ref this.state, value);
        }
        public string CurrentTask
        {
            get => this.currentTask;
            protected set => this.SetField(ref this.currentTask, value);
        }
        public TimeSpan Duration { get => this.terminator == null ? TimeSpan.Zero : this.terminator.Duration; }
        public double? Progress { get => this.terminator?.Progress; }
        public TimeSpan? TimeRemaining { get => this.terminator?.TimeRemaining; }
        public GeneralRecordingSettings GeneralSettings { get; protected set; }

        
        protected IRecordingLengthStrategy TerminatorFactory()
        {
            switch (this.GeneralSettings.RecordingMode)
            {
                case RecordingMode.TimeCount:
                    this.terminator = new TimeBasedRecordingLength(this.GeneralSettings.RecordingTime);
                    break;
                case RecordingMode.Indeterminate:
                default:
                    this.terminator = new IndeterminantRecordingLength();
                    break;
            }
            this.terminator.TriggerStop += this.Terminator_TriggerStop;
            this.terminator.PropertyChanged += this.Terminator_PropertyChanged;
            return this.terminator;
        }
        protected void DisposeTerminator()
        {
            if(this.terminator == null)
            {
                throw new InvalidOperationException("Cannot dispose terminator, is already null!");
            }
            this.terminator.TriggerStop -= this.Terminator_TriggerStop;
            this.terminator.PropertyChanged -= this.Terminator_PropertyChanged;
            this.terminator = null;
        }
        public void EnsureState(RecordingManagerState State, string ExceptionMessage)
        {
            if(this.State != State)
            {
                throw new InvalidOperationException(ExceptionMessage);
            }
        }

        public void Start()
        {
            this.EnsureState(RecordingManagerState.Idle, "Recording Manager must be idle before starting!");
            

            this.State = RecordingManagerState.Starting;
            this.abortRequested = false;
            this.GeneralSettings.Basename = "session_" + DateTime.UtcNow.ToString("yyyyMMddHHmmss");

            this.CurrentTask = RecordingManagerTasks.RunningBeforeStartTriggers;
            this.triggerBus.Trigger(new BeforeRecordingStartedTrigger());

            if (this.abortRequested) return;

            this.CurrentTask = RecordingManagerTasks.WritingRecordingInfo;
            this.WriteRecordingInfo();

            this.CurrentTask = RecordingManagerTasks.StartingRecorders;
            foreach (var r in this.writers)
            {
                r.Start();
            }
            this.TerminatorFactory().Start();

            this.State = RecordingManagerState.Recording;
            this.CurrentTask = RecordingManagerTasks.RunningAfterStartTriggers;
            this.RecordingStarted?.Invoke(this, new EventArgs());
            this.triggerBus.Trigger(new AfterRecordingStartedTrigger());
            this.CurrentTask = RecordingManagerTasks.ActivelyRecording;
        }
        public void Stop()
        {
            //ensure state is correct
            this.EnsureState(RecordingManagerState.Recording, "Recording Manager must be started before stopping!");

            // run before complete triggers
            this.CurrentTask = RecordingManagerTasks.RunningBeforeCompleteTriggers;
            this.triggerBus.Trigger(new BeforeRecordingFinishedTrigger());

            //Stop the individual recorders
            this.State = RecordingManagerState.Completing;
            this.CurrentTask = RecordingManagerTasks.StoppingRecorders;
            foreach (var r in this.writers)
            {
                r.Stop();
            }
            //and dispose of the terminator
            this.terminator.Stop();
            this.DisposeTerminator();

            // run after complete triggers
            this.CurrentTask = RecordingManagerTasks.RunningAfterCompleteTriggers;
            this.RecordingFinished?.Invoke(this, new EventArgs());
            this.triggerBus.Trigger(new AfterRecordingFinishedTrigger());

            //set state to idle
            this.State = RecordingManagerState.Idle;
            this.CurrentTask = RecordingManagerTasks.None;
        }
        public void Abort()
        {
            this.abortRequested = true;
            this.State = RecordingManagerState.Completing;
            this.CurrentTask = RecordingManagerTasks.AbortingRecording;

            foreach (var r in this.writers)
            {
                if (r.IsRecording)
                {
                    r.Stop();
                }
            }
            if (this.terminator != null)
            {
                this.terminator.Stop();
                this.DisposeTerminator();
            }

            this.CurrentTask = RecordingManagerTasks.RunningAfterCompleteTriggers;
            this.RecordingAborted?.Invoke(this, new EventArgs());
            this.triggerBus.Trigger(new AfterRecordingFinishedTrigger() { Aborted = true });

            this.State = RecordingManagerState.Idle;
            this.CurrentTask = RecordingManagerTasks.None;
        }

        protected void WriteRecordingInfo()
        {
            var summary = new RecordingSummary();
            foreach(var writer in this.writers)
            {
                summary.Recorders.Add(writer.GetDeviceInfo());
            }
            string dest = Path.Combine(this.ReplyToDestinationRequest(), "info.xml");
            RecordingInfoWriter.Write(dest, summary);
        }

        private void Terminator_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.NotifyPropertyChanged(null);
        }

        private void Terminator_TriggerStop(object sender, EventArgs e)
        {
            this.Stop();
        }
    }
}
