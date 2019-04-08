using MoSeqAcquire.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MoSeqAcquire.Views
{
    /// <summary>
    /// Interaction logic for DirectoryTreeView.xaml
    /// </summary>
    public partial class SystemInfoView : UserControl
    {

        public SystemInfoView()
        {
            InitializeComponent();
            this.DataContext = new SystemMonitor();
        }

    }


    public class SystemMonitor : BaseViewModel
    {
        protected ObservableCollection<SystemMonitorItem> items;
        protected DispatcherTimer timer;
        public SystemMonitor()
        {
            this.items = new ObservableCollection<SystemMonitorItem>();
            this.Items = new ReadOnlyObservableCollection<SystemMonitorItem>(this.items);
            this.items.Add(new DiskUsageMonitor());
            this.items.Add(new RamUsageMonitor());
            this.items.Add(new CpuUsageMonitor());

            this.timer = new DispatcherTimer();
            this.timer.Interval = TimeSpan.FromMilliseconds(1000);
            this.timer.Tick += CheckTimer_Tick;
            this.timer.Start();
        }

        private void CheckTimer_Tick(object sender, EventArgs e)
        {
            this.items.ForEach(i => i.Update());
        }

        public ReadOnlyObservableCollection<SystemMonitorItem> Items { get; protected set; }
    }


    public abstract class SystemMonitorItem : BaseViewModel
    {
        private double percentUsage;
        private string statusText;
        private string title;
        private bool isAlert;

        public string Title
        {
            get => this.title;
            set => this.SetField(ref this.title, value);
        }
        public double PercentUsage
        {
            get => this.percentUsage;
            set => this.SetField(ref this.percentUsage, value);
        }
        public string StatusText
        {
            get => this.statusText;
            set => this.SetField(ref this.statusText, value);
        }
        public bool IsAlert
        {
            get => this.isAlert;
            set => this.SetField(ref this.isAlert, value);
        }

        public abstract void Update();
    }

    public class DiskUsageMonitor : SystemMonitorItem
    {
        public DiskUsageMonitor()
        {
            this.Title = "Free Disk";
        }
        public override void Update()
        {
            try
            {
                //if (Directory.Exists(SaveFolder))
                //{
                    FileInfo PathInfo = new FileInfo("C:\\");
                    DriveInfo SaveDrive = new DriveInfo(PathInfo.Directory.Root.FullName);
                    Double FreeMem = SaveDrive.AvailableFreeSpace / 1e9;
                    Double AllMem = SaveDrive.TotalSize / 1e9;
                    this.PercentUsage = (AllMem - FreeMem) / AllMem;
                    this.StatusText = String.Format("{0} {1:0.##} / {2:0.##} GB Free", SaveDrive.RootDirectory, FreeMem, AllMem);
                //}
            }
            catch
            {
                this.StatusText = "N/A";
            }
        }
    }
    public class RamUsageMonitor : SystemMonitorItem
    {
        protected PerformanceCounter counter;
        public RamUsageMonitor()
        {
            this.Title = "Free RAM";
            this.counter = new PerformanceCounter();
            this.counter.CategoryName = "Memory";
            this.counter.CounterName = "Available MBytes";
        }
        public override void Update()
        {
            this.PercentUsage = this.counter.NextValue();
            this.StatusText = this.PercentUsage.ToString("F1") + "MB Free";
        }
    }

    public class CpuUsageMonitor : SystemMonitorItem
    {
        protected PerformanceCounter counter;
        public CpuUsageMonitor()
        {
            this.Title = "Free CPU";
            this.counter = new PerformanceCounter();
            this.counter.CategoryName = "Processor";
            this.counter.CounterName = "% Processor Time";
            this.counter.InstanceName = "_Total";
        }
        public override void Update()
        {
            this.PercentUsage = this.counter.NextValue();
            this.StatusText = (100 - this.PercentUsage).ToString("F1") + "% Free";
            if(this.PercentUsage > 50)
            {
                this.IsAlert = true;
            }
        }
    }


}
