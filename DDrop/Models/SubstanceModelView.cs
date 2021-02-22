using System;
using System.ComponentModel;

namespace DDrop.Models
{
    public class SubstanceModelView : BindableObject
    {
        private Guid _substanceId;
        public Guid SubstanceId
        {
            get => _substanceId;
            set
            {
                _substanceId = value;
                RaisePropertyChanged("SubstanceId");
            }
        }

        private int _id;
        public int Id
        {
            get => _id;
            set
            {
                _id = value;
                RaisePropertyChanged("Id");
            }
        }

        private string _commonName;
        public string CommonName
        {
            get => _commonName;
            set
            {
                _commonName = value;
                RaisePropertyChanged("CommonName");
            }
        }
    }
}