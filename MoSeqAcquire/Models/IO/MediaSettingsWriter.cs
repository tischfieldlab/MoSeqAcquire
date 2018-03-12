using MoSeqAcquire.Models.Acquisition;
using MoSeqAcquire.Models.Management;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MoSeqAcquire.Models.IO
{
    class MediaSettingsWriter
    {

        public static void WriteProtocol(string filename, Protocol Configuration)
        {
            var stypes = Configuration.Configurations.Select((s) => { return s.Config.GetType(); }).ToArray();
            XmlSerializer serializer = new XmlSerializer(typeof(Protocol), stypes);
            TextWriter writer = new StreamWriter(filename);
            serializer.Serialize(writer, Configuration);
            writer.Close();
        }
        public static Protocol ReadProtocol(string filename)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Protocol));
            FileStream fs = new FileStream(filename, FileMode.Open);  
            Protocol configuration;
            configuration = (Protocol)serializer.Deserialize(fs);
            return configuration;
        }

    }
}
