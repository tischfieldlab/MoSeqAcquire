using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MoSeqAcquire.Models.Acquisition;
using MoSeqAcquire.Models.Attributes;
using MoSeqAcquire.Models.Recording;

namespace MoSeqAcquire.Models.Management
{
    public static class ProtocolHelpers
    {
        public static IEnumerable<Type> FindProviderTypes()
        {
            return Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => !t.IsAbstract && typeof(MediaSource).IsAssignableFrom(t));
        }
        public static IEnumerable<Type> GetKnownTypesForProviders()
        {
            return FindProviderTypes().SelectMany(r => Attribute.GetCustomAttributes(r, typeof(KnownTypeAttribute)).Select(kt => (kt as KnownTypeAttribute).KnownType));
        }


        public static IEnumerable<Type> FindRecorderTypes()
        {
            return Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => !t.IsAbstract && typeof(IMediaWriter).IsAssignableFrom(t));
            //.Select(t => t.FullName);
        }
        public static IEnumerable<Type> GetKnownTypesForRecorders()
        {
            return FindRecorderTypes().SelectMany(r => Attribute.GetCustomAttributes(r, typeof(KnownTypeAttribute)).Select(kt => (kt as KnownTypeAttribute).KnownType));
        }
    }
}
