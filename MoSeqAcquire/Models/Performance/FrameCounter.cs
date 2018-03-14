using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.Models.Performance
{
    class FrameCounter
    {
        private long __totalCount;
        private static readonly object lockobject = new object();

        public FrameCounter()
        {

        }
        public void Increment()
        {
            lock (lockobject)
            {
                this.__totalCount++;
            }
        }

    }
}
