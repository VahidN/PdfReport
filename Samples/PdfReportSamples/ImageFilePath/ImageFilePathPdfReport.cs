using System;
using System.Collections.Generic;
using iTextSharp.text.pdf;
using PdfReportSamples.Models;
using PdfRpt.Core.Contracts;
using PdfRpt.FluentInterface;

namespace PdfReportSamples.ImageFilePath
{
    public class ImageFilePathPdfReport
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
            })
            .MainTableDataSource(dataSource =>
            {
                var listOfRows = new List<ImageRecord>
                                             {
                                                 new ImageRecord
                                                     {
                                                         Id=1,
                                                         ImagePath =  System.IO.Path.Combine(AppPath.ApplicationPath , "Images\\01.png"),
                                                         Name = "Rnd"
                                                     },
                                                 new ImageRecord
                                                     {
                                                         Id=2,
                                                         ImagePath =  System.IO.Path.Combine(AppPath.ApplicationPath , "Images\\02.png"),
                                                         Name = "Bug"
                                                     },
                                                 new ImageRecord
                                                     {
                                                         Id=3,
                                                         ImagePath =  System.IO.Path.Combine(AppPath.ApplicationPath , "Images\\03.png"),
                                                         Name = "Stuff"
                                                     },
                                                 new ImageRecord
                                                     {
                                                         Id=4,
                                                         ImagePath =  System.IO.Path.Combine(AppPath.ApplicationPath , "Images\\04.png"),
                                                         Name = "Sun"
                                                     }
                                             };
                dataSource.StronglyTypedList(listOfRows);
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
                    column.PropertyName<ImageRecord>(x => x.Id);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(1);
                    column.Width(3);
                    column.HeaderCell("Id");
                    column.ColumnItemsTemplate(t => t.Barcode(new Barcode39()));
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<ImageRecord>(x => x.ImagePath);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(2);
                    column.Width(3);
                    column.HeaderCell("Image");
                    column.ColumnItemsTemplate(t => t.ImageFilePath(defaultImageFilePath: string.Empty, fitImages: false));
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<ImageRecord>(x => x.Name);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(3);
                    column.Width(2);
                    column.HeaderCell("Name");
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
            .Generate(data => data.AsPdfFile(string.Format("{0}\\Pdf\\RptImageFilePathSample-{1}.pdf", AppPath.ApplicationPath, Guid.NewGuid().ToString("N"))));
        }
    }
}
