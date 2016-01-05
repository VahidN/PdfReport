using System.Collections.Generic;
using iTextSharp.text;
using iTextSharp.text.pdf;
using PdfReportSamples.Models;
using PdfRpt.ColumnsItemsTemplates;
using PdfRpt.Core.Contracts;
using PdfRpt.Core.Helper;

namespace PdfReportSamples.WrapGroupsInColumns
{
    public class WGHeaders : IPageHeader
    {
        public IPdfFont PdfRptFont { set; get; }
        int _groupNumber;

        public PdfGrid RenderingGroupHeader(Document pdfDoc, PdfWriter pdfWriter, IList<CellData> newGroupInfo, IList<SummaryCellData> summaryData)
        {
            var groupName = newGroupInfo.GetSafeStringValueOf<Employee>(x => x.Department);
            var age = newGroupInfo.GetSafeStringValueOf<Employee>(x => x.Age);

            var table = new PdfGrid(relativeWidths: new[] { 1f, 1f }) { WidthPercentage = 100 };
            table.AddSimpleRow(
                (cellData, cellProperties) =>
                {
                    cellData.Value = "Department:";
                    cellProperties.PdfFont = PdfRptFont;
                    cellProperties.PdfFontStyle = DocumentFontStyle.Bold;
                    cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                },
                (cellData, cellProperties) =>
                {
                    cellData.Value = groupName;
                    cellProperties.PdfFont = PdfRptFont;
                    cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                });
            table.AddSimpleRow(
                (cellData, cellProperties) =>
                {
                    cellData.Value = "Age:";
                    cellProperties.PdfFont = PdfRptFont;
                    cellProperties.PdfFontStyle = DocumentFontStyle.Bold;
                    cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                },
                (cellData, cellProperties) =>
                {
                    cellData.Value = age;
                    cellProperties.PdfFont = PdfRptFont;
                    cellProperties.HorizontalAlignment = HorizontalAlignment.Left;
                });
            return table.AddBorderToTable(borderColor: BaseColor.LIGHT_GRAY, spacingBefore: _groupNumber++ == 0 ? 5f : 25f);
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
                   cellData.Value = "Grouping employees by department and age";
                   cellProperties.PdfFont = PdfRptFont;
                   cellProperties.PdfFontStyle = DocumentFontStyle.Bold;
                   cellProperties.HorizontalAlignment = HorizontalAlignment.Center;
               });
            return table.AddBorderToTable();
        }
    }
}
