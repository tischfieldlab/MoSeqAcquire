using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoSeqAcquire.Models.Acquisition.DirectShow;
using MoSeqAcquire.Models.Configuration;
using MoSeqAcquire.Models.Management;
using MoSeqAcquire.Models.Metadata;
using MoSeqAcquire.Models.Recording;
using MoSeqAcquire.Models.Recording.MPEGVideoWriter;
using MoSeqAcquire.ViewModels.Metadata;
using MoSeqAcquire.Views.Controls.MetadataEditor;

namespace MoSeqAcquireTests2
{
    [TestClass]
    public class UnitTest2
    {
        [TestMethod]
        public void Test_Empty()
        {
            var mc = new MetadataDefinitionCollection();

            var serialized = WriteProtocol(mc);

            Console.Write(serialized);

            var deserialized = ProtocolFromString(serialized);
            Assert.AreEqual(mc, deserialized);
        }
        [TestMethod]
        public void Test_Vanilla()
        {
            var mc = new MetadataDefinitionCollection();
            mc.Add(new MetadataItemDefinition("test", typeof(string)));

            var serialized = WriteProtocol(mc);

            Console.Write(serialized);

            var deserialized = ProtocolFromString(serialized);
            Assert.AreEqual(mc, deserialized);
        }

        [TestMethod]
        public void Test_Vanilla1()
        {
            var mc = new MetadataDefinitionCollection();
            mc.Add(new MetadataItemDefinition("test1", typeof(string))
            {
                DefaultValue = "Some Value 1"
            });
            mc.Add(new MetadataItemDefinition("test2", typeof(string))
            {
                DefaultValue = "Some Value 2"
            });

            var serialized = WriteProtocol(mc);

            Console.Write(serialized);

            var deserialized = ProtocolFromString(serialized);
            Assert.AreEqual(mc, deserialized);
        }

        [TestMethod]
        public void Test_Vanilla2()
        {
            var mc = new MetadataDefinitionCollection();
            var mdi1 = new MetadataItemDefinition("test", typeof(string))
            {
                DefaultValue = "Choice_1",
                Value = "Choice_3",
                Units = "Arbitrary Units",
                Constraint = ConstraintMode.Choices
                
            };
            var mdi1c = (mdi1.ConstraintImplementation as ChoicesConstraint);
            mdi1c.Choices.Add(new ChoicesConstraintChoice(mdi1) { Value = "Choice_1" });
            mdi1c.Choices.Add(new ChoicesConstraintChoice(mdi1) { Value = "Choice_2" });
            mdi1c.Choices.Add(new ChoicesConstraintChoice(mdi1) { Value = "Choice_3" });
            mc.Add(mdi1);

            var serialized = WriteProtocol(mc);
            Console.Write(serialized);

            var deserialized = ProtocolFromString(serialized);
            Assert.AreEqual(mc, deserialized);
        }











        public static void WriteProtocol(string filename, MetadataDefinitionCollection Configuration)
        {
            using (var writer = new StreamWriter(filename))
            {
                WriteProtocol(writer, Configuration);
            }
        }
        public static string WriteProtocol(MetadataDefinitionCollection Configuration)
        {
            using (var writer = new UTF8StringWriter())
            {
                WriteProtocol(writer, Configuration);
                return writer.ToString();
            }
        }
        public static void WriteProtocol(TextWriter stream, MetadataDefinitionCollection Configuration)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(MetadataDefinitionCollection));
            serializer.Serialize(stream, Configuration);
        }
        public static MetadataDefinitionCollection ReadProtocol(string filename)
        {
            MetadataDefinitionCollection pcol;
            using (var stream = new StreamReader(filename))
            {
                pcol = ProtocolFromStream(stream);
            }
            return pcol;
        }
        public static MetadataDefinitionCollection ProtocolFromString(string contents)
        {
            MetadataDefinitionCollection pcol;
            using (var stream = new StringReader(contents))
            {
                pcol = ProtocolFromStream(stream);
            }
            return pcol;
        }
        public static MetadataDefinitionCollection ProtocolFromStream(TextReader textReader)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(MetadataDefinitionCollection));
            /*serializer.UnknownNode += new XmlNodeEventHandler(Serializer_UnknownNode);
            serializer.UnknownAttribute += new XmlAttributeEventHandler(Serializer_UnknownAttribute);
            serializer.UnknownElement += new XmlElementEventHandler(Serializer_UnknownElement);*/
            return (MetadataDefinitionCollection)serializer.Deserialize(textReader);
        }
    }
}
