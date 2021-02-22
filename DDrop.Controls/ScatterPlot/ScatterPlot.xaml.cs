using System.ComponentModel;
using System.Windows;
using LiveCharts;
using LiveCharts.Wpf;

namespace DDrop.Controls.ScatterPlot
{
    /// <summary>
    ///     Логика взаимодействия для UserControl1.xaml
    /// </summary>
    public partial class ScatterPlot : INotifyPropertyChanged
    {
        public static readonly DependencyProperty SeriesCollectionToPlotProperty = 
            DependencyProperty.Register("SeriesCollectionToPlot", typeof(SeriesCollection), typeof(ScatterPlot));
        public SeriesCollection SeriesCollectionToPlot
        {
            get => (SeriesCollection)GetValue(SeriesCollectionToPlotProperty);
            set
            {
                SetValue(SeriesCollectionToPlotProperty, value);
                OnPropertyChanged(new PropertyChangedEventArgs("SeriesCollectionToPlot"));
            }
        }

        public static readonly DependencyProperty XAxesCollectionProperty =
            DependencyProperty.Register("XAxesCollection", typeof(AxesCollection), typeof(ScatterPlot));
        public AxesCollection XAxesCollection
        {
            get => (AxesCollection)GetValue(XAxesCollectionProperty);
            set
            {
                SetValue(XAxesCollectionProperty, value);
                OnPropertyChanged(new PropertyChangedEventArgs("XAxesCollection"));
            }
        }
        
        public static readonly DependencyProperty YAxesCollectionProperty = 
            DependencyProperty.Register("YAxesCollection", typeof(AxesCollection), typeof(ScatterPlot));
        public AxesCollection YAxesCollection
        {
            get => (AxesCollection)GetValue(YAxesCollectionProperty);
            set
            {
                SetValue(YAxesCollectionProperty, value);
                OnPropertyChanged(new PropertyChangedEventArgs("YAxesCollection"));
            }
        }
        
        public ScatterPlot()
        {
            InitializeComponent();

            if (DesignerProperties.GetIsInDesignMode(this)) return;

            Loaded += ScatterPlot_Loaded;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void ScatterPlot_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = this;
        }

        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }
    }
}