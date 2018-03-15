using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CSCore.DSP;

namespace WinformsVisualization.Visualization
{
    public class VoicePrint3DSpectrum : SpectrumBase
    {
        private readonly GradientCalculator _colorCalculator;
        private bool _isInitialized;

        private DrawingVisual visual;

        public VoicePrint3DSpectrum(FftSize fftSize)
        {
            _colorCalculator = new GradientCalculator();
            this.Colors = new Color[] {
                System.Windows.Media.Colors.Black,
                System.Windows.Media.Colors.Blue,
                System.Windows.Media.Colors.Cyan,
                System.Windows.Media.Colors.Lime,
                System.Windows.Media.Colors.Yellow,
                System.Windows.Media.Colors.Red
            };

            FftSize = fftSize;
            this.visual = new DrawingVisual();
        }

        public Color[] Colors
        {
            get { return _colorCalculator.Colors; }
            set
            {
                if (value == null || value.Length <= 0)
                    throw new ArgumentException("value");

                _colorCalculator.Colors = value;
            }
        }

        public int PointCount
        {
            get { return SpectrumResolution; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("value");
                SpectrumResolution = value;

                UpdateFrequencyMapping();
            }
        }

        public void CreateVoicePrint3D(RenderTargetBitmap bitmap, float xPos, Color background, float lineThickness = 1f)
        {
            if (!_isInitialized)
            {
                UpdateFrequencyMapping();
                _isInitialized = true;
            }

            var fftBuffer = new float[(int) FftSize];

            //get the fft result from the spectrumprovider
            if (SpectrumProvider.GetFftData(fftBuffer, this))
            {
                //prepare the fft result for rendering
                SpectrumPointData[] spectrumPoints = CalculateSpectrumPoints(1.0, fftBuffer);

                var pen = new Pen(new SolidColorBrush(background), lineThickness);

                double currentYOffset = bitmap.Height;

                using (DrawingContext graphics = this.visual.RenderOpen())
                {
                    graphics.DrawLine(pen, new Point(xPos, 0), new Point(xPos, bitmap.Height));
                    //render the fft result
                    for (int i = 0; i < spectrumPoints.Length; i++)
                    {
                        SpectrumPointData p = spectrumPoints[i];
                        
                        double pointHeight = bitmap.Height / spectrumPoints.Length;

                        //get the color based on the fft band value
                        pen.Brush = new SolidColorBrush(_colorCalculator.GetColor((float)p.Value));

                        var p1 = new Point(xPos, currentYOffset);
                        var p2 = new Point(xPos, currentYOffset - pointHeight);

                        graphics.DrawLine(pen, p1, p2);

                        currentYOffset -= pointHeight;
                    }
                }
                bitmap.Render(this.visual);
            }
        }
    }
}