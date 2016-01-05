using System;
using System.Collections.Generic;
using iTextSharp.text;
using iTextSharp.text.pdf;
using PdfReportSamples.Models;
using PdfRpt.Core.Contracts;
using PdfRpt.Core.Helper;

namespace PdfReportSamples.CustomPriceNumber
{
    public class CustomPriceCell : IColumnItemsTemplate
    {
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
            var numColumns = 10;
            var salePrice = attributes.RowData.TableRowData
                                              .GetSafeStringValueOf<Transaction>(x => x.SalePrice, nullValue: "0")
                                              .PadLeft(numColumns, ' ');

            var table = new PdfGrid(numColumns)
            {
                RunDirection = PdfWriter.RUN_DIRECTION_LTR,
                WidthPercentage = 100
            };
            for (int i = 0; i < numColumns; i++)
            {
                var character = salePrice[i].ToString();
                table.AddCell(new PdfPCell(attributes.BasicProperties.PdfFont.FontSelector.Process(character))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    BorderColor = BaseColor.GRAY,
                    UseAscender = true,
                    UseDescender = true,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    BorderWidth = 1
                });
            }

            return new PdfPCell(table);
        }
    }
}
