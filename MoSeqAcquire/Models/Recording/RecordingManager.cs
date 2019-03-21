using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;
using MoSeqAcquire.Models.Management;
using MoSeqAcquire.Models.Triggers;

namespace MoSeqAcquire.Models.Recording
{
    public class RecordingManager : ObservableObject
    {
        protected bool isInitialized;
        protected bool isRecording;
        protected IRecordingLengthStrategy terminator;
        private readonly TriggerBus triggerBus;
        protected List<MediaWriter> _writers;

        public event EventHandler RecordingStarted;
        public event EventHandler RecordingFinished;

        public RecordingManager(TriggerBus triggerBus)
        {
            this.triggerBus = triggerBus;
            this._writers = new List<MediaWriter>();
            this.GeneralSettings = new GeneralRecordingSettings();
        }

        public void AddRecorder(MediaWriter Writer)
        {
            if (this.isInitialized)
            {
                throw new InvalidOperationException("Cannot add recorder after initialization!");
            }
            Writer.RequestDestinationBase += this.ReplyToDestinationRequest;
            this._writers.Add(Writer);
        }
        public void RemoveRecorder(MediaWriter Writer)
        {
            if (this.isInitialized)
            {
                throw new InvalidOperationException("Cannot remove recorder after initialization!");
            }
            Writer.RequestDestinationBase -= this.ReplyToDestinationRequest;
            this._writers.Remove(Writer);
        }
        public void ClearRecorders()
        {
            if (this.isInitialized)
            {
                throw new InvalidOperationException("Cannot clear recorders after initialization!");
            }
            foreach(var w in this._writers)
            {
                w.RequestDestinationBase -= this.ReplyToDestinationRequest;
            }
            this._writers.Clear();
        }
        protected string ReplyToDestinationRequest()
        {
            if (!string.IsNullOrWhiteSpace(this.GeneralSettings.ComputedBasePath))
            {
                Directory.CreateDirectory(this.GeneralSettings.ComputedBasePath);
            }
            return this.GeneralSettings.ComputedBasePath ?? string.Empty;
        }
        public bool IsInitialized { get => this.isInitialized; }
        public bool IsRecording
        {
            get => this.isRecording;
            protected set => this.SetField(ref this.isRecording, value);
        }
        public TimeSpan Duration { get => this.terminator == null ? TimeSpan.Zero : this.terminator.Duration; }
        public double? Progress { get => this.terminator?.Progress; }
        public TimeSpan? TimeRemaining { get => this.terminator?.TimeRemaining; }
        public GeneralRecordingSettings GeneralSettings { get; protected set; }

        public void Initialize(GeneralRecordingSettings GeneralSettings)
        {
            this.isInitialized = true;            
        }
        protected IRecordingLengthStrategy TerminatorFactory()
        {
            IRecordingLengthStrategy terminator;
            switch (this.GeneralSettings.RecordingMode)
            {
                case RecordingMode.TimeCount:
                    terminator = new TimeBasedRecordingLength(TimeSpan.FromSeconds(this.GeneralSettings.RecordingSeconds));
                    break;
                case RecordingMode.Indeterminate:
                default:
                    terminator = new IndeterminantRecordingLength();
                    break;
            }
            return terminator;
        }


        public void Reset()
        {
            this.isInitialized = false;
        }
        public void Start()
        {
            if (!this.isInitialized)
            {
                throw new InvalidOperationException("Recording Manager must be initialized before starting!");
            }
            this.triggerBus.Trigger(new BeforeRecordingStartedTrigger());

            this.WriteRecordingInfo();

            this.terminator = this.TerminatorFactory();
            terminator.TriggerStop += this.Terminator_TriggerStop;
            terminator.PropertyChanged += this.Terminator_PropertyChanged;
            this.terminator.Start();

            foreach (var r in this._writers)
            {
                r.Start();
            }
            this.IsRecording = true;
            this.RecordingStarted?.Invoke(this, new EventArgs());
            this.triggerBus.Trigger(new AfterRecordingStartedTrigger());
        }
        public void Stop()
        {
            if (!this.IsRecording)
            {
                throw new InvalidOperationException("Recording Manager must be started before stopping!");
            }
            this.triggerBus.Trigger(new BeforeRecordingFinishedTrigger());
            foreach (var r in this._writers)
            {
                r.Stop();
            }
            this.terminator.Stop();
            this.terminator.TriggerStop -= this.Terminator_TriggerStop;
            this.terminator.PropertyChanged -= this.Terminator_PropertyChanged;
            this.terminator = null;

            this.IsRecording = false;
            this.Reset();
            this.RecordingFinished?.Invoke(this, new EventArgs());
            this.triggerBus.Trigger(new AfterRecordingFinishedTrigger());
        }

        protected void WriteRecordingInfo()
        {
            var summary = new RecordingSummary();
            foreach(var writer in this._writers)
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
