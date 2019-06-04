using MoSeqAcquire.Models.Management;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

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
            return (RecordingSummaryOutputFormat)Enum.Parse(typeof(RecordingSummaryOutputFormat),  Properties.Settings.Default.RecordingSummaryOutputFormat, true);
        }

        public static void Write(string filename, RecordingSummary Configuration)
        {
            if (GetOutputFormat() == RecordingSummaryOutputFormat.JSON)
            {
                var xml = new XmlDocument();
                xml.LoadXml(Write(Configuration));
                using (TextWriter writer = new StreamWriter(filename+".json"))
                {
                    writer.Write(JsonConvert.SerializeXmlNode(xml));
                }
            }
            else
            {
                using (TextWriter writer = new StreamWriter(filename+".xml"))
                {
                    Write(writer, Configuration);
                }
            }
        }
        public static string Write(RecordingSummary Configuration)
        {
            using (var writer = new UTF8StringWriter())
            {
                Write(writer, Configuration);
                return writer.ToString();
            }
        }
        public static void Write(TextWriter stream, RecordingSummary Configuration)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(RecordingSummary), GetSerializedTypes());
            serializer.Serialize(stream, Configuration);
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
