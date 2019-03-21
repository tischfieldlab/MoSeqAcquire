using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ComponentModel;

namespace MoSeqAcquire.Views.Controls
{
    public enum InputType
    {
        Text,
        TextArea,
        Password
    }
    /// <summary>
    /// Interaction logic for PromptDialog.xaml
    /// </summary>
    public partial class PromptDialog : Window
    {
        protected PromptDialogViewModel viewModel;


        public PromptDialog(string Question, string Title, string DefaultValue = "", InputType InputType = InputType.Text)
        {
            this.viewModel = new PromptDialogViewModel(Question, InputType, DefaultValue)
            {
                ButtonType = MessageBoxButton.OKCancel
            };
            this.DataContext = this.viewModel;
            InitializeComponent();
            this.Title = Title;
            this.Loaded += new RoutedEventHandler(PromptDialog_Loaded);
        }

        void PromptDialog_Loaded(object sender, RoutedEventArgs e)
        {
            this.InputContainer.Focus();
        }

        public static string Prompt(string question, string title, string defaultValue = "", InputType inputType = InputType.Text)
        {
            PromptDialog inst = new PromptDialog(question, title, defaultValue, inputType);
            inst.ShowDialog();
            if (inst.DialogResult == true)
            {
                return inst.ResponseText;
            }
            return null;
        }
        
        public string ResponseText
        {
            get { return this.viewModel.Response; }
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }



    public class PromptDialogViewModel : INotifyPropertyChanged{

        protected InputType inputType;
        protected MessageBoxButton buttonType;    
        protected String question;
        protected String response;

        public PromptDialogViewModel(String Question, InputType InputType, String DefaultResponse = "")
        {
            this.question = Question;
            this.inputType = InputType;
            this.response = DefaultResponse;
        }

        public String Question{
            get{ return this.question; }
            set{ this.question = value; this.NotifyPropertyChanged("Question"); }
        }
        public InputType InputType{
            get{ return this.inputType; }
            set{ this.inputType = value; this.NotifyPropertyChanged("InputType"); }
        }
        public String Response{
            get{ return this.response; }
            set{ this.response = value; this.NotifyPropertyChanged("Response"); }
        }
        public MessageBoxButton ButtonType
        {
            get { return this.buttonType; }
            set { this.buttonType = value; this.NotifyPropertyChanged("ButtonType"); }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string prop)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
