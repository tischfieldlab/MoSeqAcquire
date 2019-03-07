using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoSeqAcquire.Models.Management;

namespace MoSeqAcquire.Models.Recording
{
    public class RecordingSummary
    {
        public RecordingSummary()
        {
            this.Recorders = new List<RecordingDevice>();
        }
        public List<RecordingDevice> Recorders { get; set; }
    }

    public class RecordingDevice : RecorderInfo
    {
        public RecordingDevice()
        {
            this.Records = new List<RecordingRecord>();
        }
        public List<RecordingRecord> Records { get; set; }

    }

    public class RecordingRecord
    {
        public string Filename { get; set; }
        public List<string> Channels { get; set; }
    }
}
