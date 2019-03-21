using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CSCore.DSP;

namespace WinformsVisualization.Visualization
{
    public class LineSpectrum : SpectrumBase
    {
        private int _barCount;
        private double _barSpacing;
        private double _barWidth;
        private Size _currentSize;
        private readonly DrawingVisual visual;

        public LineSpectrum(FftSize fftSize)
        {
            FftSize = fftSize;
            this.visual = new DrawingVisual();
            this.background = Colors.White;
            this.brush = new SolidColorBrush(Colors.Green);
        }
        public DrawingVisual Visual
        {
            get => this.visual;
        }

        [Browsable(false)]
        public double BarWidth
        {
            get { return _barWidth; }
        }

        public double BarSpacing
        {
            get { return _barSpacing; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("value");
                _barSpacing = value;
                UpdateFrequencyMapping();

                RaisePropertyChanged("BarSpacing");
                RaisePropertyChanged("BarWidth");
            }
        }

        public int BarCount
        {
            get { return _barCount; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("value");
                _barCount = value;
                SpectrumResolution = value;
                UpdateFrequencyMapping();

                RaisePropertyChanged("BarCount");
                RaisePropertyChanged("BarWidth");
            }
        }

        [BrowsableAttribute(false)]
        /*public Size CurrentSize
        {
            get { return _currentSize; }
            protected set
            {
                _currentSize = value;
                RaisePropertyChanged("CurrentSize");
            }
        }*/
        protected Color background;
        public Color Background
        {
            get => this.background;
            set => this.background = value;
        }
        protected Brush brush;
        public Brush Brush
        {
            get => this.brush;
            set => this.brush = value;
        }





        private float[] fftBuffer;
        public override void UpdateVisual()
        {
            //var size = this.CurrentSize;
            //if (!UpdateFrequencyMappingIfNessesary(size))
            //    return;

            if(this.fftBuffer == null)
            {
                this.fftBuffer = new float[(int)FftSize];
            }

            //get the fft result from the spectrum provider
            if (SpectrumProvider.GetFftData(this.fftBuffer, this))
            {
                var pen = new Pen(brush, (float)_barWidth);
                using (DrawingContext graphics = this.visual.RenderOpen())
                {
                    CreateSpectrumLineInternal(graphics, pen, this.fftBuffer);
                }
            }
        }

        private void CreateSpectrumLineInternal(DrawingContext graphics, Pen pen, float[] fftBuffer)
        {
            //prepare the fft result for rendering 
            SpectrumPointData[] spectrumPoints = CalculateSpectrumPoints(fftBuffer);

            //connect the calculated points with lines
            for (int i = 0; i < spectrumPoints.Length; i++)
            {
                SpectrumPointData p = spectrumPoints[i];
                int barIndex = p.SpectrumPointIndex;
                double xCoord = BarSpacing * (barIndex + 1) + (_barWidth * barIndex) + _barWidth / 2;

                var p1 = new Point(xCoord, 100);
                var p2 = new Point(xCoord, (100 - p.Value - 1));

                graphics.DrawLine(pen, p1, p2);
            }
        }

        protected override void UpdateFrequencyMapping()
        {
            _barWidth = Math.Max(((_currentSize.Width - (BarSpacing * (BarCount + 1))) / BarCount), 0.00001);
            base.UpdateFrequencyMapping();
        }

        /*private bool UpdateFrequencyMappingIfNessesary(Size newSize)
        {
            if (newSize != CurrentSize)
            {
                CurrentSize = newSize;
                UpdateFrequencyMapping();
            }

            return newSize.Width > 0 && newSize.Height > 0;
        }*/
    }
}