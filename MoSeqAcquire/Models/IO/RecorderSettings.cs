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

        [Description("Recordes a specified number of seconds")]
        [EnumDisplayName("A specific number of seconds")]
        TimeCount,


        [Description("Recordes a specified number of frames")]
        [EnumDisplayName("A specific number of frames")]
        FrameCount,

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
        protected int recordingFrameCount;
        protected int recordingSeconds;

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
        public int RecordingFrameCount
        {
            get => this.recordingFrameCount;
            set => this.SetField(ref this.recordingFrameCount, value);
        }
        public int RecordingSeconds
        {
            get => this.recordingSeconds;
            set => this.SetField(ref this.recordingSeconds, value);
        }
        public bool IsValid
        {
            get
            {
                return !string.IsNullOrWhiteSpace(this.directory)
                    && !string.IsNullOrWhiteSpace(this.basename)
                    && (this.recordingMode.Equals(RecordingMode.Indeterminate)
                        || (this.recordingMode.Equals(RecordingMode.TimeCount) && this.recordingSeconds > 0)
                        || (this.recordingMode.Equals(RecordingMode.FrameCount) && this.recordingFrameCount > 0)
                       );
            }
        }
    }    
}
