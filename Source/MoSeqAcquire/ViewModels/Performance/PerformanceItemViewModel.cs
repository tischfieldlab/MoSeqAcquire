using MoSeqAcquire.Models.Performance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.ViewModels.Performance
{
    public class PerformanceItemViewModel : BaseViewModel
    {
        public string Category { get; set; }
        public string Name { get; set; }
        public IPerformanceProvider Performance { get; set; }
    }
}
