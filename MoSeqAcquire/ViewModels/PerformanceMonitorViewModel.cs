using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MoSeqAcquire.Models.Acquisition;
using MoSeqAcquire.Models.Performance;

namespace MoSeqAcquire.ViewModels
{
    public class PerformanceMonitorViewModel : BaseViewModel
    {

        public PerformanceMonitorViewModel()
        {
            this.Performances = new ObservableCollection<PerformanceItemViewModel>();
            this.FindPerformanceProviders();
        }

        
        public ObservableCollection<PerformanceItemViewModel> Performances { get; protected set; }
        protected void FindPerformanceProviders()
        {
            new PerformanceMonitorMediaBusAdapter(this);
        }


    }
    public abstract class PerformanceMonitorAdapter
    {
        public PerformanceMonitorAdapter(PerformanceMonitorViewModel Owner)
        {
            this.Owner = Owner;
        }
        protected PerformanceMonitorViewModel Owner { get; set; }
    }

    public class PerformanceMonitorMediaBusAdapter : PerformanceMonitorAdapter
    {
        public PerformanceMonitorMediaBusAdapter(PerformanceMonitorViewModel Owner) : base(Owner)
        {
            MediaBus.Instance.SourcePublished += MediaBus_SourcePublished;
            MediaBus.Instance.SourceUnPublished += MediaBus_SourceUnPublished;
            MediaBus.Instance
                .Sources
                .SelectMany(s => s.Channels)
                .Select(c => new PerformanceItemViewModel() { Name = c.FullName, Performance = c.Performance })
                .ForEach(p => this.Owner.Performances.Add(p));
        }
        private void MediaBus_SourcePublished(object sender, SourceAvailabilityEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                e.Source
                 .Channels
                 .Select(c => new PerformanceItemViewModel() { Name = c.FullName, Performance = c.Performance })
                 .ForEach(p => this.Owner.Performances.Add(p));
            }));
        }
        private void MediaBus_SourceUnPublished(object sender, SourceAvailabilityEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                e.Source
                 .Channels
                 .Select(c => new PerformanceItemViewModel() { Name = c.FullName, Performance = c.Performance })
                 .ForEach(p => this.Owner.Performances.Add(p));
            }));
        }
    }

    public class PerformanceItemViewModel : BaseViewModel
    {
        public string Name { get; set; }
        public IPerformanceProvider Performance { get; set; }
    }
}
