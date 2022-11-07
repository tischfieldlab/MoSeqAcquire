using MoSeqAcquire.Models.Metadata.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.ViewModels.Metadata
{
    public class AvailableTypeViewModel : BaseViewModel
    {
        protected BaseDataType type;
        protected bool isEnabled;

        public AvailableTypeViewModel(BaseDataType type, bool isEnabled = true)
        {
            this.Type = type;
            this.IsEnabled = isEnabled;
        }

        public BaseDataType Type
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
