using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MaterialDesignThemes.Wpf;
using MoSeqAcquire.ViewModels.Recording;
using MoSeqAcquire.Views.RecorderViews;

namespace MoSeqAcquire.ViewModels.Commands
{
    public class ShowRecordingErrorDialogCommand : BaseCommand
    {
        public ShowRecordingErrorDialogCommand(MoSeqAcquireViewModel ViewModel) : base(ViewModel) { }

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override void Execute(object parameter)
        {
            RecordingErrorData red;
            if (parameter is RecordingErrorData)
            {
                red = parameter as RecordingErrorData;
            }
            else if (parameter is Exception)
            {
                red = new RecordingErrorData(parameter as Exception);
            }
            else
            {
                throw new ArgumentException("Parameter is not a valid recording error!");
            }

            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                DialogHost.Show(new RecordingError(red), "MainWindowDialogHost");
            }));
        }
    }
}
