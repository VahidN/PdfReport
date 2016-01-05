using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Data.Pdf;
using Nito.AsyncEx;

namespace Win81PdfToImage
{
    class Program
    {
        static void Main(string[] args)
        {
            AsyncContext.Run(async () =>
            {
                await convertPdfToImages();
            });
        }

        /// <summary>
        /// Using Windows.Data.Pdf in desktop applications
        /// </summary>
        private static async Task convertPdfToImages()
        {
            using (var randomAccessStream = File.Open("PieChartPdfReport.pdf", FileMode.Open).AsRandomAccessStream())
            {
                var pdfDocument = await PdfDocument.LoadFromStreamAsync(randomAccessStream);
                for (uint i = 0; i < pdfDocument.PageCount; i++)
                {
                    using (var page = pdfDocument.GetPage(i))
                    {
                        /*var renderOptions = new PdfPageRenderOptions
                        {
                            BackgroundColor = Colors.LightGray,
                            DestinationHeight = (uint) (page.Size.Height*10)
                        };*/

                        using (var stream = File.Open(string.Format("page-{0}.png", i + 1), FileMode.OpenOrCreate).AsRandomAccessStream())
                        {
                            await page.RenderToStreamAsync(stream/*, renderOptions*/);
                            await stream.FlushAsync();
                        }
                    }
                }
            }
        }
    }
}