using System;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using PdfRpt;
using PdfRpt.VectorCharts;
using System.Collections.Generic;

namespace PdfReportSamples.VectorBarChart
{
    public class VectorBarChartPdfReport
    {
        public string CreatePdfReport()
        {
            var fonts = new GenericFontProvider(
                                Path.Combine(Environment.GetEnvironmentVariable("SystemRoot"), "fonts\\tahoma.TTF"),
                                Path.Combine(Environment.GetEnvironmentVariable("SystemRoot"), "fonts\\verdana.ttf"));

            var finalFile = Path.Combine(AppPath.ApplicationPath, "Pdf\\BarChartPdfReport.pdf");

            using (var document = new Document(PageSize.A4))
            {
                var writer = PdfWriter.GetInstance(document, new FileStream(finalFile, FileMode.Create));
                document.Open();
                var canvas = writer.DirectContent;

                var items = new List<BarChartItem>
                      {
                          new BarChartItem(10, "Item 1 caption",  new BaseColor(130, 197, 91)),
                          new BarChartItem(100, "Item 2 caption", new BaseColor(95, 182, 85)),
                          new BarChartItem(60, "Item 3 caption",  new BaseColor(130, 197, 91)),
                          new BarChartItem(70, "Item 4 caption",  new BaseColor(88, 89, 91)),
                          new BarChartItem(120, "Item 5 caption", new BaseColor(173, 216, 230)),
                          new BarChartItem(0, "Item 6 caption",  BaseColor.YELLOW),
                          new BarChartItem(210, "Item 7 caption", BaseColor.MAGENTA),
                          new BarChartItem(150, "Item 8 caption", BaseColor.ORANGE),
                          new BarChartItem(50, "Item 9 caption", BaseColor.PINK),
                          new BarChartItem(20, "Item 10 caption", BaseColor.CYAN),
                          new BarChartItem(100, "Item 11 caption", BaseColor.BLUE),
                          new BarChartItem(90, "عنوان آيتم 12", BaseColor.GREEN),
                      };

                var img = new VerticalBarChart
                {
                    PdfFont = fonts,
                    ContentByte = canvas,
                    Items = items
                }.Draw();
                document.Add(img);

                var img2 = new HorizontalBarChart
                {
                    PdfFont = fonts,
                    ContentByte = canvas,
                    Items = items
                }.Draw();
                document.Add(img2);
            }

            return finalFile;
        }
    }
}