using System;
using System.Windows;
using System.Windows.Threading;

namespace DemosBrowser.Toolkit.Threading
{
    public static class DispatcherHelper
    {
        public static void DispatchAction(Action func)
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle, func);
        }

        public static void DispatchBeginAction(Action func)
        {
            Application.Current.Dispatcher.BeginInvoke(func, DispatcherPriority.ApplicationIdle);
        }
    }
}
