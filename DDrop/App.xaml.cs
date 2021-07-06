using System.Windows;
using AutoMapper;
using DDrop.AutoMapper;
using DDrop.BL.AppStateBL;
using DDrop.BL.Calculation;
using DDrop.BL.Comments;
using DDrop.BL.CustomPlots;
using DDrop.BL.Measurement;
using DDrop.BL.ImageProcessing.CSharp;
using DDrop.BL.ImageProcessing.Python;
using DDrop.BL.Series;
using DDrop.DAL;
using DDrop.Utility.ExceptionHandling.ExceptionHandling;
using DDrop.Utility.Logger;
using Unity;
using Unity.Injection;
using Unity.Lifetime;
using DDrop.BL.Drop;
using DDrop.BL.ExportBL;
using DDrop.BL.Radiometric.ThermalDataExtractor;
using DDrop.BL.Radiometric.ThermalPhoto;
using DDrop.BL.ReferenceBL;
using DDrop.BL.Substance;
using DDrop.BL.User;
using DDrop.Controls.AutoComplete;
using DDrop.Logic.GeometryBL;
using DDrop.Logic.Plotting;
using DDrop.Logic.SeriesLogic;
using DDrop.Utility.SettingsRepository;
using DDrop.Views;
using DDrop.Utility.ExcelOperations;
using DDrop.BL.ExcelOperations;

namespace DDrop
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly WindowExceptionHandler _exceptionHandler;

        public App()
        {
            _exceptionHandler = new WindowExceptionHandler();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            IUnityContainer container = new UnityContainer();
            container.RegisterType<IGeometryBL, GeometryBL>();
            container.RegisterType<IDropPhotoBL, DropPhotoBL>();
            container.RegisterType<IDropBL, DropBL>();
            container.RegisterType<IReferenceBl, ReferenceBl>();
            container.RegisterType<IUserBl, UserBl>();
            container.RegisterType<IMeasurementBl, MeasurementBl>();
            container.RegisterType<ISeriesBL, SeriesBL>(new InjectionConstructor(
                new ResolvedParameter<IDDropRepository>(), new ResolvedParameter<IMapper>()));
            container.RegisterType<ICalculationBL, CalculationBL>(new InjectionConstructor(
                new ResolvedParameter<IDropBL>()));
            container.RegisterType<IAppStateBL, AppStateBL>();
            container.RegisterType<IExportBl, ExportBl>();
            container.RegisterType<IPlotBl, PlotBl>();
            container.RegisterType<ICustomPlotsBl, CustomPlotsBl>();
            container.RegisterType<ISettingsRepository, SettingsRepository>();
            container.RegisterType<ISubstanceBL, SubstanceBL>();
            container.RegisterType<ISubstanceBLProxy, SubstanceBLProxy>();
            container.RegisterType<ISeriesLogic, SeriesLogic>();
            container.RegisterType<ICommonApplicationSettingsProvider, CommonApplicationSettingsProvider>(new ContainerControlledLifetimeManager());
            container.RegisterType<IThermalBL, ThermalBL>();
            container.RegisterType<ISearchDataProvider, SubstancesDataProvider>();
            container.RegisterType<IThermalPhotoBL, ThermalPhotoBL>();
            container.RegisterType<ICommentsBL, CommentsBL>();
            container.RegisterType<IDDropRepository, DDropRepository>();
            container.RegisterType<IDropletImageProcessor, DropletImageProcessor>();
            container.RegisterType<IPythonProvider, PythonProvider>();
            container.RegisterType<IExcelOperationsBl, ExcelOperationsBl>();
            container.RegisterType<ILogger, Logger>(new ContainerControlledLifetimeManager(),
                new InjectionConstructor(
                    new ResolvedParameter<IDDropRepository>()
                ));
            container.RegisterInstance(AutoMapperConfiguration.InitializeAutoMapper());


            container.Resolve<MainWindow>();
        }
    }
}