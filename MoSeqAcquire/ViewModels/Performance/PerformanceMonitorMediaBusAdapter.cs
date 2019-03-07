using MoSeqAcquire.Models.Acquisition;
using System;
using System.Linq;
using System.Windows;

namespace MoSeqAcquire.ViewModels.Performance
{
    public class PerformanceMonitorMediaBusAdapter : PerformanceMonitorAdapter
    {
        public PerformanceMonitorMediaBusAdapter(PerformanceMonitorViewModel Owner) : base(Owner)
        {
            MediaBus.Instance.SourcePublished += MediaBus_SourcePublished;
            MediaBus.Instance.SourceUnPublished += MediaBus_SourceUnPublished;
            MediaBus.Instance
                .Sources
                .SelectMany(s => s.Channels)
                .Select(c => this.ItemFactory(c))
                .ForEach(p => this.Owner.Performances.Add(p));
        }
        private PerformanceItemViewModel ItemFactory(Channel channel)
        {
            return new PerformanceItemViewModel()
            {
                Category = "Source",
                Name = channel.FullName,
                Performance = channel.Performance
            };
        }
        private void MediaBus_SourcePublished(object sender, SourceAvailabilityEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                e.Source
                 .Channels
                 .Select(c => this.ItemFactory(c))
                 .ForEach(p => this.Owner.Performances.Add(p));
            }));
        }
        private void MediaBus_SourceUnPublished(object sender, SourceAvailabilityEventArgs e)
        {
            e.Source
            .Channels
            .SelectMany(c => this.Owner.Performances.Where(p => p.Performance == c.Performance))
            .ToList()
            .ForEach(p => Application.Current.Dispatcher.Invoke(() => this.Owner.Performances.Remove(p)));
        }
    }
}
