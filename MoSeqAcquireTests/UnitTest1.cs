using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoSeqAcquire.Models.Acquisition.DirectShow;
using MoSeqAcquire.Models.Configuration;
using MoSeqAcquire.Models.Management;
using MoSeqAcquire.Models.Recording;
using MoSeqAcquire.Models.Recording.MPEGVideoWriter;

namespace MoSeqAcquireTests2
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Test_Vanilla()
        {
            var protocol = new Protocol("test");

            var serialized = MediaSettingsWriter.WriteProtocol(protocol);
            Console.Write(serialized);
            var deserialized = MediaSettingsWriter.ProtocolFromString(serialized);

            Assert.AreEqual(protocol, deserialized);
        }

        [TestMethod]
        public void Test_File()
        {
            var f = @"test_protocol1.xml";
            var deserialized = MediaSettingsWriter.ReadProtocol(f);
            var serialized = MediaSettingsWriter.WriteProtocol(deserialized);
            Console.Write(serialized);
           
            Assert.AreEqual(File.ReadAllText(f), serialized);
        }

        [TestMethod]
        public void Test_Src()
        {
            var protocol = new Protocol("test");
            protocol.Sources.Add(typeof(DirectShowSource), "test-device-id", new ConfigSnapshot());

            var serialized = MediaSettingsWriter.WriteProtocol(protocol);
            Console.Write(serialized);
            var deserialized = MediaSettingsWriter.ProtocolFromString(serialized);

            Assert.AreEqual(protocol, deserialized);
        }

        [TestMethod]
        public void Test_Src_Config()
        {
            var protocol = new Protocol("test");
            protocol.Sources.Add(typeof(DirectShowSource), "test-device-id", new ConfigSnapshot());
            protocol.Sources[0].Config.Add("test", 10, false);

            var serialized = MediaSettingsWriter.WriteProtocol(protocol);
            Console.Write(serialized);
            var deserialized = MediaSettingsWriter.ProtocolFromString(serialized);

            Assert.AreEqual(protocol, deserialized);
        }

        [TestMethod]
        public void Test_Rec()
        {
            var protocol = new Protocol("test");
            protocol.Recordings.Recorders.Add(new ProtocolRecorder()
            {
                Name = "test-recorder-1",
                Provider = typeof(MPEGVideoWriter).AssemblyQualifiedName,
                Config = new ConfigSnapshot()
            });

            var serialized = MediaSettingsWriter.WriteProtocol(protocol);
            Console.Write(serialized);
            var deserialized = MediaSettingsWriter.ProtocolFromString(serialized);

            Assert.AreEqual(protocol, deserialized);
        }

        [TestMethod]
        public void Test_Rec_Config()
        {
            var protocol = new Protocol("test");
            var rec = new ProtocolRecorder()
            {
                Name = "test-recorder-1",
                Provider = typeof(MPEGVideoWriter).AssemblyQualifiedName,
                Config = new ConfigSnapshot()
            };
            rec.Config.Add("Bitrate", 400000, false);
            rec.Config.Add("VideoCodec", Accord.Video.FFMPEG.VideoCodec.MPEG4, false);
            protocol.Recordings.Recorders.Add(rec);

            var serialized = MediaSettingsWriter.WriteProtocol(protocol);
            Console.Write(serialized);
            var deserialized = MediaSettingsWriter.ProtocolFromString(serialized);

            Assert.AreEqual(protocol, deserialized);
        }

        [TestMethod]
        public void Test_Rec_GenConfig()
        {
            var protocol = new Protocol("test");
            var gs = new GeneralRecordingSettings
            {
                Basename = "test",
                Directory = "C:\temp",
                RecordingMode = RecordingMode.TimeCount,
                RecordingTime = TimeSpan.FromSeconds(30)
            };
            protocol.Recordings.GeneralSettings = gs.GetSnapshot();
           

            var serialized = MediaSettingsWriter.WriteProtocol(protocol);
            Console.Write(serialized);
            var deserialized = MediaSettingsWriter.ProtocolFromString(serialized);

            Assert.AreEqual(protocol, deserialized);
        }

        [TestMethod]
        public void Test_Rec_GenConfig_rec()
        {
            var protocol = new Protocol("test");
            var gs = new GeneralRecordingSettings
            {
                Basename = "test",
                Directory = "C:\temp",
                RecordingMode = RecordingMode.TimeCount,
                RecordingTime = TimeSpan.FromSeconds(30)
            };
            protocol.Recordings.GeneralSettings = gs.GetSnapshot();


            var serialized = MediaSettingsWriter.WriteProtocol(protocol);
            Console.Write(serialized);
            var deserialized = MediaSettingsWriter.ProtocolFromString(serialized);

            Assert.AreEqual(protocol, deserialized);
        }

        [TestMethod]
        public void Test_Src_Rec()
        {
            var protocol = new Protocol("test");
            protocol.Sources.Add(typeof(DirectShowSource), "test-device-id", new ConfigSnapshot());
            protocol.Recordings.Recorders.Add(new ProtocolRecorder()
            {
                Name = "test-recorder-1",
                Provider = typeof(MPEGVideoWriter).AssemblyQualifiedName,
                Config = new ConfigSnapshot()
            });

            var serialized = MediaSettingsWriter.WriteProtocol(protocol);
            Console.Write(serialized);
            var deserialized = MediaSettingsWriter.ProtocolFromString(serialized);

            Assert.AreEqual(protocol, deserialized);
        }
        [TestMethod]
        public void Test_Src_Rec_w_Pins()
        {
            var protocol = new Protocol("test");
            protocol.Sources.Add(typeof(DirectShowSource), "test-device-id", new ConfigSnapshot());

            var gs = new GeneralRecordingSettings
            {
                Basename = "test",
                Directory = "C:\temp",
                RecordingMode = RecordingMode.TimeCount,
                RecordingTime = TimeSpan.FromSeconds(30)
            };
            protocol.Recordings.GeneralSettings = gs.GetSnapshot();

            var rec = new ProtocolRecorder()
            {
                Name = "test-recorder-1",
                Provider = typeof(MPEGVideoWriter).AssemblyQualifiedName,
                Config = new ConfigSnapshot()
            };
            rec.Pins.Add(new ProtocolRecorderPin() { Name = "Pin_A", Channel = "Channel_1" });
            rec.Pins.Add(new ProtocolRecorderPin() { Name = "Pin_B", Channel = "Channel_2" });
            protocol.Recordings.Recorders.Add(rec);

            var serialized = MediaSettingsWriter.WriteProtocol(protocol);
            Console.Write(serialized);
            var deserialized = MediaSettingsWriter.ProtocolFromString(serialized);

            Assert.AreEqual(protocol, deserialized);
        }
    }
}
