using DDrop.ViewModels;

namespace DDrop.Views
{
    /// <summary>
    /// Interaction logic for CommentEdit.xaml
    /// </summary>
    public partial class CommentEdit
    {
        public CommentEditViewModel ViewModel = new CommentEditViewModel();

        public CommentEdit()
        {
            InitializeComponent();

            ViewModel.RequestClose += delegate
            {
                Close();
            };

            DataContext = ViewModel;
        }
    }
}
