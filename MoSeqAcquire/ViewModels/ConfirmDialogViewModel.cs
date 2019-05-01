using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.ViewModels
{
    public class ConfirmDialogViewModel : BaseViewModel
    {
        protected string title;
        protected string message;

        public string Title
        {
            get => this.title;
            set => this.SetField(ref this.title, value);
        }
        public string Message
        {
            get => this.message;
            set => this.SetField(ref this.message, value);
        }
    }
}
