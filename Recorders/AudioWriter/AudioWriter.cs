using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoSeqAcquire.Models.Acquisition;
using MoSeqAcquire.Models.Attributes;
using MoSeqAcquire.Models.Recording;
using NAudio.Wave;

namespace AudioWriter
{
    [DisplayName("Audio Writer")]
    [SettingsImplementation(typeof(AudioWriterSettings))]
    [SupportedChannelType(MediaType.Audio, ChannelCapacity.Single)]
    public class AudioWriter : MediaWriter
    {
        protected MediaWriterPin audioPin;
        //private DataFlowToWaveProvider _backBuffer;

        private WaveFileWriter _writer;

        public AudioWriter() : base()
        {
            this.audioPin = new MediaWriterPin(MediaType.Audio, ChannelCapacity.Single, this.ActionBlock);
            this.RegisterPin(this.audioPin);
        }



        protected override string Ext => "wav";

        public override void Start()
        {
            var chanMeta = this.audioPin.Channel.Metadata as AudioChannelMetadata;
            //this._backBuffer = new DataFlowToWaveProvider(chanMeta.SampleRate, chanMeta.Channels);
            //MediaFoundationEncoder.EncodeToAac(this._backBuffer, this.FilePath);

            this._writer = new WaveFileWriter(this.FilePath,
                WaveFormat.CreateIeeeFloatWaveFormat(chanMeta.SampleRate, chanMeta.Channels));

            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
            this._writer.Flush();
            this._writer.Close();
            this._writer.Dispose();
        }

        protected void ActionBlock(ChannelFrame frame)
        {
            if (!this.IsRecording) { return; }

            this._writer.WriteSamples(frame.FrameData as float[], 0, frame.FrameData.Length);

            this.Performance.Increment();
        }
    }
}
