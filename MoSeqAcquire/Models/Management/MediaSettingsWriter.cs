using MoSeqAcquire.Models.Recording;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace MoSeqAcquire.Models.Management
{
    public class MediaSettingsWriter
    {
        public static void WriteProtocol(string filename, Protocol Configuration)
        {
            using (var writer = new StreamWriter(filename))
            {
                WriteProtocol(writer, Configuration);
            }
        }
        public static string WriteProtocol(Protocol Configuration)
        {
            using (var writer = new UTF8StringWriter())
            {
                WriteProtocol(writer, Configuration);
                return writer.ToString();
            }
        }
        public static void WriteProtocol(TextWriter stream, Protocol Configuration)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Protocol), GetSerializedTypes());
            serializer.Serialize(stream, Configuration);
        }
        public static Protocol ReadProtocol(string filename)
        {
            using (var stream = new StreamReader(filename))
            {
                return ProtocolFromStream(stream);
            }
        }
        public static Protocol ProtocolFromString(string contents)
        {
            using (var stream = new StringReader(contents))
            {
                return ProtocolFromStream(stream);
            }
        }
        public static Protocol ProtocolFromStream(TextReader textReader)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Protocol), GetSerializedTypes());
            serializer.UnknownNode += new XmlNodeEventHandler(serializer_UnknownNode);
            serializer.UnknownAttribute += new XmlAttributeEventHandler(serializer_UnknownAttribute);
            serializer.UnknownElement += new XmlElementEventHandler(serializer_UnknownElement);
            return (Protocol)serializer.Deserialize(textReader);
        }

        private static void serializer_UnknownElement(object sender, XmlElementEventArgs e)
        {
            var x = 1;
        }

        private static void serializer_UnknownAttribute(object sender, XmlAttributeEventArgs e)
        {
            var x = 1;
        }

        private static void serializer_UnknownNode(object sender, XmlNodeEventArgs e)
        {
            var x = 1;
        }

        protected static Type[] GetSerializedTypes()
        {
            var types = new List<Type>();
            types.Add(typeof(RecorderSettings));
            types.AddRange(ProtocolHelpers.GetKnownTypesForRecorders());
            types.AddRange(ProtocolHelpers.GetKnownTypesForProviders());
            return types.ToArray();
        }

    }

    public sealed class UTF8StringWriter : StringWriter
    {
        public override Encoding Encoding { get => Encoding.UTF8; }
    }
}
