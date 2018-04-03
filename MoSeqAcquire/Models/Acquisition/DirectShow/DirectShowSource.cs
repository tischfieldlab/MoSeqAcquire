using Accord.Video.DirectShow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.Models.Acquisition.DirectShow
{
    public class DirectShowSource : MediaSource
    {
        public DirectShowSource()
        {
            this.Name = "Direct Show Device";
            this.Config = new DirectShowConfig();
        }
        public VideoCaptureDevice Device { get; protected set; }
        public override bool Initalize()
        {
            var videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            this.Device = new VideoCaptureDevice(videoDevices[0].MonikerString);
            if (this.Device == null) { return false; }

            //this.Config.ReadState();
            this.RegisterChannel(new DirectShowVideoChannel(this));
            this.IsInitialized = true;
            return true;
        }
        public override void Start()
        {
            this.Device.Start();
            base.Start();
        }
        public override void Stop()
        {
            this.Device.SignalToStop();
            this.Device.WaitForStop();
            base.Stop();
        }
    }
}
