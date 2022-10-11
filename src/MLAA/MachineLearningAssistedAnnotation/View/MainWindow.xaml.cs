using Microsoft.WindowsAPICodePack.Dialogs;
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
        private void OpenVideo_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "Video files (*.mp4)|*.mp4|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                var annotationTask = new Model.AnnotationTask(new Model.VideoFrameImageSource(openFileDialog.FileName));
                var viewModel = new ViewModel.BoundingBoxAnnotationViewModel();
                //Load annotations if exist
                var annotationFilePath = System.IO.Path.ChangeExtension(openFileDialog.FileName, ".json");
                if (System.IO.File.Exists(annotationFilePath))
                {
                    annotationTask.Load(annotationFilePath);
                }
                viewModel.AnnotationTask = annotationTask;
                BBoxAnnotation.DataContext = viewModel;
                (DataContext as ViewModel.MainWindowViewModel).AnnotationTaskViewModel = new ViewModel.AnnotationTaskViewModel()
                {
                    AnnotationTask = annotationTask
                };
            }
        }

        private void OpenFolder_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                var annotationTask = new Model.AnnotationTask(new Model.ImageFolderImageSource(dialog.FileName));
                var bboxViewModel = new ViewModel.BoundingBoxAnnotationViewModel();
                //Load annotations if exist
                var annotationFilePath = System.IO.Path.ChangeExtension(dialog.FileName, ".json");
                if (System.IO.File.Exists(annotationFilePath))
                {
                    annotationTask.Load(annotationFilePath);
                }
                bboxViewModel.AnnotationTask = annotationTask;
                BBoxAnnotation.DataContext = bboxViewModel;
                (DataContext as ViewModel.MainWindowViewModel).AnnotationTaskViewModel = new ViewModel.AnnotationTaskViewModel()
                {
                    AnnotationTask = annotationTask
                };
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var datacontext = BBoxAnnotation.DataContext as ViewModel.BoundingBoxAnnotationViewModel;
            if (datacontext != null)
            {
                var annotationTask = datacontext.AnnotationTask;
                if (annotationTask != null)
                {
                    annotationTask.Save((DataContext as ViewModel.MainWindowViewModel).AnnotationTaskViewModel.SamplePath);
                }
            }
        }

        private void TestNN_Click(object sender, RoutedEventArgs e)
        {
            NeuralNetwork.Class1.Test();
        }
    }
}
