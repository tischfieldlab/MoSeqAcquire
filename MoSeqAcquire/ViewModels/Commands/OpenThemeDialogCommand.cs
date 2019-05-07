using MoSeqAcquire.ViewModels.Recording;
using MoSeqAcquire.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.ViewModels.Commands
{
    public class OpenThemeDialogCommand : BaseCommand
    {
        public OpenThemeDialogCommand(MoSeqAcquireViewModel ViewModel) : base(ViewModel)
        {
        }

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override void Execute(object parameter)
        {
            var dialog = new ThemeSettingsDialog
            {
                DataContext = this.ViewModel.Theme
            };
            var result = dialog.ShowDialog();
        }
    }
}
