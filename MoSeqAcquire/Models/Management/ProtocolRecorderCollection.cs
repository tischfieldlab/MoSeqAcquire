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
        public GeneralRecordingSettings GeneralSettings { get; set; }
        public ProtocolRecorderCollection Recorders { get; protected set; }
    }

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
        public RecorderSettings Config { get; set; }
        [XmlArray("Channels")]
        [XmlArrayItem("Channel")]
        public List<string> Channels { get; set; }

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
