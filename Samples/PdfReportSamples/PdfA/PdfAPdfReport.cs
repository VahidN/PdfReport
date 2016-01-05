using System;
using PdfReportSamples.Templates;
using PdfRpt.Core.Contracts;
using PdfRpt.FluentInterface;

namespace PdfReportSamples.PdfA
{
    // PDF/A:
    // Encryption is not allowed.
    // Embedded files are not allowed.
    // All fonts must be embedded.
    // Transparent images are forbidden.

    public class PdfAPdfReport
    {
        public IPdfReportData CreatePdfReport()
        {
            return new PdfReport().DocumentPreferences(doc =>
            {
                doc.RunDirection(PdfRunDirection.LeftToRight);
                doc.Orientation(PageOrientation.Portrait);
                doc.PageSize(PdfPageSize.A4);
                doc.DocumentMetadata(new DocumentMetadata { Author = "Vahid", Application = "PdfRpt", Keywords = "Test", Subject = "Test Rpt", Title = "Test" });
                doc.ConformanceLevel(PdfXConformance.PDFA1B);
            })
            .DefaultFonts(fonts =>
            {
                fonts.Path(System.IO.Path.Combine(Environment.GetEnvironmentVariable("SystemRoot"), "fonts\\verdana.ttf"),
                           System.IO.Path.Combine(AppPath.ApplicationPath, "fonts\\irsans.ttf"));
                fonts.Size(9);
                fonts.Color(System.Drawing.Color.Black);
            })
            .PagesHeader(header =>
            {
                header.CacheHeader(cache: true); // It's a default setting to improve the performance.
                header.DefaultHeader(h =>
                {
                    h.Message("Our new Rpt");
                    h.ImagePath(System.IO.Path.Combine(AppPath.ApplicationPath, "Images\\05.png"));
                });
            })
            .PagesFooter(footer =>
            {
                footer.DefaultFooter(DateTime.Now.ToString("MM/dd/yyyy"));
            })
            .MainTableTemplate(t => t.CustomTemplate(new TransparentTemplate()))
            .MainTablePreferences(table =>
            {
                table.ColumnsWidthsType(TableColumnWidthType.FitToContent);
            })
            .MainTableDataSource(dataSource =>
            {
                dataSource.Crosstab(TransactionsDataSource.PivotTransactionsList());
            })
            .MainTableSummarySettings(summarySettings =>
            {
                summarySettings.OverallSummarySettings("Grand Total");
                summarySettings.PreviousPageSummarySettings("PerviousPage Summary");
                summarySettings.PageSummarySettings("Page Summary");
            })
            .MainTableColumns(columns =>
            {
                columns.AddColumn(column =>
                {
                    column.PropertyName("rowNo");
                    column.IsRowNumber(true);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(0);
                    column.HeaderCell("#"); //------- Main Header Row
                    column.AddHeadingCell(string.Empty); //------- Extra Header Row - 1
                });

                addColumn(columns, propertyName: "SalesPerson", caption: "Sales Person", headingCaption: string.Empty, mergeHeaderCell: false, order: 1, showTotal: false);

                addColumn(columns, "Corolla SalePrice", "SalePrice", "Corolla", true, 2);
                addColumn(columns, "Corolla Count", "Count", string.Empty, false, 3);

                addColumn(columns, "Camry SalePrice", "SalePrice", "Camry", true, 4);
                addColumn(columns, "Camry Count", "Count", string.Empty, false, 5);

                addColumn(columns, "Prius SalePrice", "SalePrice", "Prius", true, 6);
                addColumn(columns, "Prius Count", "Count", string.Empty, false, 7);

                addColumn(columns, "SalePrice", "SalePrice", "Total", true, 8);
                addColumn(columns, "Count", "Count", string.Empty, false, 9);

            })
            .MainTableEvents(events =>
            {
                events.DataSourceIsEmpty(message: "There is no data available to display.");
            })
            .Generate(data => data.AsPdfFile(string.Format("{0}\\Pdf\\PdfASampleData-{1}.pdf", AppPath.ApplicationPath, Guid.NewGuid().ToString("N"))), debugMode: true);
        }

        private static void addColumn(MainTableColumnsBuilder columns, string propertyName, string caption, string headingCaption, bool mergeHeaderCell, int order, bool showTotal = true)
        {
            columns.AddColumn(column =>
            {
                column.PropertyName(propertyName);
                column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                column.IsVisible(true);
                column.Order(order);
                column.HeaderCell(caption); //------- Main Header Row
                column.AddHeadingCell(headingCaption, mergeHeaderCell: mergeHeaderCell); //------- Extra Header Row 
                if (showTotal)
                {
                    column.AggregateFunction(aggregateFunction =>
                    {
                        aggregateFunction.NumericAggregateFunction(AggregateFunction.Sum);
                        aggregateFunction.DisplayFormatFormula(obj => obj == null || string.IsNullOrEmpty(obj.ToString())
                                                            ? string.Empty : string.Format("{0:n0}", obj));
                    });
                }
                column.ColumnItemsTemplate(template =>
                {
                    template.TextBlock();
                    template.DisplayFormatFormula(obj => obj == null || string.IsNullOrEmpty(obj.ToString())
                                                            ? string.Empty : string.Format("{0:n0}", obj));
                });
            });
        }
    }
}
