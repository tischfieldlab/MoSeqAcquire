using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using MoSeqAcquire.Models.Utility;

namespace MoSeqAcquire.Models.Acquisition.DirectShow
{
    public class DirectSoundChannel : Channel
    {
        public DirectSoundChannel(IAudioProvider Source)
        {
            this.Device = Source;

            this.Name = "Audio Channel";
            this.DeviceName = Source.Name;
            this.Device.AudioDevice.NewFrame += this.Device_NewFrame;
            this.MediaType = MediaType.Audio;
            this.Enabled = true;
        }

        public IAudioProvider Device { get; protected set; }
        public override bool Enabled { get; set; }

        public override ChannelMetadata Metadata
        {
            get
            {
                return new AudioChannelMetadata()
                {
                    DataType = this.Device.AudioDevice.Format.ToDataType(),
                    SampleFormat = this.Device.AudioDevice.Format.ToSampleFormat(),
                    TargetFramesPerSecond = this.Device.AudioDevice.SampleRate / this.Device.AudioDevice.DesiredFrameSize,
                    SampleRate = this.Device.AudioDevice.SampleRate,
                    Channels = this.Device.AudioDevice.Channels
                };
            }
        }

        private int currentFrameId;
        private void Device_NewFrame(object sender, Accord.Audio.NewFrameEventArgs e)
        {
            if (!this.Enabled)
                return;

            if (e.Signal != null)
            {
                var frame = e.Signal;
                var meta = new AudioChannelFrameMetadata()
                {
                    AbsoluteTime = PreciseDatetime.Now,
                    FrameId = this.currentFrameId++,
                    TotalBytes = e.Signal.RawData.Length,
                    Channels = e.Signal.Channels,
                    SampleRate = e.Signal.SampleRate,
                    SampleCount = e.Signal.Samples,
                };

                switch (e.Signal.SampleFormat.ToSampleFormat())
                {
                    case SampleFormat.IeeeFloat32:
                        this.PostFrame(new ChannelFrame(e.Signal.ToFloat(), meta));
                        break;
                    default:
                        throw new NotSupportedException($"Format {e.Signal.SampleFormat.ToSampleFormat()} not supported");
                }
                
            }
        }
    }

    static class AccordExtensions
    {
        public static Type ToDataType(this Accord.Audio.SampleFormat format)
        {
            switch (format)
            {
                case Accord.Audio.SampleFormat.Format64BitIeeeFloat:
                    return typeof(double);
                case Accord.Audio.SampleFormat.Format32BitIeeeFloat:
                    return typeof(float);
                case Accord.Audio.SampleFormat.Format32Bit:
                    return typeof(int);
                case Accord.Audio.SampleFormat.Format16Bit:
                    return typeof(short);
                case Accord.Audio.SampleFormat.Format8Bit:
                    return typeof(sbyte);
                case Accord.Audio.SampleFormat.Format8BitUnsigned:
                    return typeof(byte);
                default:
                    throw new NotSupportedException($"Format {format} is not supported");
            }
        }

        public static MoSeqAcquire.Models.Acquisition.SampleFormat ToSampleFormat(this Accord.Audio.SampleFormat format)
        {
            switch (format)
            {
                case Accord.Audio.SampleFormat.Format64BitIeeeFloat:
                    return MoSeqAcquire.Models.Acquisition.SampleFormat.IeeeFloat64;
                case Accord.Audio.SampleFormat.Format32BitIeeeFloat:
                    return MoSeqAcquire.Models.Acquisition.SampleFormat.IeeeFloat32;
                case Accord.Audio.SampleFormat.Format32Bit:
                    return MoSeqAcquire.Models.Acquisition.SampleFormat.PCM32;
                case Accord.Audio.SampleFormat.Format16Bit:
                    return MoSeqAcquire.Models.Acquisition.SampleFormat.PCM16;
                //TODO: probably  incorrect to set these as the same format
                case Accord.Audio.SampleFormat.Format8Bit:
                case Accord.Audio.SampleFormat.Format8BitUnsigned:
                    return MoSeqAcquire.Models.Acquisition.SampleFormat.PCM8;
                default:
                    throw new NotSupportedException($"Format {format} is not supported");
            }
        }
    }
}
