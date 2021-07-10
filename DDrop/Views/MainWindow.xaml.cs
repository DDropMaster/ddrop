using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using AutoMapper;
using AutoMapper.Internal;
using DDrop.BE.Enums;
using DDrop.BE.Enums.Logger;
using DDrop.BE.Enums.Options;
using DDrop.BE.Models;
using DDrop.BE.Models.Thermal;
using DDrop.BL.AppStateBL;
using DDrop.BL.Calculation;
using DDrop.BL.Comments;
using DDrop.BL.CustomPlots;
using DDrop.BL.Drop;
using DDrop.BL.ExcelOperations;
using DDrop.BL.ExcelOperations.Models;
using DDrop.BL.ExportBL;
using DDrop.BL.ImageProcessing.CSharp;
using DDrop.BL.ImageProcessing.Python;
using DDrop.BL.Measurement;
using DDrop.BL.Radiometric.ThermalDataExtractor;
using DDrop.BL.Radiometric.ThermalPhoto;
using DDrop.BL.ReferenceBL;
using DDrop.BL.Series;
using DDrop.BL.Substance;
using DDrop.BL.User;
using DDrop.Controls.InputDIalog;
using DDrop.Controls.PixelDrawer.Enums;
using DDrop.Controls.PixelDrawer.Models;
using DDrop.Db.DbEntities;
using DDrop.Enums;
using DDrop.Enums.Options;
using DDrop.Logic.Plotting;
using DDrop.Logic.SeriesLogic;
using DDrop.Models;
using DDrop.Models.Thermal;
using DDrop.Properties;
using DDrop.Utility.Animation;
using DDrop.Utility.Calculation;
using DDrop.Utility.DataGrid;
using DDrop.Utility.FileOperations;
using DDrop.Utility.ImageOperations;
using DDrop.Utility.ImageOperations.ImageValidator;
using DDrop.Utility.Logger;
using DDrop.Utility.SeriesExporter;
using DDrop.Utility.SeriesLocalStorageOperations;
using Flir.Atlas.Image;
using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.Win32;
using Newtonsoft.Json;
using Ookii.Dialogs.Wpf;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;
using IGeometryBL = DDrop.Logic.GeometryBL.IGeometryBL;
using pbu = RFM.RFM_WPFProgressBarUpdate;
using Point = System.Drawing.Point;
using Series = DDrop.BE.Models.Series;

namespace DDrop.Views
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        public MainWindow(ISeriesBL seriesBL, IDropPhotoBL dropPhotoBl, IMeasurementBl measurementBl, IDropBL dropBl,
            IDropletImageProcessor dropletImageProcessor, IPythonProvider pythonProvider, ILogger logger,
            IGeometryBL geometryBL, IAppStateBL appStateBL, ICalculationBL calculationBL, IMapper mapper, IPlotBl plotBl, 
            IThermalBL thermalBl, IThermalPhotoBL thermalPhotoBl, ISubstanceBL substanceBl,
            IReferenceBl referenceBl, IUserBl userBl, ICommentsBL commentsBl, ICustomPlotsBl customPlotsBl, IExportBl exportBl,
            ISeriesLogic seriesLogic, IExcelOperationsBl excelOperationBl)
        {
            InitializeComponent();
            InitializePaths();

            FileOperations.CreateDirectory("Temp");
            FileOperations.ClearDirectory("Temp");
            FileOperations.CreateDirectory("Cache");

            if (Settings.Default.CacheDeletion == CacheDeleteVariants.OnLaunch)
            {
                FileOperations.ClearDirectory("Cache");
            }

            _seriesBL = seriesBL;
            _dropPhotoBl = dropPhotoBl;
            _dropletImageProcessor = dropletImageProcessor;
            _pythonProvider = pythonProvider;
            _logger = logger;
            _geometryBL = geometryBL;
            _appStateBL = appStateBL;
            _calculationBL = calculationBL;
            _mapper = mapper;
            _plotBl = plotBl;
            _thermalBl = thermalBl;
            _thermalPhotoBl = thermalPhotoBl;
            _substanceBl = substanceBl;
            _referenceBl = referenceBl;
            _measurementBl = measurementBl;
            _userBl = userBl;
            _dropBl = dropBl;
            _commentsBl = commentsBl;
            _customPlotsBl = customPlotsBl;
            _exportBl = exportBl;
            _seriesLogic = seriesLogic;
            _excelOperationsBl = excelOperationBl;

            AppMainWindow.Show();

            SeriesManagerIsLoading();
            ProgressBar.IsIndeterminate = true;

            Login();

            SubstancesDataProvider = new SubstancesDataProvider(_substanceBl);

            SeriesCollectionToPlot = new SeriesCollection();
            XAxesCollection = new AxesCollection();
            YAxesCollection = new AxesCollection();
            AvailableTemperaturePlots = new ObservableCollection<PlotView>();
            AvailableRadiusPlots = new ObservableCollection<PlotView>();
        }

        #region Variable Declaration

        private bool _allSelectedSeriesChanging;
        private bool? _allSelectedSeries = false;
        private bool _allSelectedPhotosChanging;
        private bool? _allSelectedPhotos = false;
        private bool _allSelectedPlotsChanging;
        private bool? _allSelectedPlots = false;
        private bool _allSelectedTemperaturePlotsChanging;
        private bool? _allSelectedTemperaturePlots = false;

        public bool DrawingHorizontalLine { get; set; }
        public bool DrawingVerticalLine { get; set; }

        private readonly ISeriesBL _seriesBL;
        private readonly IDropPhotoBL _dropPhotoBl;
        private readonly IDropletImageProcessor _dropletImageProcessor;
        private readonly IPythonProvider _pythonProvider;
        private readonly ILogger _logger;
        private readonly IGeometryBL _geometryBL;
        private readonly IMeasurementBl _measurementBl;
        private readonly IAppStateBL _appStateBL;
        private readonly ICalculationBL _calculationBL;
        private readonly IMapper _mapper;
        private readonly IPlotBl _plotBl;
        private readonly IThermalBL _thermalBl;
        private readonly IThermalPhotoBL _thermalPhotoBl;
        private readonly ISubstanceBL _substanceBl;
        private readonly IReferenceBl _referenceBl;
        private readonly IUserBl _userBl;
        private readonly IDropBL _dropBl;
        private readonly ICommentsBL _commentsBl;
        private readonly ICustomPlotsBl _customPlotsBl;
        private readonly IExportBl _exportBl;
        private readonly ISeriesLogic _seriesLogic;
        private readonly IExcelOperationsBl _excelOperationsBl;

        private readonly ObservableCollection<AutoCalculationTemplate> _autoCalculationDefaultTemplates =
            new ObservableCollection<AutoCalculationTemplate>();

        private ObservableCollection<AutoCalculationTemplate> _userAutoCalculationTemplates =
            new ObservableCollection<AutoCalculationTemplate>();

        private AutoCalculationTemplate _currentPhotoAutoCalculationTemplate;

        private int _initialXDiameterInPixels;
        private int _initialYDiameterInPixels;
        private int _initialZDiameterInPixels;
        public bool SaveRequired;

        private bool _twoLineMode;

        public bool SaveThermalPhotoRequired;
        private double _initialTemperature;

        private ThermalPhotoView _copiedThermalPhoto;

        public static readonly DependencyProperty CurrentReferencePhotoProperty =
            DependencyProperty.Register("CurrentReferencePhoto", typeof(ReferencePhotoView), typeof(MainWindow));

        public static readonly DependencyProperty SeriesCollectionToPlotProperty =
            DependencyProperty.Register("SeriesCollectionToPlot", typeof(SeriesCollection), typeof(MainWindow));

        public static readonly DependencyProperty XAxesCollectionProperty =
            DependencyProperty.Register("XAxesCollection", typeof(AxesCollection), typeof(MainWindow));

        public static readonly DependencyProperty YAxesCollectionProperty =
            DependencyProperty.Register("YAxesCollection", typeof(AxesCollection), typeof(MainWindow));

        public static readonly DependencyProperty SubstanceDataProviderProperty = 
            DependencyProperty.Register(" SubstanceDataProvider", typeof(SubstancesDataProvider), typeof(MainWindow));

        public static readonly DependencyProperty AvailableTemperaturePlotsProperty =
            DependencyProperty.Register("AvailableTemperaturePlots", typeof(ObservableCollection<PlotView>), typeof(MainWindow));

        public static readonly DependencyProperty AvailableRadiusPlotsProperty = 
            DependencyProperty.Register("AvailableRadiusPlots", typeof(ObservableCollection<PlotView>), typeof(MainWindow));

        public static readonly DependencyProperty СurrentPythonAutoCalculationTemplateProperty =
            DependencyProperty.Register("СurrentPythonAutoCalculationTemplate", typeof(AutoCalculationTemplate),
                typeof(MainWindow));

        public static readonly DependencyProperty СurrentCSharpAutoCalculationTemplateProperty =
            DependencyProperty.Register("СurrentCSharpAutoCalculationTemplate", typeof(AutoCalculationTemplate),
                typeof(MainWindow));

        public static readonly DependencyProperty PythonAutoCalculationTemplateProperty =
            DependencyProperty.Register("PythonAutoCalculationTemplate",
                typeof(ObservableCollection<AutoCalculationTemplate>), typeof(MainWindow));

        public static readonly DependencyProperty CSharpAutoCalculationTemplateProperty =
            DependencyProperty.Register("CSharpAutoCalculationTemplate",
                typeof(ObservableCollection<AutoCalculationTemplate>), typeof(MainWindow));

        public static readonly DependencyProperty ImageForEditProperty =
            DependencyProperty.Register("ImageForEdit", typeof(ImageSource), typeof(MainWindow));

        public static readonly DependencyProperty CurrentSeriesProperty =
            DependencyProperty.Register("CurrentSeries", typeof(SeriesView), typeof(MainWindow));

        public static readonly DependencyProperty CurrentMeasurementProperty =
            DependencyProperty.Register("CurrentMeasurement", typeof(MeasurementView), typeof(MainWindow));

        public static readonly DependencyProperty CurrentPlotProperty =
            DependencyProperty.Register("CurrentPlot", typeof(PlotView), typeof(MainWindow));

        public static readonly DependencyProperty CurrentThermalPhotosProperty =
            DependencyProperty.Register("CurrentThermalPhotos", typeof(ObservableCollection<ThermalPhotoView>), typeof(MainWindow));

        public static readonly DependencyProperty CurrentThermalPhotoProperty =
            DependencyProperty.Register("CurrentThermalPhoto", typeof(ThermalPhotoView), typeof(MainWindow));

        public static readonly DependencyProperty CurrentReferencePhotosProperty =
            DependencyProperty.Register("CurrentReferencePhotos", typeof(ObservableCollection<ReferencePhotoView>), typeof(MainWindow));

        public static readonly DependencyProperty CurrentDropPhotoProperty =
            DependencyProperty.Register("CurrentDropPhoto", typeof(DropPhotoView), typeof(MainWindow));

        public static readonly DependencyProperty ReferenceImageProperty =
            DependencyProperty.Register("ReferenceImage", typeof(ImageSource), typeof(MainWindow));

        public static readonly DependencyProperty UserProperty =
            DependencyProperty.Register("User", typeof(UserView), typeof(MainWindow));

        public static readonly DependencyProperty ParticularSeriesIndexProperty =
            DependencyProperty.Register("ParticularSeriesIndex", typeof(int), typeof(MainWindow));

        public static readonly DependencyProperty DrawningModeProperty =
            DependencyProperty.Register("DrawningMode", typeof(PixelDrawerMode), typeof(MainWindow));

        public static readonly DependencyProperty CurrentPhotoTypeProperty =
            DependencyProperty.Register("CurrentPhotoType", typeof(PhotoTypeView), typeof(MainWindow));

        public static readonly DependencyProperty CurrentReferencePhotoTypeProperty =
            DependencyProperty.Register("CurrentReferencePhotoType", typeof(PhotoTypeView), typeof(MainWindow));

        public static readonly DependencyProperty DrawnShapesProperty =
            DependencyProperty.Register("DrawnShapes", typeof(DrawnShapes), typeof(MainWindow));

        private readonly Notifier _notifier = new Notifier(cfg =>
        {
            cfg.PositionProvider = new WindowPositionProvider(
                Application.Current.MainWindow,
                Corner.BottomRight,
                10,
                10);

            cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                TimeSpan.FromSeconds(3),
                MaximumNotificationCount.FromCount(5));

            cfg.Dispatcher = Application.Current.Dispatcher;
        });

        #endregion

        #region Properties

        public ReferencePhotoView CurrentReferencePhoto
        {
            get => (ReferencePhotoView) GetValue(CurrentReferencePhotoProperty);
            set
            {
                SetValue(CurrentReferencePhotoProperty, value);
                OnPropertyChanged(new PropertyChangedEventArgs("CurrentReferencePhoto"));
            }
        }

        public SeriesCollection SeriesCollectionToPlot
        {
            get => (SeriesCollection)GetValue(SeriesCollectionToPlotProperty);
            set
            {
                SetValue(SeriesCollectionToPlotProperty, value);
                OnPropertyChanged(new PropertyChangedEventArgs("SeriesCollectionToPlot"));
            }
        }

        public AxesCollection XAxesCollection
        {
            get => (AxesCollection)GetValue(XAxesCollectionProperty);
            set
            {
                SetValue(XAxesCollectionProperty, value);
                OnPropertyChanged(new PropertyChangedEventArgs("XAxesCollection"));
            }
        }

        public AxesCollection YAxesCollection
        {
            get => (AxesCollection)GetValue(YAxesCollectionProperty);
            set
            {
                SetValue(YAxesCollectionProperty, value);
                OnPropertyChanged(new PropertyChangedEventArgs("YAxesCollection"));
            }
        }

        public DrawnShapes DrawnShapes
        {
            get => (DrawnShapes)GetValue(DrawnShapesProperty);
            set => SetValue(DrawnShapesProperty, value);
        }

        public PhotoTypeView CurrentReferencePhotoType
        {
            get => (PhotoTypeView) GetValue(CurrentReferencePhotoTypeProperty);
            set => SetValue(CurrentReferencePhotoTypeProperty, value);
        }

        public PhotoTypeView CurrentPhotoType
        {
            get => (PhotoTypeView)GetValue(CurrentPhotoTypeProperty);
            set => SetValue(CurrentPhotoTypeProperty, value);
        }

        public SubstancesDataProvider SubstancesDataProvider
        {
            get => (SubstancesDataProvider) GetValue(SubstanceDataProviderProperty);
            set => SetValue(SubstanceDataProviderProperty, value);
        }

        public ObservableCollection<PlotView> AvailableRadiusPlots
        {
            get => (ObservableCollection<PlotView>) GetValue(AvailableRadiusPlotsProperty);
            set => SetValue(AvailableRadiusPlotsProperty, value);
        }

        public ObservableCollection<PlotView> AvailableTemperaturePlots
        {
            get => (ObservableCollection<PlotView>)GetValue(AvailableTemperaturePlotsProperty);
            set => SetValue(AvailableTemperaturePlotsProperty, value);
        }

        public AutoCalculationTemplate СurrentPythonAutoCalculationTemplate
        {
            get => (AutoCalculationTemplate)GetValue(СurrentPythonAutoCalculationTemplateProperty);
            set => SetValue(СurrentPythonAutoCalculationTemplateProperty, value);
        }

        public AutoCalculationTemplate СurrentCSharpAutoCalculationTemplate
        {
            get => (AutoCalculationTemplate)GetValue(СurrentCSharpAutoCalculationTemplateProperty);
            set => SetValue(СurrentCSharpAutoCalculationTemplateProperty, value);
        }

        public ObservableCollection<AutoCalculationTemplate> PythonAutoCalculationTemplate
        {
            get => (ObservableCollection<AutoCalculationTemplate>)GetValue(PythonAutoCalculationTemplateProperty);
            set => SetValue(PythonAutoCalculationTemplateProperty, value);
        }

        public ObservableCollection<AutoCalculationTemplate> CSharpAutoCalculationTemplate
        {
            get => (ObservableCollection<AutoCalculationTemplate>)GetValue(CSharpAutoCalculationTemplateProperty);
            set => SetValue(CSharpAutoCalculationTemplateProperty, value);
        }

        public ImageSource ImageForEdit
        {
            get => (ImageSource)GetValue(ImageForEditProperty);
            set => SetValue(ImageForEditProperty, value);
        }

        public PlotView CurrentPlot
        {
            get => (PlotView)GetValue(CurrentPlotProperty);
            set => SetValue(CurrentPlotProperty, value);
        }

        public MeasurementView CurrentMeasurement
        {
            get => (MeasurementView)GetValue(CurrentMeasurementProperty);
            set => SetValue(CurrentMeasurementProperty, value);
        }

        public ObservableCollection<ThermalPhotoView> CurrentThermalPhotos
        {
            get => (ObservableCollection<ThermalPhotoView>)GetValue(CurrentThermalPhotosProperty);
            set => SetValue(CurrentThermalPhotosProperty, value);
        }

        public ThermalPhotoView CurrentThermalPhoto
        {
            get => (ThermalPhotoView)GetValue(CurrentThermalPhotoProperty);
            set => SetValue(CurrentThermalPhotoProperty, value);
        }

        public ObservableCollection<ReferencePhotoView> CurrentReferencePhotos
        {
            get => (ObservableCollection<ReferencePhotoView>)GetValue(CurrentReferencePhotosProperty);
            set => SetValue(CurrentReferencePhotosProperty, value);
        }

        public DropPhotoView CurrentDropPhoto
        {
            get => (DropPhotoView)GetValue(CurrentDropPhotoProperty);
            set => SetValue(CurrentDropPhotoProperty, value);
        }

        public SeriesView CurrentSeries
        {
            get => (SeriesView)GetValue(CurrentSeriesProperty);
            set => SetValue(CurrentSeriesProperty, value);
        }

        public ImageSource ReferenceImage
        {
            get => (ImageSource)GetValue(ReferenceImageProperty);
            set => SetValue(ReferenceImageProperty, value);
        }

        public UserView User
        {
            get => (UserView)GetValue(UserProperty);
            set => SetValue(UserProperty, value);
        }

        public int ParticularSeriesIndex
        {
            get => (int)GetValue(ParticularSeriesIndexProperty);
            set
            {
                SetValue(ParticularSeriesIndexProperty, value);
                OnPropertyChanged(new PropertyChangedEventArgs("ParticularSeriesIndex"));
            }
        }

        public PixelDrawerMode DrawningMode
        {
            get => (PixelDrawerMode)GetValue(DrawningModeProperty);
            set => SetValue(DrawningModeProperty, value);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        #endregion

        #region Authorization

        private void Login()
        {
            var login = new Login(_notifier, _logger, _mapper, _userBl, _seriesBL)
            {
                Owner = AppMainWindow
            };
            login.ShowDialog();

            if (login.LoginSucceeded)
            {
                User = login.UserLogin;

                SeriesDataGrid.ItemsSource = User.UserSeries;

                SeriesManagerLoadingComplete();
                ProgressBar.IsIndeterminate = false;
            }

            if (User == null)
            {
                Close();
            }
            else if (User.IsLoggedIn)
            {
                AccountMenuItem.Visibility = Visibility.Visible;
                LogInMenuItem.Visibility = Visibility.Collapsed;
                LogOutMenuItem.Visibility = Visibility.Visible;
            }
        }

        #endregion

        #region Series

        private void AllSelectedChanged()
        {
            if (User.UserSeries != null)
            {
                if (_allSelectedSeriesChanging) return;

                try
                {
                    _allSelectedSeriesChanging = true;

                    if (AllSelectedSeries == true)
                        foreach (var userSeries in User.UserSeries)
                            userSeries.IsChecked = true;
                    else if (AllSelectedSeries == false)
                        foreach (var userSeries in User.UserSeries)
                            userSeries.IsChecked = false;
                }
                finally
                {
                    _allSelectedSeriesChanging = false;
                }
            }
            else
            {
                AllSelectedSeries = false;
            }
        }

        private void RecheckAllSelected()
        {
            if (_allSelectedSeriesChanging) return;

            try
            {
                _allSelectedSeriesChanging = true;

                if (User.UserSeries.All(e => e.IsChecked))
                    AllSelectedSeries = true;
                else if (User.UserSeries.All(e => !e.IsChecked))
                    AllSelectedSeries = false;
                else
                    AllSelectedSeries = null;
            }
            finally
            {
                _allSelectedSeriesChanging = false;
            }
        }

        public bool? AllSelectedSeries
        {
            get => _allSelectedSeries;
            set
            {
                if (value == _allSelectedSeries) return;
                _allSelectedSeries = value;

                AllSelectedChanged();
                OnPropertyChanged(new PropertyChangedEventArgs("AllSelectedSeries"));
            }
        }

        private void EntryOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(SeriesView.IsChecked))
                RecheckAllSelected();
        }

        private void AllSelectedPlotsChanged()
        {
            if (AvailableRadiusPlots != null)
            {
                if (_allSelectedPlotsChanging) return;

                try
                {
                    _allSelectedPlotsChanging = true;

                    if (AllSelectedPlots == true)
                        foreach (var userPlots in AvailableRadiusPlots)
                            userPlots.IsChecked = true;
                    else if (AllSelectedPlots == false)
                        foreach (var userPlots in AvailableRadiusPlots)
                            userPlots.IsChecked = false;
                }
                finally
                {
                    _allSelectedPlotsChanging = false;
                }
            }
            else
            {
                AllSelectedPlots = false;
            }
        }

        private void RecheckAllSelectedPlots()
        {
            if (_allSelectedPlotsChanging) return;

            try
            {
                _allSelectedPlotsChanging = true;

                if (AvailableRadiusPlots.All(e => e.IsChecked))
                    AllSelectedPlots = true;
                else if (AvailableRadiusPlots.All(e => !e.IsChecked))
                    AllSelectedPlots = false;
                else
                    AllSelectedPlots = null;
            }
            finally
            {
                _allSelectedPlotsChanging = false;
            }
        }

        public bool? AllSelectedPlots
        {
            get => _allSelectedPlots;
            set
            {
                if (value == _allSelectedPlots) return;
                _allSelectedPlots = value;

                AllSelectedPlotsChanged();
                OnPropertyChanged(new PropertyChangedEventArgs("AllSelectedPlots"));
            }
        }

        private void PlotsOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(PlotView.IsChecked))
                RecheckAllSelectedPlots();
        }

        private void AllSelectedTemperaturePlotsChanged()
        {
            if (AvailableTemperaturePlots != null)
            {
                if (_allSelectedTemperaturePlotsChanging) return;

                try
                {
                    _allSelectedTemperaturePlotsChanging = true;

                    if (AllSelectedTemperaturePlots == true)
                        foreach (var userPlots in AvailableTemperaturePlots)
                            userPlots.IsChecked = true;
                    else if (AllSelectedTemperaturePlots == false)
                        foreach (var userPlots in AvailableTemperaturePlots)
                            userPlots.IsChecked = false;
                }
                finally
                {
                    _allSelectedTemperaturePlotsChanging = false;
                }
            }
            else
            {
                AllSelectedTemperaturePlots = false;
            }
        }

        private void RecheckAllSelectedTemperaturePlots()
        {
            if (_allSelectedTemperaturePlotsChanging) return;

            try
            {
                _allSelectedTemperaturePlotsChanging = true;

                if (AvailableTemperaturePlots.All(e => e.IsChecked))
                    AllSelectedTemperaturePlots = true;
                else if (AvailableTemperaturePlots.All(e => !e.IsChecked))
                    AllSelectedTemperaturePlots = false;
                else
                    AllSelectedTemperaturePlots = null;
            }
            finally
            {
                _allSelectedTemperaturePlotsChanging = false;
            }
        }

        public bool? AllSelectedTemperaturePlots
        {
            get => _allSelectedTemperaturePlots;
            set
            {
                if (value == _allSelectedTemperaturePlots) return;
                _allSelectedTemperaturePlots = value;

                AllSelectedTemperaturePlotsChanged();
                OnPropertyChanged(new PropertyChangedEventArgs("AllSelectedTemperaturePlots"));
            }
        }

        private void TemperaturePlotsOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(PlotView.IsChecked))
                RecheckAllSelectedTemperaturePlots();
        }





        private async void AddNewSeries_OnClick(object sender, RoutedEventArgs e)
        {
            if (User.UserSeries == null)
            {
                User.UserSeries = new ObservableCollection<SeriesView>();
                SeriesDataGrid.ItemsSource = User.UserSeries;
            }

            if (!string.IsNullOrWhiteSpace(OneLineSetterValue.Text))
            {
                SeriesWindowLoading();
                SeriesManagerIsLoading();

                var seriesToAdd = new SeriesView()
                {
                    SeriesId = Guid.NewGuid(),
                    Title = OneLineSetterValue.Text,
                    AddedDate = DateTime.Now,
                    CurrentUserId = User.UserId
                };

                try
                {
                    await Task.Run(() => _seriesBL.CreateSeries(_mapper.Map<SeriesView, Series>(seriesToAdd)));

                    seriesToAdd.PropertyChanged += EntryOnPropertyChanged;

                    User.UserSeries.Add(seriesToAdd);
                    SeriesDataGrid.ItemsSource = User.UserSeries;
                    OneLineSetterValue.Text = "";

                    _notifier.ShowSuccess($"Добавлена новая серия {seriesToAdd.Title}");
                    _logger.LogInfo(new LogEntry
                    {
                        Username = User.Email,
                        LogCategory = LogCategory.Series,
                        Message = $"Добавлена новая серия {seriesToAdd.Title}"
                    });
                }
                catch (TimeoutException)
                {
                    _notifier.ShowError(
                        $"Серия {seriesToAdd.Title} не добавлена. Не удалось установить подключение. Проверьте интернет соединение.");
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

                SeriesWindowLoading();
                SeriesManagerLoadingComplete();
            }
            else
            {
                _notifier.ShowInformation("Введите название серии.");
            }
        }

        private async void SeriesDataGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.RemovedItems.Count > 0)
            {
                var old = e.RemovedItems[0] as SeriesView;
                old.Loaded = false;

                old.MeasurementsSeries.Clear();
                old.ReferencePhotoForSeries.Clear();

                foreach (var oldReferencePhotoForSeries in old.ReferencePhotoForSeries)
                {
                    if (oldReferencePhotoForSeries?.Content != null)
                    {
                        oldReferencePhotoForSeries.Content = null;
                    }
                }
            }

            if (sender != e.OriginalSource) return;

            if (User.UserSeries.Count > 0 && SeriesDataGrid.SelectedItem != null)
            {
                if (CurrentDropPhoto != null)
                {
                    foreach (var line in CurrentDropPhoto?.Lines)
                    {
                        ImgCurrent.CanDrawing.Children.Remove(line.Line);
                    }
                }


                if (CurrentSeries?.RegionOfInterest != null)
                {
                    foreach (var typedRectangle in CurrentSeries.RegionOfInterest)
                    {
                        ImgCurrent.CanDrawing.Children.Remove(typedRectangle.RegionOfInterest);
                    }
                }

                if (CurrentDropPhoto?.Contour != null)
                {
                    foreach (var line in CurrentDropPhoto.Contour.Lines)
                    {
                        ImgCurrent.CanDrawing.Children.Remove(line);
                    }
                }

                SingleSeries.IsEnabled = false;
                ProgressBar.IsIndeterminate = true;
                SeriesManagerIsLoading(false);

                var seriesId = User.UserSeries[SeriesDataGrid.SelectedIndex].SeriesId;

                var fullSeries = _mapper.Map<Series, SeriesView>(await Task.Run(() => _seriesBL.GetSingleSerie(seriesId)));

                CurrentSeries = User.UserSeries[SeriesDataGrid.SelectedIndex];

                CurrentSeries.MeasurementsSeries = fullSeries.MeasurementsSeries;
                CurrentSeries.ReferencePhotoForSeries = fullSeries.ReferencePhotoForSeries;
                
                if (CurrentSeries.ReferencePhotoForSeries == null)
                {
                    CurrentSeries.ReferencePhotoForSeries = new ObservableCollection<ReferencePhotoView>();
                }

                CurrentReferencePhotos = new ObservableCollection<ReferencePhotoView>
                {
                    CurrentSeries.ReferencePhotoForSeries.FirstOrDefault(x => x.PhotoType == PhotoTypeView.FrontDropPhoto) ?? new ReferencePhotoView() { PhotoType = PhotoTypeView.FrontDropPhoto },
                    CurrentSeries.ReferencePhotoForSeries.FirstOrDefault(x => x.PhotoType == PhotoTypeView.SideDropPhoto) ?? new ReferencePhotoView() { PhotoType = PhotoTypeView.SideDropPhoto }
                };

                CurrentSeries.ReferencePhotoForSeries = CurrentReferencePhotos;

                SeriesDrawerSwap();

                SeriesManagerLoadingComplete(false);
                
                SingleSeries.IsEnabled = true;
                CurrentSeries.Loaded = true;
                ProgressBar.IsIndeterminate = false;
            }
            else
            {
                SingleSeries.IsEnabled = false;
            }
        }

        private void SeriesDrawerSwap()
        {
            for (int i = MainWindowPixelDrawer.CanDrawing.Children.Count; i-- > 1;)
            {
                MainWindowPixelDrawer.CanDrawing.Children.RemoveAt(i);
            }

            if (CurrentReferencePhoto?.Line != null)
            {
                MainWindowPixelDrawer.CanDrawing.Children.Add(CurrentReferencePhoto.Line);
            }
        }

        private CancellationTokenSource _tokenSource;

        private async void ExportSeriesButton_Click(object sender, RoutedEventArgs e)
        {
            if (User.UserSeries.Any(x => x.IsChecked))
            {
                SeriesManagerIsLoading();
                var saveFileDialog = new SaveFileDialog
                {
                    Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*",
                    AddExtension = true,
                    CheckPathExists = true
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    try
                    {
                        var checkedSeries = User.UserSeries.Where(x => x.IsChecked).ToList();
                        var userPlots = _mapper.Map<ObservableCollection<PlotView>, List<Plot>>(User.Plots);

                        var excelReport = new ExcelReport(saveFileDialog.FileName, Settings.Default.DimensionlessPlots,
                                                          _mapper.Map<List<SeriesView>, List<Series>>(checkedSeries),
                                                          User.FirstName, User.LastName, User.Email, userPlots);

                        await _excelOperationsBl.CreateSingleSeriesExcelFile(excelReport);

                        _notifier.ShowSuccess($"Файл {saveFileDialog.SafeFileName} успешно сохранен.");
                    }
                    catch (InvalidOperationException exception)
                    {
                        _notifier.ShowError($"{exception.InnerException?.InnerException?.Message}");
                    }
                }

                SeriesManagerLoadingComplete();
            }
            else
            {
                _notifier.ShowInformation("Нет выбранных серий.");
            }
        }

        private async void DeleteSeriesButton_Click(object sender, RoutedEventArgs e)
        {
            if (User.UserSeries.Count > 0)
            {
                var checkedCount = User.UserSeries.Count(x => x.IsChecked);

                var messageBoxResult =
                    MessageBox.Show(checkedCount > 0 ? "Удалить выбранные серии?" : "Удалить все серии?",
                        "Подтверждение удаления", MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    var pbuHandle1 = pbu.New(ProgressBar, 0, User.UserSeries.Count, 0);
                    SeriesWindowLoading(false);
                    SeriesManagerIsLoading();
                    for (var i = User.UserSeries.Count - 1; i >= 0; i--)
                    {
                        try
                        {
                            if (checkedCount > 0 && !User.UserSeries[i].IsChecked) continue;

                            if (User.UserSeries[i] == CurrentSeries)
                            {
                                ClearSeriesView();
                            }

                            _notifier.ShowSuccess($"Серия {User.UserSeries[i].Title} была удалена.");

                            _logger.LogInfo(new LogEntry
                            {
                                Username = User.Email,
                                LogCategory = LogCategory.Series,
                                Message = $"Серия {User.UserSeries[i].Title} была удалена."
                            });

                            await _seriesLogic.DeleteSeries(User.UserSeries[i], MainWindowPixelDrawer.CanDrawing, User.UserSeries);

                            if (CurrentDropPhoto != null)
                            {
                                foreach (var line in CurrentDropPhoto.Lines)
                                {
                                    ImgCurrent.CanDrawing.Children.Remove(line.Line);
                                }
                            }

                            if (CurrentSeries?.RegionOfInterest != null)
                            {
                                foreach (var typedRectangle in CurrentSeries.RegionOfInterest)
                                {
                                    ImgCurrent.CanDrawing.Children.Remove(typedRectangle.RegionOfInterest);
                                }
                            }
                        }
                        catch (TimeoutException)
                        {
                            _notifier.ShowError(
                                $"Не удалось удалить серию {User.UserSeries[SeriesDataGrid.SelectedIndex].Title}. Не удалось установить подключение. Проверьте интернет соединение.");
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

                        pbu.CurValue[pbuHandle1] += 1;
                    }

                    pbu.ResetValue(pbuHandle1);
                    pbu.Remove(pbuHandle1);

                    SeriesManagerLoadingComplete();
                    SeriesWindowLoading(false);
                }
            }
            else
            {
                _notifier.ShowInformation("Нет серий для удаления.");
            }
        }

        private void ClearSeriesView()
        {
            Photos.ItemsSource = null;
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is TabControl tc)
            {
                var item = (TabItem) tc.SelectedItem;

                if (item?.Name != null)
                {
                    if (item.Name == "SingleSeries")
                    {
                        Photos.ItemsSource = CurrentSeries.MeasurementsSeries;
                    }
                    else if (item.Name == "CombinedSeriesPlot")
                    {
                        UpdatePlots();
                    }
                }
            }
        }

        private async void ExportSeriesLocal_ClickAsync(object sender, RoutedEventArgs e)
        {
            if (User.UserSeries.Count > 0)
            {
                var saveFileDialog = new VistaFolderBrowserDialog
                {
                    UseDescriptionForTitle = true,
                    Description = "Выберите папку для сохранения..."
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    SeriesWindowLoading(false);
                    SeriesManagerIsLoading();

                    var checkedCount = User.UserSeries.Count(x => x.IsChecked);

                    var pbuHandle1 = pbu.New(ProgressBar, 0, checkedCount > 0 ? checkedCount : User.UserSeries.Count,
                        0);

                    foreach (var series in User.UserSeries)
                    {
                        if (checkedCount > 0 && !series.IsChecked) continue;

                        try
                        {
                            await _exportBl.ExportLocalSeriesAsync(series.SeriesId, saveFileDialog.SelectedPath);

                            _logger.LogInfo(new LogEntry
                            {
                                Username = User.Email,
                                LogCategory = LogCategory.Series,
                                Message = $"файл {series.Title}.ddrops сохранен на диске."
                            });
                            _notifier.ShowSuccess($"файл {series.Title}.ddrops сохранен на диске.");
                        }
                        catch (TimeoutException)
                        {
                            _notifier.ShowError(
                                $"Не загрузить серию {series.Title}. Не удалось установить подключение. Проверьте интернет соединение.");
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

                        pbu.CurValue[pbuHandle1] += 1;
                    }

                    pbu.ResetValue(pbuHandle1);
                    pbu.Remove(pbuHandle1);

                    SeriesManagerLoadingComplete();
                    SeriesWindowLoading(false);
                }
            }
            else
            {
                _notifier.ShowInformation("Нет серий для выгрузки.");
            }
        }

        private async void ImportLocalSeries_ClickAsync(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "DDrop files (*.ddrops)|*.ddrops|All files (*.*)|*.*",
                Multiselect = true,
                AddExtension = true,
                CheckFileExists = true,
                CheckPathExists = true
            };

            if (openFileDialog.ShowDialog() == true)
            {
                SeriesManagerIsLoading();
                SeriesWindowLoading();

                var pbuHandle1 = pbu.New(ProgressBar, 0, openFileDialog.FileNames.Length, 0);

                var userEmail = User.Email;
                try
                {
                    foreach (var fileName in openFileDialog.FileNames)
                    {
                        try
                        {
                            var dbSerieForAdd = new DbSeries();
                            var deserializedSerie = new Series();
                            var dbMeasurementForAdd = new List<DbMeasurement>();

                            using (ZipArchive zip = ZipFile.OpenRead(openFileDialog.FileName))
                            {
                                var serie = zip.Entries.FirstOrDefault(x => x.Name.Contains(".dser"));

                                var measurements = zip.Entries.Where(x => x.Name.Contains(".dmes"));

                                using (var zipEntryStream = serie.Open())
                                {
                                    dbSerieForAdd = await _exportBl.ImportLocalSeriesAsync(zipEntryStream, User.UserId);

                                    await _seriesBL.CreateFullSeries(dbSerieForAdd);
    
                                    deserializedSerie = _mapper.Map<DbSeries, Series>(dbSerieForAdd);

                                    foreach (var referencePhoto in deserializedSerie.ReferencePhotoForSeries)
                                    {
                                        if (referencePhoto?.Content != null)
                                            referencePhoto.Content = null;
                                    }
                                }

                                foreach (var measurement in measurements)
                                {
                                    using (var zipEntryStream = measurement.Open())
                                    {
                                        var deserializedMeasurement = await Task.Run(() => SeriesExporter.ImportLocalMeasurementAsync(zipEntryStream, dbSerieForAdd));

                                        await _measurementBl.CreateMeasurement(_mapper.Map<DbMeasurement, Measurement>(deserializedMeasurement), dbSerieForAdd.SeriesId);

                                        foreach (var dropPhoto in deserializedMeasurement.DropPhotos)
                                        {
                                            dropPhoto.Content = null;
                                            dropPhoto.Contour = null;
                                        }

                                        dbMeasurementForAdd.Add(deserializedMeasurement);
                                    }
                                }
                            }

                            deserializedSerie.MeasurementsSeries = _mapper.Map<List<DbMeasurement>, List<Measurement>>(dbMeasurementForAdd.OrderBy(x => x.MeasurementOrderInSeries).ToList());

                            User.UserSeries.Add(_mapper.Map<Series, SeriesView>(deserializedSerie));

                            try
                            {
                                _logger.LogInfo(new LogEntry
                                {
                                    Username = User.Email,
                                    LogCategory = LogCategory.Series,
                                    Message = $"Серия {dbSerieForAdd.Title} добавлена."
                                });
                                _notifier.ShowSuccess($"Серия {dbSerieForAdd.Title} добавлена.");
                            }
                            catch (TimeoutException)
                            {
                                _notifier.ShowError(
                                    $"Не удалось сохранить серию серию {dbSerieForAdd.Title}. Не удалось установить подключение. Проверьте интернет соединение.");
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
                        catch (JsonException exception)
                        {
                            _logger.LogError(new LogEntry
                            {
                                Username = User.Email,
                                LogCategory = LogCategory.Series,
                                Message =
                                    $"Не удалось десериализовать файл {fileName}. Файл не является файлом серии или поврежден.",
                                Exception = exception.Message,
                                Details = exception.ToString(),
                                InnerException = exception.InnerException?.Message,
                                StackTrace = exception.StackTrace
                            });
                            _notifier.ShowError(
                                $"Не удалось десериализовать файл {fileName}. Файл не является файлом серии или поврежден.");
                        }

                        pbu.CurValue[pbuHandle1] += 1;
                    }
                }
                catch (TimeoutException)
                {
                    _notifier.ShowError(
                        $"Не удалось получить информацию о пользователе {User.Email}. Не удалось установить подключение. Проверьте интернет соединение.");
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

                SeriesManagerLoadingComplete();
                SeriesWindowLoading();
                pbu.ResetValue(pbuHandle1);
                pbu.Remove(pbuHandle1);
            }

            if (SeriesDataGrid.ItemsSource == null)
                SeriesDataGrid.ItemsSource = User.UserSeries;
        }

        private async void SeriesDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            SeriesWindowLoading(false);
            SeriesManagerIsLoading();

            var seriesNameCell = e.EditingElement as TextBox;
            try
            {
                if (seriesNameCell != null)
                {
                    if (!string.IsNullOrWhiteSpace(seriesNameCell.Text))
                    {
                        var seriesId = CurrentSeries.SeriesId;
                        var text = seriesNameCell.Text;

                        await _seriesBL.UpdateSeriesName(text, seriesId);

                        _logger.LogInfo(new LogEntry
                        {
                            Username = User.Email,
                            LogCategory = LogCategory.Series,
                            Message = "Название серии изменено успешно."
                        });
                        _notifier.ShowSuccess("Название серии изменено успешно.");
                    }
                    else
                    {
                        _notifier.ShowInformation("Название серии не может быть пустым.");
                        seriesNameCell.Text = CurrentSeries.Title;
                    }
                }
            }
            catch (TimeoutException)
            {
                _notifier.ShowError(
                    "Не удалось изменить название серии. Не удалось установить подключение. Проверьте интернет соединение.");
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

            SeriesWindowLoading(false);
            SeriesManagerLoadingComplete();
        }

        private void EditIntervalBetweenPhotos_Click(object sender, RoutedEventArgs e)
        {
            SaveIntervalBetweenPhotos.Visibility = Visibility.Visible;
            EditIntervalBetweenPhotos.Visibility = Visibility.Hidden;
            IntervalBetweenPhotos.IsEnabled = true;
        }

        private async void SaveIntervalBetweenPhotos_Click(object sender, RoutedEventArgs e)
        {
            SingleSeriesLoading();
            await SaveIntervalBetweenPhotosAsync();
            SingleSeriesLoadingComplete();
        }

        private async Task SaveIntervalBetweenPhotosAsync()
        {
            if (CurrentSeries != null)
            {
                if (double.TryParse(IntervalBetweenPhotos?.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out var intervalBetweenPhotos))
                {
                    try
                    {
                        SaveIntervalBetweenPhotos.Visibility = Visibility.Hidden;
                        EditIntervalBetweenPhotos.Visibility = Visibility.Visible;
                        if (IntervalBetweenPhotos != null) IntervalBetweenPhotos.IsEnabled = false;
                        ProgressBar.IsIndeterminate = true;

                        var seriesId = CurrentSeries.SeriesId;
                        await _seriesBL.UpdateSeriesIntervalBetweenPhotos(intervalBetweenPhotos, seriesId);

                        CurrentSeries.IntervalBetweenPhotos = intervalBetweenPhotos;

                        _logger.LogInfo(new LogEntry
                        {
                            Username = User.Email,
                            LogCategory = LogCategory.Series,
                            Message =
                                $"Сохранен новый интервал между снимками для серии {CurrentSeries.Title}. Не удалось установить подключение. Проверьте интернет соединение."
                        });
                    }
                    catch (TimeoutException)
                    {
                        _notifier.ShowError(
                            $"Не удалось сохранить новый временной интервал между снимками для серии {CurrentSeries.Title}. Не удалось установить подключение. Проверьте интернет соединение.");
                        if (IntervalBetweenPhotos != null)
                            IntervalBetweenPhotos.Text =
                                CurrentSeries.IntervalBetweenPhotos.ToString(CultureInfo.InvariantCulture);
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

                    ProgressBar.IsIndeterminate = false;
                }
                else
                {
                    CurrentSeries.IntervalBetweenPhotos = 0;
                    _notifier.ShowInformation(
                        "Некорректное значение для интервала между снимками. Укажите интервал между снимками в секундах.");
                }
            }
        }

        #endregion

        #region Drop Photos

        private void AllSelectedPhotosChanged()
        {
            if (CurrentSeries.MeasurementsSeries != null)
            {
                if (_allSelectedPhotosChanging) return;

                try
                {
                    _allSelectedPhotosChanging = true;

                    if (AllSelectedPhotos == true)
                        foreach (var measurement in CurrentSeries.MeasurementsSeries)
                            measurement.IsChecked = true;
                    else if (AllSelectedPhotos == false)
                        foreach (var measurement in CurrentSeries.MeasurementsSeries)
                            measurement.IsChecked = false;
                }
                finally
                {
                    _allSelectedPhotosChanging = false;
                }
            }
            else
            {
                AllSelectedPhotos = false;
            }
        }

        private void RecheckAllSelectedPhotos()
        {
            if (_allSelectedPhotosChanging) return;

            try
            {
                _allSelectedPhotosChanging = true;

                if (CurrentSeries.MeasurementsSeries.All(e => e.IsChecked))
                    AllSelectedPhotos = true;
                else if (CurrentSeries.MeasurementsSeries.All(e => !e.IsChecked))
                    AllSelectedPhotos = false;
                else
                    AllSelectedPhotos = null;
            }
            finally
            {
                _allSelectedPhotosChanging = false;
            }
        }

        public bool? AllSelectedPhotos
        {
            get => _allSelectedPhotos;
            set
            {
                if (value == _allSelectedPhotos) return;
                _allSelectedPhotos = value;

                AllSelectedPhotosChanged();
                OnPropertyChanged(new PropertyChangedEventArgs("AllSelectedPhotos"));
            }
        }

        private void PhotosOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(MeasurementView.IsChecked))
                RecheckAllSelectedPhotos();
        }

        private void MenuItemChoosePhotos_OnClick(object sender, RoutedEventArgs e)
        {
            var measurementManager = new MeasurementsManager(_notifier, _logger, _appStateBL, User, CurrentSeries, _mapper, _measurementBl)
            {
                Owner = AppMainWindow
            };
            measurementManager.ShowDialog();

            if (measurementManager.ResultingMeasurements != null)
            {
                foreach (var measurement in measurementManager.ResultingMeasurements)
                {
                    measurement.PropertyChanged += PhotosOnPropertyChanged;
                    CurrentSeries.MeasurementsSeries.Add(measurement);
                }
            }
        }

        private void Photos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.RemovedItems.Count > 0)
            {
                var oldCurrentMeasurement = e.RemovedItems[0] as MeasurementView;
                var singleOldMeasurement = CurrentSeries.MeasurementsSeries.FirstOrDefault(x =>
                    oldCurrentMeasurement != null && x.MeasurementId == oldCurrentMeasurement.MeasurementId);

                if (singleOldMeasurement != null)
                {
                    foreach (var dropPhoto in singleOldMeasurement.DropPhotos)
                    {
                        dropPhoto.Content = null;
                        dropPhoto.Contour = null;

                        for (int i = ImgCurrent.CanDrawing.Children.Count; i-- > 1;)
                        {
                            ImgCurrent.CanDrawing.Children.RemoveAt(i);
                        }
                    }
                }

                if (singleOldMeasurement?.ThermalPhoto?.Content != null)
                    singleOldMeasurement.ThermalPhoto.Content = null;
            }

            var photosSelectedItem = (MeasurementView) Photos.SelectedItem;
            if (photosSelectedItem != null)
            {
                CurrentMeasurement = CurrentSeries.MeasurementsSeries[Photos.SelectedIndex];

                CurrentThermalPhotos = new ObservableCollection<ThermalPhotoView>();

                if (CurrentSeries?.RegionOfInterest != null)
                {
                    foreach (var typedRectangle in CurrentSeries.RegionOfInterest)
                    {
                        ImgCurrent.CanDrawing.Children.Remove(typedRectangle.RegionOfInterest);
                    }
                }

                if (CurrentMeasurement.DropPhotos.FirstOrDefault(x => x.PhotoType == PhotoTypeView.FrontDropPhoto) == null)
                {
                    CurrentMeasurement.DropPhotos.Add(new DropPhotoView { PhotoType = PhotoTypeView.FrontDropPhoto });
                }

                if (CurrentMeasurement.DropPhotos.FirstOrDefault(x => x.PhotoType == PhotoTypeView.SideDropPhoto) == null)
                {
                    CurrentMeasurement.DropPhotos.Add(new DropPhotoView { PhotoType = PhotoTypeView.SideDropPhoto });
                }

                CurrentThermalPhotos.Add(CurrentMeasurement.ThermalPhoto ?? new ThermalPhotoView {PhotoType = PhotoTypeView.ThermalPhoto});

                ImageForEdit = null;
            }
            else
            {
                ImageForEdit = null;
            }
        }


        private void ShowContourOnPhotosPreview(ContourView contour, Canvas canvas)
        {
            if (canvas != null)
            {
                if (contour != null)
                    foreach (var line in contour.Lines)
                        canvas.Children.Remove(line);

                if (contour != null && _autoCalculationModeOn)
                    foreach (var line in contour.Lines)
                        canvas.Children.Add(line);

                if (CurrentThermalPhoto.Ellipse != null)
                {
                    ImgCurrent.CanDrawing.Children.Remove(CurrentThermalPhoto.Ellipse);
                    ImgCurrent.CanDrawing.Children.Add(CurrentThermalPhoto.Ellipse);

                    Canvas.SetLeft(CurrentThermalPhoto.Ellipse, CurrentThermalPhoto.EllipseCoordinate.X);
                    Canvas.SetTop(CurrentThermalPhoto.Ellipse, CurrentThermalPhoto.EllipseCoordinate.Y);
                }

                if (CurrentSeries?.RegionOfInterest != null)
                {
                    foreach (var typedRectangle in CurrentSeries.RegionOfInterest)
                    {
                        ImgCurrent.CanDrawing.Children.Remove(typedRectangle.RegionOfInterest);
                    }
                }

                if (CurrentSeries?.RegionOfInterest != null)
                {
                    foreach (var typedRectangle in CurrentSeries.RegionOfInterest)
                    {
                        if (typedRectangle.PhotoType == CurrentPhotoType)
                            ImgCurrent.CanDrawing.Children.Add(typedRectangle.RegionOfInterest);
                    }
                }
            }
        }

        private void ShowLinesOnPhotosPreview(DropPhotoView dropPhoto, Canvas canvas)
        {
            if (dropPhoto != null && canvas != null)
            {
                if (dropPhoto.Lines != null)
                {
                    foreach (var line in dropPhoto.Lines)
                    {
                        canvas.Children.Remove(line.Line);
                    }
                }

                if (dropPhoto.Contour != null)
                    foreach (var line in dropPhoto.Contour.Lines)
                        canvas.Children.Remove(line);

                if (CurrentSeries?.RegionOfInterest != null)
                {
                    foreach (var typedRectangle in CurrentSeries.RegionOfInterest)
                    {
                        ImgCurrent.CanDrawing.Children.Remove(typedRectangle.RegionOfInterest);
                    }
                }

                if (_photoEditModeOn)
                {
                    if (dropPhoto.Lines != null)
                    {
                        foreach (var line in dropPhoto.Lines)
                        {
                            canvas.Children.Add(line.Line);
                        }
                    }
                }

                if (dropPhoto.Contour != null && _autoCalculationModeOn)
                    foreach (var line in dropPhoto.Contour.Lines)
                        canvas.Children.Add(line);
                if (CurrentSeries?.RegionOfInterest != null)
                {
                    foreach (var typedRectangle in CurrentSeries.RegionOfInterest)
                    {
                        if (typedRectangle.PhotoType == CurrentPhotoType)
                            ImgCurrent.CanDrawing.Children.Add(typedRectangle.RegionOfInterest);
                    }
                }
            }
        }

        private async void DeleteInputPhotos_OnClick(object sender, RoutedEventArgs e)
        {
            if (CurrentSeries.MeasurementsSeries.Count > 0)
            {
                var checkedCount = CurrentSeries.MeasurementsSeries.Count(x => x.IsChecked);

                var messageBoxResult =
                    MessageBox.Show(checkedCount > 0 ? "Удалить выбранные измерения?" : "Удалить все измерения?",
                        "Подтверждение удаления", MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    var pbuHandle1 = pbu.New(ProgressBar, 0,
                        checkedCount > 0 ? checkedCount : CurrentSeries.MeasurementsSeries.Count, 0);
                    SingleSeriesLoading();
                    _appStateBL.ShowAdorner(CurrentSeriesPhotoContentLoading);

                    for (var i = CurrentSeries.MeasurementsSeries.Count - 1; i >= 0; i--)
                    {
                        try
                        {
                            if (checkedCount > 0 && !CurrentSeries.MeasurementsSeries[i].IsChecked) continue;

                            await DeleteMeasurement(CurrentSeries.MeasurementsSeries[i], CurrentMeasurement, ImgCurrent.CanDrawing);

                            _notifier.ShowSuccess($"Снимок {CurrentSeries.MeasurementsSeries[i].Name} успешно удален.");
                            _logger.LogInfo(new LogEntry
                            {
                                Username = User.Email,
                                LogCategory = LogCategory.Measurement,
                                Message = $"Снимок {CurrentSeries.MeasurementsSeries[i].Name} успешно удален."
                            });

                            CurrentSeries.MeasurementsSeries.RemoveAt(i);
                        }
                        catch (TimeoutException)
                        {
                            _notifier.ShowError(
                                $"Не удалось удалить снимок {CurrentSeries.MeasurementsSeries[i].Name}. Не удалось установить подключение. Проверьте интернет соединение.");
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

                        pbu.CurValue[pbuHandle1] += 1;
                    }

                    pbu.ResetValue(pbuHandle1);
                    pbu.Remove(pbuHandle1);

                    _appStateBL.HideAdorner(CurrentSeriesPhotoContentLoading);
                    SingleSeriesLoadingComplete();
                }
            }
            else
            {
                _notifier.ShowInformation("Нет снимков для удаления.");
            }
        }

        public async Task DeleteDropPhoto(DropPhotoView dropPhoto, DropPhotoView currentDropPhoto, Canvas canvas)
        {
            if (currentDropPhoto != null && dropPhoto.PhotoId == currentDropPhoto.PhotoId)
            {
                foreach (var line in dropPhoto.Lines)
                {
                    canvas.Children.Remove(line.Line);
                }

                if (dropPhoto.Contour != null)
                    foreach (var line in dropPhoto.Contour.Lines)
                        canvas.Children.Remove(line);
            }

            await _dropPhotoBl.DeleteDropPhoto(_mapper.Map<DropPhotoView, DropPhoto>(dropPhoto));
        }

        public async Task DeleteMeasurement(MeasurementView measurement, MeasurementView currentMeasurement, Canvas canvas)
        {
            if (CurrentDropPhoto != null)
            {
                foreach (var dropPhoto in measurement.DropPhotos)
                {
                    foreach (var line in dropPhoto.Lines)
                    {
                        canvas.Children.Remove(line.Line);
                    }

                    if (dropPhoto.Contour != null)
                        foreach (var line in dropPhoto.Contour.Lines)
                            canvas.Children.Remove(line);
                }
            }

            if (measurement.ThermalPhoto != null && CurrentThermalPhotos != null &&
                CurrentThermalPhotos.Contains(measurement.ThermalPhoto))
            {
                CurrentThermalPhotos = null;
            }

            await _measurementBl.DeleteMeasurement(_mapper.Map<MeasurementView, Measurement>(measurement));
        }

        private async void DeleteSingleInputPhotoButton_Click(object sender, RoutedEventArgs e)
        {  
            var messageBoxResult =
                MessageBox.Show($"Удалить снимок {CurrentSeries.MeasurementsSeries[PhotosDetails.SelectedIndex].Name}?",
                    "Подтверждение удаления", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                ProgressBar.IsIndeterminate = true;
                SingleSeriesLoading();
                _appStateBL.ShowAdorner(CurrentSeriesPhotoContentLoading);
                try
                {
                    await DeleteDropPhoto(CurrentMeasurement.DropPhotos[PhotosDetails.SelectedIndex], CurrentDropPhoto, ImgCurrent.CanDrawing);

                    _logger.LogInfo(new LogEntry
                    {
                        Username = User.Email,
                        LogCategory = LogCategory.Measurement,
                        Message = $"Снимок {CurrentMeasurement.DropPhotos[PhotosDetails.SelectedIndex].Name} удален."
                    });

                    _notifier.ShowSuccess(
                        $"Снимок {CurrentMeasurement.DropPhotos[PhotosDetails.SelectedIndex].Name} удален.");

                    CurrentMeasurement.DropPhotos[PhotosDetails.SelectedIndex] = new DropPhotoView()
                    {
                        PhotoType = CurrentDropPhoto.PhotoType
                    };

                    if (CurrentMeasurement.DropPhotos.Any(x => x.Processed))
                    {
                        await ReCalculateDropParameters();
                    }
                }
                catch (TimeoutException)
                {
                    _notifier.ShowError(
                        $"Не удалось удалить снимок {CurrentMeasurement.DropPhotos[PhotosDetails.SelectedIndex].Name}. Не удалось установить подключение. Проверьте интернет соединение.");
                }
                catch (Exception exception)
                {
                    _logger.LogError(new LogEntry
                    {
                        Exception = exception.ToString(),
                        LogCategory = LogCategory.DropPhoto,
                        InnerException = exception.InnerException?.Message,
                        Message = exception.Message,
                        StackTrace = exception.StackTrace,
                        Username = User.Email,
                        Details = exception.TargetSite.Name
                    });
                    throw;
                }

                ProgressBar.IsIndeterminate = false;
                _appStateBL.HideAdorner(CurrentSeriesPhotoContentLoading);
                SingleSeriesLoadingComplete();
            }
        }

        private async void EditInputPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            var pixelsInMillimeter = CurrentSeries.ReferencePhotoForSeries.FirstOrDefault(x => x.PhotoType == CurrentDropPhoto.PhotoType)?.PixelsInMillimeter;

            if (pixelsInMillimeter != null && pixelsInMillimeter != 0)
            {
                await PhotoEditModeOn(CurrentDropPhoto.PhotoType);

                ShowLinesOnPhotosPreview(CurrentDropPhoto, ImgCurrent.CanDrawing);

                PhotosDetails.ItemsSource = new ObservableCollection<DropPhotoView> { CurrentDropPhoto };

                _initialXDiameterInPixels = CurrentDropPhoto.XDiameterInPixels;
                _initialYDiameterInPixels = CurrentDropPhoto.YDiameterInPixels;
                _initialZDiameterInPixels = CurrentDropPhoto.ZDiameterInPixels;

                PixelsInMillimeterHorizontalTextBox.Text = _initialXDiameterInPixels.ToString();
                PixelsInMillimeterVerticalTextBox.Text = _initialYDiameterInPixels.ToString();
                PixelsInMillimeterZTextBox.Text = _initialZDiameterInPixels.ToString();
            }
            else
            {
                _notifier.ShowInformation("Выберите референсное расстояние на референсном снимке.");
            }
        }

        private bool _photoEditModeOn;

        private async Task PhotoEditModeOn(PhotoTypeView photoType)
        {
            SaveInputPhotoEditButton.IsEnabled = true;
            DiscardManualPhotoEdit.IsEnabled = true;
            _photoEditModeOn = true;
            _autoCalculationModeOn = true;
            EditRegularPhotoColumn.Visibility = Visibility.Hidden;
            AddRegularPhotoColumn.Visibility = Visibility.Hidden;
            EditThermalPhotoColumn.Visibility = Visibility.Hidden;
            DeleteThermalPhotoColumn.Visibility = Visibility.Hidden;
            DeleteRegularPhotoColumn.Visibility = Visibility.Hidden;
            SeriesEditMenu.Visibility = Visibility.Hidden;
            Photos.Visibility = Visibility.Collapsed;
            PhotosCheckedColumn.Visibility = Visibility.Hidden;
            ImgCurrent.DrawningIsEnabled = true;

            if (photoType == PhotoTypeView.FrontDropPhoto)
            {
                await DropPhotoModeUiOn();

                ManualEditMenu.Visibility = Visibility.Visible;
                ManualThermalEditMenu.Visibility = Visibility.Hidden;
                ThermalDetails.Visibility = Visibility.Collapsed;

                PixelsInMillimeterZTextBox.Visibility = Visibility.Hidden;
                PixelsInMillimeterHorizontalTextBox.Visibility = Visibility.Visible;
                TemperatureTextBox.Visibility = Visibility.Hidden;
                TemperatureLabel.Visibility = Visibility.Hidden;
                PixelsInMillimeterLabel.Visibility = Visibility.Visible;
                DrawningMode = PixelDrawerMode.Line;
                _twoLineMode = true;
            }

            if (photoType == PhotoTypeView.SideDropPhoto)
            {
                await DropPhotoModeUiOn();

                ManualEditMenu.Visibility = Visibility.Visible;
                ManualThermalEditMenu.Visibility = Visibility.Hidden;
                ThermalDetails.Visibility = Visibility.Collapsed;

                PixelsInMillimeterHorizontalTextBox.Visibility = Visibility.Hidden;
                PixelsInMillimeterZTextBox.Visibility = Visibility.Visible;
                
                TemperatureTextBox.Visibility = Visibility.Hidden;
                TemperatureLabel.Visibility = Visibility.Hidden;
                PixelsInMillimeterLabel.Visibility = Visibility.Visible;
                DrawningMode = PixelDrawerMode.Line;
                _twoLineMode = true;
            }

            if (photoType == PhotoTypeView.ThermalPhoto)
            {
                await DropPhotoModeUiOn();

                await AnimationHelper.AnimateGridRowExpandCollapse(PhotosDetailsRow, false, PhotosDetailsRow.ActualHeight,
                    0, 0, 0, 200);

                ManualEditMenu.Visibility = Visibility.Hidden;
                ManualThermalEditMenu.Visibility = Visibility.Visible;
                PhotosDetails.Visibility = Visibility.Collapsed;

                SaveThermalPhotoEditButton.IsEnabled = true;
                DiscardThermalPhotoEdit.IsEnabled = true;

                PixelsInMillimeterHorizontalTextBox.Visibility = Visibility.Hidden;
                PixelsInMillimeterZTextBox.Visibility = Visibility.Hidden;
                
                TemperatureTextBox.Visibility = Visibility.Visible;
                TemperatureLabel.Visibility = Visibility.Visible;
                PixelsInMillimeterLabel.Visibility = Visibility.Hidden;
                DrawningMode = PixelDrawerMode.Ellipse;
                _twoLineMode = false;
            }

            PhotosPreviewGridSplitter.IsEnabled = false;

            _overrideLoadingBehaviour = true;
            SingleSeriesLoading();
            _loadPhotosContent = false;
        }

        private async Task DropPhotoModeUiOn()
        {
            VisualHelper.SetEnableRowsMove(Photos, true);
            if (Application.Current.MainWindow != null)
            {
                if (PhotosPreviewRow.ActualHeight > PhotosGrid.ActualHeight * 0.6)
                {
                    VisualHelper.SetEnableRowsMove(Photos, false);
                    await AnimationHelper.AnimateGridRowExpandCollapse(PhotosPreviewRow, false, PhotosPreviewRow.ActualHeight,
                        PhotosGrid.ActualHeight - 140, 0, 0, 200);
                }
                else
                {
                    await AnimationHelper.AnimateGridRowExpandCollapse(PhotosPreviewRow, true,
                        PhotosGrid.ActualHeight - 140,
                        PhotosGrid.ActualHeight, 0, 0, 200);
                }
            }
        }

        public async Task PhotoEditModeOff(PhotoTypeView photoType)
        {
            _photoEditModeOn = false;
            _autoCalculationModeOn = false;
            EditRegularPhotoColumn.Visibility = Visibility.Visible;
            AddRegularPhotoColumn.Visibility = Visibility.Visible;
            DeleteRegularPhotoColumn.Visibility = Visibility.Visible;
            EditThermalPhotoColumn.Visibility = Visibility.Visible;
            DeleteThermalPhotoColumn.Visibility = Visibility.Visible;
            SeriesEditMenu.Visibility = Visibility.Visible;
            Photos.Visibility = Visibility.Visible;
            PhotosDetails.Visibility = Visibility.Visible;
            PhotosCheckedColumn.Visibility = Visibility.Visible;
            ImgCurrent.DrawningIsEnabled = false;

            VisualHelper.SetEnableRowsMove(Photos, false);

            if (photoType == PhotoTypeView.SideDropPhoto || photoType == PhotoTypeView.FrontDropPhoto)
            {
                ThermalDetails.Visibility = Visibility.Visible;
                ManualEditMenu.Visibility = Visibility.Hidden;

                CurrentMeasurement.DropPhotos.Clear();

                CurrentMeasurement.DropPhotos = _mapper.Map<List<DropPhoto>, ObservableCollection<DropPhotoView>>(await _dropPhotoBl.GetDropPhotosByMeasurementId(CurrentMeasurement.MeasurementId));

                if (CurrentMeasurement.DropPhotos.FirstOrDefault(x => x.PhotoType == PhotoTypeView.FrontDropPhoto) == null)
                {
                    CurrentMeasurement.DropPhotos.Add(new DropPhotoView() { PhotoType = PhotoTypeView.FrontDropPhoto });
                }

                if (CurrentMeasurement.DropPhotos.FirstOrDefault(x => x.PhotoType == PhotoTypeView.SideDropPhoto) == null)
                {
                    CurrentMeasurement.DropPhotos.Add(new DropPhotoView() { PhotoType = PhotoTypeView.SideDropPhoto });
                }

                PhotosDetails.SelectedItem = CurrentDropPhoto;
                ImageForEdit = ImageInterpreter.LoadImage(CurrentDropPhoto.Content);
            }

            if (photoType == PhotoTypeView.ThermalPhoto)
            {
                await AnimationHelper.AnimateGridRowExpandCollapse(PhotosDetailsRow, true, 100,
                    0, 0, 0, 200);

                ManualThermalEditMenu.Visibility = Visibility.Hidden;
                PhotosDetails.Visibility = Visibility.Visible;

                CurrentThermalPhotos.Clear();

                CurrentThermalPhotos.Add(CurrentMeasurement.ThermalPhoto ?? new ThermalPhotoView() { PhotoType = PhotoTypeView.ThermalPhoto });

                ThermalDetails.SelectedItem = CurrentThermalPhotos; 
                //ImageForEdit = CurrentThermalPhoto.FlirImage.Image.ToBitmapImage();
            }

            if (PhotosPreviewRow.ActualHeight > PhotosGrid.ActualHeight * 0.5)
            {
                VisualHelper.SetEnableRowsMove(Photos, false);
                await AnimationHelper.AnimateGridRowExpandCollapse(PhotosPreviewRow, false, PhotosPreviewRow.ActualHeight, PhotosGrid.ActualHeight * 0.5, 0, 0, 200);
            }
            else
            {
                await AnimationHelper.AnimateGridRowExpandCollapse(PhotosPreviewRow, true,
                    PhotosPreviewRow.ActualHeight,
                    PhotosGrid.ActualHeight * 0.5, 0, 0, 200);
            }

            PhotosPreviewGridSplitter.IsEnabled = true;

            _overrideLoadingBehaviour = false;
            SingleSeriesLoadingComplete();
            _loadPhotosContent = true;

            Photos.ScrollIntoView(Photos.SelectedItem);
        }

        private void VerticalRulerToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            HorizontalRulerToggleButton.IsChecked = false;
            DrawingVerticalLine = true;
        }

        private void VerticalRulerToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            DrawingVerticalLine = false;
        }

        private void HorizontalRulerToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            VerticalRulerToggleButton.IsChecked = false;
            DrawingHorizontalLine = true;
        }

        private void HorizontalRulerToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            DrawingHorizontalLine = false;
        }

        private async void SaveInputPhotoEditButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsSaveRequired()) await SavePixelDiameters();

            await PhotoEditModeOff(PhotoTypeView.FrontDropPhoto);
            ShowLinesOnPhotosPreview(CurrentDropPhoto, ImgCurrent.CanDrawing);
        }

        private async Task SavePixelDiameters()
        {
            SaveInputPhotoEditButton.IsEnabled = false;
            DiscardManualPhotoEdit.IsEnabled = false;

            var xDiameterInPixelsTextBox = Convert.ToInt32(PixelsInMillimeterHorizontalTextBox.Text);
            var yDiameterInPixelsTextBox = Convert.ToInt32(PixelsInMillimeterVerticalTextBox.Text);
            var zDiameterInPixelsTextBox = Convert.ToInt32(PixelsInMillimeterZTextBox.Text);

            CurrentMeasurement.Drop.RadiusInMeters = 0;
            CurrentMeasurement.Drop.VolumeInCubicalMeters = 0;
            CurrentMeasurement.Drop.XDiameterInMeters = 0;
            CurrentMeasurement.Drop.YDiameterInMeters = 0;
            CurrentMeasurement.Drop.ZDiameterInMeters = 0;

            if (_initialXDiameterInPixels != xDiameterInPixelsTextBox)
            {
                CurrentDropPhoto.XDiameterInPixels = xDiameterInPixelsTextBox;
                _initialXDiameterInPixels = xDiameterInPixelsTextBox;
            }

            if (_initialYDiameterInPixels != yDiameterInPixelsTextBox)
            {
                CurrentDropPhoto.YDiameterInPixels = yDiameterInPixelsTextBox;
                _initialYDiameterInPixels = yDiameterInPixelsTextBox;
            }

            if (_initialZDiameterInPixels != zDiameterInPixelsTextBox)
            {
                CurrentDropPhoto.ZDiameterInPixels = zDiameterInPixelsTextBox;
                _initialZDiameterInPixels = zDiameterInPixelsTextBox;
            }

            SaveRequired = false;

            if (CurrentDropPhoto.XDiameterInPixels != 0 && CurrentDropPhoto.YDiameterInPixels != 0 || CurrentDropPhoto.ZDiameterInPixels != 0 && CurrentDropPhoto.YDiameterInPixels != 0)
            {
                _appStateBL.ShowAdorner(CurrentSeriesPhotoContentLoading);
                _appStateBL.ShowAdorner(CurrentSeriesImageLoading);

                SingleSeriesLoading();

                await ReCalculateDropParameters(CurrentMeasurement);

                _appStateBL.HideAdorner(CurrentSeriesPhotoContentLoading);
                _appStateBL.HideAdorner(CurrentSeriesImageLoading);
            }
            else
            {
                _notifier.ShowInformation(
                    $"Не указан один из диаметров. Расчет для снимка {CurrentDropPhoto.Name} не выполнен.");
            }

            await _dropPhotoBl.UpdateDropPhoto(_mapper.Map<DropPhotoView, DropPhoto>(CurrentDropPhoto));
        }

        private bool IsSaveRequired()
        {
            if (int.TryParse(PixelsInMillimeterHorizontalTextBox.Text, out var xDiameterInPixelsTextBox) &&  int.TryParse(PixelsInMillimeterVerticalTextBox.Text, out var yDiameterInPixelsTextBox) && int.TryParse(PixelsInMillimeterZTextBox.Text, out var zDiameterInPixelsTextBox)) 
                if (_initialXDiameterInPixels != xDiameterInPixelsTextBox || _initialYDiameterInPixels != yDiameterInPixelsTextBox || _initialZDiameterInPixels != zDiameterInPixelsTextBox)
                    return SaveRequired = true;

            return SaveRequired;
        }

        private async void DiscardManualPhotoEdit_OnClick(object sender, RoutedEventArgs e)
        {
            if (IsSaveRequired())
            {
                if (MessageBox.Show("Сохранить изменения?", "Предупреждение", MessageBoxButton.YesNo,
                        MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    await SavePixelDiameters();
                }
                else
                {
                    foreach (var line in CurrentDropPhoto.Lines)
                    {
                        ImgCurrent.CanDrawing.Children.Remove(line.Line);
                    }

                    if (CurrentDropPhoto.Contour != null)
                    {
                        foreach (var line in CurrentDropPhoto.Contour.Lines)
                            ImgCurrent.CanDrawing.Children.Remove(line);
                    }

                    CurrentDropPhoto = _mapper.Map<DropPhoto, DropPhotoView>(await _dropPhotoBl.GetDropPhoto(CurrentDropPhoto.PhotoId));

                    ShowLinesOnPhotosPreview(CurrentDropPhoto, ImgCurrent.CanDrawing);
                }
            }

            await PhotoEditModeOff(PhotoTypeView.FrontDropPhoto);
            ShowLinesOnPhotosPreview(CurrentDropPhoto, ImgCurrent.CanDrawing);
        }

        private async void Photos_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            SingleSeriesLoading();

            var editingElement = e.EditingElement as TextBox;
            try
            {
                if (editingElement != null)
                {
                    if (!string.IsNullOrWhiteSpace(editingElement.Text))
                    {
                        ProgressBar.IsIndeterminate = true;
                        var currentMeasurementId = CurrentMeasurement.MeasurementId;


                        switch (Photos.CurrentCell.Column.Header)
                        {
                            case "Измерение":
                            {
                                var text = editingElement.Text;
                                await _measurementBl.UpdateMeasurementName(text, currentMeasurementId);

                                _logger.LogInfo(new LogEntry
                                {
                                    Username = User.Email,
                                    LogCategory = LogCategory.Measurement,
                                    Message = "Название снимка изменено успешно."
                                });
                                _notifier.ShowSuccess("Название снимка изменено успешно.");

                                break;
                            }

                            case "Температура, °С":
                            {
                                if (CurrentMeasurement.ThermalPhoto.PhotoId != Guid.Empty)
                                {
                                    var messageBoxResult = MessageBox.Show("Точка, указанная на тепловизорном снимке будет удалена. Продолжить?",
                                        "Подтверждение", MessageBoxButton.YesNoCancel);
                                    if (messageBoxResult == MessageBoxResult.Yes)
                                    {
                                        var text = editingElement.Text;

                                        double value;

                                        if (double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                                        {
                                            await _dropBl.UpdateDropTemperature(value, CurrentMeasurement.Drop.DropId);
                                        }
                                        else
                                        {
                                            _notifier.ShowInformation("Поле имеет некорректный формат.");
                                            editingElement.Text = CurrentMeasurement.AmbientTemperature.ToString();
                                        }

                                        _logger.LogInfo(new LogEntry
                                        {
                                            Username = User.Email,
                                            LogCategory = LogCategory.Measurement,
                                            Message = "Температура калпи успешно изменена."
                                        });
                                        _notifier.ShowSuccess("Температура калпи успешно изменена.");
                                    }
                                }

                                break;
                            }

                            case "Температура ОС, °С":
                            {
                                var text = editingElement.Text;

                                double value;

                                if (double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                                {
                                    await _measurementBl.UpdateAmbientTemperature(value, currentMeasurementId);
                                }
                                else
                                {
                                    _notifier.ShowInformation("Поле имеет некорректный формат.");
                                    editingElement.Text = CurrentMeasurement.AmbientTemperature.ToString();
                                }

                                _logger.LogInfo(new LogEntry
                                {
                                    Username = User.Email,
                                    LogCategory = LogCategory.Measurement,
                                    Message = "Температура окружающей среды успешно изменена."
                                });
                                _notifier.ShowSuccess("Температура окружающей среды успешно изменена.");

                                break;
                            }
                        }
                    }
                    else
                    {
                        _notifier.ShowInformation("Поле не может быть пустым.");

                        switch (Photos.CurrentCell.Column.Header)
                        {
                            case "Измерение":
                            {
                                editingElement.Text = CurrentMeasurement.Name;

                                break;
                            }

                            case "Температура, °С":
                            {
                                editingElement.Text = CurrentMeasurement.Drop.Temperature.ToString();

                                break;
                            }

                            case "Температура ОС, °С":
                            {
                                editingElement.Text = CurrentMeasurement.AmbientTemperature.ToString();

                                break;
                            }
                        }
                    }
                }
            }
            catch (TimeoutException)
            {
                _notifier.ShowError(
                    "Не удалось изменить название снимка. Не удалось установить подключение. Проверьте интернет соединение.");
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

            SingleSeriesLoadingComplete();
            ProgressBar.IsIndeterminate = false;
        }

        private async void CreationTimeCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (CurrentSeries != null)
            {
                CurrentSeries.Settings.GeneralSeriesSettings.UseCreationDateTime = true;

                try
                {
                    ProgressBar.IsIndeterminate = true;

                    var currentSeries = CurrentSeries;
                    await Task.Run(() => _seriesBL.UpdateSeriesSettings(JsonSerializeProvider.SerializeToString(currentSeries.Settings),
                        currentSeries.SeriesId));

                    _logger.LogInfo(new LogEntry
                    {
                        Username = User.Email,
                        LogCategory = LogCategory.Measurement,
                        Message =
                            $"Серия {CurrentSeries.Title} использует время создания снимков. Порядок фотографий будет проигнорирован."
                    });
                    _notifier.ShowSuccess(
                        $"Серия {CurrentSeries.Title} использует время создания снимков. Порядок фотографий будет проигнорирован.");
                }
                catch (TimeoutException)
                {
                    _notifier.ShowError(
                        "Не удалось изменить режим построения графика. Не удалось установить подключение. Проверьте интернет соединение.");
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

                ProgressBar.IsIndeterminate = false;
            }
        }

        private async void CreationTimeCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (CurrentSeries != null)
            {
                CurrentSeries.Settings.GeneralSeriesSettings.UseCreationDateTime = false;
                try
                {
                    ProgressBar.IsIndeterminate = true;

                    var currentSeries = CurrentSeries;
                    await Task.Run(() => _seriesBL.UpdateSeriesSettings(JsonSerializeProvider.SerializeToString(currentSeries.Settings),
                        currentSeries.SeriesId));

                    _logger.LogInfo(new LogEntry
                    {
                        Username = User.Email,
                        LogCategory = LogCategory.Measurement,
                        Message = $"Серия {CurrentSeries.Title} использует интервал между снимками."
                    });
                    _notifier.ShowSuccess($"Серия {CurrentSeries.Title} использует интервал между снимками.");
                }
                catch (TimeoutException)
                {
                    _notifier.ShowError(
                        "Не удалось изменить режим построения графика. Не удалось установить подключение. Проверьте интернет соединение.");
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

                ProgressBar.IsIndeterminate = false;
            }
        }

        private async void ReCalculate_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentSeries.MeasurementsSeries.Count > 0)
            {
                await ReCalculateDropParameters(true);
            }
            else
            {
                _notifier.ShowInformation("Нет снимков для повторного расчета.");
            }
        }

        private Dictionary<Guid, int> _originalOrder;
        private bool _loadPhotosContent = true;

        private async void EditPhotosOrder_OnClick(object sender, RoutedEventArgs e)
        {
            if (CurrentSeries.MeasurementsSeries.Count > 0)
            {
                await PhotosReOrderModeOn();

                _originalOrder = new Dictionary<Guid, int>();

                foreach (var item in CurrentSeries.MeasurementsSeries)
                    _originalOrder.Add(item.MeasurementId, item.MeasurementOrderInSeries);
            }
            else
            {
                _notifier.ShowInformation("Нет снимков для повторного расчета.");
            }
        }

        private async Task PhotosReOrderModeOn()
        {
            SeriesEditMenu.Visibility = Visibility.Hidden;
            EditRegularPhotoColumn.Visibility = Visibility.Hidden;
            AddRegularPhotoColumn.Visibility = Visibility.Hidden;
            DeleteRegularPhotoColumn.Visibility = Visibility.Hidden;
            PhotosCheckedColumn.Visibility = Visibility.Hidden;
            PhotosPreviewGridSplitter.IsEnabled = false;
            VisualHelper.SetEnableRowsMove(Photos, true);
            await AnimationHelper.AnimateGridRowExpandCollapse(PhotosPreviewRow, false, 300, 0, 0, 0, 200);
            await AnimationHelper.AnimateGridRowExpandCollapse(PhotosDetailsRow, false, 90, 0, 0, 0, 200);
            SavePhotoOrderMenu.Visibility = Visibility.Visible;

            _overrideLoadingBehaviour = true;
            SingleSeriesLoading(false);
            _loadPhotosContent = false;
        }

        public async Task PhotosReOrderModeOff()
        {
            SeriesEditMenu.Visibility = Visibility.Visible;
            EditRegularPhotoColumn.Visibility = Visibility.Visible;
            PhotosCheckedColumn.Visibility = Visibility.Visible;
            PhotosPreviewGridSplitter.IsEnabled = true;
            VisualHelper.SetEnableRowsMove(Photos, false);
            await AnimationHelper.AnimateGridRowExpandCollapse(PhotosPreviewRow, true, 300, 0, 0, 0, 200);
            await AnimationHelper.AnimateGridRowExpandCollapse(PhotosDetailsRow, true, 100, 0, 0, 0, 200);
            SavePhotoOrderMenu.Visibility = Visibility.Hidden;

            _overrideLoadingBehaviour = false;
            SingleSeriesLoadingComplete(false);
            _loadPhotosContent = true;
        }

        private async void SavePhotosOrder_OnClick(object sender, RoutedEventArgs e)
        {
            var orderChanged = OrderChanged();

            if (orderChanged)
            {
                var messageBoxResult = MessageBox.Show("Сохранить новый порядок снимков?",
                    "Подтверждение выхода", MessageBoxButton.YesNoCancel);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    try
                    {
                        await _measurementBl.UpdateMeasurementsOrderInSeries(_mapper.Map<ObservableCollection<MeasurementView>, List<Measurement>>(CurrentSeries.MeasurementsSeries));

                        _logger.LogInfo(new LogEntry
                        {
                            Username = User.Email,
                            LogCategory = LogCategory.Measurement,
                            Message = $"Порядок снимков для серии {CurrentSeries.Title} обновлен."
                        });

                        await PhotosReOrderModeOff();
                        _notifier.ShowSuccess($"Порядок снимков для серии {CurrentSeries.Title} обновлен.");
                    }
                    catch (TimeoutException)
                    {
                        _notifier.ShowError(
                            $"Не удалось обновить порядок снимков для серии {CurrentSeries.Title}. Не удалось установить подключение. Проверьте интернет соединение.");
                        DiscardNewPhotosOrder();
                        await PhotosReOrderModeOff();
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
                else if (messageBoxResult == MessageBoxResult.No)
                {
                    DiscardNewPhotosOrder();

                    _logger.LogInfo(new LogEntry
                    {
                        Username = User.Email,
                        LogCategory = LogCategory.Measurement,
                        Message = "Cтарый порядок снимков восстановлен."
                    });

                    await PhotosReOrderModeOff();
                }
            }
            else
            {
                var messageBoxResult = MessageBox.Show("Закончить редактирование порядка снимков",
                    "Подтверждение выхода", MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes) await PhotosReOrderModeOff();
            }
        }

        private bool OrderChanged()
        {
            var orderChanged = false;

            foreach (var measurement in CurrentSeries.MeasurementsSeries)
                if (_originalOrder[measurement.MeasurementId] != measurement.MeasurementOrderInSeries)
                {
                    orderChanged = true;
                    break;
                }

            return orderChanged;
        }

        private void DiscardNewPhotosOrder()
        {
            foreach (var measurement in CurrentSeries.MeasurementsSeries)
                measurement.MeasurementOrderInSeries = _originalOrder[measurement.MeasurementId];

            CurrentSeries.MeasurementsSeries = OrderByPhotoOrderInSeries(CurrentSeries.MeasurementsSeries);
            _notifier.ShowInformation("Cтарый порядок снимков восстановлен.");
        }

        private async void DiscardPhotosReOrder_OnClick(object sender, RoutedEventArgs e)
        {
            if (OrderChanged())
            {
                var messageBoxResult = MessageBox.Show("Отменить изменение порядка снимков?",
                    "Подтверждение", MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    DiscardNewPhotosOrder();
                    await PhotosReOrderModeOff();
                }
            }
            else
            {
                _notifier.ShowInformation("Порядок снимков остался без изменений.");
                await PhotosReOrderModeOff();
            }
        }

        private static ObservableCollection<MeasurementView> OrderByPhotoOrderInSeries(ObservableCollection<MeasurementView> orderThoseGroups)
        {
            ObservableCollection<MeasurementView> temp;
            temp = new ObservableCollection<MeasurementView>(orderThoseGroups.OrderBy(p => p.MeasurementOrderInSeries));
            orderThoseGroups.Clear();
            foreach (var j in temp) orderThoseGroups.Add(j);
            return orderThoseGroups;
        }

        #endregion

        #region Reference Photo

        private async void ChooseReference_OnClick(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Multiselect = false,
                AddExtension = true,
                CheckFileExists = true
            };

            ImageInterpreter.GetImageFilter(openFileDialog);

            if (openFileDialog.ShowDialog() == true)
            {
                ProgressBar.IsIndeterminate = true;
                SingleSeriesLoading();
                _appStateBL.ShowAdorner(ReferenceImageLoading);

                var referencePhotoContentForAdd = ImageInterpreter.FileToByteArray(openFileDialog.FileName);

                if (ImageValidator.ValidateImage(referencePhotoContentForAdd))
                {
                    if (CurrentSeries.ReferencePhotoForSeries == null)
                    {
                        CurrentSeries.ReferencePhotoForSeries = new ObservableCollection<ReferencePhotoView>();
                    }
                    
                    var photoForChange =
                        CurrentSeries.ReferencePhotoForSeries.FirstOrDefault(x =>
                            x.PhotoType == CurrentReferencePhotoType);

                    ReferencePhotoView newReferencePhoto = new ReferencePhotoView()
                    {
                        Content = referencePhotoContentForAdd,
                        Name = openFileDialog.SafeFileNames[0],
                        PhotoId = photoForChange != null && photoForChange.PhotoId != Guid.Empty ? photoForChange.PhotoId : Guid.NewGuid(),
                        Line = new Line(),
                        PhotoType = CurrentReferencePhotoType,
                        AddedDate = DateTime.Now,
                        CreationDateTime = File.GetCreationTime(openFileDialog.FileNames[0]),
                        CurrentSeriesId = CurrentSeries.SeriesId,
                    };

                    if (CurrentReferencePhoto.SimpleLine != null)
                    {
                        newReferencePhoto.SimpleLine = CurrentReferencePhoto.SimpleLine;
                        newReferencePhoto.SimpleLine.X1 = 0;
                        newReferencePhoto.SimpleLine.X2 = 0;
                        newReferencePhoto.SimpleLine.Y1 = 0;
                        newReferencePhoto.SimpleLine.Y2 = 0;
                    }

                    try
                    {
                        var referencePhotoForAdd = _mapper.Map<ReferencePhotoView, ReferencePhoto>(newReferencePhoto);

                        await Task.Run(() => _referenceBl.UpdateReferencePhoto(referencePhotoForAdd));

                        if (CurrentReferencePhoto.Line != null)
                        {
                            MainWindowPixelDrawer.CanDrawing.Children.Remove(CurrentReferencePhoto.Line);

                            CurrentReferencePhoto.Line = null;
                            CurrentReferencePhoto.PixelsInMillimeter = 0;
                        }

                        CurrentReferencePhoto = newReferencePhoto;

                        if (photoForChange != null)
                        {
                            CurrentSeries.ReferencePhotoForSeries[CurrentSeries.ReferencePhotoForSeries.IndexOf(photoForChange)] = newReferencePhoto;
                        }

                        CurrentReferencePhotos = CurrentSeries.ReferencePhotoForSeries;

                        ReferenceImage = ImageInterpreter.LoadImage(CurrentReferencePhoto.Content);

                        MainWindowPixelDrawer.IsEnabled = false;
                        ChangeReferenceLine.Visibility = Visibility.Visible;
                        SaveReferenceLine.Visibility = Visibility.Hidden;

                        _logger.LogInfo(new LogEntry
                        {
                            Username = User.Email,
                            LogCategory = LogCategory.ReferencePhoto,
                            Message = $"Референсный снимок {CurrentReferencePhoto.Name} добавлен."
                        });
                        _notifier.ShowSuccess(
                            $"Референсный снимок {CurrentReferencePhoto.Name} добавлен.");
                    }
                    catch (TimeoutException)
                    {
                        _notifier.ShowError(
                            $"Референсный снимок {CurrentReferencePhoto.Name} не добавлен. Не удалось установить подключение. Проверьте интернет соединение.");
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

                    ProgressBar.IsIndeterminate = false;
                    SingleSeriesLoadingComplete();
                    _appStateBL.HideAdorner(ReferenceImageLoading);
                }
                else
                {
                    _notifier.ShowError($"Файл {openFileDialog.FileName} имеет неизвестный формат.");
                }
            }
        }

        private async void DeleteReferencePhotoButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentReferencePhoto?.Content != null)
            {
                var messageBoxResult = MessageBox.Show("Удалить референсный снимок?",
                    "Подтверждение удаления", MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    SingleSeriesLoading();
                    _appStateBL.ShowAdorner(ReferenceImageLoading);

                    ProgressBar.IsIndeterminate = true;
                    MainWindowPixelDrawer.IsEnabled = false;
                    ChangeReferenceLine.Visibility = Visibility.Visible;
                    SaveReferenceLine.Visibility = Visibility.Hidden;

                    try
                    {
                        var referencePhotoId = CurrentReferencePhoto.PhotoId;
                        await _referenceBl.DeleteReferencePhoto(referencePhotoId);
                        
                        MainWindowPixelDrawer.CanDrawing.Children.Remove(CurrentReferencePhoto.Line);

                        _notifier.ShowSuccess(
                            $"Референсный снимок {CurrentReferencePhoto.Name} удален.");
                        _logger.LogInfo(new LogEntry
                        {
                            Username = User.Email,
                            LogCategory = LogCategory.ReferencePhoto,
                            Message = $"Референсный снимок {CurrentReferencePhoto.Name} удален."
                        });

                        CurrentReferencePhoto.Name = null;
                        CurrentReferencePhoto.Line = null;
                        CurrentReferencePhoto.PixelsInMillimeter = 0;
                        CurrentReferencePhoto.Content = null;
                        ReferenceImage = null;
                        CurrentSeries.ReferencePhotoForSeries[CurrentSeries.ReferencePhotoForSeries.IndexOf(CurrentReferencePhoto)] = new ReferencePhotoView() { PhotoType = CurrentReferencePhotoType, PhotoId = Guid.Empty };
                    }
                    catch (TimeoutException)
                    {
                        _notifier.ShowError(
                            $"Референсный снимок {CurrentReferencePhoto.Name} не удален. Не удалось установить подключение. Проверьте интернет соединение.");
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

                    SingleSeriesLoadingComplete();
                    _appStateBL.HideAdorner(ReferenceImageLoading);
                    ProgressBar.IsIndeterminate = false;
                }
            }
            else
            {
                _notifier.ShowInformation("Нет референсного снимка для удаления.");
            }
        }

        private async void SaveReferenceLine_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentReferencePhoto.SimpleLine != null && IsReferencePhotoLineChanged())
            {
                SingleSeriesLoading();
                _appStateBL.ShowAdorner(ReferenceImageLoading);

                try
                {
                    ProgressBar.IsIndeterminate = true;

                    var referencePhoto = _mapper.Map<ReferencePhotoView, ReferencePhoto>(CurrentReferencePhoto);

                    await Task.Run(() => _referenceBl.UpdateReferencePhoto(referencePhoto));

                    _logger.LogInfo(new LogEntry
                    {
                        Username = User.Email,
                        LogCategory = LogCategory.ReferencePhoto,
                        Message = $"Сохранено новое референсное расстояние для серии {CurrentSeries.Title}."
                    });
                    _notifier.ShowSuccess($"Сохранено новое референсное расстояние для серии {CurrentSeries.Title}.");

                    if (CurrentSeries.MeasurementsSeries.Any(x => x.Processed))
                        await ReCalculateDropParameters();
                }
                catch (TimeoutException)
                {
                    _notifier.ShowError(
                        $"Не удалось сохранить новое референсное расстояние для серии {CurrentSeries.Title}.");
                    DiscardReferenceLineChanges();
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

                ReferenceEditModeOff();
                _appStateBL.HideAdorner(ReferenceImageLoading);
                ProgressBar.IsIndeterminate = false;
            }
            else
            {
                ReferenceEditModeOff();
                _notifier.ShowInformation("Нет изменений для сохранения.");
            }
        }

        private async Task ReCalculateDropParameters(bool checkForChecked = false)
        {
            var checkedCount = 0;

            if (checkForChecked)
                checkedCount = CurrentSeries.MeasurementsSeries.Count(x => x.IsChecked);

            var messageBoxResult =
                MessageBox.Show("Пересчитать параметры капель?", "Подтверждение", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                ProgressBar.IsIndeterminate = false;

                var pbuHandle1 = pbu.New(ProgressBar, 0, CurrentSeries.MeasurementsSeries.Count, 0);

                foreach (var measurement in CurrentSeries.MeasurementsSeries)
                {
                    if (checkForChecked && checkedCount > 0 && !measurement.IsChecked)
                        continue;

                    if (measurement.DropPhotos.Any(x => x.Processed))
                    {
                        await ReCalculateDropParameters(measurement);
                    }

                    pbu.CurValue[pbuHandle1] += 1;
                }

                pbu.ResetValue(pbuHandle1);
                pbu.Remove(pbuHandle1);
            }
        }

        private async Task ReCalculateDropParameters(MeasurementView measurement)
        {
            if (measurement.Drop != null)
            {
                var tempDrop = new DropView()
                {
                    DropId = measurement.Drop.DropId,
                    RadiusInMeters = measurement.Drop.RadiusInMeters,
                    VolumeInCubicalMeters = measurement.Drop.VolumeInCubicalMeters,
                    XDiameterInMeters = measurement.Drop.XDiameterInMeters,
                    YDiameterInMeters = measurement.Drop.YDiameterInMeters,
                    ZDiameterInMeters = measurement.Drop.ZDiameterInMeters
                };

                try
                {
                    var frontProcessed = measurement.DropPhotos.FirstOrDefault(x => x.PhotoType == PhotoTypeView.FrontDropPhoto)?.Processed ?? false;
                    var sideProcessed = measurement.DropPhotos.FirstOrDefault(x => x.PhotoType == PhotoTypeView.SideDropPhoto)?.Processed ?? false;

                    var drop = await _calculationBL.CalculateDropParameters(_mapper.Map<MeasurementView, Measurement>(measurement), _mapper.Map<ObservableCollection<ReferencePhotoView>, List<ReferencePhoto>> (CurrentSeries.ReferencePhotoForSeries), frontProcessed, sideProcessed);

                    measurement.Drop = _mapper.Map<Drop, DropView>(drop);

                    _logger.LogInfo(new LogEntry
                    {
                        Username = User.Email,
                        LogCategory = LogCategory.Calculation,
                        Message = $"Расчет для снимка {measurement.Name} выполнен."
                    });
                    _notifier.ShowSuccess($"Расчет для снимка {measurement.Name} выполнен.");
                }
                catch (TimeoutException)
                {
                    measurement.Drop = tempDrop;
                    _notifier.ShowError(
                        "Не удалось сохранить результаты расчета. Не удалось установить подключение. Проверьте интернет соединение.");
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
        }

        private int _storedReferencePhotoPixelsInMillimeter;
        private Line _storedReferenceLine;

        private void ChangeReferenceLine_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentReferencePhoto?.Content != null)
            {
                if (CurrentReferencePhoto.SimpleLine != null)
                {
                    _storedReferencePhotoPixelsInMillimeter = CurrentReferencePhoto.PixelsInMillimeter;
                    _storedReferenceLine = new Line
                    {
                        X1 = CurrentReferencePhoto.SimpleLine.X1,
                        X2 = CurrentReferencePhoto.SimpleLine.X2,
                        Y1 = CurrentReferencePhoto.SimpleLine.Y1,
                        Y2 = CurrentReferencePhoto.SimpleLine.Y2,
                        Stroke = System.Windows.Media.Brushes.DeepPink,
                        StrokeThickness = 2
                    };
                }

                ReferenceEditModeOn();
            }
            else
            {
                _notifier.ShowInformation("Загрузите референсный снимок.");
            }
        }

        private async void ReferenceEditModeOn()
        {
            MainWindowPixelDrawer.IsEnabled = true;
            MainWindowPixelDrawer.DrawningIsEnabled = true;
            ChangeReferenceLine.Visibility = Visibility.Hidden;
            SaveReferenceLine.Visibility = Visibility.Visible;
            CancelReferencePhotoEditing.Visibility = Visibility.Visible;

            CurrentReferencePhotos = new ObservableCollection<ReferencePhotoView>()
            {
                CurrentReferencePhotos.FirstOrDefault(x => x == CurrentReferencePhoto)
            };

            PhotosTab.IsEnabled = false;
            SeriesManager.IsEnabled = false;
            ReferenceTab.IsEnabled = false;
            MainMenuBar.IsEnabled = false;
            DeleteButton.Visibility = Visibility.Hidden;
            ChooseReferenceButton.Visibility = Visibility.Hidden;

            DrawningMode = PixelDrawerMode.Line;
            _twoLineMode = false;

            VisualHelper.SetEnableRowsMove(ReferencePhotoDataGrid, true);
            if (Application.Current.MainWindow != null)
            {
                if (PhotosPreviewRow.ActualHeight > ReferenceGrid.ActualHeight * 0.6)
                {
                    VisualHelper.SetEnableRowsMove(ReferencePhotoDataGrid, false);
                    await AnimationHelper.AnimateGridRowExpandCollapse(ReferencePreviewRow, false, ReferencePreviewRow.ActualHeight,
                        ReferencePhotoDataGrid.ActualHeight - 140, 0, 0, 200);
                }
                else
                {
                    await AnimationHelper.AnimateGridRowExpandCollapse(ReferencePreviewRow, true,
                        ReferencePhotoDataGrid.ActualHeight - 140,
                        ReferencePhotoDataGrid.ActualHeight, 0, 0, 200);
                }
            }
        }

        public void ReferenceEditModeOff()
        {
            MainWindowPixelDrawer.IsEnabled = false;
            MainWindowPixelDrawer.DrawningIsEnabled = false;
            ChangeReferenceLine.Visibility = Visibility.Visible;
            SaveReferenceLine.Visibility = Visibility.Hidden;
            CancelReferencePhotoEditing.Visibility = Visibility.Hidden;

            CurrentReferencePhotos = CurrentSeries.ReferencePhotoForSeries;

            SingleSeriesLoadingComplete();
        }

        private void CancelReferencePhotoEditing_OnClick(object sender, RoutedEventArgs e)
        {
            var messageBoxResult =
                MessageBox.Show("Отменить изменения?", "Подтверждение", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes) DiscardReferenceLineChanges();
        }

        private void DiscardReferenceLineChanges()
        {
            if (_storedReferenceLine != null)
            {
                MainWindowPixelDrawer.CanDrawing.Children.Remove(CurrentReferencePhoto.Line);

                CurrentReferencePhoto.Line.X1 = _storedReferenceLine.X1;
                CurrentReferencePhoto.Line.X2 = _storedReferenceLine.X2;
                CurrentReferencePhoto.Line.Y1 = _storedReferenceLine.Y1;
                CurrentReferencePhoto.Line.Y2 = _storedReferenceLine.Y2;
                CurrentReferencePhoto.Line.Stroke = _storedReferenceLine.Stroke;
                CurrentReferencePhoto.Line.StrokeThickness = _storedReferenceLine.StrokeThickness;

                CurrentReferencePhoto.SimpleLine.X1 = _storedReferenceLine.X1;
                CurrentReferencePhoto.SimpleLine.X2 = _storedReferenceLine.X2;
                CurrentReferencePhoto.SimpleLine.Y1 = _storedReferenceLine.Y1;
                CurrentReferencePhoto.SimpleLine.Y2 = _storedReferenceLine.Y2;

                CurrentReferencePhoto.PixelsInMillimeter = _storedReferencePhotoPixelsInMillimeter;

                MainWindowPixelDrawer.CanDrawing.Children.Add(CurrentReferencePhoto.Line);
            }
            else
            {
                CurrentReferencePhoto.PixelsInMillimeter = 0;
                MainWindowPixelDrawer.CanDrawing.Children.Remove(CurrentReferencePhoto.Line);
                CurrentReferencePhoto.Line = null;
                CurrentReferencePhoto.SimpleLine = null;
            }

            ReferenceEditModeOff();
        }

        private bool IsReferencePhotoLineChanged()
        {
            if (_storedReferenceLine == null && CurrentReferencePhoto.Line != null) return true;

            if (CurrentReferencePhoto.Line != null && _storedReferenceLine != null)
                if (Math.Abs(CurrentReferencePhoto.Line.X1 - _storedReferenceLine.X1) > 0.001)
                    return true;

            if (CurrentReferencePhoto.Line != null && _storedReferenceLine != null)
                if (Math.Abs(CurrentReferencePhoto.Line.X2 - _storedReferenceLine.X2) > 0.001)
                    return true;

            if (CurrentReferencePhoto.Line != null && _storedReferenceLine != null)
                if (Math.Abs(CurrentReferencePhoto.Line.Y1 - _storedReferenceLine.Y1) > 0.001)
                    return true;

            if (CurrentReferencePhoto.Line != null && _storedReferenceLine != null)
                if (Math.Abs(CurrentReferencePhoto.Line.Y2 - _storedReferenceLine.Y2) > 0.001)
                    return true;

            return false;
        }

        #endregion

        #region AutoCalculation

        private bool _overrideLoadingBehaviour;

        private async void StartAutoCalculate_OnClick(object sender, RoutedEventArgs e)
        {
            if (CurrentSeries.MeasurementsSeries.Count > 0)
            {
                InitilizeTemplates();
                EndPythonTemplateAdding();
                EndCSharpTemplateAdding();

                DrawningMode = PixelDrawerMode.Rectangle;
                ImgCurrent.DrawningIsEnabled = true;
                _autoCalculationModeOn = true;
                _photoEditModeOn = true;
                SeriesEditMenu.Visibility = Visibility.Hidden;
                EditRegularPhotoColumn.Visibility = Visibility.Hidden;
                DeleteRegularPhotoColumn.Visibility = Visibility.Hidden;
                EditThermalPhotoColumn.Visibility = Visibility.Hidden;
                DeleteThermalPhotoColumn.Visibility = Visibility.Hidden;
                AutoCalculationGridSplitter.IsEnabled = true;
                
                if (CurrentDropPhoto != null)
                    ShowLinesOnPhotosPreview(CurrentDropPhoto, ImgCurrent.CanDrawing);

                AutoCalculationMenu.Visibility = Visibility.Visible;

                SingleSeriesLoading(false);
                _overrideLoadingBehaviour = true;

                await AnimationHelper.AnimateGridColumnExpandCollapseAsync(AutoCalculationColumn, true, 300, 0,
                    AutoCalculationColumn.MinWidth, 0, 200);
            }
            else
            {
                _notifier.ShowInformation("Нет снимков для автоматического расчета.");
            }
        }

        private void InitilizeTemplates()
        {
            _autoCalculationDefaultTemplates.Clear();
            _userAutoCalculationTemplates.Clear();

            InitilizeUserTemplates();
            InitilizeDefaultTemplates();
            BuildTemplates();
        }

        private async void Calculate_OnClick(object sender, RoutedEventArgs e)
        {
            if (CurrentSeries.MeasurementsSeries.Count > 0)
            {
                _appStateBL.ShowAdorner(CurrentSeriesImageLoading);
                _appStateBL.ShowAdorner(CurrentSeriesPhotoContentLoading);
                AutoCalculationMenu.IsEnabled = false;

                await AnimationHelper.AnimateGridColumnExpandCollapseAsync(AutoCalculationColumn, false, 300, 0,
                    AutoCalculationColumn.MinWidth, 0, 200);

                var checkedCount = CurrentSeries.MeasurementsSeries.Count(x => x.IsChecked);

                var pbuHandle1 = pbu.New(ProgressBar, 0,
                    checkedCount > 0 ? checkedCount : CurrentSeries.MeasurementsSeries.Count, 0);

                for (var i = 0; i < CurrentSeries.MeasurementsSeries.Count; i++)
                {
                    if (checkedCount > 0 && !CurrentSeries.MeasurementsSeries[i].IsChecked) continue;

                    AutoCalculationParametersView parameters;
                    var currentCalculationVariant = (CalculationVariantsView) Settings.Default.AutoCalculationType;

                    if (currentCalculationVariant == CalculationVariantsView.CalculateWithCSharp)
                        parameters = new AutoCalculationParametersView()
                        {
                            Ksize = Convert.ToInt32(CShrpKsize.Text),
                            Size1 = Convert.ToInt32(CShrpSize1.Text),
                            Size2 = Convert.ToInt32(CShrpSize2.Text),
                            Treshold1 = Convert.ToInt32(CShrpThreshold1.Text),
                            Treshold2 = Convert.ToInt32(CShrpThreshold2.Text)
                        };
                    else
                        parameters = new AutoCalculationParametersView()
                        {
                            Ksize = Convert.ToInt32(Ksize.Text),
                            Size1 = Convert.ToInt32(Size1.Text),
                            Size2 = Convert.ToInt32(Size2.Text),
                            Treshold1 = Convert.ToInt32(Threshold1.Text),
                            Treshold2 = Convert.ToInt32(Threshold2.Text)
                        };

                    foreach (var photo in CurrentSeries.MeasurementsSeries[i].DropPhotos)
                    {
                        photo.Content = await GetContent(photo);

                        var points = GetPoints(photo, photo.Content, parameters);

                        if (points == null)
                        {
                            photo.Content = null;
                            pbu.CurValue[pbuHandle1] += 1;

                            continue;
                        }

                        photo.Contour = _geometryBL.CreateContour(photo.Contour, points, currentCalculationVariant, parameters,
                            CurrentDropPhoto?.Contour, ImgCurrent);
                        photo.ContourId = photo.Contour.ContourId;

                        _geometryBL.CreateDiameters(photo, points);

                        if (CurrentDropPhoto != null && CurrentDropPhoto.PhotoId == photo.PhotoId)
                        {
                            ShowLinesOnPhotosPreview(CurrentDropPhoto, ImgCurrent.CanDrawing);
                        }
                        else
                        {
                            photo.Content = null;
                        }

                        await _dropPhotoBl.UpdateDropPhoto(_mapper.Map<DropPhotoView, DropPhoto>(photo));
                        await _dropBl.UpdateDrop(_mapper.Map<DropView, Drop>(CurrentSeries.MeasurementsSeries[i].Drop));

                        photo.Contour = null;
                    }

                    if (CurrentSeries.MeasurementsSeries[i].ThermalPhoto != null)
                    {
                        var photo = CurrentSeries.MeasurementsSeries[i].ThermalPhoto;

                        photo.Content = await GetContent(photo);

                        if (CurrentSeries.Settings.AutoCalculationSettings.ThermalAutoCalculationSettings !=
                            ThermalAutoCalculationSettingsView.InRoi)
                        {
                            var points = GetPoints(photo, photo.FlirImage.Image.GetRGBValues(), parameters);
                            photo.Contour = _geometryBL.CreateContour(photo.Contour, points, currentCalculationVariant,
                                parameters, photo.Contour, ImgCurrent);
                            photo.ContourId = photo.Contour.ContourId;
                        }
                        else
                        {
                            photo.Contour = null;
                        }

                        var tempFilePath =
                            ImageInterpreter.ByteArrayToFile(photo.Content, photo.Name, "Temp");
                        photo.FlirImage = _mapper.Map<FlirImage, FlirImageView>(_thermalBl.ProcessImage(tempFilePath));

                        var thermalData = GetTemperature(photo);

                        photo.EllipseCoordinate = new Point(thermalData.X, thermalData.Y);

                        await _thermalPhotoBl.UpdateThermalPhoto(_mapper.Map<ThermalPhotoView, ThermalPhoto>(photo));
                        await _dropBl.UpdateDrop(_mapper.Map<DropView, Drop>(CurrentSeries.MeasurementsSeries[i].Drop));
                    }

                    pbu.CurValue[pbuHandle1] += 1;

                    if (CurrentThermalPhoto != null && CurrentSeries.MeasurementsSeries[i].ThermalPhoto != null &&
                        CurrentThermalPhoto.PhotoId == CurrentSeries.MeasurementsSeries[i].ThermalPhoto.PhotoId)
                    {
                        ShowContourOnPhotosPreview(CurrentThermalPhoto.Contour, ImgCurrent.CanDrawing);
                    }
                    else
                    {
                        var thermalPhoto = CurrentSeries.MeasurementsSeries[i].ThermalPhoto;
                        if (thermalPhoto != null)
                            thermalPhoto.Content = null;
                    }

                    CurrentSeries.MeasurementsSeries[i] = _mapper.Map<Measurement, MeasurementView>(
                        _calculationBL.ReCalculateAllParametersFromLines(
                            _mapper.Map<MeasurementView, Measurement>(CurrentSeries.MeasurementsSeries[i]),
                            _mapper.Map<ObservableCollection<ReferencePhotoView>, List<ReferencePhoto>>(CurrentSeries.ReferencePhotoForSeries)));
                }

                pbu.ResetValue(pbuHandle1);
                pbu.Remove(pbuHandle1);
                _notifier.ShowSuccess("Расчет завершен.");

                await AnimationHelper.AnimateGridColumnExpandCollapseAsync(AutoCalculationColumn, true, 300, 0,
                    AutoCalculationColumn.MinWidth, 0, 200);
                _appStateBL.HideAdorner(CurrentSeriesPhotoContentLoading);
                Photos.IsEnabled = true;
                AutoCalculationMenu.IsEnabled = true;
                _appStateBL.HideAdorner(CurrentSeriesImageLoading);
            }
            else
            {
                _notifier.ShowInformation("Нет фотографий для расчета.");
            }
        }

        private ThermalDataView GetTemperature(ThermalPhotoView photo)
        {
            if (CurrentSeries.Settings.AutoCalculationSettings.UseThermalRoi)
            {
                var testedRoi = CurrentSeries.RegionOfInterest.Where(x => x.PhotoType == PhotoTypeView.ThermalPhoto).FirstOrDefault();

                PointF[] testedContour = new PointF[5];

                testedContour[0] = new PointF(testedRoi.Rectangle.X, testedRoi.Rectangle.Y);
                testedContour[1] = new PointF(testedRoi.Rectangle.X + testedRoi.Rectangle.Width, testedRoi.Rectangle.Y);
                testedContour[2] = new PointF(testedRoi.Rectangle.X + testedRoi.Rectangle.Width, testedRoi.Rectangle.Y + testedRoi.Rectangle.Height);
                testedContour[3] = new PointF(testedRoi.Rectangle.X, testedRoi.Rectangle.Y + testedRoi.Rectangle.Height);
                testedContour[4] = new PointF(testedRoi.Rectangle.X, testedRoi.Rectangle.Y);

                return GetTemperatureFromShape(photo, testedContour);
            }
            else
            {
                PointF[] testedContour = new PointF[photo.Contour.SimpleLines.Count];

                for (int i = 0; i < photo.Contour.SimpleLines.Count; i++)
                {
                    testedContour[i] = new PointF((float)photo.Contour.SimpleLines[i].X1, (float)photo.Contour.SimpleLines[i].Y1);
                }

                return GetTemperatureFromShape(photo, testedContour);
            }
        }

        private ThermalDataView GetTemperatureFromShape(ThermalPhotoView photo, PointF[] testedContour)
        {
            switch (CurrentSeries.Settings.AutoCalculationSettings.TemperatureDetectingMode)
            {
                case TemperatureDetectingModeView.Max:
                {
                    ThermalDataView currentMax = new ThermalDataView();

                    foreach (var thermalData in photo.FlirImage.ThermalData)
                    {
                        if (IsPointInPolygon4(testedContour, new PointF(thermalData.X, thermalData.Y)))
                        {
                            if (currentMax.TemperatureValue < thermalData.TemperatureValue)
                            {
                                currentMax = thermalData;
                            }
                        }
                    }

                    return currentMax;
                }
                case TemperatureDetectingModeView.Min:
                {
                    ThermalDataView currentMin = new ThermalDataView();

                    foreach (var thermalData in photo.FlirImage.ThermalData)
                    {
                        if (IsPointInPolygon4(testedContour, new PointF(thermalData.X, thermalData.Y)))
                        {
                            if (currentMin.TemperatureValue > thermalData.TemperatureValue)
                            {
                                currentMin = thermalData;
                            }
                        }
                    }

                    return currentMin;
                }
                case TemperatureDetectingModeView.Average:
                    var pointsCounter = 0;
                    ThermalDataView sumTemperature = new ThermalDataView();

                    foreach (var thermalData in photo.FlirImage.ThermalData)
                    {
                        if (IsPointInPolygon4(testedContour, new PointF(thermalData.X, thermalData.Y)))
                        {
                            sumTemperature.TemperatureValue += thermalData.TemperatureValue;
                            pointsCounter++;
                        }
                    }

                    sumTemperature.TemperatureValue = sumTemperature.TemperatureValue / pointsCounter;

                    return sumTemperature;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Determines if the given point is inside the polygon
        /// </summary>
        /// <param name="polygon">the vertices of polygon</param>
        /// <param name="testPoint">the given point</param>
        /// <returns>true if the point is inside the polygon; otherwise, false</returns>
        public static bool IsPointInPolygon4(PointF[] polygon, PointF testPoint)
        {
            bool result = false;
            int j = polygon.Count() - 1;
            for (int i = 0; i < polygon.Count(); i++)
            {
                if (polygon[i].Y < testPoint.Y && polygon[j].Y >= testPoint.Y || polygon[j].Y < testPoint.Y && polygon[i].Y >= testPoint.Y)
                {
                    if (polygon[i].X + (testPoint.Y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y) * (polygon[j].X - polygon[i].X) < testPoint.X)
                    {
                        result = !result;
                    }
                }
                j = i;
            }
            return result;
        }

        private Point[] GetPoints(BasePhotoView dropPhoto, byte[] content, AutoCalculationParametersView parameters)
        {
            Point[] points;

            var region = System.Drawing.Rectangle.Empty;

            var currentRegion = CurrentSeries.RegionOfInterest?.FirstOrDefault(x => x.PhotoType == dropPhoto.PhotoType);

            if (currentRegion != null)
            {
                region = currentRegion.Rectangle;
            }

            switch ((CalculationVariantsView)Settings.Default.AutoCalculationType)
            {
                case CalculationVariantsView.CalculateWithPython:
                {
                    try
                    {
                        if (!PythonFilesIsSelected())
                        {
                            throw new InvalidOperationException("Выберите интерпритатор python и исполняемый скрипт.");
                        }

                        points = CalculateWithPython(dropPhoto, parameters);

                        return points;
                    }
                    catch (InvalidOperationException exception)
                    {
                        _notifier.ShowError($"{exception.Message} для снимка {dropPhoto.Name}.");

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
                    }
                    catch (Win32Exception exception)
                    {
                        _notifier.ShowError("Указанный в качестве интерпретатора файл не является исполняемым.");

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
                    }
                    catch (FormatException exception)
                    {
                        _notifier.ShowError("Не удалось построить точки.");

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
                    }

                    break;
                }
                case CalculationVariantsView.CalculateWithCSharp:
                {
                    try
                    {
                        points = _dropletImageProcessor.GetDiameters(content, region, parameters.Ksize,
                            parameters.Treshold1, parameters.Treshold2, parameters.Size1, parameters.Size2);

                        return points;
                    }
                    catch (InvalidOperationException exception)
                    {
                        _notifier.ShowError($"{exception.Message} для снимка {dropPhoto.Name}. Попробуйте изменить параметры расчета.");
                    }

                    break;
                }
                default:
                {
                    _notifier.ShowInformation("Не выбран обработчик для расчета.");
                    break;
                }
            }

            return null;
        }

        private async Task<byte[]> GetContent(BasePhotoView dropPhoto)
        {
            if (dropPhoto.PhotoType == PhotoTypeView.FrontDropPhoto || dropPhoto.PhotoType == PhotoTypeView.SideDropPhoto)
            {
                return await _dropPhotoBl.GetDropPhotoContent(dropPhoto.PhotoId, CancellationToken.None,
                    Settings.Default.UseCache);
            }

            if (dropPhoto.PhotoType == PhotoTypeView.ThermalPhoto)
            {
                return await _thermalPhotoBl.GetThermalPhotoContent(dropPhoto.PhotoId, CancellationToken.None,
                    Settings.Default.UseCache);
            }

            return null;
        }

        private void ComboBox_OnDropDownClosed(object sender, EventArgs e)
        {
            var comboBox = sender as ComboBox;

            if (comboBox != null && comboBox.SelectedIndex == -1)
            {
                comboBox.SelectedIndex = Settings.Default.AutoCalculationType;
            }
            else
            {
                if (comboBox != null) Settings.Default.AutoCalculationType = comboBox.SelectedIndex;
                Settings.Default.Save();
            }
        }

        private void InitializePaths()
        {
            InterpreterTextBox.Text = Settings.Default.Interpreter;
            ScriptToRunTextBox.Text = Settings.Default.ScriptToRun;
        }

        private void ChooseFilePath_OnClick(object sender, RoutedEventArgs e)
        {
            var button = (Button) sender;
            var openFileDialog = new OpenFileDialog
            {
                Multiselect = false,
                AddExtension = true
            };

            if (openFileDialog.ShowDialog() == true)
                UpdateOptions((OptionsEnumView) Enum.Parse(typeof(OptionsEnumView), button.Name), openFileDialog.FileName);
        }

        private void UpdateOptions(OptionsEnumView option, object value)
        {
            switch (option)
            {
                case OptionsEnumView.Interpreter:
                    Settings.Default.Interpreter = (string) value;
                    Settings.Default.Save();
                    InterpreterTextBox.Text = (string) value;
                    break;
                case OptionsEnumView.ScriptToRun:
                    Settings.Default.ScriptToRun = (string) value;
                    Settings.Default.Save();
                    ScriptToRunTextBox.Text = (string) value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(option), option, null);
            }
        }

        private Point[] CalculateWithPython(BasePhotoView dropPhoto, AutoCalculationParametersView parameters)
        {
            if (PythonFilesIsSelected())
                return _pythonProvider.GetDiameters(dropPhoto.Content, dropPhoto.Name,
                    Settings.Default.ScriptToRun, Settings.Default.Interpreter, parameters.Ksize, parameters.Treshold1,
                    parameters.Treshold2, parameters.Size1, parameters.Size2);

            _notifier.ShowInformation("Выберите интерпритатор python и исполняемый скрипт.");

            return null;
        }

        private bool PythonFilesIsSelected()
        {
            return !string.IsNullOrWhiteSpace(Settings.Default.ScriptToRun) ||
                                            !string.IsNullOrWhiteSpace(Settings.Default.Interpreter);
        }
        

        private async void DiscardCalculationResults_OnClick(object sender, RoutedEventArgs e)
        {
            await AutoCalculationModeOff();
            ShowLinesOnPhotosPreview(CurrentDropPhoto, ImgCurrent.CanDrawing);
        }


        private bool _autoCalculationModeOn;
        private async Task AutoCalculationModeOff()
        {
            SeriesEditMenu.Visibility = Visibility.Visible;
            EditRegularPhotoColumn.Visibility = Visibility.Visible;
            DeleteRegularPhotoColumn.Visibility = Visibility.Visible;
            EditThermalPhotoColumn.Visibility = Visibility.Visible;
            DeleteThermalPhotoColumn.Visibility = Visibility.Visible;
            AutoCalculationGridSplitter.IsEnabled = false;

            AutoCalculationMenu.Visibility = Visibility.Hidden;

            _autoCalculationModeOn = false;
            _photoEditModeOn = false;

            await AnimationHelper.AnimateGridColumnExpandCollapseAsync(AutoCalculationColumn, false, 300, 0,
                AutoCalculationColumn.MinWidth, 0, 200);
            _overrideLoadingBehaviour = false;
            SingleSeriesLoadingComplete(false);
        }

        private void InitilizeUserTemplates()
        {
            if (!string.IsNullOrEmpty(Settings.Default.AutoCalculationTemplates))
                _userAutoCalculationTemplates =
                    JsonSerializeProvider.DeserializeFromString<ObservableCollection<AutoCalculationTemplate>>(
                        Settings.Default.AutoCalculationTemplates);
            else
                _userAutoCalculationTemplates = new ObservableCollection<AutoCalculationTemplate>();
        }

        private void InitilizeDefaultTemplates()
        {
            _autoCalculationDefaultTemplates.Add(new AutoCalculationTemplate
            {
                Title = "Default",
                Parameters = new AutoCalculationParametersView()
                {
                    Ksize = 9,
                    Size1 = 100,
                    Size2 = 250,
                    Treshold1 = 50,
                    Treshold2 = 100
                },
                TemplateType = CalculationVariantsView.CalculateWithCSharp
            });

            _autoCalculationDefaultTemplates.Add(new AutoCalculationTemplate
            {
                Title = "Default",
                Parameters = new AutoCalculationParametersView()
                {
                    Ksize = 9,
                    Size1 = 100,
                    Size2 = 250,
                    Treshold1 = 50,
                    Treshold2 = 5
                },
                TemplateType = CalculationVariantsView.CalculateWithPython
            });
        }

        private void BuildTemplates()
        {
            PythonAutoCalculationTemplate = new ObservableCollection<AutoCalculationTemplate>();
            CSharpAutoCalculationTemplate = new ObservableCollection<AutoCalculationTemplate>();

            foreach (var autoCalculationDefaultTemplate in _autoCalculationDefaultTemplates)
            {
                if (autoCalculationDefaultTemplate.TemplateType == CalculationVariantsView.CalculateWithPython)
                    PythonAutoCalculationTemplate.Add(autoCalculationDefaultTemplate);
                if (autoCalculationDefaultTemplate.TemplateType == CalculationVariantsView.CalculateWithCSharp)
                    CSharpAutoCalculationTemplate.Add(autoCalculationDefaultTemplate);
            }

            foreach (var userAutoCalculationTemplate in _userAutoCalculationTemplates)
                switch (userAutoCalculationTemplate.TemplateType)
                {
                    case CalculationVariantsView.CalculateWithPython:
                        PythonAutoCalculationTemplate.Add(userAutoCalculationTemplate);
                        break;
                    case CalculationVariantsView.CalculateWithCSharp:
                        CSharpAutoCalculationTemplate.Add(userAutoCalculationTemplate);
                        break;
                    case CalculationVariantsView.Common:
                        CSharpAutoCalculationTemplate.Add(userAutoCalculationTemplate);
                        PythonAutoCalculationTemplate.Add(userAutoCalculationTemplate);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

            if (_currentPhotoAutoCalculationTemplate != null)
            {
                if (_currentPhotoAutoCalculationTemplate.TemplateType == CalculationVariantsView.CalculateWithCSharp)
                {
                    CSharpAutoCalculationTemplate.Add(_currentPhotoAutoCalculationTemplate);
                    CSharpTemplatesCombobox.SelectedIndex = CSharpAutoCalculationTemplate.Count - 1;
                    СurrentCSharpAutoCalculationTemplate = _currentPhotoAutoCalculationTemplate;

                    СurrentPythonAutoCalculationTemplate = null;
                    PythonTemplatesCombobox.SelectedIndex = -1;
                }
                else
                {
                    PythonAutoCalculationTemplate.Add(_currentPhotoAutoCalculationTemplate);
                    PythonTemplatesCombobox.SelectedIndex = PythonAutoCalculationTemplate.Count - 1;
                    СurrentPythonAutoCalculationTemplate = _currentPhotoAutoCalculationTemplate;

                    СurrentCSharpAutoCalculationTemplate = null;
                    CSharpTemplatesCombobox.SelectedIndex = -1;
                }
            }
            else
            {
                CSharpTemplatesCombobox.SelectedIndex = 0;
                PythonTemplatesCombobox.SelectedIndex = 0;
                СurrentCSharpAutoCalculationTemplate =
                    _autoCalculationDefaultTemplates.FirstOrDefault(x =>
                        x.TemplateType == CalculationVariantsView.CalculateWithCSharp);
                СurrentPythonAutoCalculationTemplate =
                    _autoCalculationDefaultTemplates.FirstOrDefault(x =>
                        x.TemplateType == CalculationVariantsView.CalculateWithPython);
            }
        }

        private void PythonTemplatesCombobox_OnDropDownClosed(object sender, EventArgs e)
        {
            var combobox = sender as ComboBox;

            СurrentPythonAutoCalculationTemplate = new AutoCalculationTemplate();

            if (combobox != null && combobox.SelectedIndex != -1)
                СurrentPythonAutoCalculationTemplate = PythonAutoCalculationTemplate[combobox.SelectedIndex];
            else
                СurrentPythonAutoCalculationTemplate = null;
        }

        private void CSharpTemplatesCombobox_OnDropDownClosed(object sender, EventArgs e)
        {
            var combobox = sender as ComboBox;

            СurrentCSharpAutoCalculationTemplate = new AutoCalculationTemplate();

            if (combobox != null && combobox.SelectedIndex != -1)
                СurrentCSharpAutoCalculationTemplate = CSharpAutoCalculationTemplate[combobox.SelectedIndex];
            else
                СurrentCSharpAutoCalculationTemplate = null;
        }

        private void SavePythonTemplate_OnClick(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TextBoxPythonTemplateName.Text))
            {
                _userAutoCalculationTemplates.Add(new AutoCalculationTemplate
                {
                    Title = TextBoxPythonTemplateName.Text,
                    Id = Guid.NewGuid(),
                    TemplateType = CalculationVariantsView.CalculateWithPython,
                    Parameters = new AutoCalculationParametersView()
                    {
                        Ksize = Ksize.Value ?? 1,
                        Size1 = Size1.Value ?? 1,
                        Size2 = Size2.Value ?? 1,
                        Treshold1 = Threshold1.Value ?? 1,
                        Treshold2 = Threshold2.Value ?? 1
                    }
                });

                Settings.Default.AutoCalculationTemplates =
                    JsonSerializeProvider.SerializeToString(_userAutoCalculationTemplates);

                Settings.Default.Save();
                InitilizeTemplates();

                EndPythonTemplateAdding();
            }
            else
            {
                _notifier.ShowInformation("Введите название для шаблона.");
            }
        }

        private void EndPythonTemplateAdding()
        {
            PythonTemplateName.Visibility = Visibility.Hidden;
            TextBoxPythonTemplateName.Visibility = Visibility.Hidden;
            SavePythonTemplate.Visibility = Visibility.Hidden;
            CancelPythonTemplateAdding.Visibility = Visibility.Hidden;

            AddPythonTemplate.Visibility = Visibility.Visible;
            PythonTemplatesCombobox.Visibility = Visibility.Visible;
            ChoosePythonTemplate.Visibility = Visibility.Visible;
        }

        private void SaveCSharpTemplate_OnClick(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TextBoxCSharpTemplateName.Text))
            {
                _userAutoCalculationTemplates.Add(new AutoCalculationTemplate
                {
                    Title = TextBoxCSharpTemplateName.Text,
                    Id = Guid.NewGuid(),
                    TemplateType = CalculationVariantsView.CalculateWithCSharp,
                    Parameters = new AutoCalculationParametersView()
                    {
                        Ksize = CShrpKsize.Value ?? 1,
                        Size1 = CShrpSize1.Value ?? 1,
                        Size2 = CShrpSize2.Value ?? 1,
                        Treshold1 = CShrpThreshold1.Value ?? 1,
                        Treshold2 = CShrpThreshold2.Value ?? 1
                    }
                });

                Settings.Default.AutoCalculationTemplates =
                    JsonSerializeProvider.SerializeToString(_userAutoCalculationTemplates);

                Settings.Default.Save();
                InitilizeTemplates();

                EndCSharpTemplateAdding();
            }
            else
            {
                _notifier.ShowInformation("Введите название для шаблона.");
            }
        }

        private void EndCSharpTemplateAdding()
        {
            CSharpTemplateName.Visibility = Visibility.Hidden;
            TextBoxCSharpTemplateName.Visibility = Visibility.Hidden;
            SaveCSharpTemplate.Visibility = Visibility.Hidden;
            CancelCSharpTemplateAdding.Visibility = Visibility.Hidden;

            AddCSharpTemplate.Visibility = Visibility.Visible;
            CSharpTemplatesCombobox.Visibility = Visibility.Visible;
            ChooseCSharpTemplate.Visibility = Visibility.Visible;
        }

        private void AddPythonTemplate_OnClick(object sender, RoutedEventArgs e)
        {
            PythonTemplateName.Visibility = Visibility.Visible;
            TextBoxPythonTemplateName.Visibility = Visibility.Visible;
            SavePythonTemplate.Visibility = Visibility.Visible;
            CancelPythonTemplateAdding.Visibility = Visibility.Visible;

            AddPythonTemplate.Visibility = Visibility.Hidden;
            PythonTemplatesCombobox.Visibility = Visibility.Hidden;
            ChoosePythonTemplate.Visibility = Visibility.Hidden;
        }

        private void AddCSharpTemplate_OnClick(object sender, RoutedEventArgs e)
        {
            CSharpTemplateName.Visibility = Visibility.Visible;
            TextBoxCSharpTemplateName.Visibility = Visibility.Visible;
            SaveCSharpTemplate.Visibility = Visibility.Visible;
            CancelCSharpTemplateAdding.Visibility = Visibility.Visible;

            AddCSharpTemplate.Visibility = Visibility.Hidden;
            CSharpTemplatesCombobox.Visibility = Visibility.Hidden;
            ChooseCSharpTemplate.Visibility = Visibility.Hidden;
        }

        private void CancelPythonTemplateAdding_OnClick(object sender, RoutedEventArgs e)
        {
            EndPythonTemplateAdding();
        }

        private void CancelCSharpTemplateAdding_OnClick(object sender, RoutedEventArgs e)
        {
            EndCSharpTemplateAdding();
        }

        #endregion

        #region Account

        private void LoginMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SeriesManagerIsLoading();
            ProgressBar.IsIndeterminate = true;

            Login();
        }

        private void AccountMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var accountMenu = new Account(User, _notifier, _logger, _mapper, _userBl);
            accountMenu.ShowDialog();
        }

        private void LogoutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var messageBoxResult = MessageBox.Show($"Выйти из учетной записи {User.Email}?",
                "Подтверждение выхода", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                MainTabControl.SelectedIndex = 0;

                if (CurrentSeries != null)
                {
                    foreach (var referencePhotoView in CurrentSeries.ReferencePhotoForSeries)
                    {
                        if (referencePhotoView?.Line != null)
                            MainWindowPixelDrawer.CanDrawing.Children.Remove(referencePhotoView.Line);
                    }
                }

                if (User.UserSeries != null)
                {
                    User.UserSeries.Clear();
                    User = null;
                }

                if (CurrentSeries?.MeasurementsSeries != null)
                {
                    CurrentSeries.MeasurementsSeries.Clear();
                    CurrentSeries = null;
                }

                AccountMenuItem.Visibility = Visibility.Collapsed;
                LogOutMenuItem.Visibility = Visibility.Collapsed;
                LogInMenuItem.Visibility = Visibility.Visible;

                SeriesManagerIsLoading();
                ProgressBar.IsIndeterminate = true;

                Login();
            }
        }

        #endregion

        #region Menu

        private void MenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Options_OnClick(object sender, RoutedEventArgs e)
        {
            var options = new Options(_notifier, _logger, User);
            options.ShowDialog();

            if (options.DimensionlessPlotsIsChanged)
            {
                UpdatePlots(true);
            }
        }

        private void AppMainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (User != null)
            {
                var messageBoxResult =
                    MessageBox.Show("Закрыть приложение?", "Подтверждение выхода", MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    FileOperations.ClearDirectory("Temp");
                    _notifier.Dispose();
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }

        private void Information_Click(object sender, RoutedEventArgs e)
        {
            var information = new Information();
            information.ShowDialog();
        }

        #endregion

        #region Utility

        private void SeriesManagerIsLoading(bool blockSeriesTable = true)
        {
            SeriesManager.IsEnabled = false;
            if (blockSeriesTable)
                SeriesDataGrid.IsEnabled = false;
            MainMenuBar.IsEnabled = false;
            AddSeriesButton.IsEnabled = false;
            OneLineSetterValue.IsEnabled = false;
            ExportSeriesLocal.IsEnabled = false;
            ImportLocalSeries.IsEnabled = false;
            ExportSeriesButton.IsEnabled = false;
            DeleteSeriesButton.IsEnabled = false;
        }

        private void SeriesManagerLoadingComplete(bool blockSeriesTable = true)
        {
            SeriesManager.IsEnabled = true;
            if (blockSeriesTable)
                SeriesDataGrid.IsEnabled = true;
            MainMenuBar.IsEnabled = true;
            AddSeriesButton.IsEnabled = true;
            OneLineSetterValue.IsEnabled = true;
            ExportSeriesLocal.IsEnabled = true;
            ImportLocalSeries.IsEnabled = true;
            ExportSeriesButton.IsEnabled = true;
            DeleteSeriesButton.IsEnabled = true;
        }

        private async void SingleSeriesLoading(bool disablePhotos = true)
        {
            if (CurrentSeries != null)
                CurrentSeries.Loaded = false;
            if (disablePhotos)
                Photos.IsEnabled = false;
            PhotosTab.IsEnabled = false;
            SeriesManager.IsEnabled = false;
            ReferenceTab.IsEnabled = false;
            AddPhotoButton.IsEnabled = false;
            DeleteInputPhotosButton.IsEnabled = false;
            EditPhotosOrder.IsEnabled = false;
            EditIntervalBetweenPhotos.IsEnabled = false;
            ReCalculate.IsEnabled = false;
            StartAutoCalculate.IsEnabled = false;
            CancelReferencePhotoEditing.IsEnabled = false;
            SaveReferenceLine.IsEnabled = false;

            if (IntervalBetweenPhotos.IsEnabled)
            {
                await SaveIntervalBetweenPhotosAsync();
                IntervalBetweenPhotos.IsEnabled = false;
            }

            CreationTimeCheckBox.IsEnabled = false;
            MainMenuBar.IsEnabled = false;
            ChangeReferenceLine.Visibility = Visibility.Hidden; 
            DeleteButton.Visibility = Visibility.Hidden;
            ChooseReferenceButton.Visibility = Visibility.Hidden;
        }

        private async void SingleSeriesLoadingComplete(bool disablePhotos = true)
        {
            if (_overrideLoadingBehaviour)
            {
                Photos.IsEnabled = true;
                return;
            }

            if (CurrentSeries != null)
                CurrentSeries.Loaded = true;
            if (disablePhotos)
                Photos.IsEnabled = true;
            PhotosTab.IsEnabled = true;
            SeriesManager.IsEnabled = true;
            ReferenceTab.IsEnabled = true;
            AddPhotoButton.IsEnabled = true;
            DeleteInputPhotosButton.IsEnabled = true;
            EditPhotosOrder.IsEnabled = true;
            EditIntervalBetweenPhotos.IsEnabled = true;
            ReCalculate.IsEnabled = true;
            StartAutoCalculate.IsEnabled = true;
            CancelReferencePhotoEditing.IsEnabled = true;
            SaveReferenceLine.IsEnabled = true;

            if (IntervalBetweenPhotos.IsEnabled)
            {
                await SaveIntervalBetweenPhotosAsync();

                IntervalBetweenPhotos.IsEnabled = false;
            }

            CreationTimeCheckBox.IsEnabled = true;
            MainMenuBar.IsEnabled = true;
            ChangeReferenceLine.Visibility = Visibility.Visible;
            DeleteButton.Visibility = Visibility.Visible;
            ChooseReferenceButton.Visibility = Visibility.Visible;
        }

        private void SeriesWindowLoading(bool indeterminateLoadingBar = true)
        {
            if (indeterminateLoadingBar)
                ProgressBar.IsIndeterminate = !ProgressBar.IsIndeterminate;
            SeriesLoading.IsAdornerVisible = !SeriesLoading.IsAdornerVisible;
        }

        private async void MainWindow_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_photoEditModeOn)
            {
                if (Application.Current.MainWindow != null)
                {
                    if (PhotosPreviewRow.ActualHeight > PhotosGrid.ActualHeight * 0.6)
                    {
                        VisualHelper.SetEnableRowsMove(Photos, false);
                        await AnimationHelper.AnimateGridRowExpandCollapse(PhotosPreviewRow, false, PhotosPreviewRow.ActualHeight, PhotosGrid.ActualHeight * 0.6, 0, 0, 200);
                    }
                    else
                    {
                        await AnimationHelper.AnimateGridRowExpandCollapse(PhotosPreviewRow, true,
                            PhotosGrid.ActualHeight * 0.6,
                            PhotosGrid.ActualHeight * 0.5, 0, 0, 200);
                    }
                }
            }
            else
            {
                PhotosPreviewRow.Height = new GridLength(PhotosGrid.ActualHeight / 2);
            }
        }

        private void PhotosGrid_OnLoaded(object sender, RoutedEventArgs e)
        {
            PhotosPreviewRow.Height = new GridLength(PhotosGrid.ActualHeight / 2);
        }

        #endregion

        private async void PhotosDetails_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var photosSelectedItem = (DropPhotoView)PhotosDetails.SelectedItem;
            if (photosSelectedItem != null)
            {
                try
                {
                    CurrentThermalPhoto = null;

                    for (int i = ImgCurrent.CanDrawing.Children.Count; i-- > 1;)
                    {
                        ImgCurrent.CanDrawing.Children.RemoveAt(i);
                    }

                    ProgressBar.IsIndeterminate = true;
                    ImageForEdit = null;
                    _appStateBL.ShowAdorner(CurrentSeriesImageLoading);
                    SingleSeriesLoading(false);

                    if (e.RemovedItems.Count > 0)
                    {
                        var oldCurrentPhoto = e.RemovedItems[0] as DropPhotoView;

                        var photoToClear =  CurrentMeasurement.DropPhotos.FirstOrDefault(x => oldCurrentPhoto != null && x.PhotoId == oldCurrentPhoto.PhotoId);
                        photoToClear.Content = null;
                        photoToClear.Contour = null;
                        photoToClear.Lines = null;
                        photoToClear.SimpleLines = null;
                    }

                    _tokenSource?.Cancel();

                    _tokenSource = new CancellationTokenSource();

                    CurrentDropPhoto = CurrentMeasurement.DropPhotos[PhotosDetails.SelectedIndex];
                    CurrentPhotoType = CurrentMeasurement.DropPhotos[PhotosDetails.SelectedIndex].PhotoType;

                    if (CurrentMeasurement.DropPhotos[PhotosDetails.SelectedIndex].Contour?.Parameters != null)
                    {
                        _currentPhotoAutoCalculationTemplate = new AutoCalculationTemplate
                        {
                            Title = "Текущий контур",
                            TemplateType = CurrentDropPhoto.Contour
                                .CalculationVariants,
                            Parameters = CurrentDropPhoto.Contour.Parameters
                        };
                    }
                    else
                    {
                        _currentPhotoAutoCalculationTemplate = null;
                        CSharpTemplatesCombobox.SelectedIndex = 0;
                        PythonTemplatesCombobox.SelectedIndex = 0;
                        СurrentCSharpAutoCalculationTemplate =
                            _autoCalculationDefaultTemplates.FirstOrDefault(x =>
                                x.TemplateType == CalculationVariantsView.CalculateWithCSharp);
                        СurrentPythonAutoCalculationTemplate =
                            _autoCalculationDefaultTemplates.FirstOrDefault(x =>
                                x.TemplateType == CalculationVariantsView.CalculateWithPython);
                    }

                    BuildTemplates();

                    if (_loadPhotosContent && CurrentDropPhoto.PhotoId != Guid.Empty)
                    {
                        CurrentDropPhoto.Content = await Task.Run(() =>
                            _dropPhotoBl.GetDropPhotoContent(photosSelectedItem.PhotoId, _tokenSource.Token, Settings.Default.UseCache));

                        ImageForEdit = ImageInterpreter.LoadImage(CurrentDropPhoto.Content);

                        if (CurrentDropPhoto.ContourId.HasValue)
                        {
                            CurrentDropPhoto.Contour = _mapper.Map<Contour, ContourView>(await _dropPhotoBl.GetDropPhotoContour(CurrentDropPhoto.ContourId.Value));
                        }

                        CurrentDropPhoto.SimpleLines = _mapper.Map<List<SimpleLine>,ObservableCollection<SimpleLineView>>(await _dropPhotoBl.GetDropPhotoLines(CurrentDropPhoto.PhotoId));

                        if (CurrentDropPhoto.SimpleLines != null)
                        {
                            CurrentDropPhoto.Lines = _mapper.Map<ObservableCollection<SimpleLineView>, ObservableCollection<TypedLineView>>(CurrentDropPhoto.SimpleLines);
                        }
                    }

                    ShowLinesOnPhotosPreview(CurrentDropPhoto, ImgCurrent.CanDrawing);

                    SingleSeriesLoadingComplete(false);
                }
                catch (OperationCanceledException)
                {
                    if (CurrentMeasurement != null)
                    {
                        foreach (var line in CurrentDropPhoto.Lines)
                        {
                            ImgCurrent.CanDrawing.Children.Remove(line.Line);
                        }
                    }
                }
                catch (TimeoutException)
                {
                    _notifier.ShowError(
                        $"Не удалось загрузить снимок {photosSelectedItem.Name}. Не удалось установить подключение. Проверьте интернет соединение.");
                    SingleSeriesLoadingComplete(false);
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

                ProgressBar.IsIndeterminate = false;
                _appStateBL.HideAdorner(CurrentSeriesImageLoading);
            }
            else
            {
                ImageForEdit = null;
            }
        }

        private async void AddDropPhoto_OnClick(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Multiselect = false,
                AddExtension = true,
                CheckFileExists = true,
                CheckPathExists = true
            };

            ImageInterpreter.GetImageFilter(openFileDialog);

            if (openFileDialog.ShowDialog() == true)
            {
                SingleSeriesLoading();
                _appStateBL.ShowAdorner(CurrentSeriesPhotoContentLoading);

                var imageForAdding = new BasePhotoView()
                {
                    PhotoId = CurrentPhotoType == PhotoTypeView.ThermalPhoto ? CurrentMeasurement.MeasurementId : Guid.NewGuid(),
                    Name = openFileDialog.SafeFileNames[0],
                    Content = File.ReadAllBytes(openFileDialog.FileNames[0]),
                    AddedDate = DateTime.Now,
                    CreationDateTime = File.GetCreationTime(openFileDialog.FileNames[0]),
                    PhotoType = CurrentPhotoType
                };

                if (ImageValidator.ValidateImage(imageForAdding.Content))
                {
                    try
                    {
                        var owningMeasurement = CurrentMeasurement;

                        switch (CurrentPhotoType)
                        {
                            case PhotoTypeView.FrontDropPhoto:
                            case PhotoTypeView.SideDropPhoto:
                            {
                                var dropPhoto = _mapper.Map<BasePhotoView, DropPhotoView>(imageForAdding);

                                if (CurrentDropPhoto.PhotoId != Guid.Empty)
                                {
                                    CurrentDropPhoto.Contour = null;
                                    CurrentDropPhoto.Lines = null;
                                    CurrentDropPhoto.SimpleLines = null;
                                    CurrentDropPhoto.Content = imageForAdding.Content;
                                    CurrentDropPhoto.Name = imageForAdding.Name;
                                    CurrentDropPhoto.AddedDate = imageForAdding.AddedDate;
                                    CurrentDropPhoto.CreationDateTime = imageForAdding.CreationDateTime;
                                    CurrentDropPhoto.XDiameterInPixels = 0;
                                    CurrentDropPhoto.YDiameterInPixels = 0;
                                    CurrentDropPhoto.ZDiameterInPixels = 0;

                                    var photoForUpdate = CurrentDropPhoto;

                                    for (int i = ImgCurrent.CanDrawing.Children.Count; i-- > 1;)
                                    {
                                        ImgCurrent.CanDrawing.Children.RemoveAt(i);
                                    }

                                    await _dropPhotoBl.UpdateDropPhoto(_mapper.Map<DropPhotoView, DropPhoto>(photoForUpdate), true);

                                    if (_loadPhotosContent && CurrentDropPhoto.PhotoId != Guid.Empty)
                                    {
                                        ImageForEdit = ImageInterpreter.LoadImage(CurrentDropPhoto.Content);
                                    }
                                }
                                else
                                {
                                    await Task.Run(() => _dropPhotoBl.CreateDropPhoto(_mapper.Map<DropPhotoView, DropPhoto>(dropPhoto), _mapper.Map<MeasurementView, Measurement>(owningMeasurement)));

                                    imageForAdding.Content = null;

                                    switch (CurrentDropPhoto.PhotoType)
                                    {
                                        case PhotoTypeView.FrontDropPhoto:
                                            if (CurrentMeasurement != null)
                                            {
                                                if (dropPhoto != null)
                                                {
                                                    CurrentMeasurement.DropPhotos[0] = dropPhoto;
                                                }
                                            }
                                            break;
                                        case PhotoTypeView.SideDropPhoto:
                                            if (CurrentMeasurement != null)
                                            {
                                                if (dropPhoto != null)
                                                {
                                                    CurrentMeasurement.DropPhotos[1] = dropPhoto;
                                                }
                                            }
                                            break;
                                        default:
                                            throw new ArgumentOutOfRangeException();
                                    }
                                }

                                break;
                            }
                            case PhotoTypeView.ThermalPhoto:
                            {
                                var thermalPhoto = _mapper.Map<BasePhotoView, ThermalPhotoView>(imageForAdding);

                                if (CurrentThermalPhoto.PhotoId != Guid.Empty)
                                {
                                    CurrentThermalPhoto.Content = imageForAdding.Content;
                                    CurrentThermalPhoto.Name = imageForAdding.Name;
                                    CurrentThermalPhoto.AddedDate = imageForAdding.AddedDate;
                                    CurrentThermalPhoto.CreationDateTime = imageForAdding.CreationDateTime;

                                    var tempFilePath = ImageInterpreter.ByteArrayToFile(CurrentThermalPhoto.Content, CurrentThermalPhoto.Name, "Temp");
                                    CurrentThermalPhoto.FlirImage = _mapper.Map<FlirImage, FlirImageView>(_thermalBl.ProcessImage(tempFilePath));

                                    var photoForUpdate = CurrentThermalPhoto;

                                    for (int i = ImgCurrent.CanDrawing.Children.Count; i-- > 1;)
                                    {
                                        ImgCurrent.CanDrawing.Children.RemoveAt(i);
                                    }

                                    await _thermalPhotoBl.UpdateThermalPhoto(_mapper.Map<ThermalPhotoView, ThermalPhoto>(photoForUpdate), true);

                                    if (_loadPhotosContent && CurrentDropPhoto.PhotoId != Guid.Empty)
                                    {
                                        ImageForEdit = CurrentThermalPhoto.FlirImage.Image.ToBitmapImage();
                                    }
                                }
                                else
                                {
                                    if (owningMeasurement != null)
                                    {
                                        if (thermalPhoto != null)
                                        {
                                            owningMeasurement.ThermalPhoto = thermalPhoto;
                                        }
                                    }

                                    await Task.Run(() => _thermalPhotoBl.CreateThermalPhoto(_mapper.Map<ThermalPhotoView, ThermalPhoto>(thermalPhoto)));

                                    imageForAdding.Content = null;

                                    if (thermalPhoto != null)
                                    {
                                        CurrentMeasurement.ThermalPhoto = thermalPhoto;
                                    }

                                    CurrentThermalPhotos[0] = thermalPhoto;
                                }

                                break;
                            }
                                
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        _logger.LogInfo(new LogEntry
                        {
                            Username = User.Email,
                            LogCategory = LogCategory.DropPhoto,
                            Message = $"Снимок {imageForAdding.Name} добавлен."
                        });
                        _notifier.ShowSuccess($"Снимок {imageForAdding.Name} добавлен.");
                    }
                    catch (TimeoutException)
                    {
                        _notifier.ShowError(
                            $"Снимок {imageForAdding.Name} не добавлен. Не удалось установить подключение. Проверьте интернет соединение.");
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
                    _notifier.ShowError($"Файл {imageForAdding.Name} имеет неизвестный формат.");

                SingleSeriesLoadingComplete();
                _appStateBL.HideAdorner(CurrentSeriesPhotoContentLoading);
                _notifier.ShowSuccess("Новые снимок успешно добавлен.");
            }
        }

        private async void PhotosDetails_OnCellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            SingleSeriesLoading();

            var photoNameCell = e.EditingElement as TextBox;
            try
            {
                if (photoNameCell != null)
                {
                    if (!string.IsNullOrWhiteSpace(photoNameCell.Text))
                    {
                        ProgressBar.IsIndeterminate = true;

                        var text = photoNameCell.Text;
                        var editedPhotoId = CurrentDropPhoto.PhotoId;

                        await Task.Run(() => _dropPhotoBl.UpdateDropPhotoName(text, editedPhotoId));

                        _logger.LogInfo(new LogEntry
                        {
                            Username = User.Email,
                            LogCategory = LogCategory.DropPhoto,
                            Message = "Название снимка изменено успешно."
                        });
                        _notifier.ShowSuccess("Название снимка изменено успешно.");
                    }
                    else
                    {
                        _notifier.ShowInformation("Название снимка не может быть пустым.");
                        photoNameCell.Text = CurrentDropPhoto.Name;
                    }
                }
            }
            catch (TimeoutException)
            {
                _notifier.ShowError(
                    "Не удалось изменить название снимка. Не удалось установить подключение. Проверьте интернет соединение.");
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

            SingleSeriesLoadingComplete();
            ProgressBar.IsIndeterminate = false;
        }

        private async void ThermalDetails_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var photosSelectedItem = (ThermalPhotoView)ThermalDetails.SelectedItem;
            if (photosSelectedItem != null)
            {
                FileOperations.ClearDirectory("Temp");

                try
                {
                    CurrentDropPhoto = null;

                    for (int i = ImgCurrent.CanDrawing.Children.Count; i-- > 1;)
                    {
                        ImgCurrent.CanDrawing.Children.RemoveAt(i);
                    }

                    ProgressBar.IsIndeterminate = true;
                    ImageForEdit = null;
                    _appStateBL.ShowAdorner(CurrentSeriesImageLoading);
                    SingleSeriesLoading(false);

                    if (e.RemovedItems.Count > 0)
                    {
                        var oldCurrentPhoto = e.RemovedItems[0] as ThermalPhotoView;
                        CurrentThermalPhotos.FirstOrDefault(x =>
                            oldCurrentPhoto != null && x.PhotoId == oldCurrentPhoto.PhotoId).Content = null;
                    }

                    _tokenSource?.Cancel();

                    _tokenSource = new CancellationTokenSource();

                    CurrentThermalPhoto = CurrentThermalPhotos[ThermalDetails.SelectedIndex];
                    CurrentPhotoType = CurrentThermalPhotos[ThermalDetails.SelectedIndex].PhotoType;

                    if (_loadPhotosContent && CurrentThermalPhoto.PhotoId != Guid.Empty)
                    {
                        CurrentThermalPhoto.Content = await Task.Run(() =>
                            _thermalPhotoBl.GetThermalPhotoContent(photosSelectedItem.PhotoId, _tokenSource.Token, Settings.Default.UseCache));

                        var tempFilePath =
                            ImageInterpreter.ByteArrayToFile(CurrentThermalPhoto.Content, CurrentThermalPhoto.Name, "Temp");
                        CurrentThermalPhoto.FlirImage = _mapper.Map<FlirImage, FlirImageView>(_thermalBl.ProcessImage(tempFilePath));
                        ImageForEdit = CurrentThermalPhoto.FlirImage.Image.ToBitmapImage();
                    }

                    if (CurrentMeasurement.Drop.Temperature != null)
                    {
                        ImgCurrent.CanDrawing.Children.Add(CurrentThermalPhoto.Ellipse);

                        Canvas.SetLeft(CurrentThermalPhoto.Ellipse, CurrentThermalPhoto.EllipseCoordinate.X);
                        Canvas.SetTop(CurrentThermalPhoto.Ellipse, CurrentThermalPhoto.EllipseCoordinate.Y);
                    }

                    ShowContourOnPhotosPreview(CurrentThermalPhoto.Contour, ImgCurrent.CanDrawing);

                    SingleSeriesLoadingComplete(false);
                }
                catch (TimeoutException)
                {
                    _notifier.ShowError(
                        $"Не удалось загрузить снимок {photosSelectedItem.Name}. Не удалось установить подключение. Проверьте интернет соединение.");
                    SingleSeriesLoadingComplete(false);
                }
                catch (ThermalException)
                {
                    _notifier.ShowError($"Не удалось извлечь термическую информацию снимка {photosSelectedItem.Name}.");
                    ImageForEdit = ImageInterpreter.LoadImage(CurrentThermalPhoto.Content);
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

                ProgressBar.IsIndeterminate = false;
                _appStateBL.HideAdorner(CurrentSeriesImageLoading);
            }
            else
            {
                ImageForEdit = null;
            }
        }

        private async void DeleteThermalPhoto_OnClick(object sender, RoutedEventArgs e)
        {
            var messageBoxResult =
                MessageBox.Show($"Удалить снимок {CurrentSeries.MeasurementsSeries[ThermalDetails.SelectedIndex].Name}?",
                    "Подтверждение удаления", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                ProgressBar.IsIndeterminate = true;
                SingleSeriesLoading();
                _appStateBL.ShowAdorner(CurrentSeriesPhotoContentLoading);
                try
                {
                    await _thermalPhotoBl.DeleteThermalPhoto(_mapper.Map<ThermalPhotoView, ThermalPhoto>(CurrentThermalPhotos[ThermalDetails.SelectedIndex]));

                    _logger.LogInfo(new LogEntry
                    {
                        Username = User.Email,
                        LogCategory = LogCategory.ThermalPhoto,
                        Message = $"Снимок {CurrentThermalPhotos[ThermalDetails.SelectedIndex].Name} удален."
                    });

                    _notifier.ShowSuccess(
                        $"Снимок {CurrentThermalPhotos[ThermalDetails.SelectedIndex].Name} удален.");

                    CurrentSeries.MeasurementsSeries[ThermalDetails.SelectedIndex].ThermalPhoto = null;

                    CurrentThermalPhotos[ThermalDetails.SelectedIndex] = new ThermalPhotoView()
                    {
                        PhotoType = PhotoTypeView.ThermalPhoto
                    };
                }
                catch (TimeoutException)
                {
                    _notifier.ShowError(
                        $"Не удалось удалить снимок {CurrentThermalPhotos[ThermalDetails.SelectedIndex].Name}. Не удалось установить подключение. Проверьте интернет соединение.");
                }
                catch (Exception exception)
                {
                    _logger.LogError(new LogEntry
                    {
                        Exception = exception.ToString(),
                        LogCategory = LogCategory.ThermalPhoto,
                        InnerException = exception.InnerException?.Message,
                        Message = exception.Message,
                        StackTrace = exception.StackTrace,
                        Username = User.Email,
                        Details = exception.TargetSite.Name
                    });
                    throw;
                }

                ProgressBar.IsIndeterminate = false;
                _appStateBL.HideAdorner(CurrentSeriesPhotoContentLoading);
                SingleSeriesLoadingComplete();
            }
        }

        private async void ThermalDetails_OnCellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            SingleSeriesLoading();

            var photoNameCell = e.EditingElement as TextBox;
            try
            {
                if (photoNameCell != null)
                {
                    if (!string.IsNullOrWhiteSpace(photoNameCell.Text))
                    {
                        ProgressBar.IsIndeterminate = true;

                        var text = photoNameCell.Text;
                        var editedPhotoId = CurrentThermalPhoto.PhotoId;

                        await _thermalPhotoBl.UpdateThermalPhotoName(text, editedPhotoId);

                        _logger.LogInfo(new LogEntry
                        {
                            Username = User.Email,
                            LogCategory = LogCategory.DropPhoto,
                            Message = "Название снимка изменено успешно."
                        });
                        _notifier.ShowSuccess("Название снимка изменено успешно.");
                    }
                    else
                    {
                        _notifier.ShowInformation("Название снимка не может быть пустым.");
                        photoNameCell.Text = CurrentThermalPhoto.Name;
                    }
                }
            }
            catch (TimeoutException)
            {
                _notifier.ShowError(
                    "Не удалось изменить название снимка. Не удалось установить подключение. Проверьте интернет соединение.");
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

            SingleSeriesLoadingComplete();
            ProgressBar.IsIndeterminate = false;
        }

        private async void EditThermalPhotoButton_OnClick(object sender, RoutedEventArgs e)
        {
            await PhotoEditModeOn(PhotoTypeView.ThermalPhoto);

            _copiedThermalPhoto = new ThermalPhotoView
            {
                PhotoId = CurrentThermalPhoto.PhotoId,
                Ellipse = CurrentThermalPhoto.Ellipse,
                EllipseCoordinate = CurrentThermalPhoto.EllipseCoordinate
            };

            _initialTemperature = CurrentMeasurement.Drop.Temperature ?? 0;
        }

        private async void SaveThermalPhotoEditButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (IsThermalPhotoSaveRequired()) await SaveTemperature();

            await PhotoEditModeOff(PhotoTypeView.ThermalPhoto);
        }

        private async void DiscardThermalPhotoEdit_OnClick(object sender, RoutedEventArgs e)
        {
            if (IsThermalPhotoSaveRequired())
            {
                if (MessageBox.Show("Сохранить изменения?", "Предупреждение", MessageBoxButton.YesNo,
                        MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    await SaveTemperature();
                }
                else
                {
                    ImgCurrent.CanDrawing.Children.Remove(CurrentThermalPhoto.Ellipse);

                    CurrentThermalPhoto.Ellipse = _copiedThermalPhoto.Ellipse;
                    CurrentThermalPhoto.EllipseCoordinate = _copiedThermalPhoto.EllipseCoordinate;

                    if (CurrentThermalPhoto.Ellipse != null)
                    {
                        ImgCurrent.CanDrawing.Children.Add(CurrentThermalPhoto.Ellipse);
                    }
                }
            }

            await PhotoEditModeOff(PhotoTypeView.ThermalPhoto);
        }

        private bool IsThermalPhotoSaveRequired()
        {
            if (double.TryParse(TemperatureTextBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture ,out var temperatureTextBox))
                if (Math.Abs(_initialTemperature - temperatureTextBox) > 0.0001)
                    return SaveThermalPhotoRequired = true;

            return SaveThermalPhotoRequired;
        }

        private async Task SaveTemperature()
        {
            SaveThermalPhotoEditButton.IsEnabled = false;
            DiscardThermalPhotoEdit.IsEnabled = false;

            var temperatureTextBox = double.Parse(TemperatureTextBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture);
            
            if (Math.Abs(_initialTemperature - temperatureTextBox) > 0.0001)
            {
                CurrentMeasurement.Drop.Temperature = temperatureTextBox;
                _initialTemperature = temperatureTextBox;
            }

            SaveThermalPhotoRequired = false;

            await _thermalPhotoBl.UpdateThermalPhotoEllipseCoordinate(JsonSerializeProvider.SerializeToString(CurrentThermalPhoto.EllipseCoordinate), CurrentThermalPhoto.PhotoId);
            await _dropBl.UpdateDrop(_mapper.Map<DropView, Drop>(CurrentMeasurement.Drop));
        }

        private void TextBoxAutoComplete_OnSaveClick(object sender, RoutedEventArgs e)
        {
            if (User.UserSeries[SeriesDataGrid.SelectedIndex].Sub != null)
            {
                _substanceBl.SaveSubstance(_mapper.Map<SubstanceModelView, SubstanceModel>(User.UserSeries[SeriesDataGrid.SelectedIndex].Substance));
                User.UserSeries[SeriesDataGrid.SelectedIndex].IsSubstanceEdited = false;
                _notifier.ShowSuccess("Вещество сохранено");
            }
            else
            {
                _notifier.ShowInformation("Начните вводить название вещества и выберите его из выпадающего списка");
            }
        }

        private void TextBoxAutoComplete_OnDeleteClick(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Удалить вещество?", "Предупреждение", MessageBoxButton.YesNo,
                MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                _substanceBl.DeleteSubstance(_mapper.Map<SubstanceModelView, SubstanceModel>(User.UserSeries[SeriesDataGrid.SelectedIndex].Substance));
                User.UserSeries[SeriesDataGrid.SelectedIndex].Substance = null;
                User.UserSeries[SeriesDataGrid.SelectedIndex].Sub = null;
                _notifier.ShowSuccess("Вещество удалено");
            }
        }

        private async void PhotosCommentToolTip_OnClick(object sender, RoutedEventArgs e)
        {
            if (CurrentSeries.Comment == null)
            {
                CurrentSeries.CommentId = Guid.NewGuid();
                CurrentSeries.Comment = new CommentView()
                {
                    CommentId = CurrentSeries.CommentId.Value,
                    Type = CommentableEntityTypeView.Series
                };
            }

            await UpdateComment(CurrentSeries.Comment, CurrentSeries.SeriesId);
        }

        private async void MeasurementCommentToolTip_OnClick(object sender, RoutedEventArgs e)
        {
            if (CurrentMeasurement.Comment == null)
            {
                CurrentMeasurement.CommentId = Guid.NewGuid();
                CurrentMeasurement.Comment = new CommentView()
                {
                    CommentId = CurrentMeasurement.CommentId.Value,
                    Type = CommentableEntityTypeView.Measurement
                };
            }

            await UpdateComment(CurrentMeasurement.Comment, CurrentMeasurement.MeasurementId);
        }

        private async void DropPhotoCommentToolTip_OnClick(object sender, RoutedEventArgs e)
        {
            if (CurrentDropPhoto.Comment == null)
            {
                CurrentDropPhoto.CommentId = Guid.NewGuid();
                CurrentDropPhoto.Comment = new CommentView()
                {
                    CommentId = CurrentDropPhoto.CommentId.Value,
                    Type = CommentableEntityTypeView.DropPhoto
                };
            }

            await UpdateComment(CurrentDropPhoto.Comment, CurrentDropPhoto.PhotoId);
        }

        private async void ThermalPhotoCommentToolTip_OnClick(object sender, RoutedEventArgs e)
        {
            if (CurrentThermalPhoto.Comment == null)
            {
                CurrentThermalPhoto.CommentId = Guid.NewGuid();
                CurrentThermalPhoto.Comment = new CommentView()
                {
                    CommentId = CurrentThermalPhoto.CommentId.Value,
                    Type = CommentableEntityTypeView.ThermalPhoto
                };
            }

            await UpdateComment(CurrentThermalPhoto.Comment, CurrentThermalPhoto.PhotoId);
        }

        private async Task UpdateComment(CommentView comment, Guid entityId)
        {
            var commentWindow = new CommentEdit
            {
                Owner = AppMainWindow,
            };

            commentWindow.ViewModel.CommentText = comment.Content;
            commentWindow.ShowDialog();

            if (commentWindow.ViewModel.RequireSaving)
            {
                comment.Content = commentWindow.ViewModel.CommentText;
                await _commentsBl.UpdateComment(_mapper.Map<CommentView, Comment>(comment), entityId);
                _notifier.ShowSuccess("Комментарий добавлен");
            }
        }

        private void PixelDrawer_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (DrawnShapes == null || (!ImgCurrent.DrawningIsEnabled && !MainWindowPixelDrawer.DrawningIsEnabled)) return;

            if (DrawnShapes.Rectangle == null && DrawnShapes.Line == null && DrawnShapes.Ellipse == null) return;

            if (e.ChangedButton != MouseButton.Right) return;

            switch (DrawningMode)
            {
                case PixelDrawerMode.Rectangle:
                {
                    if (CurrentSeries.RegionOfInterest == null)
                    {
                        CurrentSeries.RegionOfInterest = new ObservableCollection<TypedRectangleView>();
                    }

                    if (DrawnShapes.Rectangle != null) DrawnShapes.Rectangle.Stroke = System.Windows.Media.Brushes.Red;

                    var currentRectangle = CurrentSeries.RegionOfInterest.FirstOrDefault(x => x.PhotoType == CurrentPhotoType);

                    if (currentRectangle?.RegionOfInterest != null)
                    {
                        ImgCurrent.CanDrawing.Children.Remove(currentRectangle.RegionOfInterest);
                    }

                    var simpleRectangle = new System.Drawing.Rectangle(
                        (int) Canvas.GetLeft(DrawnShapes.Rectangle),
                        (int) Canvas.GetTop(DrawnShapes.Rectangle),
                        (int) DrawnShapes.Rectangle.ActualWidth,
                        (int) DrawnShapes.Rectangle.ActualHeight);

                    if (currentRectangle != null)
                    {
                        currentRectangle.RegionOfInterest = DrawnShapes.Rectangle;
                        currentRectangle.Rectangle = simpleRectangle;

                        CurrentSeries.RegionOfInterest = new ObservableCollection<TypedRectangleView>(User.UserSeries[SeriesDataGrid.SelectedIndex].RegionOfInterest);
                    }
                    else
                    {
                        CurrentSeries.RegionOfInterest.Add(new TypedRectangleView()
                        {
                            PhotoType = CurrentPhotoType,
                            RegionOfInterest = DrawnShapes.Rectangle,
                            Rectangle = simpleRectangle
                        });

                        CurrentSeries.RegionOfInterest = new ObservableCollection<TypedRectangleView>(User.UserSeries[SeriesDataGrid.SelectedIndex].RegionOfInterest);
                    }

                    _seriesBL.UpdateSeriesRegionOfInterest(
                        JsonSerializeProvider.SerializeToString(_mapper.Map<ObservableCollection<TypedRectangleView>, List<TypedRectangle>>(CurrentSeries.RegionOfInterest)),
                        CurrentSeries.SeriesId);

                    DrawnShapes.Rectangle = null;
                    break;
                }
                case PixelDrawerMode.Ellipse:
                {
                    if (DrawnShapes.Ellipse == null)
                    {
                        ImgCurrent.CanDrawing.Children.Remove(DrawnShapes.Ellipse);
                    }

                    if (DrawnShapes.Ellipse != null) DrawnShapes.Ellipse.Stroke = System.Windows.Media.Brushes.DeepPink;

                    ImgCurrent.CanDrawing.Children.Remove(CurrentThermalPhoto.Ellipse);

                    CurrentThermalPhoto.Ellipse = DrawnShapes.Ellipse;
                    CurrentThermalPhoto.EllipseCoordinate = new Point((int)e.GetPosition(ImgCurrent.CanDrawing).X, (int)e.GetPosition(ImgCurrent.CanDrawing).Y);

                    CurrentMeasurement.Drop.Temperature = CurrentThermalPhoto.FlirImage.ThermalData
                        .FirstOrDefault(td =>
                            td.X == CurrentThermalPhoto.EllipseCoordinate.X && td.Y == CurrentThermalPhoto.EllipseCoordinate.Y)
                        ?.TemperatureValue;
                    break;
                }
                case PixelDrawerMode.Line:
                {
                    CreateLine();
                    break;
                }
            }
        }

        private void CreateLine()
        {
            if (DrawnShapes.Line.X1 == DrawnShapes.Line.X2 && DrawnShapes.Line.Y1 == DrawnShapes.Line.Y2 || DrawnShapes.Line == null)
            {
                ImgCurrent.CanDrawing.Children.Remove(DrawnShapes.Line);
            }
            else
            {
                var point11 = new Point(Convert.ToInt32(DrawnShapes.Line.X1), Convert.ToInt32(DrawnShapes.Line.Y1));
                var point22 = new Point(Convert.ToInt32(DrawnShapes.Line.X2), Convert.ToInt32(DrawnShapes.Line.Y2));

                if (_twoLineMode)
                {
                    var lineType = GetLineMode();

                    CreateLines(point11, point22, lineType);
                }
                else
                {
                    DrawnShapes.Line.Stroke = System.Windows.Media.Brushes.DeepPink;

                    MainWindowPixelDrawer.CanDrawing.Children.Remove(CurrentReferencePhoto.Line);

                    CurrentReferencePhoto.Line = DrawnShapes.Line;
                    var simpleReferenceLineForAdd = new SimpleLineView()
                    {
                        X1 = DrawnShapes.Line.X1,
                        X2 = DrawnShapes.Line.X2,
                        Y1 = DrawnShapes.Line.Y1,
                        Y2 = DrawnShapes.Line.Y2
                    };

                    CurrentReferencePhoto.SimpleLine = simpleReferenceLineForAdd;

                    CurrentReferencePhoto.PixelsInMillimeter =
                        LineLengthHelper.GetPointsOnLine(point11, point22).Count;
                }
            }
        }

        private void CreateLines(Point point11, Point point22, LineTypeView lineType)
        {
            DrawnShapes.Line.Stroke = lineType == LineTypeView.Horizontal ? System.Windows.Media.Brushes.DeepPink : System.Windows.Media.Brushes.Green;

            var lineToRemove = CurrentDropPhoto.Lines.FirstOrDefault(x => x.LineType == lineType);

            if (lineToRemove != null)
            {
                ImgCurrent.CanDrawing.Children.Remove(lineToRemove.Line);
                CurrentDropPhoto.Lines.First(x => x.LineType == lineType).Line = DrawnShapes.Line;
            }
            else
            {
                CurrentDropPhoto.Lines.Add(new TypedLineView
                {
                    LineType = lineType,
                    Line = DrawnShapes.Line
                });
            }

            if (CurrentDropPhoto.Contour != null)
            {
                foreach (var line in CurrentDropPhoto.Contour.Lines) ImgCurrent.CanDrawing.Children.Remove(line);

                CurrentDropPhoto.Contour.SimpleLines.Clear();
                CurrentDropPhoto.Contour.Lines.Clear();

                CurrentDropPhoto.Contour = null;
            }

            var lineForAdd = new SimpleLineView()
            {
                X1 = DrawnShapes.Line.X1,
                X2 = DrawnShapes.Line.X2,
                Y1 = DrawnShapes.Line.Y1,
                Y2 = DrawnShapes.Line.Y2,
                LineType = lineType
            };

            var simpleLine = CurrentDropPhoto.SimpleLines.FirstOrDefault(l => l.LineType == lineType);

            if (simpleLine != null)
            {
                simpleLine = lineForAdd;
            }
            else
            {
                CurrentDropPhoto.SimpleLines.Add(lineForAdd);
            }

            if (CurrentDropPhoto.PhotoType == PhotoTypeView.FrontDropPhoto)
            {
                switch (lineType)
                {
                    case LineTypeView.Horizontal:
                        CurrentDropPhoto.XDiameterInPixels = LineLengthHelper.GetPointsOnLine(point11, point22).Count;
                        break;
                    case LineTypeView.Vertical:
                        CurrentDropPhoto.YDiameterInPixels = LineLengthHelper.GetPointsOnLine(point11, point22).Count;
                        break; 
                }
            }

            if (CurrentDropPhoto.PhotoType == PhotoTypeView.SideDropPhoto)
            {
                switch (lineType)
                {
                    case LineTypeView.Horizontal:
                        CurrentDropPhoto.ZDiameterInPixels = LineLengthHelper.GetPointsOnLine(point11, point22).Count;
                        break;
                    case LineTypeView.Vertical:
                        CurrentDropPhoto.YDiameterInPixels = LineLengthHelper.GetPointsOnLine(point11, point22).Count;
                        break;
                }
            }                
        }

        private LineTypeView GetLineMode()
        {
            if (Math.Abs(DrawnShapes.Line.X1 - DrawnShapes.Line.X2) >= Math.Abs(DrawnShapes.Line.Y1 - DrawnShapes.Line.Y2) && !DrawingVerticalLine || DrawingHorizontalLine)
            {
                return LineTypeView.Horizontal;
            }
            else if (Math.Abs(DrawnShapes.Line.X1 - DrawnShapes.Line.X2) < Math.Abs(DrawnShapes.Line.Y1 - DrawnShapes.Line.Y2) && !DrawingHorizontalLine || DrawingVerticalLine)
            {
                return LineTypeView.Vertical;
            }

            return LineTypeView.Undefined;
        }

        private async void EditPlots_OnClick(object sender, RoutedEventArgs e)
        {
            EditPlots.Visibility = Visibility.Hidden;
            StopEditPlots.Visibility = Visibility.Visible;
            await AnimationHelper.AnimateGridColumnExpandCollapseAsync(CustomPlotsColumn, true, 300, 0,
                CustomPlotsColumn.MinWidth, 0, 200);
        }

        private void DeleteSinglePlotButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Удалить график?", "Предупреждение", MessageBoxButton.YesNo,
                MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                var plotForForRemove = AvailableRadiusPlots[CustomPlots.SelectedIndex];

                _customPlotsBl.DeletePlot(_mapper.Map<PlotView, Plot>(plotForForRemove));
                AvailableRadiusPlots.Remove(plotForForRemove);
                _notifier.ShowSuccess("График удален");
            }
        }

        private void DeleteSingleTemperaturePlotButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Удалить график?", "Предупреждение", MessageBoxButton.YesNo,
                MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                var plotForForRemove = AvailableTemperaturePlots[TemperaturePlots.SelectedIndex];

                _customPlotsBl.DeletePlot(_mapper.Map<PlotView, Plot>(plotForForRemove));
                AvailableTemperaturePlots.Remove(plotForForRemove);
                _notifier.ShowSuccess("График удален");
            }
        }

        private async void EditPlotButton_OnClick(object sender, RoutedEventArgs e)
        {
            await EditPlotModeOn();
        }

        private PlotView _plotBackUp;
        private async Task EditPlotModeOn()
        {
            StopEditPlots.Visibility = Visibility.Hidden;

            DiscardPlotEdit.Visibility = Visibility.Visible;
            SavePlot.Visibility = Visibility.Visible;
            ImportPlot.Visibility = Visibility.Visible;

            XDimensionlessDividerLabel.Visibility = Visibility.Visible;
            XDimensionlessDividerTextBox.Visibility = Visibility.Visible;
            YDimensionlessDividerLabel.Visibility = Visibility.Visible;
            YDimensionlessDividerTextBox.Visibility = Visibility.Visible;

            switch (_currentPlotType)
            {
                case PlotTypeView.Radius:
                {
                    int index;
                    if (CustomPlots.SelectedIndex == -1)
                    {
                        index = AvailableTemperaturePlots.Count - 1;
                    }
                    else
                    {
                        index = CustomPlots.SelectedIndex;
                    }
                    CurrentPlot = AvailableRadiusPlots[index];
                    break;
                }
                case PlotTypeView.Temperature:
                {
                    int index;
                    if (TemperaturePlots.SelectedIndex == -1)
                    {
                        index = AvailableTemperaturePlots.Count - 1;
                    }
                    else
                    {
                        index = TemperaturePlots.SelectedIndex;
                    }
                    CurrentPlot = AvailableTemperaturePlots[index];
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (CurrentPlot.Points != null)
            {
                _plotBackUp = new PlotView()
                {
                    Points = new ObservableCollection<SimplePointView>()
                };
                foreach (var point in CurrentPlot.Points)
                {
                    _plotBackUp.Points.Add(new SimplePointView()
                    {
                        X = point.X,
                        Y = point.Y
                    });
                }
            }

            await AnimationHelper.AnimateGridColumnExpandCollapseAsync(CustomPlotsColumn, false, 300, 0,
                CustomPlotsColumn.MinWidth, 0, 200);
            await AnimationHelper.AnimateGridRowExpandCollapse(CustomPlotsRow, true, Plot.ActualHeight, 0,
                CustomPlotsRow.MinHeight, 0, 200);
        }

        private async Task EditPlotModeOff()
        {
            StopEditPlots.Visibility = Visibility.Visible;

            DiscardPlotEdit.Visibility = Visibility.Hidden;
            SavePlot.Visibility = Visibility.Hidden;
            ImportPlot.Visibility = Visibility.Hidden;
            XDimensionlessDividerLabel.Visibility = Visibility.Hidden;
            XDimensionlessDividerTextBox.Visibility = Visibility.Hidden;
            YDimensionlessDividerLabel.Visibility = Visibility.Hidden;
            YDimensionlessDividerTextBox.Visibility = Visibility.Hidden;

            await AnimationHelper.AnimateGridColumnExpandCollapseAsync(CustomPlotsColumn, true, 300, 0,
                CustomPlotsColumn.MinWidth, 0, 200);
            await AnimationHelper.AnimateGridRowExpandCollapse(CustomPlotsRow, false, Plot.ActualHeight, 0,
                CustomPlotsRow.MinHeight, 0, 200);
        }

        private async void StopEditPlots_OnClick(object sender, RoutedEventArgs e)
        {
            EditPlots.Visibility = Visibility.Visible;
            StopEditPlots.Visibility = Visibility.Hidden;
            await AnimationHelper.AnimateGridColumnExpandCollapseAsync(CustomPlotsColumn, false, 300, 0,
                CustomPlotsColumn.MinWidth, 0, 200);
        }

        private async void AddPlots_OnClick(object sender, RoutedEventArgs e)
        {
            InputDialog inputDialog = new InputDialog("Введите название");
            inputDialog.ShowDialog();

            if (!inputDialog.Canceled || !string.IsNullOrEmpty(inputDialog.Answer))
            {
                _appStateBL.ShowAdorner(PlotToolboxLoading);
                ProgressBar.IsIndeterminate = true;

                PlotView plotForAdd = new PlotView()
                {
                    CurrentUserId = User.UserId,
                    PlotId = Guid.NewGuid(),
                    Name = inputDialog.Answer,
                    PlotType = _currentPlotType,
                    Points = new ObservableCollection<SimplePointView>()
                };

                await SaveAddedPlot(plotForAdd);

                _notifier.ShowSuccess("График добавлен");

                _appStateBL.HideAdorner(PlotToolboxLoading);
                ProgressBar.IsIndeterminate = false;

                await EditPlotModeOn();
            }
        }

        private async Task SaveAddedPlot(PlotView plotForAdd)
        {
            await _customPlotsBl.CreatePlot(_mapper.Map<PlotView, Plot>(plotForAdd));

            User.Plots.Add(plotForAdd);

            switch (_currentPlotType)
            {
                case PlotTypeView.Radius:
                    plotForAdd.PropertyChanged += PlotsOnPropertyChanged;
                    AvailableRadiusPlots.Add(plotForAdd);
                    break;
                case PlotTypeView.Temperature:
                    plotForAdd.PropertyChanged += TemperaturePlotsOnPropertyChanged;
                    AvailableTemperaturePlots.Add(plotForAdd);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _notifier.ShowSuccess("График добавлен");
        }

        private async void CustomPlots_OnCellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            await EditPlotName(e);
        }

        private void CustomPlots_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CustomPlots.SelectedIndex != -1)
            {
                CurrentPlot = AvailableRadiusPlots[CustomPlots.SelectedIndex];
            }
        }

        private async void DiscardPlotEdit_OnClick(object sender, RoutedEventArgs e)
        {
            if (_plotBackUp != null)
            {
                CurrentPlot.Points = _plotBackUp.Points;
            }
            await EditPlotModeOff();
        }

        private async void SavePlot_OnClick(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(XDimensionlessDividerTextBox.Text))
            {
                CurrentPlot.Settings.DimensionlessSettings.XDimensionlessDivider =
                    double.Parse(XDimensionlessDividerTextBox.Text, CultureInfo.InvariantCulture);
            }

            if (!string.IsNullOrEmpty(YDimensionlessDividerTextBox.Text))
            {
                CurrentPlot.Settings.DimensionlessSettings.YDimensionlessDivider =
                    double.Parse(YDimensionlessDividerTextBox.Text, CultureInfo.InvariantCulture);
            }

            await _customPlotsBl.UpdatePlot(_mapper.Map<PlotView, Plot>(CurrentPlot));
            _notifier.ShowSuccess("График обновлен");
            await EditPlotModeOff();
        }

        private PlotTypeView _currentPlotType;

        private void RefreshAvailablePlots(bool dimensionlessChanged)
        {
            if (RadiusTabItem.IsSelected)
            {
                _currentPlotType = PlotTypeView.Radius;
            }

            if (TemperatureTabItem.IsSelected)
            {
                _currentPlotType = PlotTypeView.Temperature;
            }

            AvailableRadiusPlots = PreparePlots(AvailableRadiusPlots, PlotTypeView.Radius, dimensionlessChanged);
            AvailableTemperaturePlots = PreparePlots(AvailableTemperaturePlots, PlotTypeView.Temperature, dimensionlessChanged);
        }

        private void CreateAxes()
        {
            XAxesCollection.Clear();
            XAxesCollection.Add(new Axis
            {
                Title = Settings.Default.DimensionlessPlots ? "Время" : "Время, с",
            });

            YAxesCollection.Clear();

            if (AvailableRadiusPlots.Any(x => x.IsChecked))
            {
                YAxesCollection.Add(new Axis
                {
                    Title = Settings.Default.DimensionlessPlots ? "Радиус" : "Радиус, м"
                });
            }

            if (AvailableTemperaturePlots.Any(x => x.IsChecked))
            {
                YAxesCollection.Add(new Axis
                {
                    Title = Settings.Default.DimensionlessPlots ? "Температура" : "Термпература, °С"
                });
            }
        }

        private ObservableCollection<PlotView> PreparePlots(ObservableCollection<PlotView> plots, PlotTypeView plotType, bool dimensionlessChanged)
        {
            foreach (var series in User.UserSeries)
            {
                var plot = _plotBl.CreatePlot(series, plotType, Settings.Default.DimensionlessPlots);
                
                if (plot != null)
                {
                    var duplicate = plots.FirstOrDefault(x => x.PlotId == plot.PlotId);

                    if (duplicate == null)
                    {
                        plots.Add(plot);
                    }
                    else
                    {
                        var original = plots[plots.IndexOf(duplicate)];
                        plot.IsChecked = original.IsChecked;
                        plots[plots.IndexOf(duplicate)] = plot;
                    }
                }
                else
                {
                    plots.Remove(plots.FirstOrDefault(x => x.PlotId == series.SeriesId));
                }
            }

            foreach (var userPlot in User.Plots)
            {
                if (userPlot.PlotType == plotType)
                {
                    userPlot.IsDeletable = true;
                    userPlot.IsEditable = true;

                    if (Settings.Default.DimensionlessPlots && dimensionlessChanged)
                    {
                        foreach (var point in userPlot.Points)
                        {
                            if (userPlot.Settings?.DimensionlessSettings?.XDimensionlessDivider != null)
                                point.X = point.X / userPlot.Settings.DimensionlessSettings.XDimensionlessDivider;

                            if (userPlot.Settings?.DimensionlessSettings?.YDimensionlessDivider != null)
                                point.Y = point.Y / userPlot.Settings.DimensionlessSettings.YDimensionlessDivider;
                        }
                    }

                    if (!Settings.Default.DimensionlessPlots && dimensionlessChanged)
                    {
                        foreach (var point in userPlot.Points)
                        {
                            if (userPlot.Settings?.DimensionlessSettings?.XDimensionlessDivider != null)
                                point.X = point.X * userPlot.Settings.DimensionlessSettings.XDimensionlessDivider;

                            if (userPlot.Settings?.DimensionlessSettings?.YDimensionlessDivider != null)
                                point.Y = point.Y * userPlot.Settings.DimensionlessSettings.YDimensionlessDivider;
                        }
                    }

                    if (plots.FirstOrDefault(x => x.PlotId == userPlot.PlotId) == null)
                    {
                        plots.Add(userPlot);
                    }
                }
            }

            return plots;
        }

        private void OnPlotsChecked(object sender, RoutedEventArgs e)
        {
            CreateAxes();

            foreach (var plot in AvailableRadiusPlots)
            {
                var notAddedYet = SeriesCollectionToPlot.OfType<LineSeriesId>()
                    .FirstOrDefault(x => x.Id == plot.PlotId);

                if (notAddedYet == null && plot.IsChecked)
                {
                    SeriesCollectionToPlot.Add(_plotBl.CreatePlot(plot, YAxesCollection.Count == 2));
                }
            }
        }

        private void ReCalculatePlots()
        {
            foreach (var availableRadiusPlot in AvailableRadiusPlots)
            {
                if (availableRadiusPlot.IsChecked)
                {
                    var added = SeriesCollectionToPlot.OfType<LineSeriesId>().FirstOrDefault(x => x.Id == availableRadiusPlot.PlotId);

                    if (added != null)
                    {
                        SeriesCollectionToPlot[SeriesCollectionToPlot.IndexOf(added)] = _plotBl.CreatePlot(availableRadiusPlot, YAxesCollection.Count == 2);
                    }
                }
            }

            foreach (var availableTemperaturePlot in AvailableTemperaturePlots)
            {
                if (availableTemperaturePlot.IsChecked)
                {
                    var added = SeriesCollectionToPlot.OfType<LineSeriesId>().FirstOrDefault(x => x.Id == availableTemperaturePlot.PlotId);

                    if (added != null)
                    {
                        SeriesCollectionToPlot[SeriesCollectionToPlot.IndexOf(added)] = _plotBl.CreatePlot(availableTemperaturePlot, YAxesCollection.Count == 2);
                    }
                }
            }
        }

        private void OnPlotsUnchecked(object sender, RoutedEventArgs e)
        {
            CreateAxes();

            foreach (var availableRadiusPlot in AvailableRadiusPlots)
            {
                var toRemove = SeriesCollectionToPlot.OfType<LineSeriesId>().FirstOrDefault(x => x.Id == availableRadiusPlot.PlotId);

                if (toRemove != null && !availableRadiusPlot.IsChecked)
                {
                    SeriesCollectionToPlot.Remove(toRemove);
                }
            }

            if (YAxesCollection.Count == 1)
            {
                foreach (var s in SeriesCollectionToPlot)
                {
                    s.ScalesYAt = 0;
                }
            }
        }

        private void UpdatePlots(bool dimensionlessChanged = false)
        {
            RefreshAvailablePlots(dimensionlessChanged);
            ReCalculatePlots();
        }

        private void TemperaturePlots_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TemperaturePlots.SelectedIndex != -1)
            {
                CurrentPlot = AvailableTemperaturePlots[TemperaturePlots.SelectedIndex];
            }
        }

        private async void TemperaturePlots_OnCellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            await EditPlotName(e);
        }

        private async Task EditPlotName(DataGridCellEditEndingEventArgs e)
        {
            _appStateBL.ShowAdorner(PlotToolboxLoading);
            ProgressBar.IsIndeterminate = true;

            var plotNameCell = e.EditingElement as TextBox;
            try
            {
                if (plotNameCell != null)
                {
                    if (!string.IsNullOrWhiteSpace(plotNameCell.Text))
                    {
                        var text = plotNameCell.Text;
                        var currentPlotPlotId = CurrentPlot.PlotId;

                        await Task.Run(() => _customPlotsBl.UpdatePlotName(text, currentPlotPlotId));

                        _logger.LogInfo(new LogEntry
                        {
                            Username = User.Email,
                            LogCategory = LogCategory.Plots,
                            Message = "Название графика изменено успешно."
                        });
                        _notifier.ShowSuccess("Название графика изменено успешно.");
                    }
                    else
                    {
                        _notifier.ShowInformation("Название графика не может быть пустым.");
                        plotNameCell.Text = CurrentDropPhoto.Name;
                    }
                }
            }
            catch (TimeoutException)
            {
                _notifier.ShowError(
                    "Не удалось изменить название графика. Не удалось установить подключение. Проверьте интернет соединение.");
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

            _appStateBL.HideAdorner(PlotToolboxLoading);
            ProgressBar.IsIndeterminate = false;
        }

        private void OnTemperaturePlotsChecked(object sender, RoutedEventArgs e)
        {
            CreateAxes();

            foreach (var plot in AvailableTemperaturePlots)
            {
                var notAddedYet = SeriesCollectionToPlot.OfType<LineSeriesId>().FirstOrDefault(x => x.Id == plot.PlotId);

                if (notAddedYet == null && plot.IsChecked)
                {
                    SeriesCollectionToPlot.Add(_plotBl.CreatePlot(plot, YAxesCollection.Count == 2));
                }
            }
        }

        private void OnTemperaturePlotsUnchecked(object sender, RoutedEventArgs e)
        {
            CreateAxes();

            foreach (var availableTemperaturePlot in AvailableTemperaturePlots)
            {
                var toRemove = SeriesCollectionToPlot.OfType<LineSeriesId>().FirstOrDefault(x => x.Id == availableTemperaturePlot.PlotId);

                if (toRemove != null && !availableTemperaturePlot.IsChecked)
                {
                    SeriesCollectionToPlot.Remove(toRemove);
                }
            }

            if (YAxesCollection.Count == 1)
            {
                foreach (var s in SeriesCollectionToPlot)
                {
                    s.ScalesYAt = 0;
                }
            }
        }

        private void PlotToolBoxTabControl_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RadiusTabItem.IsSelected)
            {
                _currentPlotType = PlotTypeView.Radius;
            }

            if (TemperatureTabItem.IsSelected)
            {
                _currentPlotType = PlotTypeView.Temperature;
            }

            e.Handled = true;
        }

        private void DeleteThermalRoiButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Удалить выделенную область?", "Предупреждение", MessageBoxButton.YesNo,
                MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                var toRemove = CurrentSeries.RegionOfInterest.FirstOrDefault(x => x.PhotoType == PhotoTypeView.ThermalPhoto);

                RemoveRoi(toRemove);
            }
        }

        private void DeleteSideRoiButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Удалить выделенную область?", "Предупреждение", MessageBoxButton.YesNo,
                MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                var toRemove = CurrentSeries.RegionOfInterest.FirstOrDefault(x => x.PhotoType == PhotoTypeView.SideDropPhoto);

                RemoveRoi(toRemove);
            }
        }

        private void DeleteFrontRoiButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Удалить выделенную область?", "Предупреждение", MessageBoxButton.YesNo,
                MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                var toRemove = CurrentSeries.RegionOfInterest.FirstOrDefault(x => x.PhotoType == PhotoTypeView.FrontDropPhoto);

                RemoveRoi(toRemove);

                _notifier.ShowSuccess("Выделенная область удалена");
            }
        }

        private void RemoveRoi(TypedRectangleView toRemove)
        {
            if (toRemove != null)
            {
                CurrentSeries.RegionOfInterest.Remove(toRemove);
                _seriesBL.UpdateSeriesRegionOfInterest(
                    JsonSerializeProvider.SerializeToString(CurrentSeries.RegionOfInterest),
                    CurrentSeries.SeriesId);

                CurrentSeries.RegionOfInterest = new ObservableCollection<TypedRectangleView>(User.UserSeries[SeriesDataGrid.SelectedIndex].RegionOfInterest);
                ImgCurrent.CanDrawing.Children.Remove(toRemove.RegionOfInterest);
            }
        }

        private void SideRoiCheckbox_OnClick(object sender, RoutedEventArgs e)
        {
            _seriesBL.UpdateSeriesSettings(JsonSerializeProvider.SerializeToString(CurrentSeries.Settings),
                CurrentSeries.SeriesId);

            _notifier.ShowSuccess("Настройки обновлены");
        }

        private void FrontRoiCheckbox_OnClick(object sender, RoutedEventArgs e)
        {
            _seriesBL.UpdateSeriesSettings(JsonSerializeProvider.SerializeToString(CurrentSeries.Settings),
                CurrentSeries.SeriesId);

            _notifier.ShowSuccess("Настройки обновлены");
        }
        
        private void ThermalRoiCheckbox_OnClick(object sender, RoutedEventArgs e)
        {
            if (ThermalRoiCheckbox.IsChecked.HasValue && !ThermalRoiCheckbox.IsChecked.Value)
            {
                AutoCalcSearchModeCombobox.SelectedValue = ThermalAutoCalculationSettings.InContour;
            }

            _seriesBL.UpdateSeriesSettings(JsonSerializeProvider.SerializeToString(CurrentSeries.Settings),
                CurrentSeries.SeriesId);

            _notifier.ShowSuccess("Настройки обновлены");
        }

        private void UseRoiCheckbox_OnClick(object sender, RoutedEventArgs e)
        {
            if (UseRoiCheckbox.IsChecked.HasValue && !UseRoiCheckbox.IsChecked.Value)
            {
                AutoCalcSearchModeCombobox.SelectedValue = ThermalAutoCalculationSettings.InContour;
            }

            _seriesBL.UpdateSeriesSettings(JsonSerializeProvider.SerializeToString(CurrentSeries.Settings),
                CurrentSeries.SeriesId);

            _notifier.ShowSuccess("Настройки обновлены");
        }

        private void AutoCalcSearchModeCombobox_OnDropDownClosed(object sender, EventArgs e)
        {
            if (CurrentSeries.Settings.AutoCalculationSettings.ThermalAutoCalculationSettings ==
                ThermalAutoCalculationSettingsView.InRoi)
            {
                ThermalRoiCheckbox.IsChecked = true;
                UseRoiCheckbox.IsChecked = true;
            }

            if (CurrentSeries.Settings.AutoCalculationSettings.ThermalAutoCalculationSettings ==
                ThermalAutoCalculationSettingsView.InContour)
            {
                ThermalRoiCheckbox.IsChecked = false;
            }

            _seriesBL.UpdateSeriesSettings(JsonSerializeProvider.SerializeToString(CurrentSeries.Settings),
                CurrentSeries.SeriesId);

            _notifier.ShowSuccess("Настройки обновлены");
        }

        private void TemperatureDetectionModeCombobox_OnDropDownClosed(object sender, EventArgs e)
        {
            _seriesBL.UpdateSeriesSettings(JsonSerializeProvider.SerializeToString(CurrentSeries.Settings),
                CurrentSeries.SeriesId);

            _notifier.ShowSuccess("Настройки обновлены");
        }

        private void ImportPlot_OnClick(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Excel files (*.xlsx)|*.xlsx|(*.xls)|*.xls|All files (*.*)|*.*",
                Multiselect = false,
                AddExtension = true,
                CheckFileExists = true,
                CheckPathExists = true
            };

            if (openFileDialog.ShowDialog() == true)
            {
                CurrentPlot.Points = _mapper.Map<List<SimplePoint>, ObservableCollection<SimplePointView>>(_excelOperationsBl.GetPlotPointsFromFile(openFileDialog.FileName));
            }
        }

        private void CheckNextNotCalculated_OnClick(object sender, RoutedEventArgs e)
        {
            foreach (var item in CurrentSeries.MeasurementsSeries)
            {
                if (!item.Processed)
                {
                    item.IsChecked = true;
                }
            }
        }

        private async void UseThermalPlotCheckBox_OnChecked(object sender, RoutedEventArgs e)
        {
            if (CurrentSeries != null)
            {
                CurrentSeries.Settings.GeneralSeriesSettings.UseThermalPlot = true;

                try
                {
                    ProgressBar.IsIndeterminate = true;

                    var currentSeries = CurrentSeries;
                    await Task.Run(() => _seriesBL.UpdateSeriesSettings(JsonSerializeProvider.SerializeToString(currentSeries.Settings),
                        currentSeries.SeriesId));

                    if (currentSeries.ThermalPlot == null)
                    {
                        PlotView plotForAdd = new PlotView()
                        {
                            CurrentUserId = User.UserId,
                            PlotId = Guid.NewGuid(),
                            Name = CurrentSeries.Title,
                            PlotType = PlotTypeView.Temperature,
                            SeriesId = CurrentSeries.SeriesId,
                            Points = new ObservableCollection<SimplePointView>(),
                        };

                        await _customPlotsBl.CreatePlot(_mapper.Map<PlotView, Plot>(plotForAdd));

                        CurrentSeries.ThermalPlot = plotForAdd;

                        UpdatePlots();
                    }

                    _logger.LogInfo(new LogEntry
                    {
                        Username = User.Email,
                        LogCategory = LogCategory.Measurement,
                        Message =
                            $"Серия {CurrentSeries.Title} использует термический график. Тепловизорные фотографии будут проигнорированы."
                    });
                    _notifier.ShowSuccess(
                        $"Серия {CurrentSeries.Title} использует термический график. Тепловизорные фотографии будут проигнорированы.");
                }
                catch (TimeoutException)
                {
                    _notifier.ShowError(
                        "Не удалось изменить режим построения графика. Не удалось установить подключение. Проверьте интернет соединение.");
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

                ProgressBar.IsIndeterminate = false;
            }
        }

        private async void UseThermalPlotCheckBox_OnUnchecked(object sender, RoutedEventArgs e)
        {
            if (CurrentSeries?.Settings != null)
            {
                CurrentSeries.Settings.GeneralSeriesSettings.UseThermalPlot = false;

                try
                {
                    ProgressBar.IsIndeterminate = true;

                    var currentSeries = CurrentSeries;
                    await Task.Run(() => _seriesBL.UpdateSeriesSettings(JsonSerializeProvider.SerializeToString(currentSeries.Settings),
                        currentSeries.SeriesId));

                    _logger.LogInfo(new LogEntry
                    {
                        Username = User.Email,
                        LogCategory = LogCategory.Measurement,
                        Message =
                            $"Серия {CurrentSeries.Title} использует тепловизорные фотографии. Термический график будет проигнорирован."
                    });
                    _notifier.ShowSuccess(
                        $"Серия {CurrentSeries.Title} использует тепловизорные фотографии. Термический график будет проигнорирован.");
                }
                catch (TimeoutException)
                {
                    _notifier.ShowError(
                        "Не удалось изменить режим построения графика. Не удалось установить подключение. Проверьте интернет соединение.");
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

                ProgressBar.IsIndeterminate = false;
            }
        }

        private async void ReferencePhotoDataGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ReferencePhotoDataGrid.SelectedIndex == -1) return;

            try
            {
                _appStateBL.ShowAdorner(ReferenceImageLoading);

                CurrentReferencePhoto = CurrentReferencePhotos[ReferencePhotoDataGrid.SelectedIndex];

                CurrentReferencePhoto.Content = await _referenceBl.GetReferencePhotoContent(CurrentReferencePhoto.PhotoId);

                CurrentReferencePhotoType = CurrentReferencePhoto.PhotoType;
            }
            catch (TimeoutException)
            {
                _notifier.ShowError(
                    "Не удалось загрузить референсный снимок. Не удалось установить подключение. Проверьте интернет соединение.");
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
                    Details = exception.HelpLink
                });
                throw;
            }

            SeriesDrawerSwap();

            if (CurrentReferencePhoto?.Content != null)
            {
                ReferenceImage = ImageInterpreter.LoadImage(CurrentReferencePhoto.Content);
            }
            else
            {
                ReferenceImage = null;
            }

            _appStateBL.HideAdorner(ReferenceImageLoading);
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            if (!uint.TryParse(e.Text, out var result))
            {
                e.Handled = true;
            }
        }

        private void SelectNotProcessed_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (CurrentSeries?.MeasurementsSeries?.Count > 0 && uint.TryParse(SelectNotProcessed.Text, out var result))
            {
                CurrentSeries.MeasurementsSeries.ForAll(x => x.IsChecked = false);

                for (int i = 0; i < result; i++)
                {
                    if (CurrentSeries.MeasurementsSeries.ElementAtOrDefault(i) == null)
                    {
                        break;
                    }

                    var notProcessed = CurrentSeries.MeasurementsSeries.FirstOrDefault(x => !x.Processed & !x.IsChecked);

                    if (notProcessed != null)
                    {
                        notProcessed.IsChecked = true;
                    }
                }
            }
        }

        private async void OnAcousticChecked(object sender, RoutedEventArgs e)
        {
            if (CurrentSeries != null)
            {
                var currentSeries = CurrentSeries;

                await Task.Run(() => _seriesBL.UpdateSeriesSettings(JsonSerializeProvider.SerializeToString(currentSeries.Settings), currentSeries.SeriesId));
            }
        }
    }
}