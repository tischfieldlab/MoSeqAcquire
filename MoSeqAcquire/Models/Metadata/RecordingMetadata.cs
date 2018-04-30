using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.Models.Metadata
{
    public class RecordingMetadata : List<RecordingMetadataItem>
    {
    }

    public class RecordingMetadataItem
    {
        public string Name { get; set; }
        public Type ValueType { get; set; }
        public object Value { get; set; }
    }
}
