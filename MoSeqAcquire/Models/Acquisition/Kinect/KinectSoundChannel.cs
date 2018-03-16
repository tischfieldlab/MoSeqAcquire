using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Windows.Media;
using Microsoft.Kinect;

namespace MoSeqAcquire.Models.Acquisition.Kinect
{
    public class KinectSoundChannel : KinectChannel
    {
        public KinectSoundChannel(KinectManager Kinect) : base(Kinect)
        {
            this.Name = "Kinect Sound Channel";
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

        public Stream InnerStream { get; protected set; }

        protected bool __isEnabled;
        public override bool Enabled
        {
            get => this.__isEnabled;
            set
            {
                if (this.Enabled)
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
                    this.readingThread = new Thread(this.AudioReadingThread);
                    this.readingThread.Start();
                }
                //this.Kinect.Config.ReadState();
            }
        }
        

        private byte[] __data;
        private Thread readingThread;
        private void AudioReadingThread()
        {
            while (this.Enabled)
            {
                int readCount = this.InnerStream.Read(this.__data, 0, this.__data.Length);                
                this.Buffer.Post(new ChannelFrame(this.__data));
            }
        }
    }
}
