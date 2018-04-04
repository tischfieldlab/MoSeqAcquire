using MoSeqAcquire.Models.Acquisition;
using MoSeqAcquire.Models.IO;
using MoSeqAcquire.Models.Management;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MoSeqAcquire.Models.Management
{
    class MediaSettingsWriter
    {

        public static void WriteProtocol(string filename, Protocol Configuration)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Protocol), GetSerializedTypes());
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
            //types.Add(typeof(Acquisition.Kinect.KinectConfigSnapshot));
            types.Add(typeof(RecorderSettings));
            types.AddRange(ProtocolHelpers.GetKnownTypesForRecorders());
            types.AddRange(ProtocolHelpers.GetKnownTypesForProviders());
            return types.ToArray();
        }

    }
}
