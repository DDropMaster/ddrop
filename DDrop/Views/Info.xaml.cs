using System.ComponentModel;
using System.Windows;
using System.Windows.Documents;

namespace DDrop.Views
{
    /// <summary>
    /// Логика взаимодействия для Info.xaml
    /// </summary>
    public partial class Info : Window, INotifyPropertyChanged
    {
        public static readonly DependencyProperty DocumentProperty = DependencyProperty.Register("Document", typeof(string), typeof(Info));
        public string Document
        {
            get => (string)GetValue(DocumentProperty);
            set
            {
                SetValue(DocumentProperty, value);
                OnPropertyChanged(new PropertyChangedEventArgs("Document"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        public Info()
        {
            InitializeComponent();
        }

        private void About_Selected(object sender, RoutedEventArgs e)
        {
            Document = Properties.Resources.About;
            Logo.Visibility = Visibility.Visible;
            e.Handled = true;
        }

        private void Development_Selected(object sender, RoutedEventArgs e)
        {
            Document = Properties.Resources.Developers;
            Logo.Visibility = Visibility.Hidden;
            e.Handled = true;
        }

        private void Testers_Selected(object sender, RoutedEventArgs e)
        {
            Document = Properties.Resources.Testers;
            Logo.Visibility = Visibility.Hidden;
            e.Handled = true;
        }

        private void InterfaceMenu_Selected(object sender, RoutedEventArgs e)
        {
            Document = Properties.Resources.InterfaceMenu;
            Logo.Visibility = Visibility.Hidden;
            e.Handled = true;
        }

        private void InterfaceAccount_Selected(object sender, RoutedEventArgs e)
        {
            Document = Properties.Resources.InterfaceAccount;
            Logo.Visibility = Visibility.Hidden;
            e.Handled = true;
        }

        private void InterfaceSeriesManager_Selected(object sender, RoutedEventArgs e)
        {
            Document = Properties.Resources.InterfaceSeriesManager;
            Logo.Visibility = Visibility.Hidden;
            e.Handled = true;
        }

        private void InterfacePlot_Selected(object sender, RoutedEventArgs e)
        {
            Document = Properties.Resources.InterfacePlot;
            Logo.Visibility = Visibility.Hidden;
            e.Handled = true;
        }

        private void InterfaceCommonSeriesInformation_Selected(object sender, RoutedEventArgs e)
        {
            Document = Properties.Resources.InterfaceCommonSeriesInformation;
            Logo.Visibility = Visibility.Hidden;
        }

        private void InterfaceAutoCalculation_Selected(object sender, RoutedEventArgs e)
        {
            Document = Properties.Resources.InterfaceAutoCalculation;
            Logo.Visibility = Visibility.Hidden;
            e.Handled = true;
        }

        private void InterfaceAutoCalculationTemplates_Selected(object sender, RoutedEventArgs e)
        {
            Document = Properties.Resources.InterfaceAutoCalculationTemplates;
            Logo.Visibility = Visibility.Hidden;
            e.Handled = true;
        }

        private void InterfaceInformation_Selected(object sender, RoutedEventArgs e)
        {
            Document = Properties.Resources.InterfaceInformation;
            Logo.Visibility = Visibility.Hidden;
            e.Handled = true;
        }

        private void InterfaceLocalStoredUsers_Selected(object sender, RoutedEventArgs e)
        {
            Document = Properties.Resources.InterfaceLocalStoredUsers;
            Logo.Visibility = Visibility.Hidden;
            e.Handled = true;
        }

        private void InterfaceCommonOptions_Selected(object sender, RoutedEventArgs e)
        {
            Document = Properties.Resources.InterfaceCommonOptions;
            Logo.Visibility = Visibility.Hidden;
            e.Handled = true;
        }

        private void InterfaceReOrdering_Selected(object sender, RoutedEventArgs e)
        {
            Document = Properties.Resources.OperationsPhotosReOrder;
            Logo.Visibility = Visibility.Hidden;
            e.Handled = true;
        }

        private void InterfaceManualPhotoEdit_Selected(object sender, RoutedEventArgs e)
        {
            Document = Properties.Resources.InterfaceManualEdit;
            Logo.Visibility = Visibility.Hidden;
            e.Handled = true;
        }

        private void InterfaceManualThermalPhotoEdit_Selected(object sender, RoutedEventArgs e)
        {
            Document = Properties.Resources.InterfaceManualThermalEdit;
            Logo.Visibility = Visibility.Hidden;
            e.Handled = true;
        }

        private void CalculationModel_Selected(object sender, RoutedEventArgs e)
        {
            Document = Properties.Resources.CalculationModel;
            Logo.Visibility = Visibility.Hidden;
            e.Handled = true;
        }

        private void OperationsCommonOperations_Selected(object sender, RoutedEventArgs e)
        {
            Document = Properties.Resources.OperationsCommonOperations;
            Logo.Visibility = Visibility.Hidden;
            e.Handled = true;
        }

        private void OperationsAutoCalcAddTemplate_Selected(object sender, RoutedEventArgs e)
        {
            Document = Properties.Resources.OperationsAutoCalcAddTemplate;
            Logo.Visibility = Visibility.Hidden;
            e.Handled = true;
        }

        private void OperationsAutoCalcPython_Selected(object sender, RoutedEventArgs e)
        {
            Document = Properties.Resources.OperationsAutoCalcPython;
            Logo.Visibility = Visibility.Hidden;
            e.Handled = true;
        }

        private void OperationsAutoCalculationCommon_Selected(object sender, RoutedEventArgs e)
        {
            Document = Properties.Resources.OperationsAutoCalculationCommon;
            Logo.Visibility = Visibility.Hidden;
            e.Handled = true;
        }

        private void OperationsLocalStoredUsers_Selected(object sender, RoutedEventArgs e)
        {
            Document = Properties.Resources.OperationsLocalStoredUsersOp;
            Logo.Visibility = Visibility.Hidden;
            e.Handled = true;
        }

        private void OperationsAutoCalculationTemplatesOp_Selected(object sender, RoutedEventArgs e)
        {
            Document = Properties.Resources.OperationsAutoCalculationTemplatesOp;
            Logo.Visibility = Visibility.Hidden;
            e.Handled = true;
        }

        private void OperationsAutoExcelReport_Selected(object sender, RoutedEventArgs e)
        {
            Document = Properties.Resources.OperationsAutoExcelReport;
            Logo.Visibility = Visibility.Hidden;
            e.Handled = true;
        }

        private void OperationsImportExport_Selected(object sender, RoutedEventArgs e)
        {
            Document = Properties.Resources.OperationsImportExport;
            Logo.Visibility = Visibility.Hidden;
            e.Handled = true;
        }
    }
}
