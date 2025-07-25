﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using Microsoft.Kinect;

namespace MoSeqAcquire.Models.Acquisition.Kinect360
{
    public class KinectSoundChannel : KinectChannel
    {
        public KinectSoundChannel(KinectManager Kinect) : base(Kinect)
        {
            this.Name = "Audio Channel";
            this.DeviceName = "Microsoft Kinect";
            this.MediaType = MediaType.Audio;
            Kinect.Sensor.AudioSource.BeamAngleChanged += AudioSource_BeamAngleChanged;
            Kinect.Sensor.AudioSource.SoundSourceAngleChanged += AudioSource_SoundSourceAngleChanged;
            this.__data = new byte[50 * 16 * 2]; //AudioPollingInterval * SamplesPerMillisecond * BytesPerSample
        }

        private void AudioSource_SoundSourceAngleChanged(object sender, SoundSourceAngleChangedEventArgs e)
        {
            //do nothing
        }

        private void AudioSource_BeamAngleChanged(object sender, BeamAngleChangedEventArgs e)
        {
            //do nothing
        }
        public override ChannelMetadata Metadata
        {
            get { return null; }
        }

        public Stream InnerStream { get; protected set; }

        protected bool __isEnabled;
        public override bool Enabled
        {
            get => this.__isEnabled;
            set
            {
                if (this.__isEnabled != value)
                {
                    if (this.__isEnabled) //TODO: This logic looks a little fishy....
                    {
                        this.__isEnabled = false;
                        if (null != readingThread)
                        {
                            readingThread.Join();
                        }

                        if (null != this.Kinect.Sensor)
                        {
                            //this.Kinect.Sensor.AudioSource.BeamAngleChanged -= this.AudioSourceBeamChanged;
                            //this.Kinect.Sensor.AudioSource.SoundSourceAngleChanged -= this.AudioSourceSoundSourceAngleChanged;
                            this.Kinect.Sensor.AudioSource.Stop();
                        }
                    }
                    else
                    {
                        this.InnerStream = this.Kinect.Sensor.AudioSource.Start(TimeSpan.MaxValue);
                        this.__isEnabled = true;
                        this.readingThread = new Thread(this.AudioReadingThread)
                        {
                            Name = "Kinect Audio Reading Thread"
                        };
                        this.readingThread.Start();
                    }
                }
                //this.Kinect.Config.ReadState();
            }
        }
        

        private readonly byte[] __data;
        private Thread readingThread;
        private void AudioReadingThread()
        {
            while (this.__isEnabled)
            {
                int readCount = this.InnerStream.Read(this.__data, 0, this.__data.Length);                
                this.PostFrame(new ChannelFrame(this.__data));
            }
        }

        internal override void BindConfig()
        {
            KinectConfig cfg = this.Kinect.Settings as KinectConfig;
            KinectAudioSource kas = this.Kinect.Sensor.AudioSource;

            cfg.RegisterComplexProperty(nameof(cfg.BeamAngleMode), new EnumKinectPropertyItem(kas, nameof(kas.BeamAngleMode)));
            cfg.RegisterComplexProperty(nameof(cfg.ManualBeamAngle), new RangedKinectPropertyItem(kas, nameof(kas.ManualBeamAngle), nameof(KinectAudioSource.MinBeamAngle), nameof(KinectAudioSource.MaxBeamAngle)));
            cfg.RegisterComplexProperty(nameof(cfg.AutomaticGainControlEnabled), new SimpleKinectPropertyItem(kas, nameof(kas.AutomaticGainControlEnabled)));
            cfg.RegisterComplexProperty(nameof(cfg.NoiseSuppression), new SimpleKinectPropertyItem(kas, nameof(kas.NoiseSuppression)));
            cfg.RegisterComplexProperty(nameof(cfg.EchoCancellationMode), new EnumKinectPropertyItem(kas, nameof(kas.EchoCancellationMode)));
            cfg.RegisterComplexProperty(nameof(cfg.EchoCancellationSpeakerIndex), new SimpleKinectPropertyItem(kas, nameof(kas.EchoCancellationSpeakerIndex)));
        }
    }
}
