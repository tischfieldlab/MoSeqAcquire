using MoSeqAcquire.Models.Acquisition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MoSeqAcquire.Models.Management
{
    public class Protocol
    {
        protected Protocol() { }
        public Protocol(String Name)
        {
            this.Name = Name;
            this.Configurations = new List<ProtocolSource>();
        }
        public string Name { get; set; }
        public bool Locked { get; set; }
        [XmlArray("Configurations")]
        [XmlArrayItem(ElementName="Source")]
        public List<ProtocolSource> Configurations {get; set;}
        public void RegisterProvider(Type Provider, ConfigSnapshot Config)
        {
            if (typeof(MediaSource).IsAssignableFrom(Provider))
            {
                var position = this.Configurations.FindIndex(s => s.GetProviderType() == Provider);
                if(position == -1)
                {
                    this.Configurations.Add(new ProtocolSource(){
                        Provider = Provider.AssemblyQualifiedName,
                        Config = Config
                    });
                }
                else
                {
                    this.Configurations.ElementAt(position).Config = Config;
                }
            }
        }
        public T CreateProvider<T>() where T: MediaSource
        {
            return (T)this.CreateProvider(typeof(T));
        }
        public MediaSource CreateProvider(Type type)
        {
            var item = this.Configurations.Find(s => s.GetProviderType() == type);
            if (item != null)
            {
                var ms = (MediaSource)Activator.CreateInstance(type);
                //ms.Config.ApplySnapshot(item.Config);
                return ms;
            }
            return null;
        }

        
    }

    [XmlType("Source")]
    public class ProtocolSource
    {
        [XmlAttribute]
        public string Provider { get; set; }
        [XmlElement]
        public ConfigSnapshot Config { get; set; }

        public Type GetProviderType()
        {
            return Type.GetType(this.Provider);
        }
    }
}
