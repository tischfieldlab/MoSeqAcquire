using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Video.DirectShow;
using MoSeqAcquire.Models.Configuration;

namespace MoSeqAcquire.Models.Acquisition.DirectShow
{
    public class DirectShowConfig : MediaSourceConfig
    {
        private VideoCapabilities imageFormat;




        public DirectShowConfig(DirectShowSource Source)
        {
            this.Source = Source;
        }
        protected DirectShowSource Source { get; set; }
        public override void ReadState()
        {
            //throw new NotImplementedException();
        }

        public VideoCapabilities ImageFormat
        {
            get => this.imageFormat;
            set => this.SetField(ref this.imageFormat, value, () => { this.Source.Device.VideoResolution = value; });
        }
        public IEnumerable<VideoCapabilities> ImageFormatChoices()
        {
            return this.Source.Device.VideoCapabilities;
        }

        
    }
}
