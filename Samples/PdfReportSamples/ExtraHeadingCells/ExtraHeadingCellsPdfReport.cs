using System;
using PdfReportSamples.Templates;
using PdfRpt.Core.Contracts;
using PdfRpt.FluentInterface;

namespace PdfReportSamples.ExtraHeadingCells
{
    public class ExtraHeadingCellsPdfReport
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
                 fonts.Path(System.IO.Path.Combine(Environment.GetEnvironmentVariable("SystemRoot"), "fonts\\verdana.ttf"),
                            System.IO.Path.Combine(AppPath.ApplicationPath, "fonts\\irsans.ttf"));
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
                     defaultHeader.Message("Our new Rpt");
                 });
             })
             .MainTableTemplate(template =>
             {
                 template.CustomTemplate(new TransparentTemplate());
             })
             .MainTablePreferences(table =>
             {
                 table.ColumnsWidthsType(TableColumnWidthType.FitToContent);
             })
             .MainTableDataSource(dataSource =>
             {
                 dataSource.Crosstab(DataGenerator.ContactsList(), topFieldsAreVariableInEachRow: true);
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
                     column.HeaderCell("#");//------- Main Header Row
                     column.AddHeadingCell("Contacts List", mergeHeaderCell: true);//------- Extra Header Row - 1
                     column.AddHeadingCell(string.Empty, mergeHeaderCell: false);//------- Extra Header Row - 2
                 });

                 columns.AddColumn(column =>
                 {
                     column.PropertyName("Id");
                     column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                     column.IsVisible(true);
                     column.Order(1);
                     column.HeaderCell("Id");//------- Main Header Row
                     column.AddHeadingCell(string.Empty, mergeHeaderCell: true);//------- Extra Header Row - 1
                     column.AddHeadingCell("Person", mergeHeaderCell: true);//------- Extra Header Row - 2
                 });

                 columns.AddColumn(column =>
                 {
                     column.PropertyName("FirstName");
                     column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                     column.IsVisible(true);
                     column.Order(2);
                     column.HeaderCell("Name");//------- Main Header Row
                     column.AddHeadingCell(string.Empty, mergeHeaderCell: true);//------- Extra Header Row - 1
                     column.AddHeadingCell(string.Empty, mergeHeaderCell: true);//------- Extra Header Row - 2
                 });

                 columns.AddColumn(column =>
                 {
                     column.PropertyName("LastName");
                     column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                     column.IsVisible(true);
                     column.Order(3);
                     column.HeaderCell("Last Name");//------- Main Header Row
                     column.AddHeadingCell(string.Empty, mergeHeaderCell: true);//------- Extra Header Row - 1
                     column.AddHeadingCell(string.Empty, mergeHeaderCell: false);//------- Extra Header Row - 2
                 });

                 columns.AddColumn(column =>
                 {
                     column.PropertyName("Home");
                     column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                     column.IsVisible(true);
                     column.Order(4);
                     column.HeaderCell("Home");//------- Main Header Row
                     column.AddHeadingCell(string.Empty, mergeHeaderCell: true);//------- Extra Header Row - 1
                     column.AddHeadingCell("Phones", mergeHeaderCell: true);//------- Extra Header Row - 2
                 });

                 columns.AddColumn(column =>
                 {
                     column.PropertyName("Office");
                     column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                     column.IsVisible(true);
                     column.Order(5);
                     column.HeaderCell("Office");//------- Main Header Row
                     column.AddHeadingCell(string.Empty, mergeHeaderCell: true);//------- Extra Header Row - 1
                     column.AddHeadingCell(string.Empty, mergeHeaderCell: true);//------- Extra Header Row - 2
                 });

                 columns.AddColumn(column =>
                 {
                     column.PropertyName("Cell");
                     column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                     column.IsVisible(true);
                     column.Order(6);
                     column.HeaderCell("Cell");//------- Main Header Row
                     column.AddHeadingCell(string.Empty, mergeHeaderCell: true);//------- Extra Header Row - 1
                     column.AddHeadingCell(string.Empty, mergeHeaderCell: true);//------- Extra Header Row - 2
                 });

                 columns.AddColumn(column =>
                 {
                     column.PropertyName("Fax");
                     column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                     column.IsVisible(true);
                     column.Order(7);
                     column.HeaderCell("Fax");//------- Main Header Row
                     column.AddHeadingCell(string.Empty, mergeHeaderCell: true);//------- Extra Header Row - 1
                     column.AddHeadingCell(string.Empty, mergeHeaderCell: true);//------- Extra Header Row - 2
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
             .Generate(data => data.AsPdfFile(string.Format("{0}\\Pdf\\ExtraHeadingCellsSample-{1}.pdf", AppPath.ApplicationPath, Guid.NewGuid().ToString("N"))));
        }
    }
}
