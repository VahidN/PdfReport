using System;
using System.Collections.Generic;
using System.Linq;
using PdfReportSamples.Models;
using PdfRpt.Core.Contracts;
using PdfRpt.FluentInterface;
using iTextSharp.text;
using PdfRpt.Core.Helper;
using PdfRpt.Core.Helper.HtmlToPdf;

namespace PdfReportSamples.Grouping
{
    public class GroupingPdfReport
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
                 header.CustomHeader(new GroupingHeaders { PdfRptFont = header.PdfFont });
             })
             .MainTableTemplate(template =>
             {
                 template.BasicTemplate(BasicTemplate.SilverTemplate);
             })
             .MainTablePreferences(table =>
             {
                 table.ColumnsWidthsType(TableColumnWidthType.Relative);
                 table.GroupsPreferences(new GroupsPreferences
                 {
                     GroupType = GroupType.HideGroupingColumns,
                     RepeatHeaderRowPerGroup = true,
                     ShowOneGroupPerPage = false,
                     SpacingBeforeAllGroupsSummary = 5f,
                     NewGroupAvailableSpacingThreshold = 150,
                     SpacingAfterAllGroupsSummary = 5f
                 });
                 table.SpacingAfter(4f);
             })
             .MainTableDataSource(dataSource =>
             {
                 var listOfRows = new List<Employee>();
                 var rnd = new Random();
                 for (int i = 0; i < 170; i++)
                 {
                     listOfRows.Add(
                         new Employee
                         {
                             Age = rnd.Next(25, 35),
                             Id = i + 1000,
                             Salary = rnd.Next(1000, 4000),
                             Name = "Employee " + i,
                             Department = "Department " + rnd.Next(1, 3)
                         });
                 }

                 listOfRows = listOfRows.OrderBy(x => x.Department).ThenBy(x => x.Age).ToList();
                 dataSource.StronglyTypedList(listOfRows);
             })
             .MainTableSummarySettings(summarySettings =>
             {
                 summarySettings.PreviousPageSummarySettings("Cont.");
                 summarySettings.OverallSummarySettings("Sum");
                 summarySettings.AllGroupsSummarySettings("Groups Sum");
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
                     column.Width(20);
                     column.HeaderCell("#");
                 });

                 columns.AddColumn(column =>
                 {
                     column.PropertyName<Employee>(x => x.Department);
                     column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                     column.Order(1);
                     column.Width(20);
                     column.HeaderCell("Department");
                     column.Group(
                     (val1, val2) =>
                     {
                         return val1.ToString() == val2.ToString();
                     });
                 });

                 columns.AddColumn(column =>
                 {
                     column.PropertyName<Employee>(x => x.Age);
                     column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                     column.Order(2);
                     column.Width(20);
                     column.HeaderCell("Age");
                     column.Group(
                     (val1, val2) =>
                     {
                         return (int)val1 == (int)val2;
                     });
                 });

                 columns.AddColumn(column =>
                 {
                     column.PropertyName<Employee>(x => x.Id);
                     column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                     column.IsVisible(true);
                     column.Order(3);
                     column.Width(20);
                     column.HeaderCell("Id");
                 });

                 columns.AddColumn(column =>
                 {
                     column.PropertyName<Employee>(x => x.Name);
                     column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                     column.IsVisible(true);
                     column.Order(4);
                     column.Width(20);
                     column.HeaderCell("Name");
                 });

                 columns.AddColumn(column =>
                 {
                     column.PropertyName<Employee>(x => x.Salary);
                     column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                     column.IsVisible(true);
                     column.Order(5);
                     column.Width(20);
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
             .MainTableEvents(events =>
             {
                 events.DataSourceIsEmpty(message: "There is no data available to display.");
                 events.GroupAdded(args =>
                     {
                         //args.PdfDoc.Add(new Phrase("\nGroup added event."));

                         /*var data = args.ColumnCellsSummaryData
                             .Where(data => data.CellData.PropertyName.Equals("propertyName")
                                    && data.GroupNumber == 1);*/

                         var salary = args.LastOverallAggregateValueOf<Employee>(x => x.Salary);
                         var table = new PdfGrid(1)
                         {
                             RunDirection = (int)PdfRunDirection.LeftToRight,
                             WidthPercentage = args.PageSetup.MainTablePreferences.WidthPercentage
                         };
                         var htmlCell = new XmlWorkerHelper
                         {
                             // the registered fonts (DefaultFonts section) should be specified here
                             Html = string.Format(@"<br/><span style='font-size:9pt;font-family:verdana;'>
                                                    <b>Group <i>added</i> event.</b>
                                                    Total Salary: {0}</span>", salary),
                             RunDirection = PdfRunDirection.LeftToRight,
                             CssFilesPath = null, // optional
                             ImagesPath = null, // optional
                             InlineCss = null, // optional
                             DefaultFont = args.PdfFont.Fonts[1] // verdana
                         }.RenderHtml();
                         htmlCell.Border = 0;
                         table.AddCell(htmlCell);
                         table.SpacingBefore = args.PageSetup.MainTablePreferences.SpacingBefore;

                         args.PdfDoc.Add(table);
                     });
             })
             .Export(export =>
             {
                 export.ToExcel();
             })
             .Generate(data => data.AsPdfFile(string.Format("{0}\\Pdf\\RptGroupingSample-{1}.pdf", AppPath.ApplicationPath, Guid.NewGuid().ToString("N"))));
        }
    }
}
