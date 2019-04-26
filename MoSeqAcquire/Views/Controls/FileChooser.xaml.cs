using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MaterialDesignThemes.Wpf;

namespace MoSeqAcquire.Views.Controls
{
    /// <summary>
    /// Interaction logic for FileChooser.xaml
    /// </summary>
    public partial class FileChooser : UserControl, INotifyDataErrorInfo
    {
        public enum PickerTypes
        {
            Save, Load, Folder
        }


        public static readonly DependencyProperty PickerTypeProperty = DependencyProperty.Register("PickerType", typeof(PickerTypes), typeof(FileChooser), new PropertyMetadata(PickerTypes.Load, new PropertyChangedCallback(TypeChangedCallBack)));
        public static readonly DependencyProperty SelectedPathProperty = DependencyProperty.Register("SelectedPath", typeof(String), typeof(FileChooser), new FrameworkPropertyMetadata(String.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(ValidatePropertyWhenChangedCallback)));
        public static readonly DependencyProperty MultiselectProperty = DependencyProperty.Register("Multiselect", typeof(Boolean), typeof(FileChooser), new PropertyMetadata(false));
        public static readonly DependencyProperty FileExtensionProperty = DependencyProperty.Register("FileExtension", typeof(String), typeof(FileChooser), new PropertyMetadata("*"));
        public static readonly DependencyProperty FileDescriptionProperty = DependencyProperty.Register("FileDescription", typeof(String), typeof(FileChooser), new PropertyMetadata("All Files"));
        public static readonly DependencyProperty FileFilterProperty = DependencyProperty.Register("FileFilter", typeof(String), typeof(FileChooser));
        public static readonly DependencyProperty ForcePickerUseProperty = DependencyProperty.Register("ForcePickerUse", typeof(Boolean), typeof(FileChooser), new PropertyMetadata(false));


        public FileChooser()
        {
            InitializeComponent();
            this.UpdateButtonIcon();
        }
        public PickerTypes PickerType
        {
            get { return (PickerTypes)GetValue(PickerTypeProperty); }
            set { SetValue(PickerTypeProperty, value); }
        }
        public String SelectedPath
        {
            get { return (string)GetValue(SelectedPathProperty); }
            set { SetValue(SelectedPathProperty, value); }
        }
        public Boolean Multiselect
        {
            get { return (Boolean)GetValue(MultiselectProperty); }
            set { SetValue(MultiselectProperty, value); }
        }
        public Boolean ForcePickerUse
        {
            get { return (Boolean)GetValue(ForcePickerUseProperty); }
            set { SetValue(ForcePickerUseProperty, value); }
        }
        public String FileExtension
        {
            get { return (string)GetValue(FileExtensionProperty); }
            set { SetValue(FileExtensionProperty, value); }
        }
        public String FileDescription
        {
            get { return (string)GetValue(FileDescriptionProperty); }
            set { SetValue(FileDescriptionProperty, value); }
        }
        public String FileFilter
        {
            get { return (string)GetValue(FileFilterProperty); }
            set { SetValue(FileFilterProperty, value); }
        }
        protected void UpdateButtonIcon()
        {
            this.ButtonImage.Kind = this.ButtonIcon; //.Source = new BitmapImage(this.ButtonIcon);
        }
        protected PackIconKind ButtonIcon
        {
            get
            {
                switch (this.PickerType)
                {
                    case PickerTypes.Save:
                        return PackIconKind.ContentSave;//new Uri("pack://application:,,,/MoSeqAcquire;component/Images/saveHS.png");
                    case PickerTypes.Load:
                    case PickerTypes.Folder:
                    default:
                        return PackIconKind.Folder;//new Uri("pack://application:,,,/MoSeqAcquire;component/Images/openHS.png");
                }
            }
        }
        static void TypeChangedCallBack(DependencyObject property, DependencyPropertyChangedEventArgs args)
        {
            FileChooser TextBox = (FileChooser)property;
            TextBox.UpdateButtonIcon();
        }


        private void ButtonClicked(object sender, RoutedEventArgs e)
        {
            switch (this.PickerType)
            {
                case PickerTypes.Load:
                    this.ShowLoadDialog();
                    break;
                case PickerTypes.Save:
                    this.ShowSaveDialog();
                    break;
                case PickerTypes.Folder:
                    this.ShowDirectoryDialog();
                    break;
            }
        }

        protected void ShowSaveDialog()
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog
            {
                AddExtension = true,
                DefaultExt = this.FileExtension
            };
            if (this.FileFilter == null)
            {
                dlg.Filter = this.FileDescription + " (." + this.FileExtension + ")|*." + this.FileExtension;
            }
            else
            {
                dlg.Filter = this.FileFilter;
            }
            dlg.FileName = this.SelectedPath;
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                this.SelectedPath = dlg.FileName;
            }
        }
        protected void ShowLoadDialog()
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            if (this.Multiselect)
            {
                dlg.Multiselect = this.Multiselect;
            }
            dlg.DefaultExt = this.FileExtension;
            if (this.FileFilter == null)
            {
                dlg.Filter = this.FileDescription + " (." + this.FileExtension + ")|*." + this.FileExtension;
            }
            else
            {
                dlg.Filter = this.FileFilter;
            }
            dlg.FileName = this.SelectedPath;
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                if (this.Multiselect)
                {
                    this.SelectedPath = String.Join(";", dlg.FileNames);
                }
                else
                {
                    this.SelectedPath = dlg.FileName;
                }
            }
        }
        protected void ShowDirectoryDialog()
        {
            var dlg = new System.Windows.Forms.FolderBrowserDialog
            {
                RootFolder = System.Environment.SpecialFolder.MyComputer,
                SelectedPath = this.SelectedPath,
                ShowNewFolderButton = true
            };
            System.Windows.Forms.DialogResult result = dlg.ShowDialog();
            if (result.Equals(System.Windows.Forms.DialogResult.OK))
            {
                this.SelectedPath = dlg.SelectedPath;
            }
        }

        private void PathText_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                this.SelectedPath = files.FirstOrDefault();
            }
        }






        public delegate void ErrorsChangedEventHandler(object sender, DataErrorsChangedEventArgs e);

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public bool HasErrors
        {
            get
            {
                var validationErrors = Validation.GetErrors(this);
                return validationErrors.Any();
            }
        }

        public IEnumerable GetErrors(string propertyName)
        {
            var validationErrors = Validation.GetErrors(this);
            var specificValidationErrors =
                validationErrors.Where(
                    error => ((BindingExpression)error.BindingInError).TargetProperty.Name == propertyName).ToList();
            var specificValidationErrorMessages = specificValidationErrors.Select(valError => valError.ErrorContent);
            return specificValidationErrorMessages;
        }

        public void NotifyErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        protected static void ValidatePropertyWhenChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            ((FileChooser)dependencyObject).NotifyErrorsChanged(dependencyPropertyChangedEventArgs.Property.Name);
        }
    }
}