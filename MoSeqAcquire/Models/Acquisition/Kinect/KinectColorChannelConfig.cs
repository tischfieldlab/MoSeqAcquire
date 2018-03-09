using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.Models.Acquisition.Kinect
{
    public class KinectColorChannelConfig : MediaSourceConfig
    {
        public KinectColorChannelConfig(KinectColorChannel Channel)
        {
            this.Channel = Channel;
        }
        protected KinectColorChannel Channel { get; set; }

        [ConfigurationProperty("brightness", DefaultValue = 0, IsRequired = false)]
        public double Brightness
        {
            get { return (double)this["brightness"]; }
            set
            {
                
                var args = new ConfigChangedEventArgs("ElevationAngle", this["brightness"], value);
                this["brightness"] = value;
                this.NotifyConfigChanged(args);
            }
        }
        [ConfigurationProperty("forceInfraredEmitterOff", DefaultValue = false, IsRequired = false)]
        public bool ForceInfraredEmitterOff {
            get
            {
                return (bool)this["forceInfraredEmitterOff"];
            }
            set
            {
                var args = new ConfigChangedEventArgs("ForceInfraredEmitterOff", this["forceInfraredEmitterOff"], value);
                this["forceInfraredEmitterOff"] = value;
                this.NotifyConfigChanged(args);
            }
        }
    }
}
