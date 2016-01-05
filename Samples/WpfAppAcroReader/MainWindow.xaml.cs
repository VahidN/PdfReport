using WpfPdfViewer.Controls;
using PdfReportSamples.IList;

namespace WpfPdfViewer
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            createRpt();
        }

        private void createRpt()
        {
            // To view its implementation, right click on the method and then select `go to implementation` 
            var rpt = new IListPdfReport().CreatePdfReport();
            WindowsFormsHost1.Child = new AcroReader(rpt.FileName);
        }
    }
}
