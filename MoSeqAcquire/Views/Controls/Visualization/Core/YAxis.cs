using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using MoSeqAcquire.Models.Utility;

namespace MoSeqAcquire.Views.Controls.Visualization.Core
{
    public class YAxis : BaseAxis
    {
        private readonly ConcurrentCircularBuffer<double> _rangeBuffer = new ConcurrentCircularBuffer<double>(100);
        private readonly ConcurrentCircularBuffer<bool> _negSeenBuffer = new ConcurrentCircularBuffer<bool>(100);
        private bool _lastNegativeSeen;
        private double _curMaxSeen;

        public YAxis(Canvas TargetCanvas) : base(TargetCanvas)
        {
        }

        public override double Max => this._curMaxSeen;
        public override bool HasNegatives => this._lastNegativeSeen;

        //Returns (Left, Top)
        public override (double, double) GetLabelPosition(double value)
        {
            if (value < 0)
            {
                return (0, (this._targetCanvas.ActualHeight / 2) + Math.Abs(value * this.Scale));
            }
            else
            {
                return (0, (this._targetCanvas.ActualHeight / (this._lastNegativeSeen ? 2 : 1)) - Math.Abs(value * this.Scale));
            }
        }

        //Returns (Height, Top)
        public override (double, double) GetDataPosition(double value)
        {
            double height = Math.Abs(value * this.Scale);
            if (value < 0)
            {
                return (height, (this._targetCanvas.ActualHeight / 2) + height);
            }
            else
            {
                return (height, (this._targetCanvas.ActualHeight / (this._lastNegativeSeen ? 2 : 1)) - height);
            }
        }

        public override void UpdateExtents(double[] data)
        {
            double max = double.MinValue;
            bool wasNegSeen = false;
            foreach (var t in data)
            {
                max = Math.Max(max, Math.Abs(t));
                if (!wasNegSeen && t < 0)
                {
                    wasNegSeen = true;
                }
            }

            this._rangeBuffer.Put(max);
            this._negSeenBuffer.Put(wasNegSeen);

            this._curMaxSeen = this._rangeBuffer.Read().Max();
            this._lastNegativeSeen = this._negSeenBuffer.Read().Any(b => b);

            this.Scale = this._targetCanvas.ActualHeight / (this._curMaxSeen * (this._lastNegativeSeen ? 2 : 1));
        }

        public override void UpdateExtents(float[] data)
        {
            float max = float.MinValue;
            bool wasNegSeen = false;
            foreach (var t in data)
            {
                max = Math.Max(max, Math.Abs(t));
                if (!wasNegSeen && t < 0)
                {
                    wasNegSeen = true;
                }
            }

            this._rangeBuffer.Put(max);
            this._negSeenBuffer.Put(wasNegSeen);
            this._curMaxSeen = this._rangeBuffer.Read().Max();
            this._lastNegativeSeen = this._negSeenBuffer.Read().Any(b => b);

            this.Scale = this._targetCanvas.ActualHeight / (this._curMaxSeen * (this._lastNegativeSeen ? 2 : 1));
        }
    }
}
