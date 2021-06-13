using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using AutoMapper;
using DDrop.BE.Enums;
using DDrop.BE.Enums.Logger;
using DDrop.BE.Models;
using DDrop.BL.AppStateBL;
using DDrop.BL.Measurement;
using DDrop.Controls.InputDIalog;
using DDrop.Enums;
using DDrop.Models;
using DDrop.Utility.ImageOperations;
using DDrop.Utility.ImageOperations.ImageValidator;
using DDrop.Utility.Logger;
using Microsoft.Win32;
using ToastNotifications;
using ToastNotifications.Messages;
using pbu = RFM.RFM_WPFProgressBarUpdate;

namespace DDrop.Views
{
    /// <summary>
    /// Interaction logic for MeasurementsManager.xaml
    /// </summary>
    public partial class MeasurementsManager : INotifyPropertyChanged
    {
        private readonly ILogger _logger;
        private readonly Notifier _notifier;
        private readonly IAppStateBL _appStateBL;
        private readonly IMapper _mapper;
        private readonly IMeasurementBl _measurementBl;
        private UserView _currentUser;
        private SeriesView _currentSeries;

        public static readonly DependencyProperty ResultingMeasurementsProperty = 
            DependencyProperty.Register("ResultingMeasurements", typeof(ObservableCollection<MeasurementView>), typeof(MeasurementsManager));
        public ObservableCollection<MeasurementView> ResultingMeasurements
        {
            get => (ObservableCollection<MeasurementView>)GetValue(ResultingMeasurementsProperty);
            set => SetValue(ResultingMeasurementsProperty, value);
        }

        public static readonly DependencyProperty SaveButtonIsEnabledProperty = 
            DependencyProperty.Register("SaveButtonIsEnabled", typeof(bool), typeof(MeasurementsManager));
        public bool SaveButtonIsEnabled
        {
            get => (bool)GetValue(SaveButtonIsEnabledProperty);
            set => SetValue(SaveButtonIsEnabledProperty, value);
        }


        private UploadingPhoto _currentFrontUploadingPhoto;
        private UploadingPhoto _currentSideUploadingPhoto;
        private UploadingPhoto _currentThermalUploadingPhoto;

        private bool _allSelectedFrontPhotosChanging;
        private bool? _allSelectedFrontPhotos = false;

        private void AllSelectedFrontChanged()
        {
            if (FrontDropPhotos != null)
            {
                if (_allSelectedFrontPhotosChanging) return;

                try
                {
                    _allSelectedFrontPhotosChanging = true;

                    if (AllSelectedFrontPhotos == true)
                        foreach (var uploadingPhoto in FrontDropPhotos)
                            uploadingPhoto.IsChecked = true;
                    else if (AllSelectedFrontPhotos == false)
                        foreach (var uploadingPhoto in FrontDropPhotos)
                            uploadingPhoto.IsChecked = false;
                }
                finally
                {
                    _allSelectedFrontPhotosChanging = false;
                }
            }
            else
            {
                AllSelectedFrontPhotos = false;
            }
        }

        private void RecheckAllSelectedFront()
        {
            if (_allSelectedFrontPhotosChanging) return;

            try
            {
                _allSelectedFrontPhotosChanging = true;

                if (FrontDropPhotos.All(e => e.IsChecked))
                    AllSelectedFrontPhotos = true;
                else if (FrontDropPhotos.All(e => !e.IsChecked))
                    AllSelectedFrontPhotos = false;
                else
                    AllSelectedFrontPhotos = null;
            }
            finally
            {
                _allSelectedFrontPhotosChanging = false;
            }
        }

        public bool? AllSelectedFrontPhotos
        {
            get => _allSelectedFrontPhotos;
            set
            {
                if (value == _allSelectedFrontPhotos) return;
                _allSelectedFrontPhotos = value;

                AllSelectedFrontChanged();
                OnPropertyChanged(new PropertyChangedEventArgs("AllSelectedFrontPhotos"));
            }
        }

        private void EntryFrontOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(UploadingPhoto.IsChecked))
                RecheckAllSelectedFront();
        }
        
        private bool _allSelectedSidePhotosChanging;
        private bool? _allSelectedSidePhotos = false;

        private void AllSelectedSideChanged()
        {
            if (SideDropPhotos != null)
            {
                if (_allSelectedSidePhotosChanging) return;

                try
                {
                    _allSelectedSidePhotosChanging = true;

                    if (AllSelectedSidePhotos == true)
                        foreach (var uploadingPhoto in SideDropPhotos)
                            uploadingPhoto.IsChecked = true;
                    else if (AllSelectedSidePhotos == false)
                        foreach (var uploadingPhoto in SideDropPhotos)
                            uploadingPhoto.IsChecked = false;
                }
                finally
                {
                    _allSelectedSidePhotosChanging = false;
                }
            }
            else
            {
                AllSelectedSidePhotos = false;
            }
        }

        private void RecheckAllSelectedSide()
        {
            if (_allSelectedSidePhotosChanging) return;

            try
            {
                _allSelectedSidePhotosChanging = true;

                if (SideDropPhotos.All(e => e.IsChecked))
                    AllSelectedSidePhotos = true;
                else if (SideDropPhotos.All(e => !e.IsChecked))
                    AllSelectedSidePhotos = false;
                else
                    AllSelectedSidePhotos = null;
            }
            finally
            {
                _allSelectedSidePhotosChanging = false;
            }
        }

        public bool? AllSelectedSidePhotos
        {
            get => _allSelectedSidePhotos;
            set
            {
                if (value == _allSelectedSidePhotos) return;
                _allSelectedSidePhotos = value;

                AllSelectedSideChanged();
                OnPropertyChanged(new PropertyChangedEventArgs("AllSelectedSidePhotos"));
            }
        }

        private void EntrySideOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(UploadingPhoto.IsChecked))
                RecheckAllSelectedSide();
        }



        private bool _allSelectedThermalPhotosChanging;
        private bool? _allSelectedThermalPhotos = false;

        private void AllSelectedThermalChanged()
        {
            if (ThermalPhotos != null)
            {
                if (_allSelectedThermalPhotosChanging) return;

                try
                {
                    _allSelectedThermalPhotosChanging = true;

                    if (AllSelectedThermalPhotos == true)
                        foreach (var uploadingPhoto in ThermalPhotos)
                            uploadingPhoto.IsChecked = true;
                    else if (AllSelectedThermalPhotos == false)
                        foreach (var uploadingPhoto in ThermalPhotos)
                            uploadingPhoto.IsChecked = false;
                }
                finally
                {
                    _allSelectedThermalPhotosChanging = false;
                }
            }
            else
            {
                AllSelectedThermalPhotos = false;
            }
        }

        private void RecheckAllSelectedThermal()
        {
            if (_allSelectedThermalPhotosChanging) return;

            try
            {
                _allSelectedThermalPhotosChanging = true;

                if (ThermalPhotos.All(e => e.IsChecked))
                    AllSelectedThermalPhotos = true;
                else if (ThermalPhotos.All(e => !e.IsChecked))
                    AllSelectedThermalPhotos = false;
                else
                    AllSelectedThermalPhotos = null;
            }
            finally
            {
                _allSelectedThermalPhotosChanging = false;
            }
        }

        public bool? AllSelectedThermalPhotos
        {
            get => _allSelectedThermalPhotos;
            set
            {
                if (value == _allSelectedThermalPhotos) return;
                _allSelectedThermalPhotos = value;

                AllSelectedThermalChanged();
                OnPropertyChanged(new PropertyChangedEventArgs("AllSelectedThermalPhotos"));
            }
        }

        private void EntryThermalOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(UploadingPhoto.IsChecked))
                RecheckAllSelectedThermal();
        }


        public ObservableCollection<UploadingPhoto> FrontDropPhotos { get; set; }
        
        public ObservableCollection<UploadingPhoto> SideDropPhotos { get; set; }

        public ObservableCollection<UploadingPhoto> ThermalPhotos { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        public MeasurementsManager(Notifier notifier, ILogger logger, IAppStateBL appStateBL, UserView currentUser, SeriesView currentSeries, IMapper mapper, IMeasurementBl measurementBl)
        {
            _notifier = notifier;
            _logger = logger;
            _appStateBL = appStateBL;
            _currentUser = currentUser;
            _currentSeries = currentSeries;
            _mapper = mapper;
            _measurementBl = measurementBl;

            FrontDropPhotos = new ObservableCollection<UploadingPhoto>();
            SideDropPhotos = new ObservableCollection<UploadingPhoto>();
            ThermalPhotos = new ObservableCollection<UploadingPhoto>();
            SaveButtonIsEnabled = true;
            
            InitializeComponent();
        }

        private void AddNewFrontDropPhotos(object sender, RoutedEventArgs e)
        {
            LoadingFront();
            AddDropPhotos(FrontDropPhotos, PhotoTypeView.FrontDropPhoto);
            LoadingFrontComplete();
        }

        private void AddNewSideDropPhotos(object sender, RoutedEventArgs e)
        {
            LoadingSide();
            AddDropPhotos(SideDropPhotos, PhotoTypeView.SideDropPhoto);
            LoadingSideComplete();
        }

        private void AddNewThermalDropPhotos(object sender, RoutedEventArgs e)
        {
            LoadingThermal();
            AddDropPhotos(ThermalPhotos, PhotoTypeView.ThermalPhoto);
            LoadingThermalComplete();
        }

        private void AddDropPhotos(ObservableCollection<UploadingPhoto> addedPhotos, PhotoTypeView type)
        {
            var openFileDialog = new OpenFileDialog
            {
                Multiselect = true,
                AddExtension = true,
                CheckFileExists = true,
                CheckPathExists = true
            };

            ImageInterpreter.GetImageFilter(openFileDialog);

            if (openFileDialog.ShowDialog() == true)
            {
                var pbuHandle1 = pbu.New(ProgressBar, 0, openFileDialog.FileNames.Length, 0);

                if (type == PhotoTypeView.FrontDropPhoto)
                    _appStateBL.ShowAdorner(FrontPhotosLoading);

                if (type == PhotoTypeView.SideDropPhoto)
                    _appStateBL.ShowAdorner(SidePhotosLoading);

                if (type == PhotoTypeView.ThermalPhoto)
                    _appStateBL.ShowAdorner(ThermalPhotosLoading);

                for (var i = 0; i < openFileDialog.FileNames.Length; ++i)
                {
                    pbu.CurValue[pbuHandle1] += 1;

                    var photoForAdd = new UploadingPhoto
                    {
                        PhotoType = type,
                        FilePath = openFileDialog.FileNames[i],
                        Name = openFileDialog.SafeFileNames[i]
                    };

                    if (ImageValidator.ValidateImage(File.ReadAllBytes(photoForAdd.FilePath)))
                    {
                        if (type == PhotoTypeView.FrontDropPhoto)
                            photoForAdd.PropertyChanged += EntryFrontOnPropertyChanged;

                        if (type == PhotoTypeView.SideDropPhoto)
                            photoForAdd.PropertyChanged += EntrySideOnPropertyChanged;

                        if (type == PhotoTypeView.ThermalPhoto)
                            photoForAdd.PropertyChanged += EntryThermalOnPropertyChanged;

                        addedPhotos.Add(photoForAdd);
                    }
                    else
                        _notifier.ShowError($"Файл {photoForAdd.Name} имеет неизвестный формат.");
                }

                if (type == PhotoTypeView.FrontDropPhoto)
                    _appStateBL.HideAdorner(FrontPhotosLoading);

                if (type == PhotoTypeView.SideDropPhoto)
                    _appStateBL.HideAdorner(SidePhotosLoading);

                if (type == PhotoTypeView.ThermalPhoto)
                    _appStateBL.HideAdorner(ThermalPhotosLoading);

                _notifier.ShowSuccess("Новые снимки успешно добавлены.");
                pbu.ResetValue(pbuHandle1);
                pbu.Remove(pbuHandle1);
            }
        }

        private void LoadingFront()
        {
            AddBlankRowFrontButton.IsEnabled = false;
            DeleteFrontUploadedPhotosButton.IsEnabled = false;
            AddFrontDropPhoto.IsEnabled = false;
        }

        private void LoadingFrontComplete()
        {
            AddBlankRowFrontButton.IsEnabled = true;
            DeleteFrontUploadedPhotosButton.IsEnabled = true;
            AddFrontDropPhoto.IsEnabled = true;
        }

        private void LoadingSide()
        {
            AddBlankRowSideButton.IsEnabled = false;
            DeleteSideUploadedPhotosButton.IsEnabled = false;
            AddSidePhoto.IsEnabled = false;
        }

        private void LoadingSideComplete()
        {
            AddBlankRowSideButton.IsEnabled = true;
            DeleteSideUploadedPhotosButton.IsEnabled = true;
            AddSidePhoto.IsEnabled = true;
        }

        private void LoadingThermal()
        {
            AddBlankRowThermalButton.IsEnabled = false;
            DeleteThermalUploadedPhotosButton.IsEnabled = false;
            AddThermalPhoto.IsEnabled = false;
        }

        private void LoadingThermalComplete()
        {
            AddBlankRowThermalButton.IsEnabled = true;
            DeleteThermalUploadedPhotosButton.IsEnabled = true;
            AddThermalPhoto.IsEnabled = true;
        }

        private void DeleteFrontUploadedPhotos(object sender, RoutedEventArgs e)
        {
            LoadingFront();
            DeleteUploadingPhotos(FrontDropPhotos, PhotoType.FrontDropPhoto);
            FrontDropPhotosTable.SelectedIndex = -1;
            LoadingFrontComplete();
        }

        private void DeleteSideUploadedPhotos(object sender, RoutedEventArgs e)
        {
            LoadingSide();
            DeleteUploadingPhotos(SideDropPhotos, PhotoType.SideDropPhoto);
            SideDropPhotosTable.SelectedIndex = -1;
            LoadingSideComplete();
        }

        private void DeleteThermalUploadedPhotos(object sender, RoutedEventArgs e)
        {
            LoadingThermal();
            DeleteUploadingPhotos(SideDropPhotos, PhotoType.ThermalPhoto);
            ThermalPhotosTable.SelectedIndex = -1;
            LoadingThermalComplete();
        }

        private void DeleteUploadingPhotos(ObservableCollection<UploadingPhoto> photosForDeletion, PhotoType type)
        {
            if (photosForDeletion.Count > 0)
            {
                var checkedCount = photosForDeletion.Count(x => x.IsChecked);

                var messageBoxResult =
                    MessageBox.Show(checkedCount > 0 ? "Удалить выбранные фотографии?" : "Удалить все фотографии?",
                        "Подтверждение удаления", MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    if (type == PhotoType.FrontDropPhoto)
                        _appStateBL.ShowAdorner(FrontPhotosLoading);

                    if (type == PhotoType.SideDropPhoto)
                        _appStateBL.ShowAdorner(SidePhotosLoading);

                    if (type == PhotoType.ThermalPhoto)
                        _appStateBL.ShowAdorner(ThermalPhotosLoading);

                    var pbuHandle1 = pbu.New(ProgressBar, 0, photosForDeletion.Count, 0);

                    for (var i = photosForDeletion.Count - 1; i >= 0; i--)
                    {
                        if (checkedCount > 0 && !photosForDeletion[i].IsChecked) continue;

                        if (photosForDeletion[i] == _currentFrontUploadingPhoto)
                        {
                            ImgPreviewFront.Source = null;
                        } 
                        else if (photosForDeletion[i] == _currentSideUploadingPhoto)
                        {
                            ImgPreviewSide.Source = null;
                        }

                        if (!string.IsNullOrEmpty(photosForDeletion[i].Name))
                        {
                            _notifier.ShowSuccess($"Фотография {photosForDeletion[i].Name} была удалена.");
                        }
                        else
                        {
                            _notifier.ShowSuccess("Пустая строка была удалена.");
                        }
                        

                        _logger.LogInfo(new LogEntry
                        {
                            Username = _currentUser.Email,
                            LogCategory = LogCategory.DropPhoto,
                            Message = !string.IsNullOrEmpty(photosForDeletion[i].Name) ? $"Фотография {photosForDeletion[i].Name} была удалена." : "Пустая строка была удалена."
                        });

                        photosForDeletion.Remove(photosForDeletion[i]);

                        pbu.CurValue[pbuHandle1] += 1;
                    }

                    if (type == PhotoType.FrontDropPhoto)
                        _appStateBL.HideAdorner(FrontPhotosLoading);

                    if (type == PhotoType.SideDropPhoto)
                        _appStateBL.HideAdorner(SidePhotosLoading);

                    if (type == PhotoType.ThermalPhoto)
                        _appStateBL.HideAdorner(ThermalPhotosLoading);

                    pbu.ResetValue(pbuHandle1);
                    pbu.Remove(pbuHandle1);
                }
            }
            else
            {
                _notifier.ShowInformation("Нет фотографий для удаления.");
            }
        }

        private void FrontDropPhotosSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _currentFrontUploadingPhoto = (UploadingPhoto)FrontDropPhotosTable.SelectedItem;
            
            if (!string.IsNullOrEmpty(_currentFrontUploadingPhoto?.FilePath))
                ImgPreviewFront.Source = ImageInterpreter.LoadImage(File.ReadAllBytes(_currentFrontUploadingPhoto.FilePath));
        }
        private void SideDropPhotosSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _currentSideUploadingPhoto = (UploadingPhoto)SideDropPhotosTable.SelectedItem;

            if (!string.IsNullOrEmpty(_currentSideUploadingPhoto?.FilePath))
                ImgPreviewSide.Source = ImageInterpreter.LoadImage(File.ReadAllBytes(_currentSideUploadingPhoto.FilePath));
        }

        private void ThermalPhotosSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _currentThermalUploadingPhoto = (UploadingPhoto)ThermalPhotosTable.SelectedItem;

            if (!string.IsNullOrEmpty(_currentThermalUploadingPhoto?.FilePath))
                ImgPreviewThermal.Source = ImageInterpreter.LoadImage(File.ReadAllBytes(_currentThermalUploadingPhoto.FilePath));
        }

        private async void SaveUploadedPhotosAsync(object sender, RoutedEventArgs e)
        {
            if (FrontDropPhotos.Count > 0 || SideDropPhotos.Count > 0 || ThermalPhotos.Count > 0)
            {
                SaveButtonIsEnabled = false;
                LoadingFront();
                LoadingSide();
                LoadingThermal();

                int longestCollectionCount;

                if (FrontDropPhotos.Count > SideDropPhotos.Count)
                {
                    if (FrontDropPhotos.Count > ThermalPhotos.Count)
                    {
                        longestCollectionCount = FrontDropPhotos.Count;
                    }
                    else
                    {
                        longestCollectionCount = ThermalPhotos.Count;
                    }
                }
                else
                {
                    if (SideDropPhotos.Count > ThermalPhotos.Count)
                    {
                        longestCollectionCount = SideDropPhotos.Count;
                    }
                    else
                    {
                        longestCollectionCount = ThermalPhotos.Count;
                    }
                }
                
                ResultingMeasurements = new ObservableCollection<MeasurementView>();

                var pbuHandle1 = pbu.New(ProgressBar, 0, longestCollectionCount, 0);

                for (int i = 0; i < longestCollectionCount; i++)
                {
                    if (FrontDropPhotos.Count == i)
                    {
                        FrontDropPhotos.Add(new UploadingPhoto());
                    }

                    if (SideDropPhotos.Count == i)
                    {
                        SideDropPhotos.Add(new UploadingPhoto());
                    }

                    if (ThermalPhotos.Count == i)
                    {
                        ThermalPhotos.Add(new UploadingPhoto());
                    }

                    ResultingMeasurements.Add(new MeasurementView()
                    {
                        MeasurementId = Guid.NewGuid(),
                        Name = (_currentSeries.MeasurementsSeries.Count + i).ToString(),
                        AddedDate = DateTime.Now,
                        CurrentSeriesId = _currentSeries.SeriesId,
                        CreationDateTime = DateTime.Now,
                        MeasurementOrderInSeries = _currentSeries.MeasurementsSeries.Count + i,
                    });

                    var drop = new DropView()
                    {
                        DropId = ResultingMeasurements[i].MeasurementId,
                    };

                    ResultingMeasurements[i].Drop = drop;

                    if (!string.IsNullOrEmpty(FrontDropPhotos[i]?.FilePath))
                    {
                        var frontPhotoId = Guid.NewGuid();
                        ResultingMeasurements[i].FrontDropPhotoId = frontPhotoId;
                        ResultingMeasurements[i].FrontDropPhoto = new DropPhotoView()
                        {
                            PhotoId = frontPhotoId,
                            Name = FrontDropPhotos[i].Name,
                            Content = File.ReadAllBytes(FrontDropPhotos[i].FilePath),
                            AddedDate = DateTime.Now,
                            CreationDateTime = File.GetCreationTime(FrontDropPhotos[i].FilePath),                           
                            PhotoType = PhotoTypeView.FrontDropPhoto
                        };
                    }

                    if (!string.IsNullOrEmpty(SideDropPhotos[i]?.FilePath))
                    {
                        var sidePhotoId = Guid.NewGuid();

                        ResultingMeasurements[i].SideDropPhotoId = sidePhotoId;
                        ResultingMeasurements[i].SideDropPhoto = new DropPhotoView()
                        {
                            PhotoId = sidePhotoId,
                            Name = SideDropPhotos[i].Name,
                            Content = File.ReadAllBytes(SideDropPhotos[i].FilePath),
                            AddedDate = DateTime.Now,
                            CreationDateTime = File.GetCreationTime(SideDropPhotos[i].FilePath),
                            PhotoType = PhotoTypeView.SideDropPhoto
                        };
                    }

                    if (!string.IsNullOrEmpty(ThermalPhotos[i]?.FilePath))
                    {
                        ResultingMeasurements[i].ThermalPhoto = new ThermalPhotoView()
                        {
                            PhotoId = ResultingMeasurements[i].MeasurementId,
                            Name = ThermalPhotos[i].Name,
                            Content = File.ReadAllBytes(ThermalPhotos[i].FilePath),
                            AddedDate = DateTime.Now,
                            CreationDateTime = File.GetCreationTime(ThermalPhotos[i].FilePath),
                            PhotoType = PhotoTypeView.ThermalPhoto
                        };
                    }

                    try
                    {
                        var seriesId = _currentSeries.SeriesId;
                        var measurementForAdding = ResultingMeasurements[i];
                        await Task.Run(() => _measurementBl.CreateMeasurement(_mapper.Map<MeasurementView, Measurement>(measurementForAdding), seriesId));

                        if (ResultingMeasurements[i].FrontDropPhoto != null)
                            ResultingMeasurements[i].FrontDropPhoto.Content = null;

                        if (ResultingMeasurements[i].SideDropPhoto != null)
                            ResultingMeasurements[i].SideDropPhoto.Content = null;

                        if (ResultingMeasurements[i].ThermalPhoto != null)
                            ResultingMeasurements[i].ThermalPhoto.Content = null;

                        _logger.LogInfo(new LogEntry
                        {
                            Username = _currentUser.Email,
                            LogCategory = LogCategory.Measurement,
                            Message = $"Снимок {ResultingMeasurements[i].Name} добавлен."
                        });
                        _notifier.ShowSuccess($"Снимок {ResultingMeasurements[i].Name} добавлен.");
                    }
                    catch (TimeoutException)
                    {
                        _notifier.ShowError(
                            $"Измерение {ResultingMeasurements[i].Name} не добавлен. Не удалось установить подключение. Проверьте интернет соединение.");
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
                            Username = _currentUser.Email,
                            Details = exception.TargetSite.Name
                        });
                        throw;
                    }

                    pbu.CurValue[pbuHandle1] += 1;
                }

                LoadingFrontComplete();
                LoadingSideComplete();
                LoadingThermalComplete();
                SaveButtonIsEnabled = true;
                pbu.ResetValue(pbuHandle1);
                pbu.Remove(pbuHandle1);

                Close();
            }
            else
            {
                _notifier.ShowInformation("Нет фотографий для сохранения.");
            }
        }

        private void AddBlankRowFront(object sender, RoutedEventArgs e)
        {
            InputDialog rowAmountDialog = new InputDialog("Сколько добавить строк?");
            rowAmountDialog.ShowDialog();

            for (int i = 0; i < Convert.ToInt64(rowAmountDialog.Answer); i++)
            {
                FrontDropPhotos.Add(new UploadingPhoto());
            }
        }

        private void AddBlankRowSide(object sender, RoutedEventArgs e)
        {
            InputDialog rowAmountDialog = new InputDialog("Сколько добавить строк?");
            rowAmountDialog.ShowDialog();

            for (int i = 0; i < Convert.ToInt64(rowAmountDialog.Answer); i++)
            {

                SideDropPhotos.Add(new UploadingPhoto());
            }
        }

        private void AddBlankRowThermal(object sender, RoutedEventArgs e)
        {
            InputDialog rowAmountDialog = new InputDialog("Сколько добавить строк?");
            rowAmountDialog.ShowDialog();

            for (int i = 0; i < Convert.ToInt64(rowAmountDialog.Answer); i++)
            {

                ThermalPhotos.Add(new UploadingPhoto());
            }
        }

        private void DeleteSideUploadingPhoto(object sender, RoutedEventArgs e)
        {
            var message = !string.IsNullOrEmpty(SideDropPhotos[SideDropPhotosTable.SelectedIndex].Name)
                ? $"Удалить фотографию {SideDropPhotos[SideDropPhotosTable.SelectedIndex].Name}?"
                : "Удалить пустую строку?";

            var messageBoxResult = MessageBox.Show(message, "Подтверждение удаления", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                SideDropPhotos.RemoveAt(SideDropPhotosTable.SelectedIndex);
                ImgPreviewSide.Source = null;
            }
        }

        private void DeleteFrontUploadingPhoto(object sender, RoutedEventArgs e)
        {
            var message = !string.IsNullOrEmpty(FrontDropPhotos[FrontDropPhotosTable.SelectedIndex].Name)
                ? $"Удалить фотографию {FrontDropPhotos[FrontDropPhotosTable.SelectedIndex].Name}?"
                : "Удалить пустую строку?";

            var messageBoxResult = MessageBox.Show(message, "Подтверждение удаления", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                FrontDropPhotos.RemoveAt(FrontDropPhotosTable.SelectedIndex);
                ImgPreviewFront.Source = null;
            }
        }

        private void DeleteThermalUploadingPhoto(object sender, RoutedEventArgs e)
        {
            var message = !string.IsNullOrEmpty(ThermalPhotos[ThermalPhotosTable.SelectedIndex].Name)
                ? $"Удалить фотографию {ThermalPhotos[ThermalPhotosTable.SelectedIndex].Name}?"
                : "Удалить пустую строку?";

            var messageBoxResult = MessageBox.Show(message, "Подтверждение удаления", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                ThermalPhotos.RemoveAt(ThermalPhotosTable.SelectedIndex);
                ImgPreviewFront.Source = null;
            }
        }
    }
}
