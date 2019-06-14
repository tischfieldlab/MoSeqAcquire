using MoSeqAcquire.Views.MediaSources.Visualization;

namespace MoSeqAcquire.ViewModels.MediaSources.Visualization.Audio
{
    class PolylineWaveFormVisualization : IAudioVisualizationPlugin
    {
        private readonly PolylineWaveFormControl polylineWaveFormControl = new PolylineWaveFormControl();

        public string Name => "Polyline WaveForm Visualization";

        public object Content => polylineWaveFormControl;

        public void OnMaxCalculated(float min, float max)
        {
            polylineWaveFormControl.AddValue(max, min);
        }

        public void OnFftCalculated(NAudio.Dsp.Complex[] result)
        {
            // nothing to do
        }
        public void ProcessSample(SampleData data)
        {
            polylineWaveFormControl.AddValue(data.MinSample, data.MaxSample);
        }
    }
}
