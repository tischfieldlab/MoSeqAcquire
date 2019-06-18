using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using MoSeqAcquire.Models.Utility;

namespace MoSeqAcquire.Models.Acquisition.DirectShow
{
    public class SynapseCaptureChannel : Channel
    {
        protected Task listenTask;
        //protected bool closeRequested;
        protected CancellationTokenSource tokenSource;

        public SynapseCaptureChannel(SynapseCaptureSource Source) : base()
        {
            this.Device = Source;
            this.Name = "Synapse UDP Channel";
            this.DeviceName = Source.Name;
            this.MediaType = MediaType.Data;

            this.Device.Device.DataRecieved += Device_DataRecieved;
        }

        private void Device_DataRecieved(object sender, SynapseTools.DataReceivedEventArgs e)
        {
            if (this.Enabled)
            {
                var metadata = new DataChannelFrameMetadata()
                {
                    AbsoluteTime = PreciseDatetime.Now,
                    TotalBytes = System.Buffer.ByteLength(e.Data),
                    DataType = e.Data.GetType().GetElementType()
                };
                this.PostFrame(new ChannelFrame(e.Data, metadata));
            }
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
