using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.Models.Utility
{
    public static class ExceptionExtensions
    {
        public static string GetAllMessages(this Exception ex, string separator = "\r\nInnerException: ")
        {
            if (ex.InnerException == null)
                return ex.Message;

            return ex.Message + separator + GetAllMessages(ex.InnerException, separator);
        }
    }
}
