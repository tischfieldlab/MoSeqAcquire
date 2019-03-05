using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.Models.Performance
{
    public interface IPerformanceProvider { }
    public interface IFrameRateProvider : IPerformanceProvider
    {
        double FrameRate { get; }
    }
    public interface ITotalFrameCountProvider : IPerformanceProvider
    {
        long TotalFrames { get; }
    }
    public interface IDurationProvider : IPerformanceProvider
    {
        TimeSpan Duration { get; }
    }
    public interface ITimeRemainingProvider : IPerformanceProvider
    {
        TimeSpan? TimeRemaining { get; }
    }
    public interface IProgressProvider : IPerformanceProvider
    {
        double? Progress { get; }
    }
}
