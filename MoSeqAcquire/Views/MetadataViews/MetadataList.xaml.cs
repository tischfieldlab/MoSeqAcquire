using MoSeqAcquire.Views.Controls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MoSeqAcquire.Views.Metadata
{
    /// <summary>
    /// Interaction logic for PropertyView.xaml
    /// </summary>
    public partial class MetadataList : SubsystemControl
    {
        public MetadataList()
        {
            InitializeComponent();
        }

        private void DataGridCell_OnSelected(object sender, RoutedEventArgs e)
        {
            DataGridCell cell;
            // Lookup for the source to be DataGridCell
            if (e.OriginalSource.GetType() == typeof(DataGridCell))
            {
                cell = e.OriginalSource as DataGridCell;
            }
            else
            {
                cell = GetParentByType<DataGridCell>(e.OriginalSource as DependencyObject);
            }

            if (cell == null || cell.IsReadOnly)
                return;

            if (!cell.IsEditing)
            {
                // Starts the Edit on the row;
                DataGrid grd = (DataGrid) sender;
                grd.BeginEdit(e);
            }

            Control control = GetFirstChildByType<Control>(cell);
            if (control != null && control != e.OriginalSource)
            {
                if (!control.IsKeyboardFocusWithin)
                {
                    control.Focus();
                }

                if (control is ComboBox)
                {
                    (control as ComboBox).IsDropDownOpen = true;
                }
            }
        }

        private T GetFirstChildByType<T>(DependencyObject prop) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(prop); i++)
            {
                if (!(VisualTreeHelper.GetChild((prop), i) is DependencyObject child))
                    continue;

                if (child is T castedProp)
                    return castedProp;

                castedProp = GetFirstChildByType<T>(child);

                if (castedProp != null)
                    return castedProp;
            }
            return null;
        }
        private T GetParentByType<T>(DependencyObject prop) where T : DependencyObject
        {
            //get parent item
            DependencyObject parentObject = VisualTreeHelper.GetParent(prop);

            //we've reached the end of the tree
            if (parentObject == null) return null;

            //check if the parent matches the type we're looking for
            if (parentObject is T parent)
                return parent;
            else
                return GetParentByType<T>(parentObject);
        }
    }
}
