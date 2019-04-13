using System;
using System.ComponentModel;
using System.IO;
using MoSeqAcquire.Models.Configuration;
using MoSeqAcquire.Views.Extensions;

namespace MoSeqAcquire.Models.Recording
{
    public enum RecordingMode
    {
        [Description("Recordes until stopped")]
        [EnumDisplayName("Until stopped")]
        Indeterminate,

        [Description("Recordes the specified amount of time")]
        [EnumDisplayName("A specific amount of time")]
        TimeCount
    }
    public abstract class RecorderSettings : BaseConfiguration
    {
    }
    public class GeneralRecordingSettings : BaseConfiguration
    {
        public GeneralRecordingSettings()
        {
            this.PropertyChanged += (s, e) => { if (!"IsValid".Equals(e.PropertyName)){ this.NotifyPropertyChanged("IsValid"); } };
        }

        protected string directory = "";
        protected string basename = "";
        protected RecordingMode recordingMode;
        protected TimeSpan recordingTime;

        public string Directory
        {
            get => this.directory;
            set => this.SetField(ref this.directory, value);
        }
        public string Basename
        {
            get => this.basename;
            set => this.SetField(ref this.basename, value);
        }
        public string ComputedBasePath
        {
            get => Path.Combine(this.Directory, this.Basename);
        }
        public RecordingMode RecordingMode
        {
            get => this.recordingMode;
            set => this.SetField(ref this.recordingMode, value);
        }

        public TimeSpan RecordingTime
        {
            get => this.recordingTime;
            set => this.SetField(ref this.recordingTime, value);
        }
        public bool IsValid
        {
            get
            {
                return !string.IsNullOrWhiteSpace(this.directory)
                    && !string.IsNullOrWhiteSpace(this.basename)
                    && (this.recordingMode.Equals(RecordingMode.Indeterminate)
                        || (this.recordingMode.Equals(RecordingMode.TimeCount) && this.recordingTime.TotalSeconds > 0)
                       );
            }
        }
    }    
}
