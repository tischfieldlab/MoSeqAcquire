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
    public class TriggerEditorViewModel : BaseViewModel
    {
        protected bool isNewTrigger;
        protected int currentStep;
        protected MoSeqAcquireViewModel rootViewModel;
        protected TriggerViewModel triggerViewModel;
        protected Type selectedTriggerActionType;

        public event EventHandler CancelRequested;
        public event EventHandler Completed;

        public TriggerEditorViewModel(MoSeqAcquireViewModel rootViewModel, TriggerViewModel triggerViewModel = null)
        {
            this.rootViewModel = rootViewModel;
            this.PopulateAvailableTypes();
            if (triggerViewModel != null)
            {
                this.TriggerViewModel = triggerViewModel;
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
            get => this.TriggerViewModel is null ? "Select New Trigger Action Type" : "Configure " + this.TriggerViewModel.Specification.DisplayName;
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
            get => this.selectedTriggerActionType;
            set => this.SetField(ref this.selectedTriggerActionType, value);
        }
        public TriggerViewModel TriggerViewModel
        {
            get => this.triggerViewModel;
            set => this.SetField(ref this.triggerViewModel, value);
        }
        public ReadOnlyObservableCollection<AvailableTriggerTypeViewModel> AvailableTriggerTypes { get; protected set; }
        public ReadOnlyObservableCollection<AvailableActionTypeViewModel> AvailableActionTypes { get; protected set; }
        protected void PopulateAvailableTypes()
        {
            var oc1 = new ObservableCollection<AvailableTriggerTypeViewModel>(ProtocolHelpers.FindTriggerEvents().Select(t => new AvailableTriggerTypeViewModel(t.ComponentType)));
            this.AvailableTriggerTypes = new ReadOnlyObservableCollection<AvailableTriggerTypeViewModel>(oc1);

            var oc2 = new ObservableCollection<AvailableActionTypeViewModel>(ProtocolHelpers.FindTriggerActions().Select(t => new AvailableActionTypeViewModel(t.ComponentType)));
            this.AvailableActionTypes = new ReadOnlyObservableCollection<AvailableActionTypeViewModel>(oc2);
        }

        public ICommand CancelCommand => new ActionCommand((param) => { this.CancelRequested?.Invoke(this, new EventArgs()); });
        public ICommand NextCommand => new ActionCommand(
            (param) =>
            {
                if (this.CurrentStep == 0)
                {
                    if (this.selectedTriggerActionType != null)
                    {
                        this.TriggerViewModel = new TriggerViewModel(this.selectedTriggerActionType);
                        this.TriggerViewModel.TriggerType = this.AvailableTriggerTypes.FirstOrDefault().TriggerType;
                        this.CurrentStep = 1;
                        this.NotifyPropertyChanged(null);
                    }
                }
                else if (this.CurrentStep == 1)
                {
                    if (!this.rootViewModel.Triggers.Triggers.Contains(this.TriggerViewModel))
                    {
                        //only add if was not in the collection already
                        this.rootViewModel.Triggers.AddTrigger(this.TriggerViewModel);
                    }
                    this.Completed?.Invoke(this, new EventArgs());
                }
            },
            (param) =>
            {
                if (this.CurrentStep == 0)
                {
                    return this.selectedTriggerActionType != null;
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
