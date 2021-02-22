using System.Security;
using System.Windows;
using System.Windows.Controls;

namespace DDrop.Controls.PasswordControl
{
    /// <summary>
    /// Interaction logic for PasswordUserControl.xaml
    /// </summary>
    public partial class PasswordUserControl : UserControl
    {
        public SecureString Password
        {
            get { return (SecureString)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register("Password", typeof(SecureString), typeof(PasswordUserControl),
                new PropertyMetadata(default(SecureString)));


        public PasswordUserControl()
        {
            InitializeComponent();

            // Update DependencyProperty whenever the password changes
            PasswordBox.PasswordChanged += (sender, args) => {
                Password = ((PasswordBox)sender).SecurePassword;
            };
        }

        public event RoutedEventHandler PasswordChanged
        {
            add => PasswordBox.PasswordChanged += value;
            remove => PasswordBox.PasswordChanged -= value;
        }
    }
}
