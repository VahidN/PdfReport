using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

namespace DemosBrowser.Behaviors
{
    public class WindowsFormsHostBehavior : DependencyObject
    {
        public static readonly DependencyProperty BindableChildProperty =
                                    DependencyProperty.RegisterAttached("BindableChild",
                                    typeof(Control),
                                    typeof(WindowsFormsHostBehavior),
                                    new UIPropertyMetadata(null, BindableChildPropertyChanged));

        public static Control GetBindableChild(DependencyObject obj)
        {
            return (Control)obj.GetValue(BindableChildProperty);
        }

        public static void SetBindableChild(DependencyObject obj, Control value)
        {
            obj.SetValue(BindableChildProperty, value);
        }

        public static void BindableChildPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var windowsFormsHost = o as WindowsFormsHost;
            if (windowsFormsHost == null)
                throw new InvalidOperationException("This behavior can only be attached to a WindowsFormsHost.");

            var control = (Control)e.NewValue;
            windowsFormsHost.Child = control;
        }
    }
}
