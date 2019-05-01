using MoSeqAcquire.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoSeqAcquire.Models.Metadata;

namespace MoSeqAcquire.Views.Controls.MetadataEditor.Rules
{
    public abstract class BaseRule : BaseViewModel
    {
        public BaseRule(string Name)
        {
            this.Name = Name;
        }
        public string Name { get; protected set; }
        public bool IsActive { get; set; }
        public abstract void Attach(MetadataItemDefinition Item);
    }
}
