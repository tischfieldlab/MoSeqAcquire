using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.Models.Configuration
{
    public abstract class ConfigSnapshot// : ISerializable
    {
        protected ConfigSnapshot() { }

        public static ConfigSnapshot GetDefault() { throw new NotImplementedException(); }
    }
}
