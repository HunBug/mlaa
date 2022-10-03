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
            var annotationTask = new Model.AnnotationTask(new Model.VideoFrameImageSource(@"C:\Users\akoss\Downloads\file_example_MP4_1920_18MG.mp4 "));
            annotationTask.Samples.Add(
                new Model.Sample
                {
                    FrameIndex = 0,
                    Annotations = new List<Model.Annotation>
                    {
                        new Model.Annotation {BoundingBox = new Model.Rectangle {Top= 50,   Left= 0, Width= 100, Height= 100}, Label= "Car" },
                        new Model.Annotation {BoundingBox = new Model.Rectangle {Top= 100, Left= 100, Width= 150, Height= 100}, Label= "Car" },
                        new Model.Annotation {BoundingBox = new Model.Rectangle {Top= 200, Left= 200, Width= 100, Height= 150}, Label= "Car" },
                    }
                }
            );
            var viewModel = new ViewModel.BoundingBoxAnnotationViewModel();
            viewModel.AnnotationTask = annotationTask;
            BBoxAnnotation.DataContext = viewModel;
            
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // sava datacontext to file
            var datacontext = BBoxAnnotation.DataContext as ViewModel.BoundingBoxAnnotationViewModel;
            if (datacontext != null)
            {
                var annotationTask = datacontext.AnnotationTask;
                if (annotationTask != null)
                {
                    annotationTask.Save(@"C:\Users\akoss\Downloads\file_example_MP4_1920_18MG.json");
                }
            }
        }
    }
}
