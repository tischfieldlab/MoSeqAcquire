using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MoSeqAcquire.Models.Acquisition;
using MoSeqAcquire.ViewModels.MediaSources.Visualization;
using MoSeqAcquire.ViewModels.MediaSources.Visualization.Audio;
using NAudio.Wave;

namespace MoSeqAcquire.ViewModels.MediaSources
{
    public class AudioChannelViewModel : ChannelViewModel
    {

        public AudioChannelViewModel(Channel channel) : base(channel)
        {
            
            this.AvailableViews.Add(new SelectableVisualizationPluginViewModel(new SpectrumAnalyzerVisualization()));
            this.AvailableViews.Add(new SelectableVisualizationPluginViewModel(new PolylineWaveFormVisualization()));
            this.AvailableViews.Add(new SelectableVisualizationPluginViewModel(new PolygonWaveFormVisualization()));
            this.SetChannelViewCommand.Execute(this.AvailableViews.First());

        }

        private BufferedWaveProvider provider;
        private SampleAggregator sampleProvider;
        public override void BindChannel()
        {
            var meta = this.channel.Metadata as AudioChannelMetadata;
            var format = WaveFormat.CreateIeeeFloatWaveFormat((int)meta.TargetFramesPerSecond, meta.Channels);
            this.provider = new BufferedWaveProvider(format)
            {
                DiscardOnBufferOverflow = true
            };
            this.sampleProvider = new SampleAggregator(this.provider.ToSampleProvider())
            {
                PerformFFT = true,
                NotificationCount = (int)meta.TargetFramesPerSecond / 100
            };
            this.sampleProvider.FftCalculated += SampleProvider_FftCalculated;
            this.sampleProvider.MaximumCalculated += SampleProvider_MaximumCalculated;
            
            MediaBus.Instance.Subscribe(bc => bc.Channel == this.channel, new ActionBlock<ChannelFrame>(frame => 
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    this.provider.AddSamples((byte[])frame.FrameData, 0, frame.Metadata.TotalBytes);
                    this.sampleProvider.Read(new float[frame.Metadata.TotalBytes / 4], 0, frame.Metadata.TotalBytes / 4);
                    this.Performance.Increment();
                }));
            }));

        }

        private void SampleProvider_MaximumCalculated(object sender, MaxSampleEventArgs e)
        {
            (this.SelectedView.VisualizationPlugin as IAudioVisualizationPlugin)?.OnMaxCalculated(e.MinSample, e.MaxSample);
        }

        private void SampleProvider_FftCalculated(object sender, FftEventArgs e)
        {
            (this.SelectedView.VisualizationPlugin as IAudioVisualizationPlugin)?.OnFftCalculated(e.Result);
        }
    }

}
