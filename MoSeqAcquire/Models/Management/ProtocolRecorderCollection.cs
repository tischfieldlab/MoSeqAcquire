using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using MoSeqAcquire.Models.Acquisition;
using MoSeqAcquire.Models.Configuration;
using MoSeqAcquire.Models.IO;

namespace MoSeqAcquire.Models.Management
{
    [XmlRoot("Recorders")]
    public class ProtocolRecorderCollection : Collection<ProtocolRecorder>
    {

        public void Add(Type Type, RecorderSettings Settings)
        {
            base.Add(new ProtocolRecorder()
            {
                Provider = Type.FullName,
                Config = Settings
            });
        }

    }

    [XmlRoot("Recorder")]
    public class ProtocolRecorder
    {
        [XmlAttribute]
        public string Name { get; set; }
        [XmlElement]
        public string Provider { get; set; }
        [XmlElement]
        public ConfigSnapshot Config { get; set; }

        public Type GetProviderType()
        {
            return Type.GetType(this.Provider);
        }
        public MediaSource Create()
        {
            return (MediaSource)Activator.CreateInstance(this.GetProviderType());
        }
    }
}
