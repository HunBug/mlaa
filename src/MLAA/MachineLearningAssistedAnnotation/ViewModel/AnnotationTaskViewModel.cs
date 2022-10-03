using Mlaa.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mlaa.ViewModel
{
    internal class AnnotationTaskViewModel : ViewModelBase
    {
        private AnnotationTask? annotationTask;

        public AnnotationTaskViewModel()
        {
        }

        public string Path { get => annotationTask?.FrameImageSource.Path ?? ""; }
        public string SamplePath { get => System.IO.Path.ChangeExtension(annotationTask?.FrameImageSource.Path, ".json") ?? ""; }

        public AnnotationTask? AnnotationTask
        {
            get => annotationTask;
            set
            {
                annotationTask = value;
                NotifyPropertyChanged();
            }
        }
    }
}
