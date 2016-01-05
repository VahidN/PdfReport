using System;
using System.Collections.Generic;
using iTextSharp.text;
using iTextSharp.text.pdf;
using PdfReportSamples.Models;
using PdfRpt.Core.Contracts;
using PdfRpt.Core.Helper;

namespace PdfReportSamples.Barcodes
{
    public class QRCodeTemplate : IColumnItemsTemplate
    {
        public void CellRendered(PdfPCell cell, Rectangle position, PdfContentByte[] canvases, CellAttributes attributes)
        {
        }

        public CellBasicProperties BasicProperties { set; get; }
        public Func<IList<CellData>, CellBasicProperties> ConditionalFormatFormula { set; get; }

        public PdfPCell RenderingCell(CellAttributes attributes)
        {
            var data = attributes.RowData.TableRowData;
            var id = data.GetSafeStringValueOf<User>(x => x.Id);

            var qrcode = new BarcodeQRCode(id, 1, 1, null);
            var image = qrcode.GetImage();
            var mask = qrcode.GetImage();
            mask.MakeMask();
            image.ImageMask = mask; // making the background color transparent
            var pdfCell = new PdfPCell(image, fit: false);

            return pdfCell;
        }
    }
}