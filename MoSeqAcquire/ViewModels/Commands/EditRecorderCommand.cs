using MoSeqAcquire.ViewModels.Recording;
using MoSeqAcquire.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MoSeqAcquire.ViewModels.Commands
{
    public class EditRecorderCommand : BaseCommand
    {
        public EditRecorderCommand(MoSeqAcquireViewModel ViewModel) : base(ViewModel)
        {
        }

        public override bool CanExecute(object parameter)
        {
            if (this.ViewModel.Recorder.SelectedRecorder != null)
            {
                return true;
            }
            return false;
        }

        public override void Execute(object parameter)
        {
            RecorderViewModel recorderViewModel = null;
            if (parameter != null && parameter is RecorderViewModel)
            {
                recorderViewModel = parameter as RecorderViewModel;
            }
            else if(this.ViewModel.Recorder.SelectedRecorder != null)
            {
                recorderViewModel = this.ViewModel.Recorder.SelectedRecorder;
            }
            
            var dialog = new RecorderEditor();
            dialog.DataContext = new RecorderEditorViewModel(this.ViewModel, recorderViewModel);
            dialog.ShowDialog();
        }
    }
}
