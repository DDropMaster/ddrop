using System;
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

        private DateTime? _addedDate;
        public DateTime? AddedDate
        {
            get => _addedDate;
            set
            {
                _addedDate = value;
                RaisePropertyChanged("AddedDate");
            }
        }

        private DateTime? _creationDateTime;
        public DateTime? CreationDateTime
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