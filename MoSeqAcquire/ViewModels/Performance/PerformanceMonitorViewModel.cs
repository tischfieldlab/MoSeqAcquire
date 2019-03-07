using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MoSeqAcquire.Models.Acquisition;
using MoSeqAcquire.Models.Performance;

namespace MoSeqAcquire.ViewModels.Performance
{
    public class PerformanceMonitorViewModel : BaseViewModel
    {

        public PerformanceMonitorViewModel(MoSeqAcquireViewModel Root)
        {
            this.Root = Root;
            this.Performances = new ObservableCollection<PerformanceItemViewModel>();
            this.FindPerformanceProviders();
        }
        public MoSeqAcquireViewModel Root { get; protected set; }
        public ObservableCollection<PerformanceItemViewModel> Performances { get; protected set; }

        protected void FindPerformanceProviders()
        {
            new PerformanceMonitorMediaBusAdapter(this);
            new PerformanceMonitorPreviewAdapter(this);
        }
    }
}
