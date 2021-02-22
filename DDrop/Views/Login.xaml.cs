using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Reflection;
using System.Security;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AutoMapper;
using DDrop.BE.Enums.Logger;
using DDrop.BE.Models;
using DDrop.BL.Series;
using DDrop.BL.User;
using DDrop.Models;
using DDrop.Properties;
using DDrop.Utility.Cryptography;
using DDrop.Utility.Logger;
using DDrop.Utility.SeriesLocalStorageOperations;
using ToastNotifications;
using ToastNotifications.Messages;

namespace DDrop.Views
{
    /// <summary>
    ///     Логика взаимодействия для Login.xaml
    /// </summary>
    public partial class Login
    {
        public static readonly DependencyProperty LocalStoredUsersProperty =
            DependencyProperty.Register("LocalStoredUsers", typeof(LocalStoredUsers), typeof(Login));
        public LocalStoredUsers LocalStoredUsers
        {
            get => (LocalStoredUsers)GetValue(LocalStoredUsersProperty);
            set => SetValue(LocalStoredUsersProperty, value);
        }

        public static readonly DependencyProperty UserLoginProperty =
            DependencyProperty.Register("UserLogin", typeof(UserView), typeof(Login));
        public UserView UserLogin
        {
            get => (UserView)GetValue(UserLoginProperty);
            set => SetValue(UserLoginProperty, value);
        }

        public static readonly DependencyProperty ErrorMessageTextProperty =
            DependencyProperty.Register("ErrorMessageText", typeof(string), typeof(Login));
        public string ErrorMessageText
        {
            get => (string)GetValue(ErrorMessageTextProperty);
            set => SetValue(ErrorMessageTextProperty, value);
        }

        public static readonly DependencyProperty TextBoxEmailTextProperty =
            DependencyProperty.Register("TextBoxEmailText", typeof(string), typeof(Login));
        public string TextBoxEmailText
        {
            get => (string)GetValue(TextBoxEmailTextProperty);
            set => SetValue(TextBoxEmailTextProperty, value);
        }

        public static readonly DependencyProperty PasswordUnmaskTextProperty =
            DependencyProperty.Register("PasswordUnmaskText", typeof(string), typeof(Login));
        public string PasswordUnmaskText
        {
            get => (string)GetValue(PasswordUnmaskTextProperty);
            set => SetValue(PasswordUnmaskTextProperty, value);
        }

        public static readonly DependencyProperty IsTextBoxEmailFocusedProperty =
            DependencyProperty.Register("IsTextBoxEmailFocused", typeof(bool), typeof(Login));
        public bool IsTextBoxEmailFocused
        {
            get => (bool)GetValue(IsTextBoxEmailFocusedProperty);
            set => SetValue(IsTextBoxEmailFocusedProperty, value);
        }

        public static readonly DependencyProperty IsSelectTextBoxEmailTextProperty =
            DependencyProperty.Register("IsSelectTextBoxEmailText", typeof(bool), typeof(Login));
        public bool IsSelectTextBoxEmailText
        {
            get => (bool)GetValue(IsSelectTextBoxEmailTextProperty);
            set => SetValue(IsSelectTextBoxEmailTextProperty, value);
        }

        public static readonly DependencyProperty LoginPasswordBoxVisibilityProperty =
            DependencyProperty.Register("LoginPasswordBoxVisibility", typeof(Enum), typeof(Login));
        public Enum LoginPasswordBoxVisibility
        {
            get => (Enum)GetValue(LoginPasswordBoxVisibilityProperty);
            set => SetValue(LoginPasswordBoxVisibilityProperty, value);
        }

        public static readonly DependencyProperty PasswordUnmaskVisibilityProperty =
            DependencyProperty.Register("PasswordUnmaskVisibility", typeof(Enum), typeof(Login));
        public Enum PasswordUnmaskVisibility
        {
            get => (Enum)GetValue(PasswordUnmaskVisibilityProperty);
            set => SetValue(PasswordUnmaskVisibilityProperty, value);
        }

        public static readonly DependencyProperty LoginButtonIsEnabledProperty =
            DependencyProperty.Register("LoginButtonIsEnabled", typeof(bool), typeof(Login));
        public bool LoginButtonIsEnabled
        {
            get => (bool)GetValue(LoginButtonIsEnabledProperty);
            set => SetValue(LoginButtonIsEnabledProperty, value);
        }

        public static readonly DependencyProperty RegistrationButtonIsEnabledProperty =
            DependencyProperty.Register("RegistrationButtonIsEnabled", typeof(bool), typeof(Login));
        public bool RegistrationButtonIsEnabled
        {
            get => (bool)GetValue(RegistrationButtonIsEnabledProperty);
            set => SetValue(RegistrationButtonIsEnabledProperty, value);
        }

        public static readonly DependencyProperty LoadingAdornerIsAdornerVisibleProperty =
            DependencyProperty.Register("LoadingAdornerIsAdornerVisible", typeof(bool), typeof(Login));
        public bool LoadingAdornerIsAdornerVisible
        {
            get => (bool)GetValue(LoadingAdornerIsAdornerVisibleProperty);
            set => SetValue(LoadingAdornerIsAdornerVisibleProperty, value);
        }

        public SecureString LoginPasswordBoxPassword
        {
            get { return (SecureString)GetValue(LoginPasswordBoxPasswordProperty); }
            set { SetValue(LoginPasswordBoxPasswordProperty, value); }
        }
        public static readonly DependencyProperty LoginPasswordBoxPasswordProperty =
            DependencyProperty.Register("LoginPasswordBoxPassword", typeof(SecureString), typeof(Login),
                new PropertyMetadata(default(SecureString)));

        private readonly ILogger _logger;
        private readonly Notifier _notifier;
        private readonly IMapper _mapper;
        private readonly IUserBl _userBl;
        private readonly ISeriesBL _seriesBl;

        public bool LoginSucceeded;

        public Login(Notifier notifier, ILogger logger, IMapper mapper, IUserBl userBl, ISeriesBL seriesBl)
        {
            _notifier = notifier;
            _logger = logger;
            _mapper = mapper;
            _userBl = userBl;
            _seriesBl = seriesBl;

            LocalStoredUsers = new LocalStoredUsers();

            if (!string.IsNullOrEmpty(Settings.Default.StoredUsers))
                LocalStoredUsers =
                    JsonSerializeProvider.DeserializeFromString<LocalStoredUsers>(Settings.Default.StoredUsers);

            InitializeComponent();

            PasswordUnmaskVisibility = Visibility.Hidden;
            LoginButtonIsEnabled = true;
            RegistrationButtonIsEnabled = true;

            DataContext = this;
        }
        
        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            await Auth();
        }

        private async Task Auth()
        {
            ErrorMessageText = "";
            if (string.IsNullOrEmpty(TextBoxEmailText))
            {
                ErrorMessageText = "Введите электронную почту.";
                IsTextBoxEmailFocused = true;
            }
            else if (!Regex.IsMatch(TextBoxEmailText, @"^[a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$"))
            {
                ErrorMessageText = "Электронная почта имеет не верный формат.";
                IsSelectTextBoxEmailText = false;
                IsSelectTextBoxEmailText = true;
                IsTextBoxEmailFocused = true;
            }
            else
            {
                var email = TextBoxEmailText;
                var password = new NetworkCredential("", LoginPasswordBoxPassword).Password;

                await TryLogin(email, password, true);
            }
        }

        private async Task TryLogin(string email, string password, bool shouldRemember)
        {
            try
            {
                LoginWindowLoading();
                var user = await Task.Run(() => _userBl.GetUserByLogin(email));

                if (user != null && PasswordOperations.PasswordsMatch(password, user.Password))
                {
                    UserLogin = _mapper.Map<User, UserView>(user);
                    UserLogin.IsLoggedIn = true;

                    try
                    {
                        var dbSeries = await Task.Run(() => _seriesBl.GetSeriesByUserId(user.UserId));
                        
                        UserLogin.UserSeries = _mapper.Map<List<Series>, ObservableCollection<SeriesView>>(dbSeries);

                        foreach (var series in UserLogin.UserSeries)
                        {
                            series.CurrentUser = UserLogin;
                        }

                        LoginSucceeded = true;

                        if (RememberMe.IsChecked == true && shouldRemember) StoreUserLocal(email, password);

                        _logger.LogInfo(new LogEntry
                        {
                            Username = user.Email,
                            LogCategory = LogCategory.Authorization,
                            Message = $"Пользователь {user.Email} успешно авторизован."
                        });
                        _notifier.ShowSuccess($"Пользователь {user.Email} авторизован.");
                        LoginWindowLoading();
                        Close();
                    }
                    catch (TimeoutException)
                    {
                        _notifier.ShowError(
                            $"Не удалось получить список серий пользователя {UserLogin.Email}. Не удалось установить подключение. Проверьте интернет соединение.");
                        LoginSucceeded = false;
                        LoginWindowLoading();
                    }
                    catch (Exception exception)
                    {
                        _logger.LogError(new LogEntry
                        {
                            Exception = exception.ToString(),
                            LogCategory = LogCategory.Common,
                            InnerException = exception.InnerException?.Message,
                            Message = exception.Message,
                            StackTrace = exception.StackTrace,
                            Username = user.Email,
                            Details = exception.TargetSite.Name
                        });
                        throw;
                    }
                }
                else
                {
                    ErrorMessageText = "Неверный логин или пароль.";
                    LoginSucceeded = false;
                    LoginWindowLoading();
                }
            }
            catch (TimeoutException)
            {
                LoginWindowLoading();
                _notifier.ShowError("Не удалось установить соединение. Проверьте интернет подключение.");
            }
            catch (Exception exception)
            {
                _logger.LogError(new LogEntry
                {
                    Exception = exception.ToString(),
                    LogCategory = LogCategory.Common,
                    InnerException = exception.InnerException?.Message,
                    Message = exception.Message,
                    StackTrace = exception.StackTrace,
                    Username = email,
                    Details = exception.TargetSite.Name
                });
                throw;
            }
        }

        private void StoreUserLocal(string email, string password)
        {
            var newUser = true;

            if (LocalStoredUsers.Users != null)
            {
                foreach (var localUser in LocalStoredUsers.Users)
                {
                    if (localUser.Login == email)
                    {
                        newUser = false;

                        if (localUser.Password != PasswordOperations.HashPassword(password))
                        {
                            localUser.Password = password;

                            Settings.Default.StoredUsers = JsonSerializeProvider.SerializeToString(LocalStoredUsers);

                            Settings.Default.Save();
                        }


                        break;
                    }
                }
            }
            else
            {
                LocalStoredUsers.Users = new ObservableCollection<LocalStoredUser>();
            }
            
            if (newUser)
            {
                LocalStoredUsers.Users.Add(new LocalStoredUser
                {
                    Login = email,
                    Password = password
                });

                Settings.Default.StoredUsers = JsonSerializeProvider.SerializeToString(LocalStoredUsers);

                Settings.Default.Save();
            }
        }

        private void LoginWindowLoading()
        {
            LoginButtonIsEnabled = !LoginButtonIsEnabled;
            RegistrationButtonIsEnabled = !RegistrationButtonIsEnabled;
            LoadingAdornerIsAdornerVisible = !LoadingAdornerIsAdornerVisible;
        }

        private void RegistrationButton_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Collapsed;
            var registrationWindow = new Registration(_notifier, _logger, _mapper, _userBl)
            {
                Owner = this
            };
            registrationWindow.ShowDialog();
            Visibility = Visibility.Visible;

            if (registrationWindow.RegistrationSucceeded)
            {
                UserLogin = registrationWindow.UserLogin;
                LoginSucceeded = true;
                Close();
            }
            else
            {
                LoginSucceeded = false;
                ShowDialog();
            }
        }

        private void TextBoxLogin_OnTextChanged(object sender, TextCompositionEventArgs textCompositionEventArgs)
        {
            ErrorMessageText = "";
        }

        private void ToggleButton_OnChecked(object sender, RoutedEventArgs e)
        {
            LoginPasswordBoxVisibility = Visibility.Hidden;
            PasswordUnmaskVisibility = Visibility.Visible;
            PasswordUnmaskText = new NetworkCredential("", LoginPasswordBoxPassword).Password;
        }

        private void ToggleButton_OnUnchecked(object sender, RoutedEventArgs e)
        {
            LoginPasswordBoxVisibility = Visibility.Visible;
            PasswordUnmaskVisibility = Visibility.Hidden;
            LoginPasswordBoxPassword = new NetworkCredential("", PasswordUnmaskText).SecurePassword;
        }

        private void LoginPasswordBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space) e.Handled = true;
        }

        private void PasswordUnmask_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space) e.Handled = true;
        }

        private void SetSelection(PasswordBox passwordBox, int start, int length)
        {
            passwordBox.GetType().GetMethod("Select", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.Invoke(passwordBox, new object[] {start, length});
        }

        private void LoginPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox1)
            {
                ErrorMessageText = "";

                if (string.IsNullOrWhiteSpace(passwordBox1.Password.Trim()))
                {
                    PasswordUnmaskText = passwordBox1.Password;
                    SetSelection(passwordBox1, passwordBox1.Password.Length, passwordBox1.Password.Length);
                }
            }
        }

        private void PasswordUnmask_TextChanged(object sender, TextChangedEventArgs e)
        {
            ErrorMessageText = "";

            var textBox1 = sender as TextBox;
            LoginPasswordBoxPassword = textBox1?.Text != null 
                ? new NetworkCredential("", textBox1.Text).SecurePassword 
                : new NetworkCredential("", "").SecurePassword;
        }

        private async void LocalUsersCombobox_DropDownClosed(object sender, EventArgs e)
        {
            var comboBox = (ComboBox) sender;

            if (comboBox.SelectedIndex > -1)
            {
                var storedUser = (LocalStoredUser) comboBox.SelectedItem;

                LoginPasswordBoxPassword = new NetworkCredential("", storedUser.Password).SecurePassword;
                TextBoxEmailText = storedUser.Login;
                PasswordUnmaskText = storedUser.Password;

                await TryLogin(storedUser.Login, storedUser.Password, false);
            }
        }

        private void TextBoxEmail_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            ErrorMessageText = "";
        }
    }
}