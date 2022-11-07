using MoSeqAcquire.Models.Triggers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MoSeqAcquire.ViewModels.Commands;
using MoSeqAcquire.ViewModels.Triggers;

namespace MoSeqAcquire.ViewModels.AppSettings
{
    public class SystemSettingsViewModel : BaseComponentSettingsViewModel
    {
        public SystemSettingsViewModel()
        {
            this.Header = "System";

            this.PluginPaths = new ObservableCollection<PluginPathItem>(Properties.Settings.Default.PluginPaths.Cast<string>().Select(p => new PluginPathItem(){ Path = p}));
            this.PluginPaths.CollectionChanged += PluginPaths_CollectionChanged;
        }

        private void PluginPaths_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            e.NewItems?.OfType<PluginPathItem>().ForEach(p => p.PropertyChanged += PluginPathItem_PropertyChanged);
            e.OldItems?.OfType<PluginPathItem>().ForEach(p => p.PropertyChanged -= PluginPathItem_PropertyChanged);
            this.PushPluginPaths();
        }
        private void PluginPathItem_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.PushPluginPaths();
        }
        private void PushPluginPaths()
        {
            Properties.Settings.Default.PluginPaths.Clear();
            Properties.Settings.Default.PluginPaths.AddRange(this.PluginPaths.Select(p => p.Path).ToArray());
            this.NotifyPropertyChanged(nameof(this.PluginPaths));
        }

        public ObservableCollection<PluginPathItem> PluginPaths { get; protected set; }

        public ICommand AddPluginPath => new ActionCommand((param) => this.PluginPaths.Add(new PluginPathItem()));

        public ICommand RemovePluginPath => new ActionCommand((param) => this.PluginPaths.Remove(param as PluginPathItem));



        public double CpuThreshold
        {
            get => Properties.Settings.Default.CPUUsageWarningThreshold;
            set
            {
                Properties.Settings.Default.CPUUsageWarningThreshold = value;
                this.NotifyPropertyChanged();
            }
        }
        public double RamThreshold
        {
            get => Properties.Settings.Default.RamUsageWarningThreshold;
            set
            {
                Properties.Settings.Default.RamUsageWarningThreshold = value;
                this.NotifyPropertyChanged();
            }
        }

        public double MaxRam => new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory / 1e9;


        public double DiskThreshold
        {
            get => Properties.Settings.Default.DiskUsageWarningThreshold;
            set
            {
                Properties.Settings.Default.DiskUsageWarningThreshold = value;
                this.NotifyPropertyChanged();
            }
        }
        public double MaxDisk => new DriveInfo(Path.GetPathRoot(Environment.SystemDirectory)).TotalSize / 1e9;
    }

    public class PluginPathItem : BaseViewModel
    {
        protected string path;
        public string Path
        {
            get => this.path;
            set => this.SetField(ref this.path, value);
        }
    }
}
