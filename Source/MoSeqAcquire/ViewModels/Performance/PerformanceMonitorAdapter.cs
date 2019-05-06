using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.ViewModels.Performance
{
    public abstract class PerformanceMonitorAdapter
    {
        public PerformanceMonitorAdapter(PerformanceMonitorViewModel Owner)
        {
            this.Owner = Owner;
        }
        protected PerformanceMonitorViewModel Owner { get; set; }
    }
}
