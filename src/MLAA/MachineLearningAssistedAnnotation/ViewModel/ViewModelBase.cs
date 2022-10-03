using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Mlaa.ViewModel
{
    internal class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            // if an event handler has been set then invoke 
            // the delegate and pass the name of the property 
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}