using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using Windows.Data.Pdf;
using Windows.Storage.Streams;
using Microsoft.Win32;

namespace Win81PDFViewer
{
    public partial class MainWindow
    {
        private uint _currentPageIndex;
        private PdfDocument _pdfDocument;
        private IRandomAccessStream _randomAccessStream;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void BackButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (_pdfDocument == null || _currentPageIndex <= 0)
                return;

            _currentPageIndex--;
            PdfPageImage.Source = await renderPage();
            updatePageNumber();
        }

        private async void ForwardButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (_pdfDocument == null || _currentPageIndex >= (int)_pdfDocument.PageCount - 1)
                return;

            _currentPageIndex++;
            PdfPageImage.Source = await renderPage();
            updatePageNumber();
        }

        private async void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            if (_randomAccessStream != null)
            {
                _randomAccessStream.Dispose();
            }

            var openFileDialog = new OpenFileDialog
            {
                Filter = "PDF files (*.pdf)|*.pdf|All files (*.*)|*.*"
            };
            var result = openFileDialog.ShowDialog();
            if (!result.HasValue || !result.Value) return;

            _randomAccessStream = File.Open(openFileDialog.FileName, FileMode.Open).AsRandomAccessStream();
            _pdfDocument = await PdfDocument.LoadFromStreamAsync(_randomAccessStream);

            await showFirstPage();
        }

        /// <summary>
        /// Using Windows.Data.Pdf in desktop applications
        /// </summary>
        private async Task<System.Windows.Media.Imaging.BitmapImage> renderPage()
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var winrtStream = memoryStream.AsRandomAccessStream())
                {
                    using (var page = _pdfDocument.GetPage(_currentPageIndex))
                    {
                        await page.RenderToStreamAsync(winrtStream);
                        await winrtStream.FlushAsync();

                        var bitmapImage = new System.Windows.Media.Imaging.BitmapImage();
                        bitmapImage.BeginInit();
                        //Without this, BitmapImage uses lazy initialization by default and the stream will be closed by then.
                        bitmapImage.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
                        bitmapImage.StreamSource = memoryStream;
                        bitmapImage.EndInit();

                        return bitmapImage;
                    }
                }
            }
        }

        private async Task showFirstPage()
        {
            _currentPageIndex = 0;
            PdfPageImage.Source = await renderPage();
            updatePageNumber();
        }

        private void updatePageNumber()
        {
            TxtPage.Text = string.Format("{0} of {1}", _currentPageIndex + 1, _pdfDocument.PageCount);

            ForwardButton.IsEnabled = (_pdfDocument.PageCount > 1) && (_currentPageIndex < (int)_pdfDocument.PageCount - 1);
            BackButton.IsEnabled = (_pdfDocument.PageCount > 1) && (_currentPageIndex > 0);
        }
    }
}