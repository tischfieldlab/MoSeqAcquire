using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MoSeqAcquire.Views.SystemInfo
{
    public class DiskUsageMonitor : SystemMonitorItemViewModel
    {
        public DiskUsageMonitor()
        {
            this.Title = "Free Disk";
            this.Icon = MaterialDesignThemes.Wpf.PackIconKind.Harddisk;
        }
        public override void Update()
        {
            try
            {
                DriveInfo SaveDrive = new DriveInfo(Path.GetPathRoot(Environment.SystemDirectory));
                Double FreeMem = SaveDrive.AvailableFreeSpace / 1e9;
                Double AllMem = SaveDrive.TotalSize / 1e9;
                this.PercentUsage = (AllMem - FreeMem) / AllMem;
                this.StatusText = String.Format("{0} {1:0.##} / {2:0.##} GB Free", SaveDrive.RootDirectory, FreeMem, AllMem);

                //Alert if there is less free disk than user configured threshold
                this.IsAlert = (FreeMem <= Properties.Settings.Default.DiskUsageWarningThreshold);
            
            }
            catch(Exception)
            {
                this.StatusText = "N/A";
            }
        }
    }
}
