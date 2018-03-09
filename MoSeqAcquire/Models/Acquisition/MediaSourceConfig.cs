using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.Models.Acquisition
{
    public class ConfigChangedEventArgs
    {
        public ConfigChangedEventArgs(String PropertyName, Object OldValue, Object NewValue)
        {
            this.PropertyName = PropertyName;
            this.OldValue = OldValue;
            this.NewValue = NewValue;
        }
        public string PropertyName { get; protected set; }
        public object OldValue { get; protected set; }
        public object NewValue { get; protected set; }
    }
    public abstract class MediaSourceConfig : ConfigurationSection
    {
        public delegate void ConfigChangedHandler(ConfigChangedEventArgs e);
        public event ConfigChangedHandler ConfigChanged;
        protected void NotifyConfigChanged(ConfigChangedEventArgs args)
        {
            this.ConfigChanged?.Invoke(args);
        }
    }
}
