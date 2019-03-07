using Accord.Video.DirectShow;
using MoSeqAcquire.Models.Acquisition.DirectShow.Internal;
using MoSeqAcquire.Models.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.Models.Acquisition.DirectShow
{
    //[KnownType(typeof(DirectShowConfigSnapshot))]
    public class DirectShowSource : MediaSource
    {
        public DirectShowSource()
        {
            this.Name = "Direct Show Device";
            this.Config = new DirectShowConfig(this);
        }
        public ExVideoCaptureDevice Device { get; protected set; }
        public override List<Tuple<string, string>> ListAvailableDevices()
        {
            var items = new List<Tuple<string, string>>();
            var videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (var device in videoDevices)
            {
                items.Add(new Tuple<string, string>(device.Name, device.MonikerString));
            }
            return items;
        }
        public override bool Initalize(string DeviceId)
        {
            this.DeviceId = DeviceId;
            var videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if(videoDevices.FindAll(fi => fi.MonikerString.Equals(DeviceId)).Count == 0)
            {
                this.Status = "Disconnected";
                return false;
            }
            this.Name = videoDevices.Find(fi => fi.MonikerString.Equals(DeviceId)).Name;
            this.Status = "Initializing";
            this.Device = new ExVideoCaptureDevice(DeviceId);
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
            if (!this.IsInitialized) { return; }
            base.Stop();
            this.Device.SignalToStop();
            this.Device.WaitForStop();
        }
    }
}
