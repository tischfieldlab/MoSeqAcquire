using MoSeqAcquire.Models.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.Models.IO
{
    public enum RecordingMode
    {
        FrameCount,
        TimeCount,
        Indeterminate
    }
    public class RecorderSettings : ObservableObject { }
    public class GeneralRecordingSettings : ObservableObject 
    {
        public GeneralRecordingSettings() { }

        protected string directory;
        protected string basename;
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
    }    
}
