using System;
using System.ComponentModel;
using DDrop.Enums;

namespace DDrop.Models
{
    public class BasePhotoView : BindableObject
    {
        public Guid PhotoId { get; set; }

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                RaisePropertyChanged("Name");
            }
        }

        private byte[] _content;
        public byte[] Content
        {
            get => _content;
            set
            {
                _content = value;
                RaisePropertyChanged("Content");
            }
        }

        private string _addedDate;
        public string AddedDate
        {
            get => _addedDate;
            set
            {
                _addedDate = value;
                RaisePropertyChanged("AddedDate");
            }
        }

        private string _creationDateTime;
        public string CreationDateTime
        {
            get => _creationDateTime;
            set
            {
                _creationDateTime = value;
                RaisePropertyChanged("CreationDateTime");
            }
        }

        private PhotoTypeView _photoType;

        public PhotoTypeView PhotoType
        {
            get => _photoType;
            set
            {
                _photoType = value;
                RaisePropertyChanged("PhotoType");
            }
        }
    }
}