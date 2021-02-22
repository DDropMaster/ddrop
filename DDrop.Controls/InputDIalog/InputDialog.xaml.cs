using System;
using System.ComponentModel;
using System.Windows;

namespace DDrop.Controls.InputDIalog
{
    /// <summary>
    /// Interaction logic for InputDialog.xaml
    /// </summary>
    public partial class InputDialog : INotifyPropertyChanged
    {
        public static readonly DependencyProperty QuestionProperty =
            DependencyProperty.Register("Question", typeof(string), typeof(InputDialog));
        public string Question
        {
            get => (string)GetValue(QuestionProperty);
            set
            {
                SetValue(QuestionProperty, value); 
                NotifyPropertyChanged("Question");
            }
        }

        public InputDialog(string question, string defaultAnswer = "")
        {
            InitializeComponent();
            Question = question;
            TxtAnswer.Text = defaultAnswer;
        }

        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            TxtAnswer.SelectAll();
            TxtAnswer.Focus();
        }

        public string Answer => TxtAnswer.Text;
        public bool Canceled;

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Canceled = true;
        }
    }
}
