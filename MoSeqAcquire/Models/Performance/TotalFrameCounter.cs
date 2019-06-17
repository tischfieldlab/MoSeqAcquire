using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using MoSeqAcquire.Models.Core;
using MoSeqAcquire.Models.Performance;
using MoSeqAcquire.Models.Utility;

namespace MoSeqAcquire.Models.Recording
{
    public class TotalFrameCounter : ObservableObject, IFrameRateProvider, ITotalFrameCountProvider
    {
        private DateTime _lastTime;
        private long _totalCount;
        private long _countSinceLast;
        private static readonly object lockobject = new object();

        public TotalFrameCounter()
        {
            this._lastTime = PreciseDatetime.Now;
        }
        public void Start()
        {
            OracleTimer.Elapsed += this.Compute_framerate;
        }
        public void Stop()
        {
            OracleTimer.Elapsed -= this.Compute_framerate;
        }

        private void Compute_framerate(object sender, EventArgs e)
        {
            lock (lockobject)
            {
                var now = PreciseDatetime.Now;
                this.FrameRate = this._countSinceLast / (now - this._lastTime).TotalSeconds;
                this._totalCount += this._countSinceLast;
                this._countSinceLast = 0;
                this._lastTime = now;
                this.NotifyPropertyChanged(null);
            }
        }

        public void Increment()
        {
            lock (lockobject)
            {
                this._countSinceLast++;
            }
        }
        public double FrameRate { get; protected set; }
        public long TotalFrames { get => this._totalCount; }
    }
}
