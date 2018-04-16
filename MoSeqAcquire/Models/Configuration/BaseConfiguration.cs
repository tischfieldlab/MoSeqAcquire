using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.Models.Configuration
{
    public abstract class BaseConfiguration : ObservableObject, IConfigSnapshotProvider
    {
        public ConfigSnapshot GetSnapshot()
        {
            var state = new ConfigSnapshot();
            this.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(pi => pi.CanWrite)
                .ForEach((pi) => { state.Add(pi.Name, pi.GetValue(this)); });
            return state;
        }
        public void ApplySnapshot(ConfigSnapshot snapshot)
        {
            if (snapshot == null) { return; }

            foreach (string key in snapshot.Keys)
            {
                var prop = this.GetType().GetProperty(key);
                if (prop != null)
                {
                    prop.SetValue(this, snapshot[key]);
                }
            }
        }
    }
}
