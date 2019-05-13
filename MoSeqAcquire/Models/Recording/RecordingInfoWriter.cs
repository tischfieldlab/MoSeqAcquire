using MoSeqAcquire.Models.Management;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace MoSeqAcquire.Models.Recording
{
    public enum RecordingSummaryOutputFormat
    {
        XML,
        JSON
    }

    class RecordingInfoWriter
    {
        protected static RecordingSummaryOutputFormat GetOutputFormat()
        {
            //return (RecordingSummaryOutputFormat)Enum.Parse(typeof(RecordingSummaryOutputFormat),  Properties.Settings.Default.RecordingSummaryOutputFormat, true);
            return RecordingSummaryOutputFormat.XML; // TODO: Currently JSON serialization does not work as expected.
        }

        public static void Write(string filename, RecordingSummary Configuration)
        {
            if (GetOutputFormat() == RecordingSummaryOutputFormat.JSON)
            {
                JsonSerializer serializer = new JsonSerializer();
                using (StreamWriter sw = new StreamWriter(filename))
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    serializer.Serialize(writer, Configuration);
                }
            }
            else
            {
                XmlSerializer serializer = new XmlSerializer(typeof(RecordingSummary), GetSerializedTypes());
                using (TextWriter writer = new StreamWriter(filename))
                {
                    serializer.Serialize(writer, Configuration);
                }
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
