using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MoSeqAcquire.ViewModels.Commands
{
    public class BaseCommandLibrary<TVM> where TVM : BaseViewModel
    {
        protected TVM _vm;
        protected Dictionary<Type, ICommand> __commands;


        public BaseCommandLibrary(TVM ViewModel)
        {
            this._vm = ViewModel;
            this.__commands = new Dictionary<Type, ICommand>();
            this.InitializeCommands();
        }
        protected virtual void InitializeCommands()
        {
        }
        protected void RegisterCommand(ICommand Command)
        {
            this.__commands.Add(Command.GetType(), Command);
        }
        protected void RegisterCommand<TCommand>() where TCommand : BaseCommand
        {
            this.RegisterCommand((TCommand)Activator.CreateInstance(typeof(TCommand), new object[] { this._vm }));
        }
        protected T GetCommand<T>()
        {
            if (this.__commands.ContainsKey(typeof(T)))
            {
                return (T)this.__commands[typeof(T)];
            }
            return default(T);
        }



    }
}
