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
    public class AudioSpectrumVis : FrameworkElement
    {
        private VisualCollection _children;
        protected LineSpectrum _lineSpectrum;
        protected VoicePrint3DSpectrum _3DSpectrum;
        protected Timer _timer; 

        public static readonly DependencyProperty SpectrumProviderProperty = DependencyProperty.Register("SpectrumProvider", typeof(BasicSpectrumProvider), typeof(AudioSpectrumVis), new PropertyMetadata(null));
        public static readonly DependencyProperty ShowLineSpectrumProperty = DependencyProperty.Register("ShowLineSpectrum", typeof(Boolean), typeof(AudioSpectrumVis), new PropertyMetadata(false, showLineSpectrumChanged));
        public static readonly DependencyProperty Show3DSpectrumProperty = DependencyProperty.Register("Show3DSpectrum", typeof(Boolean), typeof(AudioSpectrumVis), new PropertyMetadata(false));

        public static readonly DependencyProperty UseAverageProperty = DependencyProperty.Register("UseAverage", typeof(Boolean), typeof(AudioSpectrumVis), new PropertyMetadata(true));
        public static readonly DependencyProperty IsXLogScaleProperty = DependencyProperty.Register("IsXLogScale", typeof(Boolean), typeof(AudioSpectrumVis), new PropertyMetadata(true));
        public static readonly DependencyProperty ScalingStrategyProperty = DependencyProperty.Register("ScalingStrategy", typeof(ScalingStrategy), typeof(AudioSpectrumVis), new PropertyMetadata(ScalingStrategy.Sqrt));


        public AudioSpectrumVis()
        {
            _children = new VisualCollection(this);
            this.MouseDown += AudioSpectrum_MouseDown;
            //this.LoadVisuals();
        }

        private void AudioSpectrum_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var x = 1;
            //throw new NotImplementedException();
        }

        private static void showLineSpectrumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = d as AudioSpectrumVis;
            if ((bool)e.NewValue)
            {
                self._lineSpectrum = new LineSpectrum(self.SpectrumProvider.FftSize)
                {
                    SpectrumProvider = self.SpectrumProvider,
                    UseAverage = true,
                    BarCount = 50,
                    BarSpacing = 2,
                    IsXLogScale = true,
                    ScalingStrategy = ScalingStrategy.Sqrt
                };
                self._children.Add(self._lineSpectrum.Visual);
            }
            else
            {
                self._children.Remove(self._lineSpectrum.Visual);
                self._lineSpectrum = null;
            }
        }
        /*protected void LoadVisuals()
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
            this._children.Add(this._3DSpectrum.Visual);*//*
        }*/
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
