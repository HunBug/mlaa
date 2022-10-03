﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;
using System.Xml.Linq;

namespace Mlaa.View
{
    // Based on: https://www.codeproject.com/Articles/22952/WPF-Diagram-Designer-Part-1
    internal class ResizeThumb : Thumb
    {
        public ResizeThumb()
        {
            DragDelta += new DragDeltaEventHandler(ResizeThumb_DragDelta);
        }
        
        private void ResizeThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            //TODO HACK01 remove hack, handle "itemPresenter" in the XAML file
            FrameworkElement? item = DataContext as FrameworkElement;
            FrameworkElement? itemPresenter = DataContext as FrameworkElement;

            if (item != null)
            {
                if (item.TemplatedParent is ContentPresenter contentPresenter)
                {
                    itemPresenter = contentPresenter;
                }
                double deltaVertical, deltaHorizontal;

                switch (VerticalAlignment)
                {
                    case VerticalAlignment.Bottom:
                        deltaVertical = Math.Min(-e.VerticalChange,
                            item.ActualHeight - item.MinHeight);
                        item.Height -= deltaVertical;
                        break;
                    case VerticalAlignment.Top:
                        deltaVertical = Math.Min(e.VerticalChange,
                            item.ActualHeight - item.MinHeight);
                        Canvas.SetTop(itemPresenter, Canvas.GetTop(itemPresenter) + deltaVertical);
                        item.Height -= deltaVertical;
                        break;
                    default:
                        break;
                }

                switch (HorizontalAlignment)
                {
                    case HorizontalAlignment.Left:
                        deltaHorizontal = Math.Min(e.HorizontalChange,
                            item.ActualWidth - item.MinWidth);
                        Canvas.SetLeft(itemPresenter, Canvas.GetLeft(itemPresenter) + deltaHorizontal);
                        item.Width -= deltaHorizontal;
                        break;
                    case HorizontalAlignment.Right:
                        deltaHorizontal = Math.Min(-e.HorizontalChange,
                            item.ActualWidth - item.MinWidth);
                        item.Width -= deltaHorizontal;
                        break;
                    default:
                        break;
                }
            }

            e.Handled = true;
        }
    }
}