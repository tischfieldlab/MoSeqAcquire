using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;
using MoSeqAcquire.Models.Management;

namespace MoSeqAcquire.Models.IO
{
    public class RecordingManager
    {
        protected bool isInitialized;
        protected IRecordingLengthStrategy terminator;
        protected List<IMediaWriter> _writers;

        public RecordingManager()
        {
            this._writers = new List<IMediaWriter>();
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
        /*public void AddRecorder(ProtocolRecorder WriterDefinition)
        {
            var writer = (MediaWriter)Activator.CreateInstance(WriterDefinition.GetProviderType(), new object[] { this });
            //writer.ApplySettings((RecorderSettings)this.settings.GetSnapshot());
            foreach (var c in WriterDefinition.Channels)
            {
                writer.ConnectChannel(c.Channel.Channel);
            }
            this.AddRecorder(Writer);
        }*/
        protected string ReplyToDestinationRequest()
        {
            Directory.CreateDirectory(this.GeneralSettings.ComputedBasePath);
            return this.GeneralSettings.ComputedBasePath;
        }
        public bool IsInitialized { get => this.isInitialized; }
        public bool IsRecording { get; protected set; }
        public GeneralRecordingSettings GeneralSettings { get; protected set; }

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
        }
        public void Start()
        {
            if (!this.isInitialized)
            {
                throw new InvalidOperationException("Recording Manager must be initialized before starting!");
            }
            this.IsRecording = true;
            foreach (var r in this._writers)
            {
                r.Start();
            }
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
            this.IsRecording = false;
        }
    }

    public interface IRecordingLengthStrategy
    {
        event EventHandler TriggerStop;
    }
    public interface IRecordingStats
    {
        double Progress { get; }
    }
    public class IndeterminantRecordingLength : IRecordingLengthStrategy
    {
        public event EventHandler TriggerStop;
    }
    public class TimeBasedRecordingLength : IRecordingLengthStrategy
    {
        protected Timer timer;
        protected TimeSpan targetLength;
        protected DateTime startTime;
        protected DateTime endTime;

        public TimeBasedRecordingLength(TimeSpan recordingLength)
        {
            this.targetLength = recordingLength;
            this.startTime = DateTime.UtcNow;
            this.endTime = this.startTime.Add(this.targetLength);

            this.timer = new Timer()
            {
                Interval = 100, //100 milliseconds
                AutoReset = true,
                Enabled = true
            };
            this.timer.Elapsed += this.check_condition;
        }

        private void check_condition(object sender, ElapsedEventArgs e)
        {
            if (this.endTime <= DateTime.UtcNow)
            {
                this.TriggerStop?.Invoke(this, e);
            }
        }

        public event EventHandler TriggerStop;
    }
}
