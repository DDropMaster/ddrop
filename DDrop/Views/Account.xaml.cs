using System;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AutoMapper;
using DDrop.BE.Enums.Logger;
using DDrop.BE.Models;
using DDrop.BL.User;
using DDrop.Models;
using DDrop.Properties;
using DDrop.Utility.Cryptography;
using DDrop.Utility.ImageOperations;
using DDrop.Utility.Logger;
using Microsoft.Win32;
using ToastNotifications;
using ToastNotifications.Messages;

namespace DDrop.Views
{
    /// <summary>
    ///     Interaction logic for Account.xaml
    /// </summary>
    public partial class Account : INotifyPropertyChanged
    {
        public static readonly DependencyProperty UserProperty =
            DependencyProperty.Register("User", typeof(UserView), typeof(Account));
        public UserView User
        {
            get => (UserView)GetValue(UserProperty);
            set => SetValue(UserProperty, value);
        }

        public static readonly DependencyProperty ChangeFirstNameVisibilityProperty =
            DependencyProperty.Register("ChangeFirstNameVisibility", typeof(Enum), typeof(Account));
        public Enum ChangeFirstNameVisibility
        {
            get => (Enum) GetValue(ChangeFirstNameVisibilityProperty);
            set => SetValue(ChangeFirstNameVisibilityProperty, value);
        }

        public static readonly DependencyProperty ChangeLastNameVisibilityProperty =
            DependencyProperty.Register("ChangeLastNameVisibility", typeof(Enum), typeof(Account));
        public Enum ChangeLastNameVisibility
        {
            get => (Enum)GetValue(ChangeLastNameVisibilityProperty);
            set => SetValue(ChangeLastNameVisibilityProperty, value);
        }

        public static readonly DependencyProperty ChangePasswordVisibilityProperty =
            DependencyProperty.Register("ChangePasswordVisibility", typeof(Enum), typeof(Account));
        public Enum ChangePasswordVisibility
        {
            get => (Enum)GetValue(ChangePasswordVisibilityProperty);
            set => SetValue(ChangePasswordVisibilityProperty, value);
        }

        public static readonly DependencyProperty ChooseProfilePictureVisibilityProperty =
            DependencyProperty.Register("ChooseProfilePictureVisibility", typeof(Enum), typeof(Account));
        public Enum ChooseProfilePictureVisibility
        {
            get => (Enum)GetValue(ChooseProfilePictureVisibilityProperty);
            set => SetValue(ChooseProfilePictureVisibilityProperty, value);
        }

        public static readonly DependencyProperty CurrentPasswordTextBlockVisibilityProperty =
            DependencyProperty.Register("CurrentPasswordTextBlockVisibility", typeof(Enum), typeof(Account));
        public Enum CurrentPasswordTextBlockVisibility
        {
            get => (Enum)GetValue(CurrentPasswordTextBlockVisibilityProperty);
            set => SetValue(CurrentPasswordTextBlockVisibilityProperty, value);
        }

        public static readonly DependencyProperty NewPasswordConfirmTextBlockVisibilityProperty =
            DependencyProperty.Register("NewPasswordConfirmTextBlockVisibility", typeof(Enum), typeof(Account));
        public Enum NewPasswordConfirmTextBlockVisibility
        {
            get => (Enum)GetValue(NewPasswordConfirmTextBlockVisibilityProperty);
            set => SetValue(NewPasswordConfirmTextBlockVisibilityProperty, value);
        }

        public static readonly DependencyProperty CurrentPasswordVisibilityProperty =
            DependencyProperty.Register("CurrentPasswordVisibility", typeof(Enum), typeof(Account));
        public Enum CurrentPasswordVisibility
        {
            get => (Enum)GetValue(CurrentPasswordVisibilityProperty);
            set => SetValue(CurrentPasswordVisibilityProperty, value);
        }

        public static readonly DependencyProperty NewPasswordVisibilityProperty =
            DependencyProperty.Register("NewPasswordVisibility", typeof(Enum), typeof(Account));
        public Enum NewPasswordVisibility
        {
            get => (Enum)GetValue(NewPasswordVisibilityProperty);
            set => SetValue(NewPasswordVisibilityProperty, value);
        }

        public static readonly DependencyProperty NewPasswordConfirmVisibilityProperty =
            DependencyProperty.Register("NewPasswordConfirmVisibility", typeof(Enum), typeof(Account));
        public Enum NewPasswordConfirmVisibility
        {
            get => (Enum)GetValue(NewPasswordConfirmVisibilityProperty);
            set => SetValue(NewPasswordConfirmVisibilityProperty, value);
        }

        public static readonly DependencyProperty ProfilePictureSourceProperty =
            DependencyProperty.Register("ProfilePictureSource", typeof(ImageSource), typeof(Account));
        public ImageSource ProfilePictureSource
        {
            get => (ImageSource)GetValue(ProfilePictureSourceProperty);
            set
            {
                SetValue(ProfilePictureSourceProperty, value);
                OnPropertyChanged(new PropertyChangedEventArgs("ProfilePictureSource"));
            }
        }

        private readonly Notifier _notifier;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IUserBl _userBl;

        public Account(UserView user, Notifier notifier, ILogger logger, IMapper mapper, IUserBl userBl)
        {
            InitializeComponent();
            if (!user.IsLoggedIn)
            {
                ChangeFirstNameVisibility = Visibility.Hidden;
                ChangeLastNameVisibility = Visibility.Hidden;
                ChangePasswordVisibility = Visibility.Hidden;

                ChooseProfilePictureVisibility = Visibility.Hidden;
                CurrentPasswordTextBlockVisibility = Visibility.Hidden;
                NewPasswordConfirmTextBlockVisibility = Visibility.Hidden;
                CurrentPasswordVisibility = Visibility.Hidden;
                NewPasswordVisibility = Visibility.Hidden;
                NewPasswordConfirmVisibility = Visibility.Hidden;
            }

            _notifier = notifier;
            _logger = logger;
            _mapper = mapper;
            _userBl = userBl;
            User = user;

            ProfilePictureSource = ImageInterpreter.LoadImage(User.UserPhoto);
        }

        private async void ChooseProfilePicture_OnClick(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Jpeg files (*.jpg)|*.jpg|All files (*.*)|*.*",
                Multiselect = false,
                AddExtension = true
            };
            if (openFileDialog.ShowDialog() == true)
            {
                Settings.Default.Reference = openFileDialog.FileName;
                var croppingWindow = new CropPhoto
                {
                    Height = new BitmapImage(new Uri(openFileDialog.FileName)).Height,
                    Width = new BitmapImage(new Uri(openFileDialog.FileName)).Width,
                    Owner = this
                };
                croppingWindow.CroppingControl.SourceImage.Source = new BitmapImage(new Uri(openFileDialog.FileName));
                croppingWindow.CroppingControl.SourceImage.Height =
                    new BitmapImage(new Uri(openFileDialog.FileName)).Height;
                croppingWindow.CroppingControl.SourceImage.Width =
                    new BitmapImage(new Uri(openFileDialog.FileName)).Width;

                if (SystemParameters.PrimaryScreenHeight < croppingWindow.CroppingControl.SourceImage.Height ||
                    SystemParameters.PrimaryScreenWidth < croppingWindow.CroppingControl.SourceImage.Width)
                {
                    _notifier.ShowInformation("Вертикальный или горизонтальный размер фотографии слишком велик.");
                }
                else
                {
                    if (croppingWindow.ShowDialog() == true)
                        if (croppingWindow.UserPhotoForCropp != null)
                        {
                            try
                            {
                                AccountLoadingWindow();
                                User.UserPhoto = croppingWindow.UserPhotoForCropp;
                                await Task.Run(() =>
                                {
                                    Dispatcher.InvokeAsync(() =>
                                    {
                                        var dbUserForUpdate = _mapper.Map<UserView, User>(User);
                                        _userBl.UpdateUserAsync(dbUserForUpdate);
                                    });
                                });
                                ProfilePictureSource = ImageInterpreter.LoadImage(User.UserPhoto);

                                _logger.LogInfo(new LogEntry
                                {
                                    Username = User.Email,
                                    LogCategory = LogCategory.Account,
                                    Message = "Фотография обновлена."
                                });
                                _notifier.ShowSuccess("Фотография обновлена.");
                            }
                            catch (TimeoutException)
                            {
                                _notifier.ShowError("Не удалось сохранить фотографию. Проверьте интернет соединение.");
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
                                    Username = User.Email,
                                    Details = exception.TargetSite.Name
                                });
                                throw;
                            }

                            AccountLoadingComplete();
                        }
                }
            }
        }

        private void ChangeFirstNameButton_OnClick(object sender, RoutedEventArgs e)
        {
            TextBlockFirstnameValue.IsEnabled = true;
            TextBlockFirstnameValue.Focus();
            ChangeFirstNameVisibility = Visibility.Hidden;
            SaveChangeFirstNameButton.Visibility = Visibility.Visible;
        }

        private async void SaveChangeFirstNameButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AccountLoadingWindow();

                await Task.Run(() =>
                {
                    Dispatcher.InvokeAsync(() =>
                    {
                        var dbUserForUpdate = _mapper.Map<UserView, User>(User);
                        _userBl.UpdateUserAsync(dbUserForUpdate);
                    });
                });
                User.FirstName = TextBlockFirstnameValue.Text;

                _logger.LogInfo(new LogEntry
                {
                    Username = User.Email,
                    LogCategory = LogCategory.Account,
                    Message = "Имя обновлено."
                });
                _notifier.ShowSuccess("Имя обновлено.");
            }
            catch (TimeoutException)
            {
                _notifier.ShowError("Не удалось сохранить имя. Проверьте интернет соединение.");
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
                    Username = User.Email,
                    Details = exception.TargetSite.Name
                });
                throw;
            }

            AccountLoadingComplete();
            TextBlockFirstnameValue.IsEnabled = false;
            ChangeFirstNameButton.Visibility = Visibility.Visible;
            SaveChangeFirstNameButton.Visibility = Visibility.Hidden;
        }

        private void ChangeLastNameButton_OnClick(object sender, RoutedEventArgs e)
        {
            TextBlockLastNameValue.IsEnabled = true;
            TextBlockLastNameValue.Focus();
            ChangeLastNameButton.Visibility = Visibility.Hidden;
            SaveChangeLastNameButton.Visibility = Visibility.Visible;
        }

        private async void SaveChangeLastNameButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AccountLoadingWindow();
                await Task.Run(() =>
                {
                    Dispatcher.InvokeAsync(() =>
                    {
                        var dbUserForUpdate = _mapper.Map<UserView, User>(User);
                        _userBl.UpdateUserAsync(dbUserForUpdate);
                    });
                });
                User.LastName = TextBlockLastNameValue.Text;

                _logger.LogInfo(new LogEntry
                {
                    Username = User.Email,
                    LogCategory = LogCategory.Account,
                    Message = "Фамилия обновлена."
                });
                _notifier.ShowSuccess("Фамилия обновлена.");
            }
            catch (TimeoutException)
            {
                _notifier.ShowError("Не удалось сохранить фамилию. Проверьте интернет соединение.");
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
                    Username = User.Email,
                    Details = exception.TargetSite.Name
                });
                throw;
            }

            AccountLoadingComplete();
            TextBlockLastNameValue.IsEnabled = false;
            ChangeLastNameButton.Visibility = Visibility.Visible;
            SaveChangeLastNameButton.Visibility = Visibility.Hidden;
        }

        private void ToggleButton_OnUnchecked(object sender, RoutedEventArgs e)
        {
            CurrentPassword.Visibility = Visibility.Visible;
            CurrentPasswordUnmasked.Visibility = Visibility.Hidden;
            CurrentPassword.Password = CurrentPasswordUnmasked.Text;

            NewPassword.Visibility = Visibility.Visible;
            NewPasswordUnmasked.Visibility = Visibility.Hidden;
            NewPassword.Password = NewPasswordUnmasked.Text;

            NewPasswordConfirm.Visibility = Visibility.Visible;
            NewPasswordConfirmUnmasked.Visibility = Visibility.Hidden;
            NewPasswordConfirm.Password = NewPasswordConfirmUnmasked.Text;
        }

        private void ToggleButton_OnChecked(object sender, RoutedEventArgs e)
        {
            CurrentPassword.Visibility = Visibility.Hidden;
            CurrentPasswordUnmasked.Visibility = Visibility.Visible;
            CurrentPasswordUnmasked.Text = CurrentPassword.Password;

            NewPassword.Visibility = Visibility.Hidden;
            NewPasswordUnmasked.Visibility = Visibility.Visible;
            NewPasswordUnmasked.Text = NewPassword.Password;

            NewPasswordConfirm.Visibility = Visibility.Hidden;
            NewPasswordConfirmUnmasked.Visibility = Visibility.Visible;
            NewPasswordConfirmUnmasked.Text = NewPasswordConfirm.Password;
        }

        private void SetSelection(PasswordBox passwordBox, int start, int length)
        {
            passwordBox.GetType().GetMethod("Select", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.Invoke(passwordBox, new object[] {start, length});
        }

        private void CurrentPassword_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space) e.Handled = true;
        }

        private void NewPassword_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space) e.Handled = true;
        }

        private void NewPasswordConfirm_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space) e.Handled = true;
        }

        private void CurrentPasswordUnmasked_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space) e.Handled = true;
        }

        private void NewPasswordUnmasked_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space) e.Handled = true;
        }

        private void NewPasswordConfirmUnmasked_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space) e.Handled = true;
        }

        private void NewPasswordConfirm_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NewPasswordConfirm.Password.Trim()))
            {
                NewPasswordConfirmUnmasked.Text = NewPasswordConfirm.Password;
                SetSelection(NewPasswordConfirm, NewPasswordConfirm.Password.Length,
                    NewPasswordConfirm.Password.Length);
            }
        }

        private void NewPassword_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NewPassword.Password.Trim()))
            {
                NewPasswordUnmasked.Text = NewPassword.Password;
                SetSelection(NewPassword, NewPassword.Password.Length, NewPassword.Password.Length);
            }
        }

        private void CurrentPassword_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CurrentPassword.Password.Trim()))
            {
                CurrentPasswordUnmasked.Text = CurrentPassword.Password;
                SetSelection(CurrentPassword, CurrentPassword.Password.Length, CurrentPassword.Password.Length);
            }
        }

        private void NewPasswordConfirmUnmasked_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var textBoxNewPasswordConfirm = sender as TextBox;
            NewPasswordConfirm.Password = textBoxNewPasswordConfirm?.Text != null ? textBoxNewPasswordConfirm.Text : "";
        }

        private void NewPasswordUnmasked_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var textBoxNewPassword = sender as TextBox;
            NewPassword.Password = textBoxNewPassword?.Text != null ? textBoxNewPassword.Text : "";
        }

        private void CurrentPasswordUnmasked_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var textBoxCurrentPassword = sender as TextBox;
            CurrentPassword.Password = textBoxCurrentPassword?.Text != null ? textBoxCurrentPassword.Text : "";
        }

        private async void ChangePasswordButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (PasswordOperations.PasswordsMatch(CurrentPassword.Password, User.Password))
            {
                if (NewPassword.Password == NewPasswordConfirm.Password &&
                    !string.IsNullOrWhiteSpace(NewPassword.Password))
                {
                    try
                    {
                        AccountLoadingWindow();
                        User.Password = PasswordOperations.HashPassword(NewPassword.Password);
                        await Task.Run(() =>
                        {
                            Dispatcher.InvokeAsync(() =>
                            {
                                var dbUserForUpdate = _mapper.Map<UserView, User>(User);
                                _userBl.UpdateUserAsync(dbUserForUpdate);
                            });
                        });

                        _logger.LogInfo(new LogEntry
                        {
                            Username = User.Email,
                            LogCategory = LogCategory.Account,
                            Message = "Пароль успешно изменен."
                        });
                        _notifier.ShowSuccess("Пароль успешно изменен.");
                        NewPasswordConfirm.Password = "";
                        NewPassword.Password = "";
                        CurrentPassword.Password = "";

                        AccountLoadingComplete();
                    }
                    catch (TimeoutException)
                    {
                        _notifier.ShowError("Не удалось сохранить пароль. Проверьте интернет соединение.");

                        AccountLoadingComplete();
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
                            Username = User.Email,
                            Details = exception.TargetSite.Name
                        });
                        throw;
                    }
                }
                else
                {
                    _notifier.ShowError("Новый пароль и его подтверждение не совпадают.");

                    AccountLoadingComplete();
                }
            }
            else
            {
                _notifier.ShowError("Неверный старый пароль.");

                AccountLoadingComplete();
            }
        }

        private void AccountLoadingWindow()
        {
            ChangePasswordButton.IsEnabled = false;
            SaveChangeLastNameButton.IsEnabled = false;
            ChangeLastNameButton.IsEnabled = false;
            SaveChangeFirstNameButton.IsEnabled = false;
            ChangeFirstNameButton.IsEnabled = false;
            ChooseProfilePicture.IsEnabled = false;
            AccountLoading.IsAdornerVisible = true;
        }

        private void AccountLoadingComplete()
        {
            ChangePasswordButton.IsEnabled = true;
            SaveChangeLastNameButton.IsEnabled = true;
            ChangeLastNameButton.IsEnabled = true;
            SaveChangeFirstNameButton.IsEnabled = true;
            ChangeFirstNameButton.IsEnabled = true;
            ChooseProfilePicture.IsEnabled = true;
            AccountLoading.IsAdornerVisible = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }
    }
}