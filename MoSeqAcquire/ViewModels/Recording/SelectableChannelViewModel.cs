using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.ViewModels.Recording
{
    public class SelectableChannelViewModel : BaseViewModel
    {
        protected bool isSelected;
        protected bool isEnabled;
        public SelectableChannelViewModel(ChannelViewModel Channel, bool IsSelected = false)
        {
            this.Channel = Channel;
            this.IsSelected = IsSelected;
        }
        public ChannelViewModel Channel { get; protected set; }
        public bool IsSelected
        {
            get => this.isSelected;
            set => this.SetField(ref isSelected, value);
        }
        public bool IsEnabled
        {
            get => this.isEnabled;
            set => this.SetField(ref isEnabled, value);
        }
        public override bool Equals(Object obj)
        {
            // Check for null values and compare run-time types.
            if (obj == null || GetType() != obj.GetType())
                return false;

            SelectableChannelViewModel other = (SelectableChannelViewModel)obj;
            return this.Channel.Equals(other.Channel);
        }
        public override int GetHashCode()
        {
            return this.Channel.GetHashCode();
        }
    }
}
