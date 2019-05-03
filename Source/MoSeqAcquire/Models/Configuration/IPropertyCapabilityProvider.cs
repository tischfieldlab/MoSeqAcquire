using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.Models.Configuration
{
    public interface IPropertyCapabilityProvider
    {
        bool IsPropertyComplex(string PropertyName);
        ComplexProperty GetComplexProperty(string PropertyName);
    }
}
