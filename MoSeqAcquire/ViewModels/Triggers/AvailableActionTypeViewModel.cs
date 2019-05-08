using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoSeqAcquire.Models.Core;

namespace MoSeqAcquire.ViewModels.Triggers
{
    public class AvailableActionTypeViewModel : BaseViewModel
    {
        protected ComponentSpecification actionSpec;

        public AvailableActionTypeViewModel(Type ActionType)
        {
            this.actionSpec = new ComponentSpecification(ActionType);
        }
        public Type ActionType
        {
            get => this.actionSpec.ComponentType;
        }
        public string Name
        {
            get => this.actionSpec.DisplayName;
        }
    }
}
