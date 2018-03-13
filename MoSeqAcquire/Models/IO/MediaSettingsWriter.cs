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
            using (TextWriter writer = new StreamWriter(filename))
            {
                serializer.Serialize(writer, Configuration);
            }
        }
        public static Protocol ReadProtocol(string filename)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Protocol), GetSerializedTypes());
            Protocol configuration;
            using (FileStream fs = new FileStream(filename, FileMode.Open))
            {
                configuration = (Protocol)serializer.Deserialize(fs);
            }
            return configuration;
        }

        protected static Type[] GetSerializedTypes()
        {
            var types = new List<Type>();
            types.Add(typeof(Acquisition.Kinect.KinectConfigSnapshot));
            return types.ToArray();
        }

    }
}
