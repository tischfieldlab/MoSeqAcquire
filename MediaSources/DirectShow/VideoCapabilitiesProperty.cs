using Accord.Video.DirectShow;
using MoSeqAcquire.Models.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.Models.Acquisition.DirectShow
{
    class VideoCapabilitiesProperty : ComplexProperty
    {
        protected DirectShowSource source;
        public VideoCapabilitiesProperty(DirectShowSource Source)
        {
            this.source = Source;
        }
        public override object Value
        {
            get => this.source.Device.VideoResolution;
            set => this.source.Device.VideoResolution = (VideoCapabilities)value;
        }
        public override bool IsAutomatic
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public override void ResetValue()
        {
            throw new NotImplementedException();
        }

        protected override PropertyCapability ReadCapability()
        {
            return new ChoicesPropertyCapability()
            {
                IsSupported = true,
                AllowsAuto = false,
                Choices = this.source.Device.VideoCapabilities,
                Default = this.source.Device.VideoCapabilities[0]
            };
        }

        protected override void ReadCurrentValue()
        {
            throw new NotImplementedException();
        }
        protected override void PushCurrentValue()
        {
            throw new NotImplementedException();
        }
    }
}
