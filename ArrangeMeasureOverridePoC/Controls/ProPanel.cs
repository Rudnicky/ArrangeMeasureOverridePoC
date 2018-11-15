using ArrangeMeasureOverridePoC.Utils;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace ArrangeMeasureOverridePoC.Controls
{
    public sealed class ProPanel : Panel
    {
        #region Dependency Properties
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(ProPanel), new PropertyMetadata(Orientation.Vertical));

        public Thickness Spacing
        {
            get { return (Thickness)GetValue(SpacingProperty); }
            set { SetValue(SpacingProperty, value); }
        }
        public static readonly DependencyProperty SpacingProperty =
            DependencyProperty.Register("Spacing", typeof(Thickness), typeof(ProPanel), new PropertyMetadata(new Thickness(0, 0, 0, 0)));
        #endregion

        #region Constructor
        public ProPanel()
        {

        }
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
            bool alreadyTaken = false;

            foreach (UIElement child in this.InternalChildren)
            {
                child.Measure(availableSize);
                size.Width += child.DesiredSize.Width;

                if (Orientation != Orientation.Vertical)
                {
                    if (!alreadyTaken)
                    {
                        size.Height = child.DesiredSize.Height;
                        alreadyTaken = true;
                    }
                }
                else
                {
                    size.Height += child.DesiredSize.Height;
                }
            }

            return size;
        }

        // Control authors who want to customize the arrange pass of layout processing should 
        // override this method. The implementation pattern should call Arrange(Rect) on each visible child element, 
        // and pass the final desired size for each child element as the finalRect parameter. Parent elements should 
        // call Arrange(Rect) on each child, otherwise the child elements will not be rendered. 
        protected override Size ArrangeOverride(Size finalSize)
        {
            var location = new Point();
            var listOfNotStretchedItemsWidth = new List<double>();
            var numberOfHorizontalStretched = 0;
            var lengthOfItemsControlItems = 0;
            var totalParentWidth = 0.0;
            var widthOfStretchedItem = 0.0;

            // iterate through itemscontrol children in order
            // to check how many of these children have horizontalaligmnent
            // set to stretched. These calculations are needed to split available
            // width into equal parts among stretched children
            var listView = VisualTreeTraverseHelper.FindParent<ListView>(this.TemplatedParent);
            if (listView != null && listView.Items != null)
            {
                for (int i = 0; i < listView.Items.Count; i++)
                {
                    // generate container from particular index
                    // to check if it's stretched or not
                    var childContainer = listView.ItemContainerGenerator.ContainerFromIndex(i) as FrameworkElement;
                    if (childContainer != null)
                    {
                        // change thickness depending on thickness
                        // that was given by us through dependency
                        // property called 'Spacing'
                        if (this.Orientation != Orientation.Vertical)
                        {
                            if ((Spacing.Right > 0 || Spacing.Left > 0) && i < listView.Items.Count)
                            {
                                childContainer.Margin = new Thickness(Spacing.Left, 0, Spacing.Right, 0);
                            }
                        }
                        else
                        {
                            if ((Spacing.Top > 0 || Spacing.Bottom > 0) && i < listView.Items.Count - 1)
                            {
                                childContainer.Margin = new Thickness(0, Spacing.Top, 0, Spacing.Bottom);
                            }
                        }

                        // if container has stretched alignment then
                        // increment given value for further calculations
                        if (childContainer.HorizontalAlignment == HorizontalAlignment.Stretch)
                        {
                            numberOfHorizontalStretched++;
                        }
                        else
                        {
                            // store width of non-stretched items 
                            // to calculate a proper logic for 
                            // the stretched once.
                            listOfNotStretchedItemsWidth.Add(childContainer.DesiredSize.Width);
                        }
                    }
                }

                // get number of stretched children 
                lengthOfItemsControlItems = numberOfHorizontalStretched;
            }

            // setting up HorizontalAlighment on the control itself to stretch
            // will make sure that the finalSize.Width is equal to the maximum
            // available size. It will fit it's parent.
            totalParentWidth = finalSize.Width;

            // if there's children with different state
            // of horizontalalignment then remove previously
            // stored widths from the totalParentWidths
            // in order to split stretched children into equal parts.
            if (listOfNotStretchedItemsWidth.Count > 0)
            {
                foreach (var element in listOfNotStretchedItemsWidth)
                {
                    totalParentWidth -= element;
                }
            }

            // divide width for stretched child into
            // equal parts among each stretched item.
            widthOfStretchedItem = totalParentWidth / lengthOfItemsControlItems;

            // iterate through internal children to
            // set desired size and to call Arrange() method.
            foreach (UIElement child in InternalChildren)
            {
                // change the location (placement) of an item
                // depending of the orientation dependency property
                if (Orientation != Orientation.Vertical)
                {
                    // set the rectangle that will be a direct container
                    // for our child and add same width to the location
                    child.Arrange(new Rect(location, new Size(widthOfStretchedItem, child.DesiredSize.Height)));
                    if (Spacing.Right > 0 || Spacing.Left > 0)
                    {
                        location.X += widthOfStretchedItem + Math.Max(Spacing.Right, Spacing.Left);
                    }
                    else
                    {
                        location.X += widthOfStretchedItem;
                    }
                }
                else
                {
                    child.Arrange(new Rect(location, child.DesiredSize));
                    if (Spacing.Top > 0 || Spacing.Bottom > 0)
                    {
                        location.X += widthOfStretchedItem + Math.Max(Spacing.Top, Spacing.Bottom);
                    }
                    else
                    {
                        location.Y += child.DesiredSize.Height;
                    }
                }
            }
            return finalSize;
        }
        #endregion
    }
}
