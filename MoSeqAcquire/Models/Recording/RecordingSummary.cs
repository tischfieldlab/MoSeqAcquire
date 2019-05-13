using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoSeqAcquire.Models.Configuration;
using MoSeqAcquire.Models.Management;
using MoSeqAcquire.Models.Metadata;

namespace MoSeqAcquire.Models.Recording
{
    public class RecordingSummary
    {
        public RecordingSummary()
        {
            this.Recorders = new List<RecordingDevice>();

            this.SystemIdentifier = $"{System.Environment.UserName}@{System.Environment.MachineName}";
            this.StartTime = DateTime.UtcNow;
            this.UUID = Guid.NewGuid();
        }
        public string SystemIdentifier { get; set; }
        public DateTime StartTime { get; set; }
        public Guid UUID { get; set; }

        public RecordingMetadataSnapshot Metadata { get; set; }
        public List<RecordingDevice> Recorders { get; set; }
        
    }

    public class RecordingDevice
    {       
        public RecordingDevice()
        {
            this.Records = new List<RecordingRecord>();
        }
        public string Name { get; set; }
        public string Provider { get; set; }
        public List<RecordingRecord> Records { get; set; }
        public ConfigSnapshot Config { get; set; }
    }

    public class RecordingRecord
    {
        public string Filename { get; set; }
        public List<string> Channels { get; set; }
    }
}
