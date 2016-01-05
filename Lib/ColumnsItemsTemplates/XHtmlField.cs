using System;
using System.Collections.Generic;
using iTextSharp.text;
using iTextSharp.text.pdf;
using PdfRpt.Core.Contracts;
using PdfRpt.Core.Helper;
using PdfRpt.Core.Helper.HtmlToPdf;

namespace PdfRpt.ColumnsItemsTemplates
{
    /// <summary>
    /// Using iTextSharp's HTML to PDF capabilities.
    /// This class uses the XmlWorker class of iTextSharp.
    /// </summary>
    public class XHtmlField : IColumnItemsTemplate
    {
        /// <summary>
        /// Table's Cells Definitions. If you don't set this value, it will be filled by using current template's settings internally.
        /// </summary>
        public CellBasicProperties BasicProperties { set; get; }

        /// <summary>
        /// Defines the current cell's properties, based on the other cells values. 
        /// Here IList contains actual row's cells values.
        /// It can be null.
        /// </summary>
        public Func<IList<CellData>, CellBasicProperties> ConditionalFormatFormula { set; get; }

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
        /// Optional external CSS files.
        /// </summary>
        public IList<string> CssFilesPath { set; get; }

        /// <summary>
        /// Optional inline CSS content.
        /// </summary>
        public string InlineCss { set; get; }

        /// <summary>
        /// Images directory path.
        /// </summary>
        public string ImagesPath { set; get; }

        /// <summary>
        /// Custom cell's content template as a PdfPCell
        /// </summary>
        /// <returns>Content as a PdfPCell</returns>
        public PdfPCell RenderingCell(CellAttributes attributes)
        {
            var html = FuncHelper.ApplyFormula(attributes.BasicProperties.DisplayFormatFormula, attributes.RowData.Value);
            attributes.RowData.FormattedValue = html;
            var cell = new XmlWorkerHelper
            {
                Html = html,
                RunDirection = attributes.BasicProperties.RunDirection.Value,
                CssFilesPath = CssFilesPath,
                InlineCss = InlineCss,
                ImagesPath = ImagesPath,
                DefaultFont = attributes.BasicProperties.PdfFont.Fonts[0]
            }.RenderHtml();
            return cell;
        }
    }
}