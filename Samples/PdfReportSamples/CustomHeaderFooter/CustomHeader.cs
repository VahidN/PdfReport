using System.Collections.Generic;
using iTextSharp.text;
using iTextSharp.text.pdf;
using PdfRpt.Core.Contracts;
using PdfRpt.Core.Helper;

namespace PdfReportSamples.CustomHeaderFooter
{
    public class CustomHeader : IPageHeader
    {
        public PdfGrid RenderingGroupHeader(Document pdfDoc, PdfWriter pdfWriter, IList<CellData> rowdata, IList<SummaryCellData> summaryData)
        {
            return null; // don't render anything.
        }

        Image _image;
        public PdfGrid RenderingReportHeader(Document pdfDoc, PdfWriter pdfWriter, IList<SummaryCellData> summaryData)
        {
            if (_image == null) //cache is empty
            {
                var templatePath = System.IO.Path.Combine(AppPath.ApplicationPath, "data\\PdfHeaderTemplate.pdf");
                _image = PdfImageHelper.GetITextSharpImageFromPdfTemplate(pdfWriter, templatePath);
            }

            var table = new PdfGrid(1);
            var cell = new PdfPCell(_image, true) { Border = 0 };
            table.AddCell(cell);
            return table;

            //Note: return null if you want to skip this callback and render nothing. Also in this case, you need to set the header.CacheHeader(cache: false) too.
        }
    }
}