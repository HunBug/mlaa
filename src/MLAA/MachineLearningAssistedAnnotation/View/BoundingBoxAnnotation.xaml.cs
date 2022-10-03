using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// Interaction logic for BoundingBoxAnnotation.xaml
    /// </summary>
    public partial class BoundingBoxAnnotation : UserControl
    {
        public BoundingBoxAnnotation()
        {
            InitializeComponent();
        }

        private bool isDraggingRectangle;
        private bool isAddingNewRectangle;
        private bool isDeleting;

        private Point draggingStartPoint;
        private Rectangle? draggedObject;
        private Ellipse? resizeObject;

        //TODO HACK01
        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            isDraggingRectangle = true;
            // save start point of dragging
            draggingStartPoint = Mouse.GetPosition(AnnotationCanvas);
            draggedObject = sender as Rectangle;
            resizeObject = sender as Ellipse;
            if (resizeObject != null)
            {

                // find the rectangle that is being resized
                var parentGrid = resizeObject.Parent as Grid;
                draggedObject = parentGrid?.Children.OfType<Rectangle>().Single();
            }
            if (e.RightButton == MouseButtonState.Pressed)
            {
                isDeleting = true;
            }
            e.Handled = true;
        }
        //TODO HACK01
        private void Rectangle_MouseMove(object sender, MouseEventArgs e)
        {
            if (draggedObject != null && isDraggingRectangle)
            {
                HandleAnnotationMoveAndResize();
            }
            e.Handled = true;
        }

        private void HandleAnnotationMoveAndResize()
        {
            if (draggedObject == null)
            {
                return;
            }
            FrameworkElement? annotationControl = draggedObject?.TemplatedParent as FrameworkElement;
            Model.Annotation? annotation = null;
            if (annotationControl?.DataContext is Model.Annotation)
            {
                annotation = (Model.Annotation)annotationControl.DataContext;
            }
            //check mouse whether left button is pressed
            bool mouseIsDown = Mouse.LeftButton == MouseButtonState.Pressed;
            if (!mouseIsDown)
            {
                isDraggingRectangle = false;
            }
            if (isDraggingRectangle && annotation != null && annotationControl != null)
            {
                // get current mouse position
                Point currentPoint = Mouse.GetPosition(AnnotationCanvas);
                // calculate delta
                double deltaX = currentPoint.X - draggingStartPoint.X;
                double deltaY = currentPoint.Y - draggingStartPoint.Y;

                var moveObject = false;
                if (resizeObject != null && draggedObject != null)
                {
                    var verticalAlignment = resizeObject.VerticalAlignment;
                    var horizontalAlignment = resizeObject.HorizontalAlignment;
                    switch (verticalAlignment)
                    {
                        case VerticalAlignment.Bottom:
                            var newHeight1 = Math.Max(annotation.BoundingBox.Height + deltaY, 1);
                            annotation.BoundingBox.Height = newHeight1;
                            draggedObject.Height = newHeight1;
                            break;
                        case VerticalAlignment.Top:
                            var newHeight2 = Math.Max(annotation.BoundingBox.Height - deltaY, 1);
                            moveObject = true;
                            annotation.BoundingBox.Height = newHeight2;
                            draggedObject.Height = newHeight2;
                            break;
                        default:
                            break;
                    }
                    switch (horizontalAlignment)
                    {
                        case HorizontalAlignment.Left:
                            var newWidth1 = Math.Max(annotation.BoundingBox.Width - deltaX, 1);
                            moveObject = true;
                            annotation.BoundingBox.Width = newWidth1;
                            draggedObject.Width = newWidth1;
                            break;
                        case HorizontalAlignment.Right:
                            var newWidth2 = Math.Max(annotation.BoundingBox.Width + deltaX, 1);
                            annotation.BoundingBox.Width = newWidth2;
                            draggedObject.Width = newWidth2;
                            break;
                        default:
                            break;
                    }
                    if (horizontalAlignment != HorizontalAlignment.Left)
                    {
                        deltaX = 0;
                    }
                    if (verticalAlignment != VerticalAlignment.Top)
                    {
                        deltaY = 0;
                    }
                }
                else
                {
                    moveObject = true;
                }
                if (moveObject)
                {
                    // update rectangle position
                    annotation.BoundingBox.Left += deltaX;
                    annotation.BoundingBox.Top += deltaY;
                    Canvas.SetLeft(annotationControl, Canvas.GetLeft(annotationControl) + deltaX);
                    Canvas.SetTop(annotationControl, Canvas.GetTop(annotationControl) + deltaY);
                }
                annotation.AnnotationSource = Model.AnnotationSource.User;
                // update start point
                draggingStartPoint = currentPoint;
            }
        }

        //TODO HACK01
        private void Rectangle_MouseUp(object sender, MouseButtonEventArgs e)
        {
            isDraggingRectangle = false;
            if (isAddingNewRectangle)
            {
                var itemContainer = sender as Grid;
                if (itemContainer?.DataContext is ViewModel.BoundingBoxAnnotationViewModel)
                {
                    var viewModel = (ViewModel.BoundingBoxAnnotationViewModel)itemContainer.DataContext;
                    var currentPosition = Mouse.GetPosition(AnnotationCanvas);
                    viewModel.AddAnnotation(new Model.Annotation() 
                    {
                        BoundingBox = new Model.Rectangle()
                        {
                            Left = draggingStartPoint.X,
                            Top = draggingStartPoint.Y,
                            Width = currentPosition.X - draggingStartPoint.X,
                            Height = currentPosition.Y - draggingStartPoint.Y
                        },
                        AnnotationSource = Model.AnnotationSource.User
                    });
                }
            }
            isAddingNewRectangle = false;
            e.Handled = true;
        }

        private void EmptyCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                isAddingNewRectangle = true;
                draggingStartPoint = Mouse.GetPosition(AnnotationCanvas);
            }
            e.Handled = true;
        }

        private void Rectangle_MouseUp_1(object sender, MouseButtonEventArgs e)
        {
            if (isDeleting)
            {
                var currentObject = sender as Rectangle;
                if (currentObject != null)
                {
                    if (currentObject == draggedObject)
                    {
                        var viewModel = AnnotationCanvas.DataContext as ViewModel.BoundingBoxAnnotationViewModel;
                        if (viewModel != null)
                        {
                            viewModel.RemoveAnnotation(currentObject.DataContext as Model.Annotation);
                        }
                    }
                }
            }
            isDeleting = false;
            e.Handled = true;
        }
    }
}
