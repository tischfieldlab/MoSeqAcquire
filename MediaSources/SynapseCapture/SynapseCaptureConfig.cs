using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoSeqAcquire.Models.Attributes;
using MoSeqAcquire.Models.Configuration;

namespace MoSeqAcquire.Models.Acquisition.DirectShow
{
    public class SynapseCaptureConfig : MediaSourceConfig
    {
        public SynapseCaptureConfig(SynapseCaptureSource Source)
        {
            this.Source = Source;
            
        }
        protected SynapseCaptureSource Source { get; set; }
        public override void ReadState()
        {
            
        }
    }
}
