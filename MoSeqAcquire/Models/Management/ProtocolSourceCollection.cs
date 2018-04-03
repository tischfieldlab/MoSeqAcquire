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

namespace MoSeqAcquire.Models.Management
{
    [XmlRoot("Sources")]
    public class ProtocolSourceCollection : Collection<ProtocolSource>
    {

        public void Add(Type Type, string DeviceId, ConfigSnapshot Settings)
        {
            base.Add(new ProtocolSource()
            {
                Provider = Type.FullName,
                DeviceId = DeviceId,
                Config = Settings
            });
        }

    }

    [XmlRoot("Source")]
    public class ProtocolSource
    {
        [XmlAttribute]
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
    }
}
