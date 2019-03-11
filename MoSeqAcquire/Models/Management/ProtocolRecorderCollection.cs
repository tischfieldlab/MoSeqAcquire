using MoSeqAcquire.Models.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;

namespace MoSeqAcquire.Models.Management
{

    public class ProtocolRecordingsSetup
    {
        public ProtocolRecordingsSetup()
        {
            this.GeneralSettings = new ConfigSnapshot();
            this.Recorders = new ProtocolRecorderCollection();
        }
        
        public ConfigSnapshot GeneralSettings { get; set; }

  
        public ProtocolRecorderCollection Recorders { get; set; }

        public override bool Equals(object obj)
        {
            var prs = obj as ProtocolRecordingsSetup;

            if (!this.GeneralSettings.Equals(prs.GeneralSettings))
                return false;
            if (!this.Recorders.SequenceEqual(prs.Recorders))
                return false;

            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = 91812158;
            hashCode = hashCode * -1521134295 + EqualityComparer<ConfigSnapshot>.Default.GetHashCode(GeneralSettings);
            hashCode = hashCode * -1521134295 + EqualityComparer<ProtocolRecorderCollection>.Default.GetHashCode(Recorders);
            return hashCode;
        }
    }

    
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

    public class ProtocolRecorder
    {
        public ProtocolRecorder()
        {
            this.Pins = new List<ProtocolRecorderPin>();
            this.Config = new ConfigSnapshot();
        }
        [XmlAttribute("Name")]
        public string Name { get; set; }
        public string Provider { get; set; }

        [XmlArray("Pins")]
        public List<ProtocolRecorderPin> Pins { get; set; }
        public ConfigSnapshot Config { get; set; }
        

        public Type GetProviderType()
        {
            return Type.GetType(this.Provider);
        }
        public override bool Equals(object obj)
        {
            var recorder = obj as ProtocolRecorder;

            if (!string.Equals(this.Name, recorder.Name))
                return false;
            if (!string.Equals(this.Provider, recorder.Provider))
                return false;
            if (!this.Pins.SequenceEqual(recorder.Pins))
                return false;
            if (!this.Config.Equals(recorder.Config))
                return false;

            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = -719071386;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Provider);
            hashCode = hashCode * -1521134295 + EqualityComparer<List<ProtocolRecorderPin>>.Default.GetHashCode(Pins);
            hashCode = hashCode * -1521134295 + EqualityComparer<ConfigSnapshot>.Default.GetHashCode(Config);
            return hashCode;
        }
    }

    [XmlType("Pin")]
    public class ProtocolRecorderPin
    {
        [XmlAttribute("Name")]
        public string Name { get; set; }
        [XmlAttribute("Channel")]
        public string Channel { get; set; }

        public override bool Equals(object obj)
        {
            var pin = obj as ProtocolRecorderPin;

            if (!string.Equals(this.Name, pin.Name))
                return false;
            if (!string.Equals(this.Channel, pin.Channel))
                return false;

            return true;
        }

        public override int GetHashCode()
        {
            var hashCode = -2055277510;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Channel);
            return hashCode;
        }
    }
}
