using System;
using iTextSharp.text;
using PdfRpt.Core.Contracts;
using PdfRpt.Core.Helper;
using PdfRpt.FluentInterface;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.html;

namespace PdfReportSamples.HtmlCellTemplate
{
    public class HtmlCellTemplatePdfReport
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
                    CompressionLevel = CompressionLevel.BestCompression,
                    EnableCompression = true
                });
            })
             .DefaultFonts(fonts =>
             {
                 fonts.Path(System.IO.Path.Combine(Environment.GetEnvironmentVariable("SystemRoot"), "fonts\\arial.ttf"),
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
                 template.BasicTemplate(BasicTemplate.SnowyPineTemplate);
             })
             .MainTablePreferences(table =>
             {
                 table.ColumnsWidthsType(TableColumnWidthType.Relative);
                 table.MultipleColumnsPerPage(new MultipleColumnsPerPage
                 {
                     ColumnsGap = 20,
                     ColumnsPerPage = 2,
                     ColumnsWidth = 250,
                     IsRightToLeft = false,
                     TopMargin = 7
                 });
             })
             .MainTableDataSource(dataSource =>
             {
                 var table = new System.Data.DataTable("SalaryList");
                 table.Columns.Add("User", typeof(string));
                 table.Columns.Add("Month", typeof(int));
                 table.Columns.Add("Salary", typeof(decimal));
                 table.Columns.Add("Photo", typeof(string));

                 var rnd = new Random();
                 for (int i = 0; i < 100; i++)
                 {
                     var photo = System.IO.Path.Combine(AppPath.ApplicationPath, "Images\\" + rnd.Next(1, 5).ToString("00") + ".png");
                     table.Rows.Add("User " + i, rnd.Next(1, 12), rnd.Next(400, 2000), photo);
                 }

                 dataSource.DataTable(table);
             })
             .MainTableEvents(events =>
             {
                 events.DataSourceIsEmpty(message: "There is no data available to display.");
                 events.CellCreated(args =>
                 {
                     //change the background color of the cell based on the value
                     if (args.RowType == RowType.DataTableRow && args.Cell.RowData.Value != null && args.Cell.RowData.Value is decimal)
                     {
                         if ((decimal)args.Cell.RowData.Value <= 1000)
                             args.Cell.BasicProperties.BackgroundColor = BaseColor.CYAN;
                     }
                 });
             })
             .MainTableSummarySettings(summary =>
             {
                 summary.OverallSummarySettings("Summary");
                 summary.PreviousPageSummarySettings("Previous Col. Summary");
                 summary.PageSummarySettings("Page Summary");
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
                     column.PropertyName("User");
                     column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                     column.IsVisible(true);
                     column.Order(1);
                     column.Width(3);
                     column.HeaderCell("User");
                     column.CalculatedField(list =>
                         {
                             var user = list.GetSafeStringValueOf("User");
                             var photo = list.GetSafeStringValueOf("Photo");
                             var image = string.Format("<img src='{0}' />", photo);
                             return
                                    @"<table style='width: 100%; font-size:9pt;'>
												<tr>
													<td align='center'>" + user + @"</td>
												</tr>
												<tr>
													<td align='center'>" + image + @"</td>
												</tr>
									   </table>
									 ";
                         });
                     column.ColumnItemsTemplate(template =>
                         {
                             template.XHtml();
                         });
                 });

                 columns.AddColumn(column =>
                 {
                     column.PropertyName("Month");
                     column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                     column.IsVisible(true);
                     column.Order(2);
                     column.Width(2);
                     column.HeaderCell("Month");
                     column.ColumnItemsTemplate(template =>
                     {
                         template.TextBlock();
                         template.ConditionalFormatFormula(list =>
                         {
                             var cellValue = int.Parse(list.GetSafeStringValueOf("Month", nullValue: "0"));
                             if (cellValue == 7)
                             {
                                 return new CellBasicProperties
                                 {
                                     PdfFontStyle = DocumentFontStyle.Bold | DocumentFontStyle.Underline,
                                     FontColor = new BaseColor(System.Drawing.Color.Brown.ToArgb()),
                                     BackgroundColor = new BaseColor(System.Drawing.Color.Yellow.ToArgb())
                                 };
                             }
                             return new CellBasicProperties { PdfFontStyle = DocumentFontStyle.Normal };
                         });
                     });
                 });

                 columns.AddColumn(column =>
                 {
                     column.PropertyName("Salary");
                     column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                     column.IsVisible(true);
                     column.Order(3);
                     column.Width(2);
                     column.HeaderCell("Salary");
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
             .Export(export =>
             {
                 export.ToXml();
                 export.ToExcel();
             })
             .Generate(data => data.AsPdfFile(string.Format("{0}\\Pdf\\HtmlCellTemplateSample-{1}.pdf", AppPath.ApplicationPath, Guid.NewGuid().ToString("N"))));
        }
    }
}
