using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoSeqAcquire.Models.Utility;

namespace MoSeqAcquire.Models.Performance
{
    public static class OracleTimer
    {
        private static readonly MultimediaTimer _timer;

        static OracleTimer()
        {
            _timer = new MultimediaTimer()
            {
                Interval = 1000,
                Resolution = 0
            };
            _timer.Start();
        }

        public static event EventHandler Elapsed
        {
            add => _timer.Elapsed += value;
            remove => _timer.Elapsed -= value;
        }
    }
}
