using MoSeqAcquire.Models.Core;
using MoSeqAcquire.Models.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MoSeqAcquire.Views.Controls.Visualization.Core
{
    public abstract class BaseChart : UserControl
    {
        public static readonly Size _infinateSize = new Size(double.PositiveInfinity, double.PositiveInfinity);

        protected BaseChart()
        {
            
        }
        private Canvas _targetCanvas;
        public Canvas TargetCanvas {
            get => this._targetCanvas;
            protected set
            {
                this._targetCanvas = value;
                this.XAxis = new XAxis(value);
                this.YAxis = new YAxis(value);
            }
        }

        public XAxis XAxis { get; protected set; }
        public YAxis YAxis { get; protected set; }
    }
}
