using Mlaa.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Mlaa.ViewModel
{
    internal class MainWindowViewModel : ViewModelBase
    {
        private AnnotationTaskViewModel? annotationTaskViewModel;

        public MainWindowViewModel()
        {
            ExitAppCommand = new RelayCommand(ExitApp, CanExit);
        }
        public ICommand ExitAppCommand { get; private set; }

        private void ExitApp(object? parameter)
        {
            Window? wnd = parameter as Window;
            wnd?.Close();
        }

        private bool CanExit(object? parameter) => true;

        public AnnotationTaskViewModel? AnnotationTaskViewModel
        {
            get => annotationTaskViewModel;
            set
            {
                annotationTaskViewModel = value;
                NotifyPropertyChanged("");
            }
        }
    }
}
