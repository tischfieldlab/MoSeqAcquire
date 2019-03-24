using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.Models.Recording
{
    public class TimestampCoWriter
    {
        protected string filename;
        protected StreamWriter writer;
        public TimestampCoWriter(string filename)
        {
            this.filename = filename;
        }
        public void Open()
        {
            this.writer = new StreamWriter(this.filename);
        }
        public void Write(DateTime time)
        {
            this.writer.WriteLine(time);
        }
        public void Close()
        {
            this.writer.Flush();
            this.writer.Close();
        }
    }
}
