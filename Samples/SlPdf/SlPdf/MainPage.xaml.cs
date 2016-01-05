using System.Windows;
using System.Windows.Browser;
using SlPdf.PdfServiceReference;

namespace SlPdf
{
    public partial class MainPage
    {
        #region Fields (1)

        HtmlElement _iFrame;

        #endregion Fields

        #region Constructors (1)

        public MainPage()
        {
            InitializeComponent();
            this.Loaded += mainPageLoaded;
        }

        #endregion Constructors

        #region Methods (9)

        // Private Methods (9) 

        private void btnShowPdfClick(object sender, RoutedEventArgs e)
        {
            hideIFrame();
            busyIndicator1.IsBusy = true;
            sendRequest();
        }

        private void findIFrame()
        {
            _iFrame = HtmlPage.Document.GetElementById("pdfFrame");
        }

        private void hideIFrame()
        {
            if (_iFrame == null) return;
            _iFrame.SetStyleAttribute("visibility", "hidden");
        }

        void mainPageLoaded(object sender, RoutedEventArgs e)
        {
            findIFrame();
        }

        private void pdfHostSizeChanged(object sender, SizeChangedEventArgs e)
        {
            setIFrameSize();
        }

        void pdfServiceClientCreateReportCompleted(object sender, CreateReportCompletedEventArgs e)
        {
            busyIndicator1.IsBusy = false;

            if (e.Error != null) return;
            var fileName = e.Result;
            setIFrameSize();
            showPdf(fileName);
        }

        private void sendRequest()
        {
            var pdfServiceClient = new PdfServiceClient();
            pdfServiceClient.CreateReportCompleted += pdfServiceClientCreateReportCompleted;
            pdfServiceClient.CreateReportAsync("test report");
        }

        private void setIFrameSize()
        {
            if (_iFrame == null) return;
            var gt = pdfHost.TransformToVisual(Application.Current.RootVisual);
            var offset = gt.Transform(new Point(0, 0));
            var controlLeft = (int)offset.X;
            var controlTop = (int)offset.Y;
            _iFrame.SetStyleAttribute("left", string.Format("{0}px", controlLeft));
            _iFrame.SetStyleAttribute("top", string.Format("{0}px", controlTop));
            _iFrame.SetStyleAttribute("visibility", "visible");
            _iFrame.SetStyleAttribute("height", string.Format("{0}px", pdfHost.ActualHeight));
            _iFrame.SetStyleAttribute("width", string.Format("{0}px", pdfHost.ActualWidth));
            _iFrame.SetStyleAttribute("z-index", "1000");
        }

        private void showPdf(string fileName)
        {
            if (_iFrame == null) return;
            _iFrame.SetProperty("src", "ShowPdf.aspx?file=" + fileName);
        }

        #endregion Methods
    }
}
