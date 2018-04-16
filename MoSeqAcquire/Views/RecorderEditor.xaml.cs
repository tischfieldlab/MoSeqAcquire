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
using MoSeqAcquire.ViewModels.Recording;

namespace MoSeqAcquire.Views
{
    /// <summary>
    /// Interaction logic for RecorderEditor.xaml
    /// </summary>
    public partial class RecorderEditor : Window
    {
        public RecorderEditor(RecorderViewModel recorderViewModel)
        {
            InitializeComponent();
            this.DataContext = new RecorderEditorViewModel();
            (this.DataContext as RecorderEditorViewModel).RecorderViewModel = recorderViewModel;
            if(recorderViewModel != null)
            {

            }
        }
    }


    public class RecorderEditorViewModel : BaseViewModel
    {
        protected int currentStep;
        protected AvailableRecorderTypeViewModel selectedRecorderType;
        public RecorderEditorViewModel(RecorderViewModel recorderViewModel)
        {
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
        public AvailableRecorderTypeViewModel SelectedRecorderType
        {
            get => this.selectedRecorderType;
            set => this.SetField(ref this.selectedRecorderType, value);
        }
        public RecorderViewModel RecorderViewModel { get; set; }
        public ReadOnlyObservableCollection<AvailableRecorderTypeViewModel> AvailableRecorderTypes { get; protected set; }
        protected void PopulateAvailableRecorderTypes()
        {
            this.AvailableRecorderTypes = new ReadOnlyObservableCollection<AvailableRecorderTypeViewModel>(new ObservableCollection<AvailableRecorderTypeViewModel>(ProtocolHelpers.FindRecorderTypes().Select(t => new AvailableRecorderTypeViewModel(t))));
        }
        
    }
}
