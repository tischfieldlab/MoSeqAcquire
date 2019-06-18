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
            
            this.RegisterViewPlugin(new SpectrumAnalyzerVisualization());
            this.RegisterViewPlugin(new PolylineWaveFormVisualization());
            this.RegisterViewPlugin(new PolygonWaveFormVisualization());
            this.SetChannelViewCommand.Execute(this.AvailableViews.First());

        }

        private BufferedWaveProvider provider;
        private SampleAggregator sampleProvider;
        /*public override void BindChannel()
        {
            var meta = this.channel.Metadata as AudioChannelMetadata;

            //if (meta.SampleFormat == SampleFormat.)

            var format = WaveFormat.CreateIeeeFloatWaveFormat((int)meta.SampleRate, meta.Channels);
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
                this.provider.AddSamples((byte[])frame.FrameData, 0, frame.Metadata.TotalBytes);
                this.sampleProvider.Read(new float[frame.Metadata.TotalBytes / 4], 0, frame.Metadata.TotalBytes / 4);
                this.Performance.Increment();
            }, new ExecutionDataflowBlockOptions() { TaskScheduler = TaskScheduler.FromCurrentSynchronizationContext() }));
        }*/
        public override void BindChannel()
        {
            var meta = this.channel.Metadata as AudioChannelMetadata;

            var sampAgg = new SampleAggregatorDataFlow();
            
            var transform = new TransformManyBlock<ChannelFrame, SampleData>((Func<ChannelFrame, IEnumerable<SampleData>>) sampAgg.ProduceSample);
            var update = new ActionBlock<SampleData>(sample =>
                {
                    
                    (this.SelectedView.VisualizationPlugin as IAudioVisualizationPlugin).ProcessSample(sample);
                    this.Performance.Increment();
                },
                new ExecutionDataflowBlockOptions() { TaskScheduler = TaskScheduler.FromCurrentSynchronizationContext(), EnsureOrdered = true });
            transform.LinkTo(update);

            MediaBus.Instance.Subscribe(bc => bc.Channel == this.channel, transform);
            
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
