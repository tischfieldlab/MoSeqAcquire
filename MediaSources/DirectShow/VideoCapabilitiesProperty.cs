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
        protected IVideoProvider source;
        public VideoCapabilitiesProperty(IVideoProvider Source)
        {
            this.source = Source;
        }
        public override object Value
        {
            get => this.source.VideoDevice.VideoResolution.ToString();
            set
            {
                this.source.VideoDevice.SignalToStop();
                this.source.VideoDevice.WaitForStop();
                this.source.VideoDevice.VideoResolution = this.source.VideoDevice.VideoCapabilities
                                                                       .Where(vc => vc.ToString().Equals(value))
                                                                       .First();
                this.source.VideoDevice.Start();
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
                Choices = this.source.VideoDevice.VideoCapabilities.Select(vc => vc.ToString())
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
