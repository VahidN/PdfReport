using System;
using PdfRpt.Core.Contracts;
using PdfRpt.FluentInterface;

namespace PdfReportSamples.DataAnnotations
{
    public class DataAnnotationsPdfReport
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
                 footer.DefaultFooter(printDate: DateTime.Now.ToString("MM/dd/yyyy"));
             })
             .PagesHeader(header =>
             {
                 header.CacheHeader(cache: true); // It's a default setting to improve the performance.
                 header.DefaultHeader(defaultHeader =>
                 {
                     defaultHeader.ImagePath(System.IO.Path.Combine(AppPath.ApplicationPath, "Images\\01.png"));
                     defaultHeader.Message("new rpt.");
                     defaultHeader.RunDirection(PdfRunDirection.LeftToRight);
                 });
             })
             .MainTableTemplate(template =>
             {
                 template.BasicTemplate(BasicTemplate.ClassicTemplate);
             })
             .MainTablePreferences(table =>
             {
                 table.ColumnsWidthsType(TableColumnWidthType.FitToContent);
             })
             .MainTableDataSource(dataSource =>
             {
                 dataSource.StronglyTypedList(PersonnelDataSource.CreatePersonnelList());
             })
             .MainTableEvents(events =>
             {
                 events.DataSourceIsEmpty(message: "There is no data available to display.");
             })
             .MainTableSummarySettings(summary =>
             {
                 summary.OverallSummarySettings("Total");
                 summary.PageSummarySettings("Page Summary");
                 summary.PreviousPageSummarySettings("Pervious Page Summary");
             })
             .MainTableAdHocColumnsConventions(adHocColumns =>
             {
                 adHocColumns.ShowRowNumberColumn(true);
                 adHocColumns.RowNumberColumnCaption("#");
             })
             .Export(export =>
             {
                 export.ToExcel();
                 export.ToXml();
             })
             .Generate(data => data.AsPdfFile(string.Format("{0}\\Pdf\\DataAnnotationsSampleRpt-{1}.pdf", AppPath.ApplicationPath, Guid.NewGuid().ToString("N"))));
        }
    }
}
