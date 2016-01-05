using System;
using System.Collections.Generic;
using iTextSharp.text;
using iTextSharp.text.pdf;
using PdfReportSamples.Models;
using PdfRpt.Core.Contracts;
using PdfRpt.Core.Helper;

namespace PdfReportSamples.CharacterMap
{
    public class CellTemplate : IColumnItemsTemplate
    {
        readonly IPdfFont _customFont;
        public CellTemplate(IPdfFont customFont)
        {
            _customFont = customFont;
        }

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

            // Please note that All columns and properties of an object will create a single cell here.

            var idx = attributes.RowData.ColumnNumber;
            var data = attributes.RowData.TableRowData;

            var character = data.GetSafeStringValueOf<CharacterInfo>(x => x.Character, propertyIndex: idx);
            table.AddCell(new PdfPCell(_customFont.FontSelector.Process(character)) { Border = 0, HorizontalAlignment = Element.ALIGN_CENTER });

            var characterCode = data.GetSafeStringValueOf<CharacterInfo>(x => x.CharacterCode, propertyIndex: idx);
            table.AddCell(new PdfPCell(attributes.BasicProperties.PdfFont.FontSelector.Process(characterCode)) { Border = 0, HorizontalAlignment = Element.ALIGN_CENTER });

            pdfCell.AddElement(table);

            return pdfCell;
        }
    }
}
