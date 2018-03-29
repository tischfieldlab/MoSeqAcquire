using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoSeqAcquire.Models.Performance;

namespace MoSeqAcquire.Models.IO
{
    public interface IRecordingLengthStrategy : INotifyPropertyChanged, IDurationProvider, IProgressProvider, ITimeRemainingProvider
    {
        event EventHandler TriggerStop;

        void Start();
        void Stop();
    }
}
