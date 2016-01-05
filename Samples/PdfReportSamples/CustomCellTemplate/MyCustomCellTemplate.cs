using System;
using System.Collections.Generic;
using iTextSharp.text;
using iTextSharp.text.pdf;
using PdfRpt.Core.Contracts;
using PdfRpt.Core.Helper;

namespace PdfReportSamples.CustomCellTemplate
{
    public class MyCustomCellTemplate : IColumnItemsTemplate
    {
        Random _rnd = new Random();

        public void CellRendered(PdfPCell cell, Rectangle position, PdfContentByte[] canvases, CellAttributes attributes)
        {
        }

        public CellBasicProperties BasicProperties { set; get; }
        public Func<IList<CellData>, CellBasicProperties> ConditionalFormatFormula { set; get; }

        public PdfPCell RenderingCell(CellAttributes attributes)
        {
            var pdfCell = new PdfPCell();
            var table = new PdfGrid(1) { RunDirection = PdfWriter.RUN_DIRECTION_LTR };

            var filePath = System.IO.Path.Combine(AppPath.ApplicationPath, "Images\\" + _rnd.Next(1, 5).ToString("00") + ".png");
            var photo = PdfImageHelper.GetITextSharpImageFromImageFile(filePath);
            table.AddCell(new PdfPCell(photo, fit: true)
            {
                Border = 0,
                VerticalAlignment = Element.ALIGN_BOTTOM,
                HorizontalAlignment = Element.ALIGN_CENTER
            });

            var name = attributes.RowData.TableRowData.GetSafeStringValueOf("User");
            table.AddCell(new PdfPCell(attributes.BasicProperties.PdfFont.FontSelector.Process(name))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_CENTER
            });

            pdfCell.AddElement(table);

            return pdfCell;
        }
    }
}
