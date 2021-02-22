using System.Windows;

namespace DDrop.Utility.Extensions
{
    public static class FocusExtension
    {
        public static bool GetIsFocused(DependencyObject dependencyObject) => (bool)dependencyObject.GetValue(IsFocusedProperty);
        public static void SetIsFocused(DependencyObject dependencyObject, bool value) => dependencyObject.SetValue(IsFocusedProperty, value);
        public static readonly DependencyProperty IsFocusedProperty =
            DependencyProperty.RegisterAttached("IsFocused", typeof(bool), typeof(FocusExtension), new PropertyMetadata(false, IsFocusChanged));
        private static void IsFocusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                (d as UIElement).Focus();
            }
        }
    }
}