using MoSeqAcquire.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmValidation;

namespace MoSeqAcquire.Models.Metadata.Rules
{
    public abstract class BaseRule : BaseViewModel
    {
        protected bool isActive;

        public BaseRule(string Name)
        {
            this.Name = Name;
        }
        public string Name { get; protected set; }
        public bool IsActive
        {
            get => this.isActive;
            set => this.SetField(ref this.isActive, value);
        }
        public abstract RuleResult Validate(MetadataItemDefinition Item);
    }
}
