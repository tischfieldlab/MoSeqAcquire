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

    public class RecorderSettings : ConfigSnapshot
    {
        public RecorderSettings() { }
        public RecorderSettings(RecorderSettings settings)
        {
            this.Directory = settings.Directory;
            this.Basename = settings.Basename;
            this.RecordingMode = settings.RecordingMode;
            this.RecordingFrameCount = settings.RecordingFrameCount;
            this.RecordingSeconds = settings.RecordingSeconds;
        }
        public string Directory { get; set; }
        public string Basename { get; set; }
        public RecordingMode RecordingMode { get; set; }
        public int RecordingFrameCount { get; set; }
        public int RecordingSeconds { get; set; }

        public virtual void ApplySnapshot(ConfigSnapshot snapshot)
        {
            var other = snapshot as RecorderSettings;
            this.Directory = other.Directory;
            this.Basename = other.Basename;
            this.RecordingMode = other.RecordingMode;
            this.RecordingFrameCount = other.RecordingFrameCount;
            this.RecordingSeconds = this.RecordingSeconds;
        }

        public ConfigSnapshot GetSnapshot()
        {
            return this;
        }
    }    
}
