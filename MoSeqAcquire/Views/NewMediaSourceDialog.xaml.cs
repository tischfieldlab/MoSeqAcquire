using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MoSeqAcquire.Models.Acquisition;
using MoSeqAcquire.Models.Management;
using MoSeqAcquire.ViewModels;

namespace MoSeqAcquire.Views
{
    /// <summary>
    /// Interaction logic for NewMediaSourceDialog.xaml
    /// </summary>
    public partial class NewMediaSourceDialog : Window
    {
        public NewMediaSourceDialog()
        {
            InitializeComponent();
            this.DataContext = new NewMediaSourceDialogViewModel();
        }
        protected AddMediaSourceDialogResult __dialogResult;
        public AddMediaSourceDialogResult GetDialogResult()
        {
            return this.__dialogResult;
        }
        private void onOkButtonClick(object sender, RoutedEventArgs e)
        {
            this.__dialogResult = new AddMediaSourceDialogResult()
            {
                Provider = (this.DataContext as NewMediaSourceDialogViewModel).SelectedProvider.Type,
                DeviceId = (this.DataContext as NewMediaSourceDialogViewModel).SelectedDevice.DeviceId
            };
            this.DialogResult = true;
            this.Close();
        }

        private void onCancelButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void onRefreshButtonClick(object sender, RoutedEventArgs e)
        {
            (this.DataContext as NewMediaSourceDialogViewModel).RequeryDevices();
        }
    }

    public class AddMediaSourceDialogResult
    {
        public Type Provider { get; set; }
        public string DeviceId { get; set; }
    }

    public class NewMediaSourceDialogViewModel : BaseViewModel
    {
        
        protected ProviderViewModel selectedProvider;
        protected DeviceItemViewModel selectedDevice;
        protected IEnumerable<DeviceItemViewModel> availableDevices;
        protected bool isBusy;

        public NewMediaSourceDialogViewModel()
        {
            this.AvailableProviders = ProtocolHelpers.FindProviderTypes()
                                                     .Select(cs => new ProviderViewModel() {
                                                         Name = cs.DisplayName,
                                                         Type = cs.ComponentType
                                                     });
            this.PropertyChanged += (s, e) =>
            {
                if ("SelectedProvider".Equals(e.PropertyName) || "SelectedDevice".Equals(e.PropertyName))
                {
                    this.NotifyPropertyChanged("IsComplete");
                }
            };
        }
        public bool IsBusy
        {
            get => this.isBusy;
            set => this.SetField(ref this.isBusy, value);
        }
        public bool IsComplete
        {
            get => this.SelectedProvider != null && this.SelectedDevice != null;
        }
        public IEnumerable<ProviderViewModel> AvailableProviders { get; protected set; }
        public ProviderViewModel SelectedProvider
        {
            get => this.selectedProvider;
            set
            {
                this.SetField(ref this.selectedProvider, value);
                this.RequeryDevices();
            }
        }
        public IEnumerable<DeviceItemViewModel> AvailableDevices
        {
            get => this.availableDevices;
            protected set => this.SetField(ref this.availableDevices, value);
        }
        public DeviceItemViewModel SelectedDevice
        {
            get => this.selectedDevice;
            set => this.SetField(ref this.selectedDevice, value);
        }
        public void RequeryDevices()
        {
            this.IsBusy = true;

            Task.Run(() =>
            {
                IEnumerable<DeviceItemViewModel> devices = null;
                if (this.selectedProvider != null)
                {
                    var provider = (MediaSource)Activator.CreateInstance(this.selectedProvider.Type, new object[] { });
                    devices = provider.ListAvailableDevices()
                                      .Where(i => MediaBus.Instance.Sources.Count(ms => ms.DeviceId.Equals(i.Item2)) == 0)
                                      .Select(i => new DeviceItemViewModel(i.Item1, i.Item2));
                }
                Application.Current.Dispatcher.Invoke(() =>
                {
                    this.AvailableDevices = devices;
                    this.IsBusy = false;
                });
            });
        }
    }
    public class ProviderViewModel
    {
        public string Name { get; set; }
        public Type Type { get; set; }
    }
    public class DeviceItemViewModel
    {
        public DeviceItemViewModel(string Name, string DeviceId)
        {
            this.Name = Name;
            this.DeviceId = DeviceId;
        }
        public string Name { get; set; }
        public string DeviceId { get; set; }
    }
}
