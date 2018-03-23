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
            this.Sources = new ProtocolSourceCollection();
            this.Recorders = new ProtocolRecorderCollection();
        }
        public string Name { get; set; }
        public bool Locked { get; set; }

        public ProtocolSourceCollection Sources { get; protected set; }
        public ProtocolRecorderCollection Recorders { get; protected set; }



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
