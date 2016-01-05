using System;
using System.Collections.Generic;
using System.Drawing;
using PdfReportSamples.Models;
using PdfRpt.Core.Contracts;
using PdfRpt.FluentInterface;

namespace PdfReportSamples.CustomHeaderFooter
{
    public class CustomHeaderFooterPdfReport
    {
        readonly CustomHeader _customHeader = new CustomHeader();
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
                footer.CustomFooter(new CustomFooter(footer.PdfFont, PdfRunDirection.LeftToRight));
            })
            .PagesHeader(header =>
            {
                header.CacheHeader(cache: true); // It's a default setting to improve the performance.
                header.CustomHeader(_customHeader);
            })
            .MainTableTemplate(template =>
            {
                template.BasicTemplate(BasicTemplate.SilverTemplate);
            })
            .MainTablePreferences(table =>
            {
                table.ColumnsWidthsType(TableColumnWidthType.Relative);
                table.MultipleColumnsPerPage(new MultipleColumnsPerPage
                {
                    ColumnsGap = 22,
                    ColumnsPerPage = 2,
                    ColumnsWidth = 250,
                    IsRightToLeft = false,
                    TopMargin = 7
                });
            })
            .MainTableDataSource(dataSource =>
            {
                var rows = new List<Task>();
                var rnd = new Random();
                for (int i = 1; i < 210; i++)
                {
                    rows.Add(new Task
                    {
                        Assignee = new User
                        {
                            Id = i,
                            Name = "user-" + i
                        },
                        IsActive = rnd.Next(0, 2) == 1,
                        Name = "task-" + i
                    });
                }
                dataSource.StronglyTypedList(rows);
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
                    column.PropertyName<Task>(x => x.Name);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(1);
                    column.Width(3);
                    column.HeaderCell("Task Name");
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<Task>(x => x.Assignee.Name); // nested property support
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(2);
                    column.Width(3);
                    column.HeaderCell("Assignee");
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<Task>(x => x.IsActive);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(3);
                    column.Width(2);
                    column.HeaderCell("Active");
                    column.ColumnItemsTemplate(template =>
                    {
                        template.Checkmark(checkmarkFillColor: Color.Green, crossSignFillColor: Color.DarkRed);
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
            .Generate(data => data.AsPdfFile(string.Format("{0}\\Pdf\\CustomHeaderFooterPdfReportSample-{1}.pdf", AppPath.ApplicationPath, Guid.NewGuid().ToString("N"))));
        }
    }
}
