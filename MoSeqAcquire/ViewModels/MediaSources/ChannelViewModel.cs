using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using MoSeqAcquire.Models.Acquisition;
using MoSeqAcquire.Models.Performance;
using MoSeqAcquire.Models.Recording;
using MoSeqAcquire.ViewModels.Commands;
using MoSeqAcquire.ViewModels.Core;
using MoSeqAcquire.ViewModels.MediaSources.Visualization;

namespace MoSeqAcquire.ViewModels.MediaSources
{
    public abstract class ChannelViewModel : BaseViewModel, IPerformanceProvider
    {
        protected Channel channel;
        protected SizeHelper sizeHelper;
        protected SelectableVisualizationPluginViewModel selectedView;


        protected ChannelViewModel(Channel channel)
        {
            this.channel = channel;
            this.AvailableViews = new ObservableCollection<SelectableVisualizationPluginViewModel>();
            this.BindChannel();
            this.Performance = new TotalFrameCounter();
            this.Performance.Start();

            this.sizeHelper = new SizeHelper(150, 500, 100, 500, 200, 150);
        }
        public abstract void BindChannel();
        
        public Channel Channel { get => this.channel; }
        public string Name { get => this.channel.Name; }
        public string DeviceName { get => this.channel.DeviceName; }
        public string FullName { get => this.channel.FullName; }

        public bool Enabled
        {
            get => this.channel.Enabled;
            set
            {
                this.channel.Enabled = value;
                this.NotifyPropertyChanged();
            }
        }

        public TotalFrameCounter Performance { get; protected set; }

        public SizeHelper DisplaySize { get => this.sizeHelper; }

        public ObservableCollection<SelectableVisualizationPluginViewModel> AvailableViews { get; private set; }
        public SelectableVisualizationPluginViewModel SelectedView
        {
            get => this.selectedView;
            set => this.SetField(ref this.selectedView, value);
        }


        public ICommand SetChannelViewCommand => new ActionCommand((p) =>
        {
            this.AvailableViews.ForEach(av => av.IsSelected = false);
            if (p is SelectableVisualizationPluginViewModel pluginViewModel)
            {
                pluginViewModel.IsSelected = true;
                this.SelectedView = pluginViewModel;
            }
        });


        public static ChannelViewModel FromChannel(Channel channel)
        {
            switch (channel.MediaType)
            {
                case MediaType.Video:
                    return new VideoChannelViewModel(channel);

                case MediaType.Audio:
                    return new AudioChannelViewModel(channel);

                case MediaType.Data:
                    return new DataChannelViewModel(channel);
            }
            throw new InvalidOperationException("Unable to determine correct implementation for channel!");
        }
    }

    public class SelectableVisualizationPluginViewModel : BaseViewModel
    {
        private IVisualizationPlugin plugin;
        private bool isSelected;

        public SelectableVisualizationPluginViewModel(IVisualizationPlugin plugin)
        {
            this.plugin = plugin;
        }
        public IVisualizationPlugin VisualizationPlugin
        {
            get => this.plugin;
            set => this.SetField(ref this.plugin, value);
        }

        public object Visualization
        {
            get => this.plugin.Content;
        }

        public string Name
        {
            get => this.plugin.Name;
        }
        public bool IsSelected
        {
            get => this.isSelected;
            set => this.SetField(ref this.isSelected, value);
        }
    }
}
