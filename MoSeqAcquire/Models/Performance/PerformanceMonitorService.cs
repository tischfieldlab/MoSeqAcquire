using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.Models.Performance
{
    public class PerformanceMonitorService
    {
        protected Dictionary<string, IPerformanceProvider> providers;


        #region Singleton
        private static PerformanceMonitorService __instance;
        public PerformanceMonitorService()
        {
            this.providers = new Dictionary<string, IPerformanceProvider>();
        }
        public static PerformanceMonitorService Instance
        {
            get
            {
                if (__instance == null)
                {
                    __instance = new PerformanceMonitorService();
                }
                return __instance;
            }
        }
        #endregion


        public void Publish(string Path, IPerformanceProvider Provider)
        {
            this.providers.Add(Path, Provider);
        }
    }
}
