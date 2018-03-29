using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.Models.Performance
{
    public interface IFrameRateProvider
    {
        FrameRateCounter FrameRate { get; }
    }
    public interface ITotalFrameCountProvider
    {
        long TotalFrames { get; }
    }
    public interface IDurationProvider
    {
        TimeSpan Duration { get; }
    }
    public interface IProgressProvider
    {
        double? Progress { get; }
    }
}
