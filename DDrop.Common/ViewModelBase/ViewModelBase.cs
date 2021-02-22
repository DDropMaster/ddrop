using System;
using System.ComponentModel;
using System.Windows.Input;

namespace DDrop.Common.ViewModelBase
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event EventHandler RequestClose;

        public ICommand ExitCommand
        { get; set; }

        public void Close()
        {
            RequestClose?.Invoke(this, EventArgs.Empty);
        }

        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}