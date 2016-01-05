using System;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using DemosBrowser.Toolkit.Helper;

namespace DemosBrowser
{
    public partial class App
    {
        public App()
        {
            this.DispatcherUnhandledException += appDispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            this.Startup += appStartup;
            this.Deactivated += appDeactivated;
        }

        void appDeactivated(object sender, EventArgs e)
        {
            Memory.ReEvaluatedWorkingSet();
        }

        void appStartup(object sender, StartupEventArgs e)
        {
            ReducingCpuConsumptionForAnimations();
        }

        void ReducingCpuConsumptionForAnimations()
        {
            Timeline.DesiredFrameRateProperty.OverrideMetadata(
                 typeof(Timeline),
                 new FrameworkPropertyMetadata { DefaultValue = 20 }
                 );
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = (Exception)e.ExceptionObject;
            MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private static void appDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = false;
        }
    }
}
