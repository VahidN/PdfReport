using System;
using PdfRpt.Core.Contracts;
using PdfRpt.FluentInterface;

namespace PdfReportSamples.ExpensesCrosstab
{
    public class ExpensesCrosstabPdfReport
    {
        public IPdfReportData CreatePdfReport()
        {
            return new PdfReport().DocumentPreferences(doc =>
            {
                doc.RunDirection(PdfRunDirection.LeftToRight);
                doc.Orientation(PageOrientation.Portrait);
                doc.PageSize(PdfPageSize.A4);
                doc.DocumentMetadata(new DocumentMetadata { Author = "Vahid", Application = "PdfRpt", Keywords = "Test", Subject = "Test Rpt", Title = "Test" });
                doc.Compression(new CompressionSettings
                {
                    EnableCompression = true,
                    EnableFullCompression = true
                });
            })
             .DefaultFonts(fonts =>
             {
                 fonts.Path(System.IO.Path.Combine(Environment.GetEnvironmentVariable("SystemRoot"), "fonts\\tahoma.ttf"),
                            System.IO.Path.Combine(Environment.GetEnvironmentVariable("SystemRoot"), "fonts\\verdana.ttf"));
                 fonts.Size(9);
                 fonts.Color(System.Drawing.Color.Black);
             })
             .PagesFooter(footer =>
             {
                 footer.DefaultFooter(DateTime.Now.ToString("MM/dd/yyyy"));
             })
             .PagesHeader(header =>
             {
                 header.CacheHeader(cache: true); // It's a default setting to improve the performance.
                 header.DefaultHeader(defaultHeader =>
                 {
                     defaultHeader.RunDirection(PdfRunDirection.LeftToRight);
                     defaultHeader.ImagePath(System.IO.Path.Combine(AppPath.ApplicationPath, "Images\\01.png"));
                     defaultHeader.Message("Our new rpt.");
                 });
             })
             .MainTableTemplate(template =>
             {
                 template.BasicTemplate(BasicTemplate.ClassicTemplate);
             })
             .MainTableSummarySettings(summarySettings =>
             {
                 summarySettings.OverallSummarySettings("Summary");
                 summarySettings.PreviousPageSummarySettings("Previous Page Summary");
                 summarySettings.PageSummarySettings("Page Summary");
             })
             .MainTablePreferences(table =>
             {
                 table.ColumnsWidthsType(TableColumnWidthType.Relative);
             })
             .MainTableDataSource(dataSource =>
             {
                 dataSource.AnonymousTypeList(ExpenseDataSource.ExpensesCrossTabList());
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
                     column.Width(1);
                     column.HeaderCell("#");
                 });

                 columns.AddColumn(column =>
                 {
                     column.PropertyName("Year");
                     column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                     column.IsVisible(true);
                     column.Order(1);
                     column.Width(2);
                     column.HeaderCell("Year");
                 });

                 columns.AddColumn(column =>
                 {
                     column.PropertyName("Month");
                     column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                     column.IsVisible(true);
                     column.Order(2);
                     column.Width(3);
                     column.HeaderCell("Month");
                 });

                 columns.AddColumn(column =>
                 {
                     column.PropertyName("ComputerDepartment");
                     column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                     column.IsVisible(true);
                     column.Order(3);
                     column.Width(3);
                     column.HeaderCell("Computer Department");
                     column.ColumnItemsTemplate(template =>
                     {
                         template.TextBlock();
                         template.DisplayFormatFormula(obj => obj == null || string.IsNullOrEmpty(obj.ToString())
                                                            ? string.Empty : string.Format("{0:n0}", obj));
                     });
                     column.AggregateFunction(aggregateFunction =>
                     {
                         aggregateFunction.NumericAggregateFunction(AggregateFunction.Sum);
                         aggregateFunction.DisplayFormatFormula(obj => obj == null || string.IsNullOrEmpty(obj.ToString())
                                                            ? string.Empty : string.Format("{0:n0}", obj));
                     });
                 });

                 columns.AddColumn(column =>
                 {
                     column.PropertyName("MathDepartment");
                     column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                     column.IsVisible(true);
                     column.Order(4);
                     column.Width(3);
                     column.HeaderCell("Math Department");
                     column.ColumnItemsTemplate(template =>
                     {
                         template.TextBlock();
                         template.DisplayFormatFormula(obj => obj == null || string.IsNullOrEmpty(obj.ToString())
                                                            ? string.Empty : string.Format("{0:n0}", obj));
                     });
                     column.AggregateFunction(aggregateFunction =>
                     {
                         aggregateFunction.NumericAggregateFunction(AggregateFunction.Sum);
                         aggregateFunction.DisplayFormatFormula(obj => obj == null || string.IsNullOrEmpty(obj.ToString())
                                                            ? string.Empty : string.Format("{0:n0}", obj));
                     });
                 });

                 columns.AddColumn(column =>
                 {
                     column.PropertyName("PhysicsDepartment");
                     column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                     column.IsVisible(true);
                     column.Order(5);
                     column.Width(3);
                     column.HeaderCell("Physics Department");
                     column.ColumnItemsTemplate(template =>
                     {
                         template.TextBlock();
                         template.DisplayFormatFormula(obj => obj == null || string.IsNullOrEmpty(obj.ToString())
                                                            ? string.Empty : string.Format("{0:n0}", obj));
                     });
                     column.AggregateFunction(aggregateFunction =>
                     {
                         aggregateFunction.NumericAggregateFunction(AggregateFunction.Sum);
                         aggregateFunction.DisplayFormatFormula(obj => obj == null || string.IsNullOrEmpty(obj.ToString())
                                                            ? string.Empty : string.Format("{0:n0}", obj));
                     });
                 });

                 columns.AddColumn(column =>
                 {
                     column.PropertyName("Total");
                     column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                     column.IsVisible(true);
                     column.Order(6);
                     column.Width(3);
                     column.HeaderCell("Total");
                     column.ColumnItemsTemplate(template =>
                     {
                         template.TextBlock();
                         template.DisplayFormatFormula(obj => obj == null || string.IsNullOrEmpty(obj.ToString())
                                                            ? string.Empty : string.Format("{0:n0}", obj));
                     });
                     column.AggregateFunction(aggregateFunction =>
                     {
                         aggregateFunction.NumericAggregateFunction(AggregateFunction.Sum);
                         aggregateFunction.DisplayFormatFormula(obj => obj == null || string.IsNullOrEmpty(obj.ToString())
                                                            ? string.Empty : string.Format("{0:n0}", obj));
                     });
                 });
             })
             .MainTableEvents(events =>
             {
                 events.DataSourceIsEmpty(message: "There is no data available to display.");
             })
             .Export(export =>
             {
                 export.ToExcel();
                 export.ToXml();
             })
             .Generate(data => data.AsPdfFile(string.Format("{0}\\Pdf\\ExpensesCrosstabSample-{1}.pdf", AppPath.ApplicationPath, Guid.NewGuid().ToString("N"))));
        }
    }
}
