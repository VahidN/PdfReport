using System;
using PdfRpt;
using PdfRpt.Core.Contracts;
using PdfRpt.FluentInterface;

namespace PdfReportSamples.AdHocColumns
{
    public class AdHocColumnsPdfReport
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
                 template.BasicTemplate(BasicTemplate.SilverTemplate);
             })
             .MainTablePreferences(table =>
             {
                 table.ColumnsWidthsType(TableColumnWidthType.Relative);
             })
             .MainTableDataSource(dataSource =>
             {
                 dataSource.GenericDataReader(
                    providerName: "System.Data.SQLite",
                    connectionString: "Data Source=" + System.IO.Path.Combine(AppPath.ApplicationPath, "data\\blogs.sqlite"),
                    sql: @"SELECT [url] as 'Url', [name] as 'Name', [NumberOfPosts] as 'Number Of Posts', [AddDate] as 'Add Date'
                           FROM [tblBlogs]
                           WHERE [NumberOfPosts]>=@p1",
                    parametersValues: new object[] { 10 }
                );
             })
             .MainTableSummarySettings(summary =>
             {
                 summary.OverallSummarySettings("Summary");
                 summary.PreviousPageSummarySettings("Previous Page Summary");
                 summary.PageSummarySettings("Page Summary");
             })
             .MainTableAdHocColumnsConventions(adHocColumns =>
             {
                 //We want sum of the int columns
                 adHocColumns.AddTypeAggregateFunction(
                     typeof(Int64),
                     new AggregateProvider(AggregateFunction.Sum)
                     {
                         DisplayFormatFormula = obj => obj == null || string.IsNullOrEmpty(obj.ToString())
                                                            ? string.Empty : string.Format("{0:n0}", obj)
                     });

                 //We want to dispaly all of the dateTimes as dd/MM/yyyy HH:mm
                 adHocColumns.AddTypeDisplayFormatFormula(
                     typeof(DateTime),
                     data => { return ((DateTime)data).ToString("dd/MM/yyyy HH:mm"); }
                 );
                 adHocColumns.ShowRowNumberColumn(true);
                 adHocColumns.RowNumberColumnCaption("#");
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
             .Generate(data => data.AsPdfFile(string.Format("{0}\\Pdf\\AdHocColumnsSampleRpt-{1}.pdf", AppPath.ApplicationPath, Guid.NewGuid().ToString("N"))));
        }
    }
}
