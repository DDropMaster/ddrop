using System;
using System.ComponentModel;
using DDrop.Enums;

namespace DDrop.Models
{
    public class CommentView : BindableObject
    {
        private Guid _commentId;
        public Guid CommentId
        {
            get => _commentId;
            set
            {
                _commentId = value;
                RaisePropertyChanged("CommentId");
            }
        }

        private string _content;
        public string Content
        {
            get => _content;
            set
            {
                _content = value;
                RaisePropertyChanged("Content");
                RaisePropertyChanged(nameof(CommentAdded));
            }
        }

        private CommentableEntityTypeView _type;
        public CommentableEntityTypeView Type
        {
            get => _type;
            set
            {
                _type = value;
                RaisePropertyChanged("Type");
            }
        }

        private bool _commentAdded;
        public bool CommentAdded
        {
            get
            {
                if (!string.IsNullOrEmpty(Content))
                {
                    return true;
                }

                return false;
            }
            set
            {
                _commentAdded = value;
                RaisePropertyChanged("CommentAdded");
            }
        }
    }
}