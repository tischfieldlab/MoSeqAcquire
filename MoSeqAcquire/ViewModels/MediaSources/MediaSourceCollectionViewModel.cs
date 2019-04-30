using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GongSolutions.Wpf.DragDrop;

namespace MoSeqAcquire.ViewModels.MediaSources
{
    public class MediaSourceCollectionViewModel : BaseViewModel
    {
        public MediaSourceCollectionViewModel()
        {
            this.Items = new ObservableCollection<MediaSourceViewModel>();
        }


        public ObservableCollection<MediaSourceViewModel> Items { get; protected set; }

    }
}
