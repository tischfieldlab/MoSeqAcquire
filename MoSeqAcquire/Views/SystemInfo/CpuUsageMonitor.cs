using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.Views.SystemInfo
{
    public class CpuUsageMonitor : SystemMonitorItemViewModel
    {
        protected PerformanceCounter counter;
        public CpuUsageMonitor()
        {
            this.Title = "Free CPU";
            this.counter = new PerformanceCounter();
            this.counter.CategoryName = "Processor";
            this.counter.CounterName = "% Processor Time";
            this.counter.InstanceName = "_Total";
            this.counter.NextValue();
        }
        public override void Update()
        {
            this.PercentUsage = this.counter.NextValue();
            this.StatusText = (100 - this.PercentUsage).ToString("F1") + "% Free";

            //Alert if there is less free CPU time  than user configured threshold
            this.IsAlert = this.PercentUsage <= Properties.Settings.Default.CPUUsageWarningThreshold;
        }
    }
}
