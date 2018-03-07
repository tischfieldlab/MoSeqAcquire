using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.Models.Acquisition.Kinect
{
    public class KinectConfig : MediaSourceConfig
    {
        public KinectConfig(KinectManager Kinect)
        {
            this.Kinect = Kinect;
        }
        protected KinectManager Kinect { get; set; }

        [ConfigurationProperty("elevationAngle", DefaultValue = 0, IsRequired = false)]
        public int ElevationAngle
        {
            get
            {
                return (int)this["elevationAngle"];
            }
            set
            {
                var args = new ConfigChangedEventArgs("ElevationAngle", this["elevationAngle"], value);
                this["elevationAngle"] = value;
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
