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

namespace MoSeqAcquire.ViewModels.Triggers
{
    public class TriggerEventEditorViewModel : BaseViewModel
    {
        protected bool isNewTrigger;
        protected int currentStep;
        protected MoSeqAcquireViewModel rootViewModel;
        protected TriggerEventViewModel triggerEventViewModel;
        protected Type selectedTriggerEventType;

        public event EventHandler CancelRequested;
        public event EventHandler Completed;

        public TriggerEventEditorViewModel(MoSeqAcquireViewModel rootViewModel, TriggerEventViewModel triggerEventViewModel = null)
        {
            this.rootViewModel = rootViewModel;
            this.PopulateAvailableTypes();
            if (triggerEventViewModel != null)
            {
                this.TriggerEventViewModel = triggerEventViewModel;
                this.CurrentStep = 1;
                this.IsNewTrigger = false;
            }
            else
            {
                this.CurrentStep = 0;
                this.IsNewTrigger = true;
            }
        }
        public string Header
        {
            get => this.TriggerEventViewModel is null ? "Select New Trigger Event Type" : "Configure " + this.TriggerEventViewModel.Specification.DisplayName;
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
        public bool IsNewTrigger
        {
            get => this.isNewTrigger;
            set => this.SetField(ref this.isNewTrigger, value);
        }
        public int CurrentStep
        {
            get => this.currentStep;
            set => this.SetField(ref this.currentStep, value);
        }
        public Type SelectedTriggerType
        {
            get => this.selectedTriggerEventType;
            set => this.SetField(ref this.selectedTriggerEventType, value);
        }
        public TriggerEventViewModel TriggerEventViewModel
        {
            get => this.triggerEventViewModel;
            set => this.SetField(ref this.triggerEventViewModel, value);
        }
        public ReadOnlyObservableCollection<AvailableTriggerTypeViewModel> AvailableEventTriggerTypes { get; protected set; }
        protected void PopulateAvailableTypes()
        {
            var oc1 = new ObservableCollection<AvailableTriggerTypeViewModel>(ProtocolHelpers.FindTriggerEvents().Select(t => new AvailableTriggerTypeViewModel(t.ComponentType)));
            this.AvailableEventTriggerTypes = new ReadOnlyObservableCollection<AvailableTriggerTypeViewModel>(oc1);
        }

        public ICommand CancelCommand => new ActionCommand((param) => { this.CancelRequested?.Invoke(this, new EventArgs()); });
        public ICommand NextCommand => new ActionCommand(
            (param) =>
            {
                if (this.CurrentStep == 0)
                {
                    if (this.selectedTriggerEventType != null)
                    {
                        this.TriggerEventViewModel = new TriggerEventViewModel(this.selectedTriggerEventType);
                        this.CurrentStep = 1;
                        this.NotifyPropertyChanged(null);
                    }
                }
                else if (this.CurrentStep == 1)
                {
                    if (!this.rootViewModel.Triggers.HasTriggerEvent(this.TriggerEventViewModel))
                    {
                        //only add if was not in the collection already
                        this.rootViewModel.Triggers.AddTriggerEvent(this.TriggerEventViewModel);
                    }
                    this.Completed?.Invoke(this, new EventArgs());
                }
            },
            (param) =>
            {
                if (this.CurrentStep == 0)
                {
                    return this.selectedTriggerEventType != null;
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
