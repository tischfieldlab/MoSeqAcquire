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
                Provider = (this.DataContext as NewMediaSourceDialogViewModel).SelectedProvider,
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
        
        protected Type selectedProvider;
        protected DeviceItemViewModel selectedDevice;
        protected IEnumerable<DeviceItemViewModel> availableDevices;

        public NewMediaSourceDialogViewModel()
        {
            this.AvailableProviders = ProtocolHelpers.FindProviderTypes();
        }
        public IEnumerable<Type> AvailableProviders { get; protected set; }
        public Type SelectedProvider
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
            if(this.selectedProvider != null)
            {
                var provider = (MediaSource)Activator.CreateInstance(this.selectedProvider, new object[] { });
                this.AvailableDevices = provider.ListAvailableDevices().Select(i => new DeviceItemViewModel(i.Item1, i.Item2));
            }
        }
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
