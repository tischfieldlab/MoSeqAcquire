using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.ViewModels.Triggers
{
    public class AvailableActionTypeViewModel : BaseViewModel
    {
        protected Type actionType;
        public AvailableActionTypeViewModel(Type ActionType)
        {
            this.actionType = ActionType;
        }
        public Type ActionType
        {
            get => this.actionType;
        }
        public string Name
        {
            get => this.actionType.AssemblyQualifiedName;
        }
    }
}
