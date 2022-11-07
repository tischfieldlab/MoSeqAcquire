using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using NAudio.Dsp;

namespace MoSeqAcquire.Views.MediaSources.Visualization
{
    /// <summary>
    /// Interaction logic for SpectrumAnalyser.xaml
    /// </summary>
    public partial class SpectrumAnalyzer : UserControl
    {
        private double xScale = 200;
        private int bins = 512; // guess a 1024 size FFT, bins is half FFT size

        private readonly int _fftSize;
        private readonly int _sampleRate;

        public SpectrumAnalyzer(int sampleRate = 44100, int fftSize = 1024)
        {
            this._fftSize = fftSize;
            this._sampleRate = sampleRate;
            InitializeComponent();
            CalculateXScale();
            SizeChanged += SpectrumAnalyzer_SizeChanged;
        }

        void SpectrumAnalyzer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            CalculateXScale();
        }

        private List<TextBlock> freqLabels = new List<TextBlock>();
        private void GenerateFrequencyLabels()
        {
            var maxFreq = (int)this._sampleRate / 2;
            var step = (int)this.CalcFreqLabelInterval(10, maxFreq);
            for (int f = 0; f < bins; f += (bins / (maxFreq / step)))
            {
                if (f == 0)
                    continue; //Skip Zero Hz
               
                var freq = f * (this._sampleRate / this._fftSize);
                TextBlock tb = this.freqLabels.FirstOrDefault(t => t.Tag.Equals(freq));
                if(tb == null)
                {
                    tb = new TextBlock()
                    {
                        Tag = freq,
                        Text = this.FreqToString(freq),
                    };
                    tb.RenderTransform = new RotateTransform(90);
                    this.freqLabels.Add(tb);
                    this.MainCanvas.Children.Add(tb);
                }
                tb.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                //Canvas.SetBottom(tb, ((float)f / (float)this._fftSize) * this.ActualHeight - (tb.DesiredSize.Height / 2));
                Canvas.SetLeft(tb, CalculateXPos(f / binsPerPoint));
                Canvas.SetBottom(tb, 0 + tb.ActualWidth - 10);
                
            }
        }

        private double CalcFreqLabelInterval(int tickCount, double range)
        {
            double unroundedTickSize = range / (tickCount - 1);
            double x = Math.Ceiling(Math.Log10(unroundedTickSize) - 1);
            double pow10x = Math.Pow(10, x);
            double roundedTickRange = Math.Ceiling(unroundedTickSize / pow10x) * pow10x;
            return roundedTickRange;
        }

        private string FreqToString(double freq)
        {
            if (freq > 1000)
            {
                return $"{(freq / 1000):F1} kHz";
            }
            else
            {
                return $"{freq:F0} Hz";
            }
        }

        private void CalculateXScale()
        {
            xScale = ActualWidth / (bins / binsPerPoint);
            GenerateFrequencyLabels();
        }

        private const int binsPerPoint = 2; // reduce the number of points we plot for a less jagged line?
        private int updateCount;

        public void Update(Complex[] fftResults)
        {
            // no need to repaint too many frames per second
            if (updateCount++ % 2 == 0)
            {
                return;
            }

            if (fftResults.Length / 2 != bins)
            {
                bins = fftResults.Length / 2;
                CalculateXScale();
            }
            
            for (int n = 0; n < fftResults.Length / 2; n+= binsPerPoint)
            {
                // averaging out bins
                double yPos = 0;
                for (int b = 0; b < binsPerPoint; b++)
                {
                    yPos += GetYPosLog(fftResults[n+b]);
                }
                AddResult(n / binsPerPoint, yPos / binsPerPoint);
            }
        }

        private double GetYPosLog(Complex c)
        {
            // not entirely sure whether the multiplier should be 10 or 20 in this case.
            // going with 10 from here http://stackoverflow.com/a/10636698/7532
            double intensityDB = 10 * Math.Log10(Math.Sqrt(c.X * c.X + c.Y * c.Y));
            double minDB = -90;
            if (intensityDB < minDB) intensityDB = minDB;
            double percent = intensityDB / minDB;
            // we want 0dB to be at the top (i.e. yPos = 0)
            double yPos = percent * ActualHeight;
            return yPos;
        }

        private void AddResult(int index, double power)
        {
            Point p = new Point(CalculateXPos(index), power);
            if (index >= SpectrumPolyline.Points.Count)
            {
                SpectrumPolyline.Points.Add(p);
            }
            else
            {
                SpectrumPolyline.Points[index] = p;
            }
        }

        private double CalculateXPos(int bin)
        {
            if (bin == 0) return 0;
            return bin * xScale; // Math.Log10(bin) * xScale;
        }
    }
}
