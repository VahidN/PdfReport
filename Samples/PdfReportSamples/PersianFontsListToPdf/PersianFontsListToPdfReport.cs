using System;
using System.Collections.Generic;
using System.Linq;
using iTextSharp.text;
using PdfReportSamples.Models;
using PdfRpt.Core.Contracts;
using PdfRpt.FluentInterface;

namespace PdfReportSamples.PersianFontsListToPdf
{
    public class PersianFontsListToPdfReport
    {
        public IPdfReportData CreatePdfReport()
        {
            return new PdfReport().DocumentPreferences(doc =>
            {
                doc.RunDirection(PdfRunDirection.RightToLeft);
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
                 fonts.Path(System.IO.Path.Combine(AppPath.ApplicationPath, "fonts\\irsans.ttf"),
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
                     defaultHeader.ImagePath(System.IO.Path.Combine(AppPath.ApplicationPath, "Images\\01.png"));
                     defaultHeader.Message("Installed 'B ' fonts list");
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
                 var listOfRows = new List<FontSample>();

                 // Register all the fonts of a directory
                 FontFactory.RegisterDirectory(System.IO.Path.Combine(Environment.GetEnvironmentVariable("SystemRoot"), "fonts"));

                 // Enumerate the current set of system fonts
                 foreach (var fontName in FontFactory.RegisteredFonts.ToList())
                 {
                     if (!fontName.ToLowerInvariant().StartsWith("b ")) continue;

                     listOfRows.Add(new FontSample
                     {
                         FontName = fontName,
                         EnglishTextSample = "Sample Text 1,2,3",
                         PersianTextSample = "نمونه متن 1,2,3"
                     });
                 }
                 dataSource.StronglyTypedList<FontSample>(listOfRows);
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
                     column.PropertyName<FontSample>(x => x.FontName);
                     column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                     column.IsVisible(true);
                     column.Order(1);
                     column.Width(2);
                     column.HeaderCell("Font name");
                 });

                 columns.AddColumn(column =>
                 {
                     column.PropertyName<FontSample>(x => x.EnglishTextSample);
                     column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                     column.IsVisible(true);
                     column.Order(2);
                     column.Width(3);
                     column.HeaderCell("Sample Text");
                     column.ColumnItemsTemplate(t => t.CustomTemplate(new FontsListCellTemplate(20)));
                 });

                 columns.AddColumn(column =>
                 {
                     column.PropertyName<FontSample>(x => x.PersianTextSample);
                     column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                     column.IsVisible(true);
                     column.Order(3);
                     column.Width(3);
                     column.HeaderCell("نمونه متن");
                     column.ColumnItemsTemplate(t => t.CustomTemplate(new FontsListCellTemplate(20)));
                 });
             })
             .MainTableEvents(events =>
             {
                 events.DataSourceIsEmpty(message: "There is no data available to display.");
             })
             .Generate(data => data.AsPdfFile(string.Format("{0}\\Pdf\\FontsListToPdfSample-{1}.pdf", AppPath.ApplicationPath, Guid.NewGuid().ToString("N"))));
        }
    }
}
