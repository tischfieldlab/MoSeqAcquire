using MoSeqAcquire.ViewModels;
using MoSeqAcquire.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MoSeqAcquire.Views.Controls.MetadataEditor
{
    public class MetadataViewModel : BaseViewModel
    {
        public MetadataViewModel()
        {
            this.AvailableTypes = new ObservableCollection<Type>()
            {
                typeof(bool), typeof(int), typeof(double), typeof(string)
            };
            this.Items = new MetadataCollection(null);
        }

        public ObservableCollection<Type> AvailableTypes { get; protected set; }

        public MetadataCollection Items { get; protected set; }

        public ICommand EditItemDefinition => new ActionCommand((o) =>
        {
            var e = new ItemDefinitionEditor(this.AvailableTypes);
            e.DataContext = o as MetadataItem;
            e.Show();
        });
    }
}
