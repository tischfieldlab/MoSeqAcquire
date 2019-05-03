using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using MoSeqAcquire.Models.Management;
using MoSeqAcquire.ViewModels.Commands;

namespace MoSeqAcquire.ViewModels.Recording
{
    public class RecorderEditorViewModel : BaseViewModel
    {
        protected bool isNewRecorder;
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
            if (recorderViewModel != null)
            {
                this.RecorderViewModel = recorderViewModel;
                this.CurrentStep = 1;
                this.IsNewRecorder = false;
            }
            else
            {
                this.CurrentStep = 0;
                this.IsNewRecorder = true;
            }
        }
        public string Header
        {
            get => this.RecorderViewModel is null ? "Select New Recorder Type" : "Configure " + this.RecorderViewModel.Specification.DisplayName;
        }
        public PackIconKind ContinueIcon
        {
            get => this.CurrentStep == 1 ? PackIconKind.Check : PackIconKind.ArrowRightBold;
        }
        public string ContinueHelp
        {
            get
            {
                if(this.CurrentStep == 1)
                {
                    return "Finish";
                }
                return "Continue";
            }
        }
        public bool IsNewRecorder
        {
            get => this.isNewRecorder;
            set => this.SetField(ref this.isNewRecorder, value);
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
            this.AvailableRecorderTypes = new ReadOnlyObservableCollection<AvailableRecorderTypeViewModel>(new ObservableCollection<AvailableRecorderTypeViewModel>(ProtocolHelpers.FindRecorderTypes().Select(t => new AvailableRecorderTypeViewModel(t.ComponentType))));
        }

        public ICommand CancelCommand => new ActionCommand((param) => { this.CancelRequested?.Invoke(this, new EventArgs()); });
        public ICommand NextCommand => new ActionCommand(
            (param) =>
            {
                if (this.CurrentStep == 0)
                {
                    if (this.selectedRecorderType != null)
                    {
                        this.RecorderViewModel = new RecorderViewModel(this.rootViewModel, this.selectedRecorderType);
                        this.CurrentStep = 1;
                        this.NotifyPropertyChanged(null);
                    }
                }
                else if (this.CurrentStep == 1)
                {
                    if (!this.rootViewModel.Recorder.Recorders.Contains(this.RecorderViewModel))
                    {
                        //only add if was not in the collection already
                        this.rootViewModel.Recorder.AddRecorder(this.RecorderViewModel);
                    }
                    this.Completed?.Invoke(this, new EventArgs());
                }
            },
            (param) =>
            {
                if (this.CurrentStep == 0)
                {
                    return this.selectedRecorderType != null;
                }
                else if (this.CurrentStep == 1)
                {
                    return true;
                }
                return false;
            }
        );
    }
}
