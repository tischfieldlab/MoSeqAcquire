using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MoSeqAcquire.Models.Core;

namespace MoSeqAcquire.Views.Controls.Visualization.Core
{
    public abstract class BaseAxis : ObservableObject
    {
        private double _scale;
        protected readonly Canvas _targetCanvas;

        public event EventHandler<EventArgs> LabelsGenerated;

        protected BaseAxis(Canvas TargetCanvas)
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
                if (this.SetField(ref this._scale, value))
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

            this.LabelsGenerated?.Invoke(this, new EventArgs());
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
}
