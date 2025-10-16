using System.Windows;
using System.Windows.Controls;

namespace SumInWord_C.Wpf.CustomControls
{

    public class PlaceHolderTextBox : TextBox
    {

        public string PlaceHolder
        {
            get { return (string)GetValue(PlaceHolderProperty); }
            set { SetValue(PlaceHolderProperty, value); }
        }

        public static readonly DependencyProperty PlaceHolderProperty =
            DependencyProperty.Register("PlaceHolder", typeof(string), typeof(PlaceHolderTextBox), new PropertyMetadata(default));

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(PlaceHolderTextBox), new PropertyMetadata(default));



        public double PlaceholderFontSize
        {
            get { return (double)GetValue(PlaceholderFontSizeProperty); }
            set { SetValue(PlaceholderFontSizeProperty, value); }
        }

        public static readonly DependencyProperty PlaceholderFontSizeProperty =
            DependencyProperty.Register("PlaceholderFontSize", typeof(double), typeof(PlaceHolderTextBox), new PropertyMetadata(double.NaN));

        // 2. PlaceholderHorizontalAlignment Dependency Property
        public HorizontalAlignment PlaceholderHorizontalAlignment
        {
            get { return (HorizontalAlignment)GetValue(PlaceholderHorizontalAlignmentProperty); }
            set { SetValue(PlaceholderHorizontalAlignmentProperty, value); }
        }

        public static readonly DependencyProperty PlaceholderHorizontalAlignmentProperty =
            DependencyProperty.Register("PlaceholderHorizontalAlignment", typeof(HorizontalAlignment), typeof(PlaceHolderTextBox), new PropertyMetadata(HorizontalAlignment.Left));

        public VerticalAlignment PlaceholderVerticalAlignment
        {
            get { return (VerticalAlignment)GetValue(PlaceholderVerticalAlignmentProperty); }
            set { SetValue(PlaceholderHorizontalAlignmentProperty, value); }
        }

        public static readonly DependencyProperty PlaceholderVerticalAlignmentProperty =
            DependencyProperty.Register("PlaceholderVerticalAlignment", typeof(VerticalAlignment), typeof(PlaceHolderTextBox), new PropertyMetadata(VerticalAlignment.Top));

        static PlaceHolderTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PlaceHolderTextBox), new FrameworkPropertyMetadata(typeof(PlaceHolderTextBox)));
        }
    }
}
