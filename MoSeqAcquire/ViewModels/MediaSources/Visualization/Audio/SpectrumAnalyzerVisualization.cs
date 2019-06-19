using MoSeqAcquire.Views.MediaSources.Visualization;
using System;
using System.Linq;

namespace MoSeqAcquire.ViewModels.MediaSources.Visualization.Audio
{
    class SpectrumAnalyzerVisualization : IAudioVisualizationPlugin
    {
        private readonly SpectrumAnalyzer spectrumAnalyzer = new SpectrumAnalyzer();

        public string Name => "2D Spectrum Analyzer";

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
