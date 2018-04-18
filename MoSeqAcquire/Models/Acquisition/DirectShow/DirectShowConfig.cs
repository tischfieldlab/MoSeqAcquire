using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Video.DirectShow;
using MoSeqAcquire.Models.Attributes;
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
            if(this.Source.Device.VideoResolution == null)
            {
                this.ImageFormat = this.Source.Device.VideoCapabilities[0];
            }
        }

        [DisplayName("Image Format")]
        [ChoicesMethod("ImageFormatChoices")]
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
