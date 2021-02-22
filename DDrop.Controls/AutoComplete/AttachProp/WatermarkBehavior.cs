#region
//Copyright (c) 2008 Jason Kemp
//
//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:
//
//The above copyright notice and this permission notice shall be included in
//all copies or substantial portions of the Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//THE SOFTWARE.
#endregion

using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;

namespace DDrop.Controls.AutoComplete.AttachProp          
{
    public static class CueBannerService
    {
        //there is absolutely no way to write this statement out
        //to look pretty
        public static readonly DependencyProperty CueBannerProperty = DependencyProperty.RegisterAttached(
           "CueBanner", typeof(object), typeof(CueBannerService),
           new FrameworkPropertyMetadata("", CueBannerPropertyChanged));

        public static object GetCueBanner(Control control)
        {
            return control.GetValue(CueBannerProperty);
        }

        public static void SetCueBanner(Control control, object value)
        {
            control.SetValue(CueBannerProperty, value);
        }

        private static void CueBannerPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Control control = (Control)d;
            control.Loaded += control_Loaded;
            if (d is ComboBox || d is TextBox)
            {
                control.GotFocus += control_GotFocus;
                control.LostFocus += control_Loaded;
            }          
        }        

        private static void control_GotFocus(object sender, RoutedEventArgs e)
        {
            Control c = (Control)sender;
            if (ShouldShowCueBanner(c))
            {
                RemoveCueBanner(c);
            }
        }

        private static void control_Loaded(object sender, RoutedEventArgs e)
        {
            Control control = (Control)sender;
            if (ShouldShowCueBanner(control))
            {
                ShowCueBanner(control);
            }
        }

        private static void RemoveCueBanner(UIElement control)
        {
            AdornerLayer layer = AdornerLayer.GetAdornerLayer(control);

            Adorner[] adorners = layer.GetAdorners(control);
            if (adorners == null) return;
            foreach (Adorner adorner in adorners)
            {
                if (adorner is CueBannerAdorner)
                {
                    adorner.Visibility = Visibility.Hidden;
                    layer.Remove(adorner);
                }
            }
        }

        private static void ShowCueBanner(Control control)
        {
            AdornerLayer layer = AdornerLayer.GetAdornerLayer(control);
            layer.Add(new CueBannerAdorner(control, GetCueBanner(control)));
        }

        private static bool ShouldShowCueBanner(Control c)
        {
            DependencyProperty dp = GetDependencyProperty(c);
            if (dp == null) return true;
            return c.GetValue(dp).Equals("");
        }

        private static DependencyProperty GetDependencyProperty(Control control)
        {
            if (control is ComboBox)
                return ComboBox.TextProperty;
            if (control is TextBoxBase)
                return TextBox.TextProperty;            
            return null;
        }
    }

    internal class CueBannerAdorner : Adorner
    {
        private readonly ContentPresenter contentPresenter;

        public CueBannerAdorner(Control adornedElement, object cueBanner) :
            base(adornedElement)
        {
            this.IsHitTestVisible = false;            
            contentPresenter = new ContentPresenter();
            contentPresenter.Content = cueBanner;            
            contentPresenter.Opacity  = .7;            
            contentPresenter.Margin =
               new Thickness(Control.Margin.Left + Control.Padding.Left + 5,
                             Control.Margin.Top + Control.Padding.Top+2, 0, 0);
        }

        private Control Control
        {
            get { return (Control)this.AdornedElement; }
        }

        protected override Visual GetVisualChild(int index)
        {
            return contentPresenter;
        }

        protected override int VisualChildrenCount
        {
            get { return 1; }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            contentPresenter.Measure(Control.RenderSize);
            return Control.RenderSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            contentPresenter.Arrange(new Rect(finalSize));
            return finalSize;
        }
    }
}

