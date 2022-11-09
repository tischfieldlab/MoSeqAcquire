using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using NAudio.Dsp;

namespace MoSeqAcquire.Views.MediaSources.Visualization
{
    /// <summary>
    /// Interaction logic for SpectrumAnalyzer.xaml
    /// </summary>
    public partial class D3SpectrumAnalyzer : UserControl
    {
        private int _updateCount;

        private readonly int _bins;
        private readonly int _fftSize;
        private readonly int _sampleRate;
        private readonly WriteableBitmap _bitmap;

        public D3SpectrumAnalyzer(int sampleRate=44100, int fftSize=1024)
        {
            this._fftSize = fftSize;
            this._bins = this._fftSize / 2;
            this._sampleRate = sampleRate;

            InitializeComponent();
            this._bitmap = new WriteableBitmap(500, this._bins, 96, 96, PixelFormats.Gray16, null);
            this.spectrum.Source = this._bitmap;
            
            this.SizeChanged += D3SpectrumAnalyzer_SizeChanged;
        }

        private void D3SpectrumAnalyzer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.GenerateFrequencyLabels();
        }

        private void GenerateFrequencyLabels()
        {
            var maxFreq = (int)this._sampleRate / 2;
            var step = (int)this.CalcFreqLabelInterval(10, maxFreq);

            this.FrequencyLabels.Children.Clear();
            for (int f = 0; f <= maxFreq; f += step)
            {
                if (f == 0)
                    continue; //Skip Zero Hz

                //var freq = Math.Round((double)f * (this._sampleRate / this._fftSize / 2.0));
                var tb = new TextBlock()
                {
                    Text = this.FreqToString(f),
                    FontSize = 10,
                };
                tb.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                
                Canvas.SetBottom(tb, this.FreqToYPos(f)- (tb.DesiredSize.Height / 2));
                //Canvas.SetLeft(tb, 10);
                this.FrequencyLabels.Children.Add(tb);
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
                return $"{(freq / 1000):F0} kHz";
            }
            else
            {
                return $"{freq:F0} Hz";
            }
        }
        private double FreqToYPos(double freq)
        {
            var bin = Math.Round(freq / (this._sampleRate / this._fftSize));
            var pct = bin / this._bins;
            return pct * this.ActualHeight;
        }

        private int currX = -1;
        private int nextX = 0;
        public void Update(Complex[] fftResults)
        {
            // no need to repaint too many frames per second
            if (this._updateCount++ % 2 == 0)
            {
                return;
            }

            this.currX = this.GetNextX(this.currX);
            this.nextX = this.GetNextX(this.currX);

            this._bitmap.Lock();
            unsafe
            {
                IntPtr pBackBuffer;

                for (int row = 0; row < this._bins; row++)
                {
                    // Find the address of the pixel to draw.
                    pBackBuffer = this._bitmap.BackBuffer;
                    pBackBuffer += row * this._bitmap.BackBufferStride;
                    pBackBuffer += this.currX * this._bitmap.Format.BitsPerPixel / 8;
                    *((ushort*)pBackBuffer) = GetIntensityLog(fftResults[row]);

                    pBackBuffer = this._bitmap.BackBuffer;
                    pBackBuffer += row * this._bitmap.BackBufferStride;
                    pBackBuffer += this.nextX * this._bitmap.Format.BitsPerPixel / 8;
                    *((ushort*)pBackBuffer) = 0;
                }

                this._bitmap.AddDirtyRect(new Int32Rect(this.currX, 0, 1, this._bitmap.PixelHeight));
                this._bitmap.AddDirtyRect(new Int32Rect(this.nextX, 0, 1, this._bitmap.PixelHeight));
            }
            this._bitmap.Unlock();
        }

        private int GetNextX(int current)
        {
            current += 1;
            if (current >= this._bitmap.PixelWidth)
                return 0;
            return current;
        }

        private ushort GetIntensityLog(Complex c)
        {
            // not entirely sure whether the multiplier should be 10 or 20 in this case.
            // going with 10 from here http://stackoverflow.com/a/10636698/7532
            float intensityDB = 10 * (float)Math.Log10(Math.Sqrt(c.X * c.X + c.Y * c.Y));
            float minDB = -90;
            if (intensityDB < minDB) intensityDB = minDB;
            float percent = intensityDB / minDB;
            // we want 0dB to be at the top (i.e. yPos = 0)
            return (ushort)(percent * ushort.MaxValue);
        }
    }
}
