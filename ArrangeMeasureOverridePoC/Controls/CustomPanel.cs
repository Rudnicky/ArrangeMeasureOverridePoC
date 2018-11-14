using System.Windows;
using System.Windows.Controls;

namespace ArrangeMeasureOverridePoC.Controls
{
    public sealed class CustomPanel : Panel
    {
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
                child.Measure(availableSize);
                size.Width += child.DesiredSize.Width;
                size.Height += child.DesiredSize.Height;
                // do something with child.DesiredSize, either sum them directly
                // or apply whatever logic your element has for reinterpreting the child sizes
                // if greater than availableSize, must decide what to do and which size to return

                // ----------------------------------------------------------------//
                // --IN THIS CASE WE'RE DOING NOTHING THEN CALCULATIN NEEDED SIZE--//
                // ----------------------------------------------------------------//
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

            foreach (UIElement child in InternalChildren)
            {
                child.Arrange(new Rect(location, child.DesiredSize));

                // this will stack items vertically 
                location.Y += child.DesiredSize.Height;

                // this will stack items horizontally
                //location.X += child.DesiredSize.Width;
            }
            return finalSize;
        }
    }
}