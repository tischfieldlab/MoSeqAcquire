using MoSeqAcquire.Models.IO;
using MoSeqAcquire.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MoSeqAcquire.ViewModels.Commands
{
    public class StartRecordingCommand : BaseCommand
    {
        public StartRecordingCommand(MoSeqAcquireViewModel ViewModel) : base(ViewModel)
        {
        }

        public override bool CanExecute(object parameter)
        {
            return this.ViewModel.Recorder.IsRecording == false;
        }

        public override void Execute(object parameter)
        {
            var recorders = new List<MediaWriter>();
            foreach(var r in this.ViewModel.Recorder.Recorders)
            {
                recorders.Add(r.MakeMediaWriter());
            }
            foreach(var r in recorders)
            {
                r.Start();
            }
        }
    }
}
