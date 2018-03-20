using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Media;
using WinformsVisualization.Visualization;

namespace MoSeqAcquire.Views.Controls
{
    public class AudioSpectrum : FrameworkElement
    {
        private VisualCollection _children;
        protected LineSpectrum _lineSpectrum;
        protected VoicePrint3DSpectrum _3DSpectrum;
        protected Timer _timer; 

        public static readonly DependencyProperty SpectrumProviderProperty = DependencyProperty.Register("SpectrumProvider", typeof(BasicSpectrumProvider), typeof(AudioSpectrum), new PropertyMetadata(null));
        public static readonly DependencyProperty ShowLineSpectrumProperty = DependencyProperty.Register("ShowLineSpectrum", typeof(Boolean), typeof(AudioSpectrum), new PropertyMetadata(false));
        public static readonly DependencyProperty Show3DSpectrumProperty = DependencyProperty.Register("Show3DSpectrum", typeof(Boolean), typeof(AudioSpectrum), new PropertyMetadata(false));

        public static readonly DependencyProperty UseAverageProperty = DependencyProperty.Register("UseAverage", typeof(Boolean), typeof(AudioSpectrum), new PropertyMetadata(true));
        public static readonly DependencyProperty IsXLogScaleProperty = DependencyProperty.Register("IsXLogScale", typeof(Boolean), typeof(AudioSpectrum), new PropertyMetadata(true));
        public static readonly DependencyProperty ScalingStrategyProperty = DependencyProperty.Register("ScalingStrategy", typeof(ScalingStrategy), typeof(AudioSpectrum), new PropertyMetadata(ScalingStrategy.Sqrt));


        public AudioSpectrum()
        {
            _children = new VisualCollection(this);
            this.LoadVisuals();
        }
        protected void LoadVisuals()
        {
            if (this.ShowLineSpectrum)
            {
                this._lineSpectrum = new LineSpectrum(this.SpectrumProvider.FftSize)
                {
                    SpectrumProvider = this.SpectrumProvider,
                    UseAverage = true,
                    BarCount = 50,
                    BarSpacing = 2,
                    IsXLogScale = true,
                    ScalingStrategy = ScalingStrategy.Sqrt
                };
                this._children.Add(this._lineSpectrum.Visual);
            }

            /*this._3DSpectrum = new VoicePrint3DSpectrum(this.SpectrumProvider.FftSize)
            {
                SpectrumProvider = this.SpectrumProvider,
                UseAverage = false,
                PointCount = 200,
                IsXLogScale = true,
                ScalingStrategy = ScalingStrategy.Sqrt,
            };
            this._children.Add(this._3DSpectrum.Visual);*/
        }
        #region FrameworkElementOverrides
        protected override int VisualChildrenCount
        {
            get { return _children.Count; }
        }
        protected override Visual GetVisualChild(int index)
        {
            if (index < 0 || index >= _children.Count)
            {
                throw new ArgumentOutOfRangeException();
            }
            return _children[index];
        }
        #endregion

        public BasicSpectrumProvider SpectrumProvider
        {
            get { return (BasicSpectrumProvider)GetValue(SpectrumProviderProperty); }
            set { SetValue(SpectrumProviderProperty, value); }
        }
        public bool ShowLineSpectrum
        {
            get { return (bool)GetValue(ShowLineSpectrumProperty); }
            set { SetValue(ShowLineSpectrumProperty, value); }
        }
        public bool Show3DSpectrum
        {
            get { return (bool)GetValue(Show3DSpectrumProperty); }
            set { SetValue(Show3DSpectrumProperty, value); }
        }
    }
}
