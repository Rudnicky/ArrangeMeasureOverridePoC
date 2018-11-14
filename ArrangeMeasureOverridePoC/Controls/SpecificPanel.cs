using ArrangeMeasureOverridePoC.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ArrangeMeasureOverridePoC.Controls
{
    public sealed class SpecificPanel : Panel
    {
        #region Private Fields
        private const double ADDITIONAL_SPACE = 20.0;
        private const int DECIMAL_PLACES = 0;
        private double _totalChildWidth;
        #endregion

        #region Protected Methods
        // measures the size in layout required for child elements 
        // and determines a size for the FrameworkElement-derived class
        // - iterate through elements of particular collection,
        // - immediately gets DesiredSize on the child,
        // - compute the net desired size of the parent based upon 
        // the measurement of the child elements
        protected override Size MeasureOverride(Size availableSize)
        {
            var size = new Size();

            foreach (UIElement child in this.InternalChildren)
            {
                // measure size
                child.Measure(availableSize);

                size.Height += child.DesiredSize.Height;
                size.Width += child.DesiredSize.Width;
            }

            return size;
        }

        // Control authors who want to customize the arrange pass of layout processing should 
        // override this method. The implementation pattern should call Arrange(Rect) on each visible child element, 
        // and pass the final desired size for each child element as the finalRect parameter. Parent elements should 
        // call Arrange(Rect) on each child, otherwise the child elements will not be rendered. 
        protected override Size ArrangeOverride(Size finalSize)
        {
            var listOfWidths = new List<double>();
            var location = new Point();

            var listView = VisualTreeTraverseHelper.FindParent<ListView>(this.TemplatedParent);
            if (listView != null)
            {
                // get all of the desired widths in order
                // to pick up the greatest width of all children
                for (int i = 0; i < listView.Items.Count; i++)
                {
                    var listViewItem = (ListViewItem)listView.ItemContainerGenerator.ContainerFromIndex(i);
                    if (listViewItem != null)
                    {
                        var rootStackPanel = VisualTreeTraverseHelper.FindDescendant<StackPanel>(listViewItem);
                        if (rootStackPanel != null)
                        {
                            foreach (var child in rootStackPanel.Children)
                            {
                                var frameworkElement = child as FrameworkElement;
                                if (frameworkElement != null)
                                {
                                    listOfWidths.Add(frameworkElement.DesiredSize.Width);
                                }
                            }
                        }
                    }
                }

                // first pass won't have any desired sizes
                // second pass should have.
                if (listOfWidths.Count > 0)
                {
                    double maxValue = listOfWidths.Max<double>();

                    // iterate through children again, but this time
                    // let's set our desired width to each of them
                    for (int i = 0; i < listView.Items.Count; i++)
                    {
                        var listViewItem = (ListViewItem)listView.ItemContainerGenerator.ContainerFromIndex(i);
                        if (listViewItem != null)
                        {
                            var rootStackPanel = VisualTreeTraverseHelper.FindDescendant<StackPanel>(listViewItem);
                            if (rootStackPanel != null)
                            {
                                foreach (var child in rootStackPanel.Children)
                                {
                                    var frameworkElement = child as FrameworkElement;
                                    if (frameworkElement != null)
                                    {
                                        frameworkElement.Width = maxValue;
                                    }
                                }
                            }
                        }
                    }

                    // calculates width of whole item
                    // 3 - is a number of items inside of our stackpanel
                    // this implementation is not generic obviously
                    _totalChildWidth = Math.Round(((maxValue * 3) + ADDITIONAL_SPACE), DECIMAL_PLACES);

                    // this must be called in order to re-calculate 
                    // each child size that it's needed to display 
                    foreach (UIElement child in InternalChildren)
                        child.Measure(new Size(_totalChildWidth, child.DesiredSize.Height));
                }
            }

            // final screen positioning
            foreach (UIElement child in InternalChildren)
            {
                // set child position on the screen
                // since location.Y has been incremented
                // each child will stack vertically
                child.Arrange(new Rect(location, child.DesiredSize));
                location.Y += child.DesiredSize.Height;
            }
            return finalSize;
        }
        #endregion
    }
}
