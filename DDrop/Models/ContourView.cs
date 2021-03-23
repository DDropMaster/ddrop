using System;
using System.Collections.ObjectModel;
using System.Windows.Shapes;
using DDrop.Enums;

namespace DDrop.Models
{
    public class ContourView : BindableObject
    {
        private Guid _contourId;

        private ObservableCollection<Line> _lines;

        private AutoCalculationParametersView _parameters;

        private ObservableCollection<SimpleLineView> _simpleLines;

        public Guid ContourId
        {
            get => _contourId;
            set
            {
                _contourId = value;
                RaisePropertyChanged("ContourId");
            }
        }

        public ObservableCollection<SimpleLineView> SimpleLines
        {
            get => _simpleLines;
            set
            {
                _simpleLines = value;
                RaisePropertyChanged("SimpleLines");
            }
        }

        public ObservableCollection<Line> Lines
        {
            get => _lines;
            set
            {
                _lines = value;
                RaisePropertyChanged("Lines");
            }
        }

        public AutoCalculationParametersView Parameters
        {
            get => _parameters;
            set
            {
                _parameters = value;
                RaisePropertyChanged("Parameters");
            }
        }

        private CalculationVariantsView _calculationVariants;
        public CalculationVariantsView CalculationVariants
        {
            get => _calculationVariants;
            set
            {
                _calculationVariants = value;
                RaisePropertyChanged("CalculationVariants");
            }
        }
    }
}