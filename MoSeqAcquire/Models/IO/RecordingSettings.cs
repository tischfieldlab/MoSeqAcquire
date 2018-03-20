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

    public class RecordingSettings
    {
        public string Directory { get; set; }
        public string Basename { get; set; }
        public RecordingMode RecordingMode { get; set; }
        public int RecordingFrameCount { get; set; }
        public int RecordingSeconds { get; set; }
    }
}
