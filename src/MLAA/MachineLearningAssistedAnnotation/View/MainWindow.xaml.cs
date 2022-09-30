using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Mlaa.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var viewModel = new ViewModel.BoundingBoxAnnotationViewModel();
            viewModel.AnnotationTask = new Model.AnnotationTask(new Model.VideoFrameImageSource(@"C:\Users\akoss\Downloads\file_example_MP4_1920_18MG.mp4 "));
            BBoxAnnotation.DataContext = viewModel;
            
        }
    }
}
