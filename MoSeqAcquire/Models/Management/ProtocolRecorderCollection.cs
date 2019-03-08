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
    [XmlRoot("Recordings")]
    public class ProtocolRecordingsSetup
    {
        public ProtocolRecordingsSetup()
        {
            this.Recorders = new ProtocolRecorderCollection();
        }
        [XmlElement("GeneralSettings")]
        public ConfigSnapshot GeneralSettings { get; set; }
        [XmlElement("Recorders")]
        public ProtocolRecorderCollection Recorders { get; protected set; }
    }

    [XmlRoot("Recorders")]
    public class ProtocolRecorderCollection : Collection<ProtocolRecorder>
    {

        public void Add(Type Type, ConfigSnapshot Settings)
        {
            base.Add(new ProtocolRecorder()
            {
                Provider = Type.AssemblyQualifiedName,
                Config = Settings
            });
        }

    }

    public class RecorderInfo
    {
        [XmlAttribute("Name")]
        public string Name { get; set; }
        [XmlElement("Provider")]
        public string Provider { get; set; }
        [XmlElement("Config")]
        public ConfigSnapshot Config { get; set; }
    }

    [XmlRoot("Recorder")]
    public class ProtocolRecorder : RecorderInfo
    {

        [XmlArray("Pins")]
        [XmlArrayItem("Pin", typeof(ProtocolRecorderPin))]
        public List<ProtocolRecorderPin> Pins { get; set; }

        public Type GetProviderType()
        {
            return Type.GetType(this.Provider);
        }
    }

    [XmlRoot("Pin")]
    public class ProtocolRecorderPin
    {
        [XmlAttribute("Name")]
        public string Name { get; set; }
        [XmlAttribute("Channel")]
        public string Channel { get; set; }
    }
}
