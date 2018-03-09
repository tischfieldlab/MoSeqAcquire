using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.Models.Acquisition.Kinect
{
    public class KinectDepthChannelConfig : MediaSourceConfig
    {
        public KinectDepthChannelConfig(KinectDepthChannel Channel)
        {
            this.Channel = Channel;
        }
        protected KinectDepthChannel Channel { get; set; }

        [ConfigurationProperty("range", DefaultValue = DepthRange.Near, IsRequired = true)]
        public DepthRange Range
        {
            get
            {
                return (DepthRange)this["range"];
            }
            set
            {
                var args = new ConfigChangedEventArgs("range", this["range"], value);
                this["range"] = value;
                this.NotifyConfigChanged(args);
            }
        }
    }
}
