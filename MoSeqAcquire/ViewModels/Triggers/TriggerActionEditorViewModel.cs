using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using MoSeqAcquire.Models.Management;
using MoSeqAcquire.ViewModels.Triggers;
using MoSeqAcquire.ViewModels.Commands;

namespace MoSeqAcquire.ViewModels.Triggers
{
    public class TriggerActionEditorViewModel : BaseViewModel
    {
        protected bool isNewTrigger;
        protected int currentStep;
        protected MoSeqAcquireViewModel rootViewModel;
        protected TriggerBindingViewModel triggerBindingViewModel;
        protected TriggerActionViewModel triggerActionViewModel;
        protected Type selectedTriggerActionType;

        public event EventHandler CancelRequested;
        public event EventHandler Completed;

        public TriggerActionEditorViewModel(MoSeqAcquireViewModel rootViewModel, TriggerBindingViewModel triggerBindingViewModel, TriggerActionViewModel triggerActionViewModel = null)
        {
            this.rootViewModel = rootViewModel;
            this.triggerBindingViewModel = triggerBindingViewModel;
            this.PopulateAvailableTypes();
            if (triggerActionViewModel != null)
            {
                this.TriggerActionViewModel = triggerActionViewModel;
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
            get => this.TriggerActionViewModel is null ? "Select New Trigger Action Type" : "Configure " + this.TriggerActionViewModel.Specification.DisplayName;
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
        public TriggerActionViewModel TriggerActionViewModel
        {
            get => this.triggerActionViewModel;
            set => this.SetField(ref this.triggerActionViewModel, value);
        }
        public ReadOnlyObservableCollection<AvailableActionTypeViewModel> AvailableActionTypes { get; protected set; }
        protected void PopulateAvailableTypes()
        {
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
                        this.TriggerActionViewModel = new TriggerActionViewModel(this.selectedTriggerActionType, this.triggerBindingViewModel);
                        this.CurrentStep = 1;
                        this.NotifyPropertyChanged(null);
                    }
                }
                else if (this.CurrentStep == 1)
                {
                    if (!this.triggerBindingViewModel.Actions.Contains(this.TriggerActionViewModel))
                    {
                        //only add if was not in the collection already
                        this.triggerBindingViewModel.Actions.Add(this.TriggerActionViewModel);
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
