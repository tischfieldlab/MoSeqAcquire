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

            var cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;
            var listener = Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    // poll hardware
                    if (this.Enabled)
                    {
                        this.PostFrame(new ChannelFrame(this.Device.Device.Receive<float>()));
                    }

                    if (token.IsCancellationRequested)
                    {
                        break;
                    }
                }
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
