using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MaterialDesignThemes.Wpf;
using MoSeqAcquire.ViewModels;

namespace MoSeqAcquire.Views.SystemInfo
{
    public abstract class SystemMonitorItemViewModel : BaseViewModel
    {
        private double percentUsage;
        private string statusText;
        private string title;
        private PackIconKind icon;
        private bool isAlert;

        public string Title
        {
            get => this.title;
            set => this.SetField(ref this.title, value);
        }
        public double Capacity;
        public double Usage;
        public string Units;

        public PackIconKind Icon
        {
            get => this.icon;
            set => this.SetField(ref this.icon, value);
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
}
