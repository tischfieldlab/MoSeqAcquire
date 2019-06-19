using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using NAudio.Dsp;

namespace MoSeqAcquire.Views.MediaSources.Visualization
{
    /// <summary>
    /// Interaction logic for SpectrumAnalyser.xaml
    /// </summary>
    public partial class D3SpectrumAnalyser : UserControl
    {
        private double timeScale = 200;
        private double freqScale = 200;
        private int bins = 512; // guess a 1024 size FFT, bins is half FFT size
        private const int binsPerPoint = 2; // reduce the number of points we plot for a less jagged line?
        private int updateCount;

        private WriteableBitmap bitmap;

        public D3SpectrumAnalyser()
        {
            InitializeComponent();
            CalculateScale();
            SizeChanged += SpectrumAnalyser_SizeChanged;
            this.bitmap = new WriteableBitmap(512, 512, 96, 96, PixelFormats.Gray32Float, null);
            this.spectrum.Source = this.bitmap;
        }

        void SpectrumAnalyser_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            CalculateScale();
        }

        private void CalculateScale()
        {
            timeScale = ActualWidth / (bins / binsPerPoint);
            freqScale = ActualHeight / (bins / binsPerPoint);
        }



        private int x;
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
                CalculateScale();
            }

            this.x++;
            if (this.x > this.bitmap.PixelWidth)
                x = 0;
            

            float[] data = new float[this.bitmap.PixelHeight];
            for (int n = 0; n < fftResults.Length / 2; n+= binsPerPoint)
            {
                // averaging out bins
                float intensity = 0;
                for (int b = 0; b < binsPerPoint; b++)
                {
                    intensity += GetIntensityLog(fftResults[n+b]);
                }
                data[n] = intensity;
            }
            AddResult(x, data);
        }

        private float GetIntensityLog(Complex c)
        {
            // not entirely sure whether the multiplier should be 10 or 20 in this case.
            // going with 10 from here http://stackoverflow.com/a/10636698/7532
            float intensityDB = 10 * (float)Math.Log10(Math.Sqrt(c.X * c.X + c.Y * c.Y));
            float minDB = -90;
            if (intensityDB < minDB) intensityDB = minDB;
            float percent = intensityDB / minDB;
            // we want 0dB to be at the top (i.e. yPos = 0)
            return percent;
        }

        private void AddResult(int timePos, float[] data)
        {
            


            this.bitmap.WritePixels(new Int32Rect(timePos, 0, 1, this.bitmap.PixelHeight), data, this.bitmap.BackBufferStride, 0);
        }

        private double CalculateXPos(int bin)
        {
            if (bin == 0) return 0;
            return bin * timeScale; // Math.Log10(bin) * xScale;
        }
    }
}
