using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using MoSeqAcquire.ViewModels;

namespace MoSeqAcquire.Views.SystemInfo
{
    class SystemMonitorViewModel : BaseViewModel
    {
        protected ObservableCollection<SystemMonitorItemViewModel> items;
        protected DispatcherTimer timer;
        public SystemMonitorViewModel()
        {
            this.items = new ObservableCollection<SystemMonitorItemViewModel>();
            this.Items = new ReadOnlyObservableCollection<SystemMonitorItemViewModel>(this.items);
            this.items.Add(new DiskUsageMonitor());
            this.items.Add(new RamUsageMonitor());
            this.items.Add(new CpuUsageMonitor());

            this.timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(1000)
            };
            this.timer.Tick += CheckTimer_Tick;
            this.timer.Start();
        }

        private void CheckTimer_Tick(object sender, EventArgs e)
        {
            Properties.Settings.Default.Reload();
            this.items.ForEach(i => i.Update());
        }

        public ReadOnlyObservableCollection<SystemMonitorItemViewModel> Items { get; protected set; }
    }
}
