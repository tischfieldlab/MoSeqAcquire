using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.Views.SystemInfo
{
    public class RamUsageMonitor : SystemMonitorItemViewModel
    {
        protected PerformanceCounter counter;
        public RamUsageMonitor()
        {
            this.Title = "Free RAM";
            this.Icon = MaterialDesignThemes.Wpf.PackIconKind.Memory;

            this.counter = new PerformanceCounter();
            this.counter.CategoryName = "Memory";
            this.counter.CounterName = "Available MBytes";
            this.counter.NextValue();
        }
        public override void Update()
        {
            this.PercentUsage = this.counter.NextValue() / 1000;
            this.StatusText = this.PercentUsage.ToString("F1") + "GB Free";

            //Alert if there is less free CPU time  than user configured threshold
            this.IsAlert = this.PercentUsage <= Properties.Settings.Default.RamUsageWarningThreshold;
        }
    }
}
