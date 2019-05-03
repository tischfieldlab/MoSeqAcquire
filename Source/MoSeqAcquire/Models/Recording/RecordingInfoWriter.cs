using MoSeqAcquire.Models.Management;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MoSeqAcquire.Models.Recording
{
    class RecordingInfoWriter
    {

        public static void Write(string filename, RecordingSummary Configuration)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(RecordingSummary), GetSerializedTypes());
            using (TextWriter writer = new StreamWriter(filename))
            {
                serializer.Serialize(writer, Configuration);
            }
        }
        public static RecordingSummary Read(string filename)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(RecordingSummary), GetSerializedTypes());
            RecordingSummary configuration;
            using (FileStream fs = new FileStream(filename, FileMode.Open))
            {
                configuration = (RecordingSummary)serializer.Deserialize(fs);
            }
            return configuration;
        }

        protected static Type[] GetSerializedTypes()
        {
            return ProtocolHelpers.GetKnownTypes().ToArray();
        }

    }
}
