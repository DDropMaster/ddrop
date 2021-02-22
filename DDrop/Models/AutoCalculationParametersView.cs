using System.ComponentModel;

namespace DDrop.Models
{
    public class AutoCalculationParametersView : BindableObject
    {
        private int _ksize;

        private int _size1;

        private int _size2;

        private int _treshold1;

        private int _treshold2;

        public int Ksize
        {
            get => _ksize;
            set
            {
                _ksize = value;
                RaisePropertyChanged("Ksize");
            }
        }

        public int Treshold1
        {
            get => _treshold1;
            set
            {
                _treshold1 = value;
                RaisePropertyChanged("Treshold1");
            }
        }

        public int Treshold2
        {
            get => _treshold2;
            set
            {
                _treshold2 = value;
                RaisePropertyChanged("Treshold2");
            }
        }

        public int Size1
        {
            get => _size1;
            set
            {
                _size1 = value;
                RaisePropertyChanged("Size1");
            }
        }

        public int Size2
        {
            get => _size2;
            set
            {
                _size2 = value;
                RaisePropertyChanged("Size2");
            }
        }
    }
}