using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoSeqAcquire.Models.Acquisition.DirectShow;
using MoSeqAcquire.Models.Configuration;
using MoSeqAcquire.Models.Management;
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
            var mc = new MetadataCollection();

            var serialized = WriteProtocol(mc);

            Console.Write(serialized);

            var deserialized = ProtocolFromString(serialized);
            Assert.AreEqual(mc, deserialized);
        }
        [TestMethod]
        public void Test_Vanilla()
        {
            var mc = new MetadataCollection();
            mc.Add(new MetadataItem("test", typeof(string)));

            var serialized = WriteProtocol(mc);

            Console.Write(serialized);

            var deserialized = ProtocolFromString(serialized);
            Assert.AreEqual(mc, deserialized);
        }

        [TestMethod]
        public void Test_Vanilla2()
        {
            var mc = new MetadataCollection();
            var mdi1 = new MetadataItem("test", typeof(string))
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


            var serialized = WriteProtocol(mc);
            Console.Write(serialized);

            var deserialized = ProtocolFromString(serialized);
            Assert.AreEqual(mc, deserialized);
        }











        public static void WriteProtocol(string filename, MetadataCollection Configuration)
        {
            using (var writer = new StreamWriter(filename))
            {
                WriteProtocol(writer, Configuration);
            }
        }
        public static string WriteProtocol(MetadataCollection Configuration)
        {
            using (var writer = new UTF8StringWriter())
            {
                WriteProtocol(writer, Configuration);
                return writer.ToString();
            }
        }
        public static void WriteProtocol(TextWriter stream, MetadataCollection Configuration)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(MetadataCollection));
            serializer.Serialize(stream, Configuration);
        }
        public static MetadataCollection ReadProtocol(string filename)
        {
            MetadataCollection pcol;
            using (var stream = new StreamReader(filename))
            {
                pcol = ProtocolFromStream(stream);
            }
            return pcol;
        }
        public static MetadataCollection ProtocolFromString(string contents)
        {
            MetadataCollection pcol;
            using (var stream = new StringReader(contents))
            {
                pcol = ProtocolFromStream(stream);
            }
            return pcol;
        }
        public static MetadataCollection ProtocolFromStream(TextReader textReader)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(MetadataCollection));
            /*serializer.UnknownNode += new XmlNodeEventHandler(Serializer_UnknownNode);
            serializer.UnknownAttribute += new XmlAttributeEventHandler(Serializer_UnknownAttribute);
            serializer.UnknownElement += new XmlElementEventHandler(Serializer_UnknownElement);*/
            return (MetadataCollection)serializer.Deserialize(textReader);
        }
    }
}
