using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows;

namespace Mlaa.View
{
    // Based on: https://www.codeproject.com/Articles/22952/WPF-Diagram-Designer-Part-1
    internal class MoveThumb : Thumb
    {
        public MoveThumb()
        {
            DragDelta += new DragDeltaEventHandler(MoveThumb_DragDelta);
        }

        private void MoveThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            FrameworkElement? item = DataContext as FrameworkElement;

            if (item != null)
            {
                //TODO HACK01 remove hack, handle "item" in the XAML file
                if (item.TemplatedParent is ContentPresenter contentPresenter)
                {
                    item = contentPresenter;
                }
                double left = Canvas.GetLeft(item);
                double top = Canvas.GetTop(item);

                Canvas.SetLeft(item, left + e.HorizontalChange);
                Canvas.SetTop(item, top + e.VerticalChange);
            }
        }
    }
}
