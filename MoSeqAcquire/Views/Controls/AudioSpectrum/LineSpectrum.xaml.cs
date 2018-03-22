using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WinformsVisualization.Visualization;

namespace MoSeqAcquire.Views.Controls.AudioSpectrum
{
    /// <summary>
    /// Interaction logic for LineSpectrum.xaml
    /// </summary>
    public partial class LineSpectrum2 : UserControl
    {
        public static readonly DependencyProperty SpectrumProviderProperty = DependencyProperty.Register("SpectrumProvider", typeof(BasicSpectrumProvider), typeof(LineSpectrum2), new PropertyMetadata(spectrumProviderChanged));

        public LineSpectrum2()
        {
            this.Lines = new ObservableCollection<Line>();
            InitializeComponent();

        }

        public BasicSpectrumProvider SpectrumProvider
        {
            get { return (BasicSpectrumProvider)GetValue(SpectrumProviderProperty); }
            set { SetValue(SpectrumProviderProperty, value); }
        }
        private static void spectrumProviderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = d as LineSpectrum2;
            self.SpectrumProvider.SpectrumUpdated += UpdateSpectrum;
        }

        private static void UpdateSpectrum()
        {
            /*SpectrumPointData[] spectrumPoints = CalculateSpectrumPoints(fftBuffer);

            //connect the calculated points with lines
            for (int i = 0; i < spectrumPoints.Length; i++)
            {
                SpectrumPointData p = spectrumPoints[i];
                int barIndex = p.SpectrumPointIndex;
                double xCoord = BarSpacing * (barIndex + 1) + (_barWidth * barIndex) + _barWidth / 2;

                var p1 = new Point(xCoord, 100);
                var p2 = new Point(xCoord, (100 - p.Value - 1));

                graphics.DrawLine(pen, p1, p2);
            }*/
        }

        protected ObservableCollection<Line> Lines { get; set; }
    }
}
