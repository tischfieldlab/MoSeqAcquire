using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MoSeqAcquire.Models.Acquisition;
using MoSeqAcquire.Models.Attributes;
using MoSeqAcquire.Models.Core;

namespace MoSeqAcquire.Models.Recording
{
    public class RecorderSpecification : ComponentSpecification
    {
        public RecorderSpecification(Type Type) : base(Type)
        {
        }

        public List<SupportedChannelTypeAttribute> SupportedChannels
        {
            get
            {
                return this.GetAttributes<SupportedChannelTypeAttribute>()
                           .Where(a => a is SupportedChannelTypeAttribute && !a.IsDefaultAttribute())
                           .ToList();
            }
        }
        /*public RecorderSettings SettingsFactory()
        {
            var sit = this.RecorderType.GetCustomAttribute<SettingsImplementationAttribute>();
            return Activator.CreateInstance(sit.SettingsImplementation) as RecorderSettings;
        }
        public MediaWriter Factory()
        {
            return (MediaWriter)Activator.CreateInstance(this.RecorderType);
        }*/
    }
}
