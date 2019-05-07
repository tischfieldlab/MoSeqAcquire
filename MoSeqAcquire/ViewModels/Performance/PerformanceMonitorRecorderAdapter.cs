using MoSeqAcquire.Models.Acquisition;
using MoSeqAcquire.ViewModels.Recording;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MoSeqAcquire.ViewModels.Performance
{
    public class PerformanceMonitorRecorderAdapter : PerformanceMonitorAdapter
    {
        public PerformanceMonitorRecorderAdapter(PerformanceMonitorViewModel Owner) : base(Owner)
        {
            Owner.Root.Recorder.Recorders.CollectionChanged += Recorders_CollectionChanged;
            Owner.Root.Recorder.Recorders
                .Select(rvm => this.ItemFactory(rvm))
                .ForEach(p => this.Owner.Performances.Add(p));
        }

        private void Recorders_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach(var item in e.NewItems)
                {
                    this.Owner.Performances.Add(this.ItemFactory(item as RecorderViewModel));
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var item in e.OldItems)
                {
                    this.Owner.Performances
                        .Where(p => p.Performance == (item as RecorderViewModel).Performance)
                        .ToList()
                        .ForEach(p => this.Owner.Performances.Remove(p));
                }
            }
        }

        private PerformanceItemViewModel ItemFactory(RecorderViewModel recorderViewModel)
        {
            return new PerformanceItemViewModel()
            {
                Category = "Recorder",
                Name = recorderViewModel.Name,
                Performance = recorderViewModel.Performance
            };
        }
    }
}
