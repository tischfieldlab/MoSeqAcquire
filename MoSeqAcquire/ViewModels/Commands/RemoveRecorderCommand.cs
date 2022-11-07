using MoSeqAcquire.ViewModels.Recording;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MaterialDesignThemes.Wpf;
using MoSeqAcquire.Views.Controls;

namespace MoSeqAcquire.ViewModels.Commands
{
    public class RemoveRecorderCommand : BaseCommand
    {
        public RemoveRecorderCommand(MoSeqAcquireViewModel ViewModel) : base(ViewModel)
        {
        }

        public override bool CanExecute(object parameter)
        {
            if (this.ViewModel.IsProtocolLocked)
                return false;

            return null != this.GetRecorderViewModel(parameter);
        }

        public override async void Execute(object parameter)
        {
            var rvm = this.GetRecorderViewModel(parameter);
            if (rvm != null)
            {
                var confirmVM = new ConfirmDialogViewModel()
                {
                    Title = "Confirm Remove Recorder",
                    Message = "Are you sure you want to remove this recorder?"
                };
                var result = await DialogHost.Show(confirmVM, "MainWindowDialogHost");

                if ((bool) result)
                {
                    this.ViewModel.Recorder.RemoveRecorder(rvm);
                }
            }
        }
        protected RecorderViewModel GetRecorderViewModel(object parameter)
        {
            RecorderViewModel recorderViewModel = null;
            if (parameter != null && parameter is RecorderViewModel)
            {
                recorderViewModel = parameter as RecorderViewModel;
            }
            else if (this.ViewModel.Recorder.SelectedRecorder != null)
            {
                recorderViewModel = this.ViewModel.Recorder.SelectedRecorder;
            }
            return recorderViewModel;
        }
    }
}
