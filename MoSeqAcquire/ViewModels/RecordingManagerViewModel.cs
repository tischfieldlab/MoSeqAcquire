using MoSeqAcquire.Models.IO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MoSeqAcquire.ViewModels
{
    public class RecordingManagerViewModel : BaseViewModel
    {
        protected MoSeqAcquireViewModel rootViewModel;
        protected ObservableCollection<RecorderViewModel> recorders;

        public RecordingManagerViewModel(MoSeqAcquireViewModel RootViewModel)
        {
            this.rootViewModel = RootViewModel;
            this.recorders = new ObservableCollection<RecorderViewModel>();
        }
        public MoSeqAcquireViewModel Root { get => this.rootViewModel; }
        
        public ObservableCollection<RecorderViewModel> Recorders { get => this.recorders; }
        protected RecorderViewModel selectedRecorder;
        public RecorderViewModel SelectedRecorder
        {
            get => this.selectedRecorder;
            set => this.SetField(ref this.selectedRecorder, value);
        }

    }
}
