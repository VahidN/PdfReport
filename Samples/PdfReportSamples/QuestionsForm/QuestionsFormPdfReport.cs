using System;
using System.Collections.Generic;
using PdfReportSamples.Models;
using PdfRpt.Core.Contracts;
using PdfRpt.FluentInterface;

namespace PdfReportSamples.QuestionsForm
{
    public class QuestionsFormPdfReport
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
                 var listOfRows = new List<Question>();
                 for (int i = 1; i <= 20; i++)
                 {
                     listOfRows.Add(new Question
                     {
                         Id = i,
                         QuestionText = "A very long text. A very long text. A very long text. A very long text. A very long text. A very long text. A very long text. A very long text. A very long text. متن " + i,
                         Answer1 = "A very long item. A very long item. A very long item. A very long item. A very long item. A very long item. A very long item. گزينه " + i,
                         Answer2 = "A very long item. A very long item. A very long item. A very long item. A very long item. A very long item. A very long item. " + i,
                         Answer3 = "A very long item. A very long item. A very long item. A very long item. A very long item. A very long item. A very long item. " + i,
                         Answer4 = "A very long item. A very long item. A very long item. A very long item. A very long item. A very long item. A very long item. " + i,
                         PicturePath = System.IO.Path.Combine(AppPath.ApplicationPath, "Images\\01.png")
                     });
                 }
                 dataSource.StronglyTypedList(listOfRows);
             })
             .MainTableEvents(events =>
             {
                 events.DataSourceIsEmpty(message: "There is no data available to display.");
             })
             .MainTableColumns(columns =>
             {
                 columns.AddColumn(column =>
                 {
                     column.PropertyName<Question>(x => x.Id);
                     column.HeaderCell(caption: "Questions");
                     column.Width(1);
                     column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                     column.IsVisible(true);
                     column.Order(1);
                     column.ColumnItemsTemplate(template =>
                     {
                         template.CustomTemplate(new EntryTemplate(PdfRunDirection.LeftToRight));
                     });
                 });
             })
             .Generate(data => data.AsPdfFile(string.Format("{0}\\Pdf\\QuestionsRpt-{1}.pdf", AppPath.ApplicationPath, Guid.NewGuid().ToString("N"))));
        }
    }
}
