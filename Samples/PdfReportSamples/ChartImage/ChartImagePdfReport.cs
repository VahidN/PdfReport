using System;
using System.Collections.Generic;
using PdfReportSamples.Models;
using PdfRpt.Core.Contracts;
using PdfRpt.Core.Helper;
using PdfRpt.FluentInterface;

namespace PdfReportSamples.ChartImage
{
    public class ChartImagePdfReport
    {
        public IPdfReportData CreatePdfReport()
        {
            var chart = new MSChartHelper
                {
                    AxisXTitle = "User",
                    AxisYTitle = "Balance",
                    ChartTitle = "Users Balance",
                    AxisTitleFont = new System.Drawing.Font("Tahoma", 12f),
                    LabelStyleFont = new System.Drawing.Font("Tahoma", 10f),
                    ChartTitleFont = new System.Drawing.Font("Arial", 16f, System.Drawing.FontStyle.Bold),
                    LegendsFont = new System.Drawing.Font("Tahoma", 12f)
                };

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
                 fonts.Path(string.Format("{0}\\fonts\\tahoma.ttf", Environment.GetEnvironmentVariable("SystemRoot")),
                            string.Format("{0}\\fonts\\verdana.ttf", Environment.GetEnvironmentVariable("SystemRoot")));
                 fonts.Size(9);
                 fonts.Color(System.Drawing.Color.Black);
             })
             .PagesFooter(footer =>
             {
                 footer.DefaultFooter(printDate: DateTime.Now.ToString("MM/dd/yyyy"));
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
             .MainTablePreferences(table =>
             {
                 table.ColumnsWidthsType(TableColumnWidthType.Relative);
             })
             .MainTableDataSource(dataSource =>
             {
                 var listOfRows = new List<User>();
                 for (var i = 0; i < 7; i++)
                 {
                     listOfRows.Add(new User { Id = i, LastName = "Last Name " + i, Name = "Name " + i, Balance = (i * 50) + 1000 });
                 }
                 dataSource.StronglyTypedList(listOfRows);
             })
             .MainTableEvents(events =>
             {
                 events.DataSourceIsEmpty(message: "There is no data available to display.");
                 events.DocumentOpened(args =>
                 {
                     chart.ChartInit(width: (int)args.PdfWriter.PageSize.Width - 100, height: 250);
                 });
                 events.RowAdded(args =>
                 {
                     if (args.RowType == RowType.DataTableRow)
                     {
                         var name = args.TableRowData.GetValueOf<User>(x => x.Name);
                         if (name == null) return;

                         var balance = args.TableRowData.GetValueOf<User>(x => x.Balance);
                         if (balance == null) return;

                         chart.AddXY(name, balance);
                     }
                 });
                 events.DocumentClosing(args =>
                 {
                     chart.AddChartToPage(args.PdfDoc);
                     chart.FreeResources();
                 });
             })
             .MainTableSummarySettings(summarySettings =>
             {
                 summarySettings.OverallSummarySettings("Summary");
                 summarySettings.PreviousPageSummarySettings("Previous Page Summary");
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
                     column.Width(1);
                     column.HeaderCell("#");
                 });

                 columns.AddColumn(column =>
                 {
                     column.PropertyName<User>(x => x.Id);
                     column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                     column.IsVisible(true);
                     column.Order(1);
                     column.Width(2);
                     column.HeaderCell("Id");
                 });

                 columns.AddColumn(column =>
                 {
                     column.PropertyName<User>(x => x.Name);
                     column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                     column.IsVisible(true);
                     column.Order(2);
                     column.Width(2);
                     column.HeaderCell("Name");
                 });

                 columns.AddColumn(column =>
                 {
                     column.PropertyName<User>(x => x.LastName);
                     column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                     column.IsVisible(true);
                     column.Order(3);
                     column.Width(3);
                     column.HeaderCell("Last Name");
                 });

                 columns.AddColumn(column =>
                 {
                     column.PropertyName<User>(x => x.Balance);
                     column.HeaderCell("Balance");
                     column.ColumnItemsTemplate(template =>
                     {
                         template.TextBlock();
                         template.DisplayFormatFormula(obj => obj == null || string.IsNullOrEmpty(obj.ToString())
                                                            ? string.Empty : string.Format("{0:n0}", obj));
                     });
                     column.Width(2);
                     column.AggregateFunction(aggregateFunction =>
                     {
                         aggregateFunction.NumericAggregateFunction(AggregateFunction.Sum);
                         aggregateFunction.DisplayFormatFormula(obj => obj == null || string.IsNullOrEmpty(obj.ToString())
                                                            ? string.Empty : string.Format("{0:n0}", obj));
                     });
                     column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                     column.IsVisible(true);
                     column.Order(4);
                 });
             })
             .Generate(data => data.AsPdfFile(string.Format("{0}\\Pdf\\RptChartSample-{1}.pdf", AppPath.ApplicationPath, Guid.NewGuid().ToString("N"))));
        }
    }
}
