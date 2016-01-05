using System;
using System.Collections.Generic;
using iTextSharp.text;
using iTextSharp.text.pdf;
using PdfReportSamples.Models;
using PdfRpt.Core.Contracts;
using PdfRpt.Core.Helper;

namespace PdfReportSamples.PersianFontsListToPdf
{
    public class FontsListCellTemplate : IColumnItemsTemplate
    {
        readonly float _fontSize;
        public FontsListCellTemplate(float fontSize)
        {
            _fontSize = fontSize;
        }

        /// <summary>
        /// This method is called at the end of the cell's rendering.
        /// </summary>
        /// <param name="cell">The current cell</param>
        /// <param name="position">The coordinates of the cell</param>
        /// <param name="canvases">An array of PdfContentByte to add text or graphics</param>
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
            return new PdfPCell(new Phrase(attributes.RowData.Value.ToSafeString(), getCellCurrentFont(attributes)));
        }

        iTextSharp.text.Font getCellCurrentFont(CellAttributes attributes)
        {
            var fontName = attributes.RowData.TableRowData.GetSafeStringValueOf<FontSample>(x => x.FontName);
            return FontFactory.GetFont(fontName, BaseFont.IDENTITY_H, true, _fontSize, (int)attributes.BasicProperties.PdfFont.Style, attributes.BasicProperties.PdfFont.Color);
        }
    }
}
