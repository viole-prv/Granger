using System.Windows;
using System.Windows.Controls;

namespace Granger
{
    public static class ScrollIntoView
    {
        public static readonly DependencyProperty SelectedValueProperty = DependencyProperty.RegisterAttached(
            "SelectedValue",
            typeof(object),
            typeof(ScrollIntoView),
            new PropertyMetadata(null, OnSelectedValueChange));

        public static void SetSelectedValue(DependencyObject DependencyObject, object Value)
        {
            DependencyObject.SetValue(SelectedValueProperty, Value);
        }

        public static object GetSelectedValue(DependencyObject DependencyObject)
        {
            return DependencyObject.GetValue(SelectedValueProperty);
        }

        private static void OnSelectedValueChange(DependencyObject DependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (DependencyObject is not ListBox Listbox || e.NewValue == null) return;

            Listbox.ScrollIntoView(e.NewValue);
        }
    }
}
