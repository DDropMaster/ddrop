using System;
using System.ComponentModel;
using DDrop.Enums;

namespace DDrop.Models
{
    public class AutoCalculationTemplate : BindableObject
    {
        private Guid _id;

        private bool _isChecked;

        private AutoCalculationParametersView _parameters;

        private CalculationVariantsView _templateType;

        private string _title;

        public Guid Id
        {
            get => _id;
            set
            {
                _id = value;
                RaisePropertyChanged("Id");
            }
        }

        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                RaisePropertyChanged("Title");
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

        public CalculationVariantsView TemplateType
        {
            get => _templateType;
            set
            {
                _templateType = value;
                RaisePropertyChanged("TemplateType");
            }
        }

        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                _isChecked = value;
                RaisePropertyChanged("IsChecked");
            }
        }
    }
}