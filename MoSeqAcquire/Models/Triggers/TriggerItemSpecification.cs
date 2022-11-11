using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoSeqAcquire.Models.Attributes;
using MoSeqAcquire.Models.Core;

namespace MoSeqAcquire.Models.Triggers
{
    public class TriggerItemSpecification : ComponentSpecification
    {
        public TriggerItemSpecification(Type Type) : base(Type)
        {
        }

        public bool HasDesignerImplementation
        {
            get => this.DesignerImplementation != null;
        }
        public Type DesignerImplementation
        {
            get
            {
                var n = (DesignerImplementationAttribute)Attribute.GetCustomAttribute(this.ComponentType, typeof(DesignerImplementationAttribute));
                return n?.DesignerImplementation;
            }
        }
    }
}
