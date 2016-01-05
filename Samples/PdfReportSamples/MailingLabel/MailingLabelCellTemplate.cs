using System;
using System.Collections.Generic;
using iTextSharp.text;
using iTextSharp.text.pdf;
using PdfReportSamples.Models;
using PdfRpt.Core.Contracts;
using PdfRpt.Core.Helper;

namespace PdfReportSamples.MailingLabel
{
    public class MailingLabelCellTemplate : IColumnItemsTemplate
    {
        Random _rnd = new Random();

        /// <summary>
        /// This method is called at the end of the cell's rendering.
        /// </summary>
        /// <param name="cell">The current cell</param>
        /// <param name="position">The coordinates of the cell</param>
        /// <param name="canvases"></param>
        /// <param name="attributes">Current cell's custom attributes</param>
        public void CellRendered(PdfPCell cell, Rectangle position, PdfContentByte[] canvases, CellAttributes attributes)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public CellBasicProperties BasicProperties { set; get; }

        /// <summary>
        /// Defines the current cell's properties, based on the other cells values. 
        /// Here IList contains actual row's cells values.
        /// It can be null.
        /// </summary>
        public Func<IList<CellData>, CellBasicProperties> ConditionalFormatFormula { set; get; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public PdfPCell RenderingCell(CellAttributes attributes)
        {
            var pdfCell = new PdfPCell();
            var table = new PdfGrid(1) { RunDirection = PdfWriter.RUN_DIRECTION_LTR };

            var photo = PdfImageHelper.GetITextSharpImageFromImageFile(System.IO.Path.Combine(AppPath.ApplicationPath, "Images\\" + _rnd.Next(1, 5).ToString("00") + ".png"));
            table.AddCell(new PdfPCell(photo) { Border = 0, MinimumHeight = photo.Height, VerticalAlignment = Element.ALIGN_BOTTOM });

            var name = attributes.RowData.TableRowData.GetSafeStringValueOf<User>(x => x.Name);
            table.AddCell(new PdfPCell(attributes.BasicProperties.PdfFont.FontSelector.Process(name)) { Border = 0 });

            var lastName = attributes.RowData.TableRowData.GetSafeStringValueOf<User>(x => x.LastName);
            table.AddCell(new PdfPCell(attributes.BasicProperties.PdfFont.FontSelector.Process(lastName)) { Border = 0 });

            pdfCell.AddElement(table);

            return pdfCell;
        }
    }
}
