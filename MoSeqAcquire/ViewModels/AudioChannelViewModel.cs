using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CSCore.DSP;
using MoSeqAcquire.Models.Acquisition;
using WinformsVisualization.Visualization;

namespace MoSeqAcquire.ViewModels
{
    public class AudioChannelViewModel : ChannelViewModel
    {
        protected FftSize fftSize = FftSize.Fft4096;
        protected BasicSpectrumProvider spectrumProvider;
        protected LineSpectrum __lineSpectrum;
        protected RenderTargetBitmap __lineSpectrumBitmap;
        protected VoicePrint3DSpectrum __voicePrint3DSpectrum;
        protected RenderTargetBitmap __voicePrint3DSpectrumBitmap;

        protected long framecount;

        public AudioChannelViewModel(Channel channel) : base(channel)
        {
            //this.VisualHost = new AudioVisualHost();
            this.spectrumProvider = new BasicSpectrumProvider(1, 16000, fftSize);
            this.__lineSpectrum = new LineSpectrum(fftSize)
            {
                SpectrumProvider = spectrumProvider,
                UseAverage = true,
                BarCount = 50,
                BarSpacing = 2,
                IsXLogScale = true,
                ScalingStrategy = ScalingStrategy.Sqrt
            };
            this.__voicePrint3DSpectrum = new VoicePrint3DSpectrum(fftSize)
            {
                SpectrumProvider = spectrumProvider,
                UseAverage = false,
                PointCount = 200,
                IsXLogScale = true,
                ScalingStrategy = ScalingStrategy.Sqrt,
            };
            
            
        }
        public BitmapSource LineSpectrum { get => this.__lineSpectrumBitmap; }
        public BitmapSource VoicePrintSpectrum { get => this.__voicePrint3DSpectrumBitmap; }

        //public AudioVisualHost VisualHost { get; protected set; }

        public override void BindChannel()
        {
            MediaBus.Instance.Subscribe(
                bc => bc.Channel == this.channel,
                new ActionBlock<ChannelFrame>(frame =>
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        var nsamples = frame.FrameData.Length / 2;
                        var buffer = new float[nsamples];
                        for (int i=0; i< nsamples; i++)
                        {
                            buffer[i] = BitConverter.ToInt16((byte[])frame.FrameData, i*2) / 32768f;
                        }
                        spectrumProvider.Add(buffer, nsamples);

                        if(this.__lineSpectrumBitmap == null)
                        {
                            this.__lineSpectrumBitmap = new RenderTargetBitmap(300, 100, 96, 96, PixelFormats.Pbgra32);
                        }
                        this.__lineSpectrum.CreateSpectrumLine(this.__lineSpectrumBitmap, Colors.Green, Colors.Red, Colors.White);

                        if(this.__voicePrint3DSpectrumBitmap == null)
                        {
                            this.__voicePrint3DSpectrumBitmap = new RenderTargetBitmap(300, 100, 96, 96, PixelFormats.Pbgra32);
                        }
                        float xpos = (float)this.framecount % this.__voicePrint3DSpectrumBitmap.PixelWidth;
                        this.__voicePrint3DSpectrum.CreateVoicePrint3D(this.__voicePrint3DSpectrumBitmap, xpos, Colors.Black);

                        this.NotifyPropertyChanged("LineSpectrum");
                        this.NotifyPropertyChanged("VoicePrintSpectrum");
                        this.framecount++;
                    }));
                }));
        }
        
    }

    /*public class AudioVisualHost : FrameworkElement
    {
        private readonly VisualCollection _children;
        public AudioVisualHost()
        {
            this._children = new VisualCollection(this);
        }
        public void AddChild(Visual visual)
        {
            this._children.Add(visual);
        }
        // Provide a required override for the VisualChildrenCount property.
        protected override int VisualChildrenCount => _children.Count;

        // Provide a required override for the GetVisualChild method.
        protected override Visual GetVisualChild(int index)
        {
            if (index < 0 || index >= _children.Count)
            {
                throw new ArgumentOutOfRangeException();
            }

            return _children[index];
        }
    }*/
}
