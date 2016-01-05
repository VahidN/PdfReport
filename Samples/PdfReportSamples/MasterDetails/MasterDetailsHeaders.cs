using System.Collections.Generic;
using iTextSharp.text;
using iTextSharp.text.pdf;
using PdfRpt.ColumnsItemsTemplates;
using PdfRpt.Core.Contracts;
using PdfRpt.Core.Helper;

namespace PdfReportSamples.MasterDetails
{
    public class MasterDetailsHeaders : IPageHeader
    {
        public IPdfFont PdfRptFont { set; get; }

        public PdfGrid RenderingGroupHeader(Document pdfDoc, PdfWriter pdfWriter, IList<CellData> newGroupInfo, IList<SummaryCellData> summaryData)
        {
            var parentName = newGroupInfo.GetSafeStringValueOf("ParentName");
            var parentLastName = newGroupInfo.GetSafeStringValueOf("ParentLastName");
            var parentBirthDate = newGroupInfo.GetSafeStringValueOf("ParentBirthDate");

            var table = new PdfGrid(relativeWidths: new[] { 1f, 5f }) { WidthPercentage = 100 };
            table.AddSimpleRow(
                (cellData, cellProperties) =>
                {
                    cellData.Value = "Name:";
                    cellProperties.PdfFont = PdfRptFont;
                    cellProperties.PdfFontStyle = DocumentFontStyle.Bold;
                    cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                },
                (cellData, cellProperties) =>
                {
                    cellData.Value = parentName;
                    cellProperties.PdfFont = PdfRptFont;
                    cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                });
            table.AddSimpleRow(
                (cellData, cellProperties) =>
                {
                    cellData.Value = "Last Name:";
                    cellProperties.PdfFont = PdfRptFont;
                    cellProperties.PdfFontStyle = DocumentFontStyle.Bold;
                    cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                },
                (cellData, cellProperties) =>
                {
                    cellData.Value = parentLastName;
                    cellProperties.PdfFont = PdfRptFont;
                    cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                });
            table.AddSimpleRow(
               (cellData, cellProperties) =>
               {
                   cellData.Value = "Birth Date:";
                   cellProperties.PdfFont = PdfRptFont;
                   cellProperties.PdfFontStyle = DocumentFontStyle.Bold;
                   cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
               },
               (cellData, cellProperties) =>
               {
                   cellData.Value = parentBirthDate;
                   cellProperties.PdfFont = PdfRptFont;
                   cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
               });
            return table.AddBorderToTable(borderColor: BaseColor.LIGHT_GRAY, spacingBefore: 5f);
        }

        public PdfGrid RenderingReportHeader(Document pdfDoc, PdfWriter pdfWriter, IList<SummaryCellData> summaryData)
        {
            var table = new PdfGrid(numColumns: 1) { WidthPercentage = 100 };
            table.AddSimpleRow(
               (cellData, cellProperties) =>
               {
                   cellData.CellTemplate = new ImageFilePathField();
                   cellData.Value = System.IO.Path.Combine(AppPath.ApplicationPath, "Images\\01.png");
                   cellProperties.HorizontalAlignment = HorizontalAlignment.Center;
               });
            table.AddSimpleRow(
               (cellData, cellProperties) =>
               {
                   cellData.Value = "Family rpt";
                   cellProperties.PdfFont = PdfRptFont;
                   cellProperties.PdfFontStyle = DocumentFontStyle.Bold;
                   cellProperties.HorizontalAlignment = HorizontalAlignment.Center;
               });
            return table.AddBorderToTable();
        }
    }
}
