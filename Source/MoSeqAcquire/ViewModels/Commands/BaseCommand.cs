using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MoSeqAcquire.ViewModels.Commands
{
    public abstract class BaseCommand : ICommand
    {
        private MoSeqAcquireViewModel viewModel;

        public BaseCommand(MoSeqAcquireViewModel ViewModel)
        {
            this.viewModel = ViewModel;
        }


        public MoSeqAcquireViewModel ViewModel
        {
            get { return this.viewModel; }
            set { this.viewModel = value; }
        }

        protected void RegisterInputGesture(KeyGesture Gesture)
        {
            Application.Current.MainWindow.InputBindings.Add(new KeyBinding(this, Gesture));
        }
        protected void RegisterRoutedCommand(ICommand Command)
        {
            Application.Current.MainWindow.CommandBindings.Add(new CommandBinding(Command,
                (s, e) => {
                    this.Execute(e.Parameter);
                },
                (s, e) => {
                    e.CanExecute = this.CanExecute(e.Parameter);
                }
            ));
        }



        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        public void RaiseCanExecuteChanged()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                CommandManager.InvalidateRequerySuggested();
            });
        }

        public abstract void Execute(object parameter);
        public abstract bool CanExecute(object parameter);
    }
}
