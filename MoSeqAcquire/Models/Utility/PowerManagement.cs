using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.Models.Utility
{
    public static class PowerManagement
    {
        public static void StartPreventSleep()
        {
            NativeMethods.SetThreadExecutionState(EXECUTION_STATE.ES_AWAYMODE_REQUIRED
                                                | EXECUTION_STATE.ES_CONTINUOUS);
        }
        public static void EndPreventSleep()
        {
            NativeMethods.SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS);
        }
    }

    public enum EXECUTION_STATE : uint
    {
        ES_CONTINUOUS        = 0x80000000,
        ES_DISPLAY_REQUIRED  = 0x00000002,
        ES_SYSTEM_REQUIRED   = 0x00000001,
        ES_AWAYMODE_REQUIRED = 0x00000040
    }

    internal partial class NativeMethods
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);
    }
}
