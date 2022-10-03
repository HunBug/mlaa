using Emgu.CV;
using Mlaa.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Mlaa.ViewModel
{
    internal class BoundingBoxAnnotationViewModel : ViewModelBase
    {
        private AnnotationTask? annotationTask;
        public BoundingBoxAnnotationViewModel()
        {
            NextFrameCommand = new RelayCommand(NextFrame, CanNextFrame);
            PreviousFrameCommand = new RelayCommand(PreviousFrame, CanPreviousFrame);
        }

        public AnnotationTask? AnnotationTask
        {
            get => annotationTask;
            set
            {
                annotationTask = value;
                NotifyPropertyChanged("");
            }
        }

        public ICommand NextFrameCommand { get; private set; }
        public ICommand PreviousFrameCommand { get; private set; }

        public int FrameIndex
        {
            get => AnnotationTask?.FrameImageSource.CurrentFrameIndex ?? 0;
            set
            {
                AnnotationTask?.FrameImageSource.GetFrameBitmap(value);
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(FrameImage));
            }
        }
        public BitmapSource? FrameImage
        {
            get { return AnnotationTask?.FrameImageSource.GetFrameBitmap(FrameIndex).ToBitmapSource() ?? null; }
        }

        public Sample? CurrentSample
        {
            get => AnnotationTask?.Samples.FirstOrDefault(s => s.FrameIndex == FrameIndex);
        }
        public ObservableCollection<Annotation> Annotations
        {
            get => new ObservableCollection<Annotation>(CurrentSample?.Annotations ?? new List<Annotation>());
        }
        private void NextFrame(object? obj)
        {
            FrameIndex++;
        }
        private void PreviousFrame(object? obj)
        {
            FrameIndex--;
        }

        private bool CanNextFrame(object? obj)
        {
            if (AnnotationTask?.FrameImageSource == null)
                return false;
            return FrameIndex < AnnotationTask.FrameImageSource.FrameCount - 1;
        }

        private bool CanPreviousFrame(object? obj)
        {
            if (AnnotationTask?.FrameImageSource == null)
                return false;
            return FrameIndex > 0;
        }
    }
}
