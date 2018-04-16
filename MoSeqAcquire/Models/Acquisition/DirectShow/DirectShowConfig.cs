using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoSeqAcquire.Models.Configuration;

namespace MoSeqAcquire.Models.Acquisition.DirectShow
{
    public class DirectShowConfig : MediaSourceConfig
    {
        public DirectShowConfig()
        {

        }
        public DirectShowConfig(DirectShowSource Source)
        {

        }
        



        /*public override void ApplySnapshot(ConfigSnapshot snapshot)
        {
            //throw new NotImplementedException();
        }

        public override ConfigSnapshot GetSnapshot()
        {
            return new DirectShowConfigSnapshot();
            //throw new NotImplementedException();
        }*/

        public override void ReadState()
        {
            //throw new NotImplementedException();
        }
    }

    public class DirectShowConfigSnapshot : ConfigSnapshot { }
}
