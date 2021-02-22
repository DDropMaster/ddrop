using System.Windows.Input;
using DDrop.Common.ViewModelBase;
using DDrop.Utility;

namespace DDrop.ViewModels
{
    public class CommentEditViewModel : ViewModelBase
    {
        public CommentEditViewModel()
        {
            CloseCommand = new RelayCommand<object>(CloseCommandExecute, CloseCommandCanExecute);
            SaveCommentCommand = new RelayCommand<object>(SaveCommentCommandExecute, SaveCommentCommandCanExecute);
        }

        private string _commentText;

        public string CommentText
        {
            get => _commentText;
            set
            {
                _commentText = value;
                RaisePropertyChanged("CommentText");
            }
        }

        private ICommand _closeCommand;
        public ICommand CloseCommand
        {
            get => _closeCommand;
            set
            {
                _closeCommand = value;
                RaisePropertyChanged(nameof(CloseCommand));
            }
        }

        private bool CloseCommandCanExecute(object obj)
        {
            return true;
        }

        private void CloseCommandExecute(object obj)
        {
            Close();
        }

        private ICommand _saveCommentCommand;
        public ICommand SaveCommentCommand
        {
            get => _saveCommentCommand;
            set
            {
                _saveCommentCommand = value;
                RaisePropertyChanged(nameof(SaveCommentCommand));
            }
        }

        private bool SaveCommentCommandCanExecute(object obj)
        {
            return true;
        }

        private void SaveCommentCommandExecute(object obj)
        {
            SaveComment();
        }

        public bool RequireSaving;

        private void SaveComment()
        {
            RequireSaving = true;
            Close();
        }
    }
}