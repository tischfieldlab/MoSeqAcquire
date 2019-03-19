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
            get => this.source.Device.VideoResolution.ToString();
            set
            {
                this.source.Device.VideoResolution = this.source.Device.VideoCapabilities
                                                                       .Where(vc => vc.ToString().Equals(value))
                                                                       .First();
            }
        }
        public override bool IsAutomatic
        {
            get => false;
            set { return; }
        }

        public override void ResetValue()
        {
            return;
        }

        protected override PropertyCapability ReadCapability()
        {
            var cpc = new ChoicesPropertyCapability()
            {
                IsSupported = true,
                AllowsAuto = false,
                Choices = this.source.Device.VideoCapabilities.Select(vc => vc.ToString())
            };
            cpc.Default = cpc.Choices.Last();
            return cpc;
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
