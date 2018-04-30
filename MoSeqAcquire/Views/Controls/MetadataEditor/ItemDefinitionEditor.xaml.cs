using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MoSeqAcquire.Views.Controls.MetadataEditor
{
    /// <summary>
    /// Interaction logic for ItemDefinitionEditor.xaml
    /// </summary>
    public partial class ItemDefinitionEditor : Window
    {
        public ItemDefinitionEditor(ObservableCollection<Type> AvailableTypes)
        {
            InitializeComponent();
            this.propertyTypeCombobox.ItemsSource = AvailableTypes;
        }

        private void add_choice(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as MetadataItem;
            object val = null;
            if(vm.ValueType == typeof(string))
            {
                val = "";
            }
            else
            {
                val = Activator.CreateInstance(vm.ValueType);
            }
            vm.Choices.Add(val);
        }
    }
}
