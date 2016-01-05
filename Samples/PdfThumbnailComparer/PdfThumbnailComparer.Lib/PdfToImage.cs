using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Acrobat; //Add a Com Object ref. to "Adobe Acrobat 10.0 Type Library" => Program Files\Adobe\Acrobat 10.0\Acrobat\acrobat.tlb
using Microsoft.Win32;

namespace PdfThumbnailComparer.Lib
{
    public class PdfToImage
    {
        const string AdobeObjectsErrorMessage = "Failed to create the PDF object.";
        const string BadFileErrorMessage = "Failed to open the PDF file.";
        const string ClipboardError = "Failed to get the image from clipboard.";
        const string SdkError = "This operation needs the Acrobat SDK(http://www.adobe.com/devnet/acrobat/downloads.html), which is combined with the full version of the Adobe Acrobat.";

        /// <summary>
        /// PDF page's thumbnails width.
        /// Its default value is 600.
        /// </summary>
        public int ThumbWidth { set; get; }

        /// <summary>
        /// PDF page's thumbnails height.
        /// Its default value is 750.
        /// </summary>
        public int ThumbHeight { set; get; }

        public string PdfFilePath { set; get; }

        CAcroPDDoc _pdfDoc;
        CAcroRect _pdfRect;

        public PdfToImage()
        {
            ThumbWidth = 600;
            ThumbHeight = 750;
        }

        public byte[] PdfPageToPng(int pageNumber = 0)
        {
            byte[] imageData = null;
            runJob(() =>
            {
                imageData = pdfPageToPng(pageNumber);
            });
            return imageData;
        }

        public void AllPdfPagesToPng(Action<ThumbData> dataCallback)
        {
            runJob(() =>
            {
                var numPages = _pdfDoc.GetNumPages();
                for (var pageNumber = 0; pageNumber < numPages; pageNumber++)
                {
                    var imageData = pdfPageToPng(pageNumber);
                    dataCallback(new ThumbData { PngImageData = imageData, CurrentPageNumber = pageNumber + 1, NumPages = numPages });
                }
            });
        }

        void runJob(Action job)
        {
            if (!File.Exists(PdfFilePath))
                throw new InvalidOperationException(BadFileErrorMessage);

            var acrobatPdfDocType = Type.GetTypeFromProgID("AcroExch.PDDoc");
            if (acrobatPdfDocType == null || !isAdobeSdkInstalled)
                throw new InvalidOperationException(SdkError);

            _pdfDoc = (CAcroPDDoc)Activator.CreateInstance(acrobatPdfDocType);
            if (_pdfDoc == null)
                throw new InvalidOperationException(AdobeObjectsErrorMessage);

            var acrobatPdfRectType = Type.GetTypeFromProgID("AcroExch.Rect");
            _pdfRect = (CAcroRect)Activator.CreateInstance(acrobatPdfRectType);

            var result = _pdfDoc.Open(PdfFilePath);
            if (!result)
                throw new InvalidOperationException(BadFileErrorMessage);

            job();

            releaseComObjects();
        }

        static bool isAdobeSdkInstalled
        {
            get
            {
                return Registry.ClassesRoot.OpenSubKey("AcroExch.PDDoc", writable: false) != null;
            }
        }

        private Bitmap pdfPageToBitmap(int pageNumber)
        {
            var pdfPage = (CAcroPDPage)_pdfDoc.AcquirePage(pageNumber);
            if (pdfPage == null)
                throw new InvalidOperationException(BadFileErrorMessage);

            var pdfPoint = (CAcroPoint)pdfPage.GetSize();

            _pdfRect.Left = 0;
            _pdfRect.right = pdfPoint.x;
            _pdfRect.Top = 0;
            _pdfRect.bottom = pdfPoint.y;

            pdfPage.CopyToClipboard(_pdfRect, 0, 0, 100);

            Bitmap pdfBitmap = null;
            var thread = new Thread(() =>
            {
                var data = Clipboard.GetDataObject();
                if (data != null && data.GetDataPresent(DataFormats.Bitmap))
                    pdfBitmap = (Bitmap)data.GetData(DataFormats.Bitmap);
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();

            Marshal.ReleaseComObject(pdfPage);

            return pdfBitmap;
        }

        private byte[] pdfPageToPng(int pageNumber)
        {
            var pdfBitmap = pdfPageToBitmap(pageNumber);
            if (pdfBitmap == null)
                throw new InvalidOperationException(ClipboardError);

            var pdfImage = pdfBitmap.GetThumbnailImage(ThumbWidth, ThumbHeight, null, IntPtr.Zero);
            // (+ 7 for template border)
            var imageData = pdfImage.ResizeImage(ThumbWidth + 7, ThumbHeight + 7);
            return imageData;
        }

        private void releaseComObjects()
        {
            _pdfDoc.Close();
            Marshal.ReleaseComObject(_pdfRect);
            Marshal.ReleaseComObject(_pdfDoc);
        }
    }
}