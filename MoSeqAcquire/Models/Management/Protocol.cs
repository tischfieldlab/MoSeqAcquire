using MoSeqAcquire.Models.Acquisition;
using MoSeqAcquire.Models.Configuration;
using MoSeqAcquire.Models.Recording;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MoSeqAcquire.Models.Management
{
    [XmlRoot("Protocol")]
    public class Protocol
    {
        public static readonly string Extension = "xml";
        public static readonly string TypeDesc = "Protocol";

        protected Protocol() { }
        public Protocol(String Name)
        {
            this.Name = Name;
            this.Sources = new ProtocolSourceCollection();
            this.Recordings = new ProtocolRecordingsSetup();
        }
        public string Name { get; set; }
        public bool Locked { get; set; }

        [XmlArray("Sources")]
        public ProtocolSourceCollection Sources { get; set; }
        public ProtocolRecordingsSetup Recordings { get; set; }

        public override bool Equals(object obj)
        {
            var pcol = obj as Protocol;

            if (!this.Name.Equals(pcol.Name))
                return false;
            if (!this.Locked.Equals(pcol.Locked))
                return false;
            if (!this.Sources.SequenceEqual(pcol.Sources))
                return false;
            if (!this.Recordings.Equals(pcol.Recordings))
                return false;
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /*public void RegisterProvider(Type Provider, ConfigSnapshot Config)
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
        }   */
    }
}
