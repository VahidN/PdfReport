using System;
using System.Linq;
using System.Collections.Generic;
using PdfRpt.Core.Contracts;
using PdfRpt.FluentInterface;
using PdfReportSamples.Models;

namespace PdfReportSamples.WrapGroupsInColumns
{
    public class WrapGroupsInColumnsPdfReport
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
                 header.CustomHeader(new WGHeaders { PdfRptFont = header.PdfFont });
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
                     ShowOneGroupPerPage = false
                 });
                 table.MultipleColumnsPerPage(new MultipleColumnsPerPage
                 {
                     ColumnsGap = 7,
                     ColumnsPerPage = 3,
                     ColumnsWidth = 170,
                     IsRightToLeft = false,
                     TopMargin = 7
                 });
             })
             .MainTableDataSource(dataSource =>
             {
                 var listOfRows = new List<Employee>();
                 var rnd = new Random();
                 for (int i = 0; i < 100; i++)
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
                     column.Width(1);
                     column.HeaderCell("#");
                 });

                 columns.AddColumn(column =>
                 {
                     column.PropertyName<Employee>(x => x.Id);
                     column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                     column.IsVisible(true);
                     column.Order(1);
                     column.Width(1);
                     column.HeaderCell("Id");
                 });

                 columns.AddColumn(column =>
                 {
                     column.PropertyName<Employee>(x => x.Name);
                     column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                     column.IsVisible(true);
                     column.Order(2);
                     column.Width(2);
                     column.HeaderCell("Name");
                 });

                 columns.AddColumn(column =>
                 {
                     column.PropertyName<Employee>(x => x.Department);
                     column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                     column.Order(3);
                     column.Width(2);
                     column.HeaderCell("Department");
                     column.Group(
                     (val1, val2) =>
                     {
                         return val1.ToString() == val2.ToString();
                     });
                 });

                 columns.AddColumn(column =>
                 {
                     column.PropertyName<Employee>(x => x.Salary);
                     column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                     column.IsVisible(true);
                     column.Order(4);
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

                 columns.AddColumn(column =>
                 {
                     column.PropertyName<Employee>(x => x.Age);
                     column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                     column.Order(5);
                     column.Width(2);
                     column.HeaderCell("Age");
                     column.Group(
                     (val1, val2) =>
                     {
                         return (int)val1 == (int)val2;
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
             })
             .Generate(data => data.AsPdfFile(string.Format("{0}\\Pdf\\RptWrapGroupsInColumns-{1}.pdf", AppPath.ApplicationPath, Guid.NewGuid().ToString("N"))));
        }
    }
}
