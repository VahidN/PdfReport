using System;
using PdfReportSamples.IList;

namespace WpfAppTests
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        void MainWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            // To view its implementation, right click on the method and then select `go to implementation` 
            var rpt = new IListPdfReport().CreatePdfReport();
            WebBrowser1.Source = new Uri(rpt.FileName + "#navpanes=0&scrollbar=0&toolbar=0");
            //<object type="application/pdf" data="file1.pdf#navpanes=0&scrollbar=0&toolbar=0" width="500" height="650" >Click <a href="file1.pdf">here</a> to view the file</object>
        }
    }
}
