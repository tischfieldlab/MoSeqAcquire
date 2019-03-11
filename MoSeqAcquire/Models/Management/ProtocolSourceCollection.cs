using MoSeqAcquire.Models.Acquisition;
using MoSeqAcquire.Models.Configuration;
using System;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace MoSeqAcquire.Models.Management
{
    [XmlRoot("Sources")]
    public class ProtocolSourceCollection : Collection<ProtocolSource>
    {

        public void Add(Type Type, string DeviceId, ConfigSnapshot Settings)
        {
            base.Add(new ProtocolSource()
            {
                Provider = Type.AssemblyQualifiedName,
                DeviceId = DeviceId,
                Config = Settings
            });
        }

    }

    [XmlType("Source")]
    public class ProtocolSource
    {
        [XmlElement]
        public string Provider { get; set; }
        [XmlElement]
        public string DeviceId { get; set; }
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
        public override bool Equals(object obj)
        {
            var source = obj as ProtocolSource;

            if (!this.Provider.Equals(source.Provider))
                return false;
            if (!this.DeviceId.Equals(source.DeviceId))
                return false;
            if (!this.Config.Equals(source.Config))
                return false;

            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
