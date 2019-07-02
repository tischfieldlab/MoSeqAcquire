using MoSeqAcquire.Models.Core;
using MoSeqAcquire.Models.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MoSeqAcquire.Views.Controls.Visualization.Data
{
    public abstract class BaseChart : UserControl
    {
        public static readonly Size _infinateSize = new Size(double.PositiveInfinity, double.PositiveInfinity);

        protected BaseChart()
        {
            this.SizeChanged += this.BaseChart_SizeChanged;
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

        private void BaseChart_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //this.XAxis.ActualSize = e.NewSize.Width;
            //this.YAxis.ActualSize = e.NewSize.Height;
        }

    }
    public abstract class BaseAxis : ObservableObject
    {
        private double _scale;
        protected readonly Canvas _targetCanvas;
        public BaseAxis(Canvas TargetCanvas)
        {
            this._targetCanvas = TargetCanvas;

            this._targetCanvas.SizeChanged += _targetCanvas_SizeChanged;
        }

        private void _targetCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.GenerateLabels();
        }

        public double Scale
        {
            get => this._scale;
            protected set
            {
                if(this.SetField(ref this._scale, value))
                    this.GenerateLabels();
            }
        }

        private readonly List<TextBlock> _labels = new List<TextBlock>();
        private void GenerateLabels()
        {
            this.ResetLabels();
            var step = this.CalcFreqLabelInterval(5, this.Max);
            for (double f = 0; f <= this.Max/*+ step-1*/; f += step)
            {
                this.RenderLabel(f);
                if (f != 0 && this.HasNegatives)
                    this.RenderLabel(-f);
            }
        }
        private void RenderLabel(double value)
        {
            TextBlock tb = this._labels.FirstOrDefault(t => t.Tag.Equals(value));
            if (tb == null)
            {
                tb = new TextBlock()
                {
                    Tag = value,
                    Text = value.ToString("F1"),
                };
                this._labels.Add(tb);
                this._targetCanvas.Children.Add(tb);
            }
            tb.Measure(BaseChart._infinateSize);
            var pos = this.GetLabelPosition(value);
            Canvas.SetLeft(tb, pos.Item1);
            Canvas.SetTop(tb, pos.Item2 - tb.ActualHeight);
        }
        public void ResetLabels()
        {
            foreach (var l in this._labels)
            {
                this._targetCanvas.Children.Remove(l);
            }
            this._labels.Clear();
        }
        private double CalcFreqLabelInterval(int tickCount, double range)
        {
            double unroundedTickSize = range / (tickCount - 1);
            double x = Math.Ceiling(Math.Log10(unroundedTickSize) - 1);
            double pow10x = Math.Pow(10, x);
            double roundedTickRange = Math.Ceiling(unroundedTickSize / pow10x) * pow10x;
            return roundedTickRange;
        }








        public abstract double Max { get; }
        public abstract bool HasNegatives { get; }
        public abstract (double, double) GetLabelPosition(double value);
        public abstract (double, double) GetDataPosition(double value);
        public abstract void UpdateExtents(double[] data);
        public abstract void UpdateExtents(float[] data);
    }
    public class XAxis : BaseAxis
    {
        private double _max;

        public XAxis(Canvas TargetCanvas) : base(TargetCanvas)
        {
        }

        public override double Max { get => this._max; }
        public override bool HasNegatives { get => false; }
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
    public class YAxis : BaseAxis
    {
        public override double Max { get => this._curMaxSeen; }
        public override bool HasNegatives { get => this._lastNegativeSeen; }

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

        private readonly ConcurrentCircularBuffer<double> _rangeBuffer = new ConcurrentCircularBuffer<double>(100);
        private readonly ConcurrentCircularBuffer<bool> _negSeenBuffer = new ConcurrentCircularBuffer<bool>(100);
        private bool _lastNegativeSeen;
        private double _curMaxSeen;

        public YAxis(Canvas TargetCanvas) : base(TargetCanvas)
        {
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
