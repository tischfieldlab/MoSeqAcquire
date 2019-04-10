using MoSeqAcquire.Models.Recording;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using System.Linq;

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
            Protocol pcol;
            using (var stream = new StreamReader(filename))
            {
                pcol = ProtocolFromStream(stream);
            }
            pcol.Name = filename;
            return pcol;
        }
        public static Protocol ProtocolFromString(string contents)
        {
            Protocol pcol;
            using (var stream = new StringReader(contents))
            {
                pcol = ProtocolFromStream(stream);
            }
            pcol.Name = "String Content";
            return pcol;
        }
        public static Protocol ProtocolFromStream(TextReader textReader)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Protocol), GetSerializedTypes());
            serializer.UnknownNode += new XmlNodeEventHandler(Serializer_UnknownNode);
            serializer.UnknownAttribute += new XmlAttributeEventHandler(Serializer_UnknownAttribute);
            serializer.UnknownElement += new XmlElementEventHandler(Serializer_UnknownElement);
            return (Protocol)serializer.Deserialize(textReader);
        }

        private static void Serializer_UnknownElement(object sender, XmlElementEventArgs e)
        {
            //var x = 1;
        }

        private static void Serializer_UnknownAttribute(object sender, XmlAttributeEventArgs e)
        {
            //var x = 1;
        }

        private static void Serializer_UnknownNode(object sender, XmlNodeEventArgs e)
        {
            //var x = 1;
        }

        protected static Type[] GetSerializedTypes()
        {
            return ProtocolHelpers.GetKnownTypes().ToArray();
        }

    }

    public sealed class UTF8StringWriter : StringWriter
    {
        public override Encoding Encoding { get => Encoding.UTF8; }
    }
}
