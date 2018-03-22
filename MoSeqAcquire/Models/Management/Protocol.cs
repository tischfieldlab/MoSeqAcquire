using MoSeqAcquire.Models.Acquisition;
using MoSeqAcquire.Models.Configuration;
using MoSeqAcquire.Models.IO;
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
        public static readonly string Extension = "xml";
        public static readonly string TypeDesc = "Protocol";

        protected Protocol() { }
        public Protocol(String Name)
        {
            this.Name = Name;
            this.Sources = new List<ProtocolItem>();
            this.Recorders = new List<ProtocolItem>();
        }
        public string Name { get; set; }
        public bool Locked { get; set; }


        [XmlArray("Sources")]
        [XmlArrayItem(ElementName = "Source")]
        public List<ProtocolItem> Sources { get; set; }

        [XmlArray("Recorders")]
        [XmlArrayItem(ElementName = "Recorder")]
        public List<ProtocolItem> Recorders { get; set; }



        public void RegisterProvider(Type Provider, ConfigSnapshot Config)
        {
            List<ProtocolItem> items = this.GetCollectionForType(Provider);

            var position = items.FindIndex(s => s.GetProviderType() == Provider);
            if(position == -1)
            {
                items.Add(new ProtocolItem(){
                    Provider = Provider.AssemblyQualifiedName,
                    Config = Config
                });
            }
            else
            {
                items.ElementAt(position).Config = Config;
            }
        }
        protected List<ProtocolItem> GetCollectionForType(Type type)
        {
            if (typeof(MediaSource).IsAssignableFrom(type))
            {
                return this.Sources;
            }
            else if (typeof(IMediaWriter).IsAssignableFrom(type))
            {
                return this.Recorders;
            }
            throw new InvalidOperationException("Type \"" + type.FullName + "\" is not a valid provider type!");
        }
        public T CreateProvider<T>() 
        {
            return (T)this.CreateProvider(typeof(T));
        }
        public object CreateProvider(Type type)
        {
            var item = this.GetCollectionForType(type).Find(s => s.GetProviderType() == type);
            if (item != null)
            {
                var ms = (MediaSource)Activator.CreateInstance(type);
                //ms.Config.ApplySnapshot(item.Config);
                return ms;
            }
            return null;
        }
       
        
    }

    public class ProtocolItem
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
