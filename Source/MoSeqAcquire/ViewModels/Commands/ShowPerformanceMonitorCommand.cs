using MoSeqAcquire.ViewModels.Performance;
using MoSeqAcquire.ViewModels.Recording;
using MoSeqAcquire.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.ViewModels.Commands
{
    public class ShowPerformanceMonitorCommand : BaseCommand
    {
        public ShowPerformanceMonitorCommand(MoSeqAcquireViewModel ViewModel) : base(ViewModel)
        {
        }

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override void Execute(object parameter)
        {
            var dialog = new PerformanceMonitor
            {
                DataContext = new PerformanceMonitorViewModel(this.ViewModel)
            };
            dialog.Show();
        }
    }
}
