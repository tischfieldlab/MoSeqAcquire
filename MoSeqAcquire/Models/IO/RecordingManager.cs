using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.Models.IO
{
    public class RecordingManager
    {
        protected List<IMediaWriter> _writers;

        public RecordingManager()
        {
            this._writers = new List<IMediaWriter>();
        }

        public void AddRecorder(IMediaWriter Writer)
        {
            this._writers.Add(Writer);
        }


        public GeneralRecordingSettings GeneralSettings { get; set; }

        public void Initialize(GeneralRecordingSettings GeneralSettings) { }
        public void Start()
        {
            this.IsRecording = true;
            this._writers = new List<IMediaWriter>();
            foreach (var r in this._writers)
            {
                r.Start();
            }
        }
        public void Stop() { }
    }

    interface IRecordingLengthStrategy
    {
        event EventHandler TriggerStop;
    }
}
