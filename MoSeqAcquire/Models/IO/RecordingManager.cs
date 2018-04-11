using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;
using MoSeqAcquire.Models.Management;

namespace MoSeqAcquire.Models.Recording
{
    public class RecordingManager : ObservableObject
    {
        protected bool isInitialized;
        protected bool isRecording;
        protected IRecordingLengthStrategy terminator;
        protected List<IMediaWriter> _writers;

        public event EventHandler RecordingStarted;
        public event EventHandler RecordingFinished;

        public RecordingManager()
        {
            this._writers = new List<IMediaWriter>();
            this.GeneralSettings = new GeneralRecordingSettings();
        }

        public void AddRecorder(IMediaWriter Writer)
        {
            if (this.isInitialized)
            {
                throw new InvalidOperationException("Cannot add recorder after initialization!");
            }
            Writer.RequestDestinationBase += this.ReplyToDestinationRequest;
            this._writers.Add(Writer);
        }
        protected string ReplyToDestinationRequest()
        {
            Directory.CreateDirectory(this.GeneralSettings.ComputedBasePath);
            return this.GeneralSettings.ComputedBasePath == null ? string.Empty : this.GeneralSettings.ComputedBasePath;
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
        public GeneralRecordingSettings GeneralSettings { get; set; }

        public void Initialize(GeneralRecordingSettings GeneralSettings)
        {
            this.isInitialized = true;
            this.GeneralSettings = GeneralSettings;
            switch (this.GeneralSettings.RecordingMode)
            {
                case RecordingMode.TimeCount:
                    this.terminator = new TimeBasedRecordingLength(TimeSpan.FromSeconds(this.GeneralSettings.RecordingSeconds));
                    break;
                case RecordingMode.Indeterminate:
                default:
                    this.terminator = new IndeterminantRecordingLength();
                    break;
            }
            this.terminator.TriggerStop += (s, e) => { this.Stop(); };
            this.terminator.PropertyChanged += (s,e) => { this.NotifyPropertyChanged(null); };
        }
        public void Start()
        {
            if (!this.isInitialized)
            {
                throw new InvalidOperationException("Recording Manager must be initialized before starting!");
            }
            
            this.IsRecording = true;
            this.terminator.Start();
            foreach (var r in this._writers)
            {
                r.Start();
            }
            this.RecordingStarted?.Invoke(this, new EventArgs());
        }
        public void Stop()
        {
            if (!this.IsRecording)
            {
                throw new InvalidOperationException("Recording Manager must be started before stopping!");
            }
            foreach (var r in this._writers)
            {
                r.Stop();
            }
            this.terminator.Stop();
            this.IsRecording = false;
            this.RecordingFinished?.Invoke(this, new EventArgs());
        }

        protected void WriteRecordingInfo()
        {
            foreach(var writer in this._writers)
            {
                var name = writer.Name;
                var dest = writer.GetChannelFileMap();
            }
        }
    }
}
