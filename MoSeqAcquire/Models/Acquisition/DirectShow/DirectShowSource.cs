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
            var videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
             
            this.Device = new VideoCaptureDevice(DeviceId);
            if (this.Device == null || this.Device.SourceObject == null) { return false; }

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
