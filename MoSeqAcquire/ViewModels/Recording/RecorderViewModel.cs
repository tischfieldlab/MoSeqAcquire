using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using MoSeqAcquire.Models.Configuration;
using MoSeqAcquire.Models.Management;
using MoSeqAcquire.Models.Recording;
using MoSeqAcquire.ViewModels.MediaSources;
using Microsoft.Extensions.DependencyInjection;

namespace MoSeqAcquire.ViewModels.Recording
{
    public class RecorderViewModel : BaseViewModel, INotifyDataErrorInfo
    {
        protected MediaWriter writer;

        protected ObservableCollection<RecorderPinViewModel> recorderPins;
        protected ObservableCollection<RecorderProduct> recorderProducts;
        protected ObservableCollection<SelectableChannelViewModel> availableChannels;

        public RecorderViewModel(Type RecorderType)
        {
            var spec = new RecorderSpecification(RecorderType);
            this.writer = spec.Factory() as MediaWriter;
            this.Initialize();
        }
        public RecorderViewModel(ProtocolRecorder Recorder)
        {
            this.writer = MediaWriter.FromProtocolRecorder(Recorder);
            this.Initialize();
        }
        public string Name
        {
            get => this.writer.Name;
            set
            {
                this.writer.Name = value;
                this.NotifyPropertyChanged("Name");
            }
        }
        public MediaWriter Writer { get => this.writer; }
        public BaseConfiguration Settings { get => this.writer.Settings; }
        public RecorderSpecification Specification { get => this.writer.Specification as RecorderSpecification; }
        public MediaWriterStats Performance { get => this.writer.Performance; }

        public ReadOnlyObservableCollection<RecorderPinViewModel> RecorderPins
        {
            get;
            protected set;
        }
        public ReadOnlyObservableCollection<SelectableChannelViewModel> AvailableChannels
        {
            get;
            protected set;
        }
        public ReadOnlyObservableCollection<RecorderProduct> Products
        {
            get;
            protected set;
        }
        public int SelectedChannelCount
        {
            get => this.RecorderPins
                       .SelectMany(rp => rp.SelectedChannels)
                       .Where((cvm) => { return cvm != null; })
                       .Count();
        }
        


        protected void Initialize()
        {
            if (this.Name == null)
            {
                this.Name = App.Current
                               .Services
                               .GetService<RecordingManagerViewModel>()
                               .GetNextDefaultRecorderName();
            }

            this.availableChannels = new ObservableCollection<SelectableChannelViewModel>();
            this.AvailableChannels = new ReadOnlyObservableCollection<SelectableChannelViewModel>(this.availableChannels);

            App.Current
               .Services
               .GetService<MediaSourceCollectionViewModel>()
               .Items
               .SelectMany(s => s.Channels.Select(c => new SelectableChannelViewModel(c)))
               .ForEach(scvm => this.availableChannels.Add(scvm));

            this.recorderProducts = new ObservableCollection<RecorderProduct>();
            this.Products = new ReadOnlyObservableCollection<RecorderProduct>(this.recorderProducts);

            this.recorderPins = new ObservableCollection<RecorderPinViewModel>();
            this.RecorderPins = new ReadOnlyObservableCollection<RecorderPinViewModel>(this.recorderPins);

            foreach (var wp in this.writer.Pins.Values)
            {
                RecorderPinViewModel pin = RecorderPinViewModel.Factory(wp);
                if (pin != null)
                {
                    pin.PropertyChanged += (s,e) => this.updateRecorderProducts();
                    pin.ErrorsChanged += (s, e) => { this.ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(null)); };
                    this.recorderPins.Add(pin);
                }
            }
            this.updateRecorderProducts();
        }

        private void updateRecorderProducts()
        {
            this.recorderProducts.Clear();
            this.writer
                .GetChannelFileMap()
                .Select(kvp => new RecorderProduct()
                {
                    Name = kvp.Key,
                    Channels = kvp.Value.Select(c => ChannelViewModel.FromChannel(c))
                }).ForEach(rp => this.recorderProducts.Add(rp));
            this.NotifyPropertyChanged(null);
        }
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
        public string Error
        {
            get => string.Join(" ", this.GetErrors(null).Cast<string>());
        }
        public bool HasErrors
        {
            get => !string.IsNullOrEmpty(this.Error);
        }
        public string this[string columnName]
        {
            get { return this.GetErrors(columnName).Cast<string>().FirstOrDefault(); }
        }

        public IEnumerable GetErrors(string propertyName)
        {
            var errors = new List<string>();
            errors.AddRange(this.RecorderPins.SelectMany(mwp => mwp.GetErrors(propertyName).Cast<string>()));
            return errors;
        }


        public ProtocolRecorder GetRecorderDefinition()
        {
            return this.writer.GetProtocolRecorder();
        }
    }
}
