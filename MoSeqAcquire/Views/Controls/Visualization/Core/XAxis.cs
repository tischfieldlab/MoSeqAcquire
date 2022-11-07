using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MoSeqAcquire.Views.Controls.Visualization.Core
{
    public class XAxis : BaseAxis
    {
        private double _max;

        public XAxis(Canvas TargetCanvas) : base(TargetCanvas)
        {
        }

        public override double Max => this._max;
        public override bool HasNegatives => false;

        public override (double, double) GetDataPosition(double value)
        {
            return (this.Scale, this.Scale * value);
        }

        public override (double, double) GetLabelPosition(double value)
        {
            return (this.Scale * value, this._targetCanvas.ActualHeight);
        }

        public override void UpdateExtents(double[] data)
        {
            this._max = data.Length;
            this.Scale = this._targetCanvas.ActualWidth / data.Length;
        }

        public override void UpdateExtents(float[] data)
        {
            this._max = data.Length;
            this.Scale = this._targetCanvas.ActualWidth / data.Length;
        }
    }
}
