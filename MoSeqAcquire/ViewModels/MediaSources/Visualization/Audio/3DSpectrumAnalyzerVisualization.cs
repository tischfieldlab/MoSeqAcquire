using MoSeqAcquire.Views.MediaSources.Visualization;
using System;
using System.Linq;

namespace MoSeqAcquire.ViewModels.MediaSources.Visualization.Audio
{
    class D3SpectrumAnalyzerVisualization : IAudioVisualizationPlugin
    {
        private readonly D3SpectrumAnalyser spectrumAnalyser = new D3SpectrumAnalyser();

        public string Name => "Spectrum Analyser";

        public object Content => spectrumAnalyser;

        public void OnMaxCalculated(float min, float max)
        {
            // nothing to do
        }

        public void OnFftCalculated(NAudio.Dsp.Complex[] result)
        {
            spectrumAnalyser.Update(result);
        }
        public void ProcessSample(SampleData data)
        {
            spectrumAnalyser.Update(data.Fft);
        }
    }
}
