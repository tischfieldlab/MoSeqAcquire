using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using MoSeqAcquire.Models.Management;
using MoSeqAcquire.ViewModels;
using MoSeqAcquire.ViewModels.Commands;
using MoSeqAcquire.ViewModels.Recording;

namespace MoSeqAcquire.Views
{
    /// <summary>
    /// Interaction logic for RecorderEditor.xaml
    /// </summary>
    public partial class RecorderEditor : Window
    {
        protected RecorderEditorViewModel vm;
        public RecorderEditor(MoSeqAcquireViewModel rootViewModel, RecorderViewModel recorderViewModel)
        {
            InitializeComponent();
            this.vm = new RecorderEditorViewModel(rootViewModel, recorderViewModel);
            this.DataContext = vm;

            this.vm.CancelRequested += Vm_CancelRequested;
            this.vm.Completed += Vm_Completed;

        }

        private void Vm_Completed(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Vm_CancelRequested(object sender, EventArgs e)
        {
            this.Close();
        }
    }


    public class RecorderEditorViewModel : BaseViewModel
    {
        protected int currentStep;
        protected MoSeqAcquireViewModel rootViewModel;
        protected RecorderViewModel recorderViewModel;
        protected Type selectedRecorderType;

        public event EventHandler CancelRequested;
        public event EventHandler Completed;

        public RecorderEditorViewModel(MoSeqAcquireViewModel rootViewModel, RecorderViewModel recorderViewModel)
        {
            this.rootViewModel = rootViewModel;
            this.PopulateAvailableRecorderTypes();
            if(recorderViewModel != null)
            {
                this.RecorderViewModel = recorderViewModel;
                this.CurrentStep = 1;
            }
            else
            {
                this.CurrentStep = 0;
            }

        }
        public string Header
        {
            get
            {
                if(RecorderViewModel is null)
                {
                    return "Select Recorder Type";
                }
                else
                {
                    return "Configure " + this.RecorderViewModel.Specification.DisplayName;
                }
            }
        }
        public int CurrentStep
        {
            get => this.currentStep;
            set => this.SetField(ref this.currentStep, value);
        }
        public Type SelectedRecorderType
        {
            get => this.selectedRecorderType;
            set => this.SetField(ref this.selectedRecorderType, value);
        }
        public RecorderViewModel RecorderViewModel
        {
            get => this.recorderViewModel;
            set => this.SetField(ref this.recorderViewModel, value);
        }
        public ReadOnlyObservableCollection<AvailableRecorderTypeViewModel> AvailableRecorderTypes { get; protected set; }
        protected void PopulateAvailableRecorderTypes()
        {
            this.AvailableRecorderTypes = new ReadOnlyObservableCollection<AvailableRecorderTypeViewModel>(new ObservableCollection<AvailableRecorderTypeViewModel>(ProtocolHelpers.FindRecorderTypes().Select(t => new AvailableRecorderTypeViewModel(t))));
        }

        public ICommand CancelCommand => new ActionCommand((param) => { this.CancelRequested?.Invoke(this, new EventArgs()); });
        public ICommand NextCommand => new ActionCommand(
            (param) =>
            {
                if (this.selectedRecorderType != null)
                {
                    this.RecorderViewModel = new RecorderViewModel(this.rootViewModel, this.selectedRecorderType);
                    this.CurrentStep = 1;
                    this.NotifyPropertyChanged(null);
                }
            },
            (param) =>
            {
                return this.selectedRecorderType != null;
            }
        );
        public ICommand CompleteCommand => new ActionCommand(
            (param) =>
            {
                this.rootViewModel.Recorder.AddRecorder(this.RecorderViewModel);
                this.Completed?.Invoke(this, new EventArgs());
            }
         );
        
    }
}
