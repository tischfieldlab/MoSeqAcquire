using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace MoSeqAcquire.Models.Acquisition.DirectShow
{
    public class SynapseCaptureChannel : Channel
    {
        public SynapseCaptureChannel(SynapseCaptureSource Source) : base()
        {
            this.Device = Source;
            this.Name = "Video Channel";
            this.DeviceName = Source.Name;
            this.MediaType = MediaType.Data;
            this.DataType = typeof(float);

            int delay = 100;
            var cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;
            var listener = Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    // poll hardware
                    if (this.Enabled)
                    {
                        this.Buffer.Post(new ChannelFrame(this.Device.Device.Receive<float>()));
                    }

                    Thread.Sleep(delay);
                    if (token.IsCancellationRequested)
                        break;
                }

                // cleanup, e.g. close connection
            }, token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }
        public SynapseCaptureSource Device { get; protected set; }
        public override ChannelMetadata Metadata
        {
            get
            {
                return new ChannelMetadata()
                {
                };
            }
        }

        public override bool Enabled { get; set; }
    }
}
