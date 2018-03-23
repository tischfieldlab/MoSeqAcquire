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
            //var stypes = Configuration.Sources.Select((s) => { return s.Config.GetType(); });
            //stypes = stypes.Concat(Configuration.Recorders.Select((s) => { return s.Config.GetType(); }));
            var stypes = GetSerializedTypes();
            XmlSerializer serializer = new XmlSerializer(typeof(Protocol), stypes.ToArray());
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
            types.Add(typeof(RecorderSettings));
            return types.ToArray();
        }

    }
}
