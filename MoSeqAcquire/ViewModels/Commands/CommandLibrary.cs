using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MoSeqAcquire.ViewModels.Commands
{
    public class CommandLibrary
    {
        protected MoSeqAcquireViewModel _vm;
        protected Dictionary<Type, ICommand> __commands;


        public CommandLibrary(MoSeqAcquireViewModel ViewModel)
        {
            this._vm = ViewModel;
            this.__commands = new Dictionary<Type, ICommand>();
            this.InitializeCommands();
        }
        protected void InitializeCommands()
        {
            this.RegisterCommand(new AddRecorderCommand(this._vm));
            this.RegisterCommand(new EditRecorderCommand(this._vm));
        }
        protected void RegisterCommand(ICommand Command)
        {
            this.__commands.Add(Command.GetType(), Command);
        }
        protected T GetCommand<T>()
        {
            if (this.__commands.ContainsKey(typeof(T)))
            {
                return (T)this.__commands[typeof(T)];
            }
            return default(T);
        }

        public AddRecorderCommand AddRecorder { get => this.GetCommand<AddRecorderCommand>(); }
        public EditRecorderCommand EditRecorder { get => this.GetCommand<EditRecorderCommand>(); }



    }
}
