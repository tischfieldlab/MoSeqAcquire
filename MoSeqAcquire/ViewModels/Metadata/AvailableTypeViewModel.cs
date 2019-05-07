using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.ViewModels.Metadata
{
    public class AvailableTypeViewModel : BaseViewModel
    {
        protected Type type;
        protected bool isEnabled;

        public AvailableTypeViewModel(Type type, bool isEnabled = true)
        {
            this.Type = type;
            this.IsEnabled = isEnabled;
        }

        public Type Type
        {
            get => this.type;
            set => this.SetField(ref type, value);
        }
        public bool IsEnabled
        {
            get => this.isEnabled;
            set => this.SetField(ref isEnabled, value);
        }
    }
}
