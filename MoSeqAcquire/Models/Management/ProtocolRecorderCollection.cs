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
using MoSeqAcquire.Models.Recording;

namespace MoSeqAcquire.Models.Management
{
    public class ProtocolRecordingsSetup
    {
        public ProtocolRecordingsSetup()
        {
            this.Recorders = new ProtocolRecorderCollection();
        }
        public ConfigSnapshot GeneralSettings { get; set; }
        public ProtocolRecorderCollection Recorders { get; protected set; }
    }

    [XmlRoot("Recorders")]
    public class ProtocolRecorderCollection : Collection<ProtocolRecorder>
    {

        public void Add(Type Type, ConfigSnapshot Settings)
        {
            base.Add(new ProtocolRecorder()
            {
                Provider = Type.FullName,
                Config = Settings
            });
        }

    }

    public class RecorderInfo
    {
        [XmlAttribute]
        public string Name { get; set; }
        [XmlElement]
        public string Provider { get; set; }
        [XmlElement]
        public ConfigSnapshot Config { get; set; }
    }

    [XmlRoot("Recorder")]
    public class ProtocolRecorder : RecorderInfo
    {

        [XmlArray("Pins")]
        [XmlArrayItem("Pin")]
        public List<ProtocolRecorderPin> Pins { get; set; }

        public Type GetProviderType()
        {
            return Type.GetType(this.Provider);
        }
    }

    public class ProtocolRecorderPin
    {
        [XmlAttribute]
        public string Name { get; set; }
        [XmlAttribute]
        public string Channel { get; set; }
    }
}
