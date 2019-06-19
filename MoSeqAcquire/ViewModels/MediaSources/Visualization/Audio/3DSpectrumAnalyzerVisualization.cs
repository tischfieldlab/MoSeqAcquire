using MoSeqAcquire.Views.MediaSources.Visualization;
using System;
using System.Linq;

namespace MoSeqAcquire.ViewModels.MediaSources.Visualization.Audio
{
    class D3SpectrumAnalyzerVisualization : IAudioVisualizationPlugin
    {
        private readonly D3SpectrumAnalyzer spectrumAnalyzer = new D3SpectrumAnalyzer();

        public string Name => "3D Spectrum Analyzer";

        public object Content => spectrumAnalyzer;

        public void OnMaxCalculated(float min, float max)
        {
            // nothing to do
        }

        public void OnFftCalculated(NAudio.Dsp.Complex[] result)
        {
            spectrumAnalyzer.Update(result);
        }
        public void ProcessSample(SampleData data)
        {
            spectrumAnalyzer.Update(data.Fft);
        }
    }
}
