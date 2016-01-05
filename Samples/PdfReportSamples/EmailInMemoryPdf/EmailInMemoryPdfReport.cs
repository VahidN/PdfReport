using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using PdfReportSamples.Models;
using PdfRpt.Core.Contracts;
using PdfRpt.FluentInterface;

namespace PdfReportSamples.EmailInMemoryPdf
{
    public class EmailInMemoryPdfReport
    {
        public MailInfo MailInfo { set; get; }

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
                 fonts.Path(Path.Combine(Environment.GetEnvironmentVariable("SystemRoot"), "fonts\\tahoma.ttf"),
                            Path.Combine(Environment.GetEnvironmentVariable("SystemRoot"), "fonts\\verdana.ttf"));
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
                     defaultHeader.ImagePath(Path.Combine(AppPath.ApplicationPath, "Images\\01.png"));
                     defaultHeader.Message("Our new rpt.");
                 });
             })
             .MainTableSummarySettings(summarySettings =>
             {
                 summarySettings.OverallSummarySettings("Summary");
                 summarySettings.PreviousPageSummarySettings("Previous Page Summary");
                 summarySettings.PageSummarySettings("Page Summary");
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
                 var listOfRows = new List<User>();
                 for (int i = 0; i < 200; i++)
                 {
                     listOfRows.Add(new User { Id = i, LastName = "Last Name " + i, Name = "Name " + i, Balance = i + 1000 });
                 }
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
                     column.PropertyName<User>(x => x.Id);
                     column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                     column.IsVisible(true);
                     column.Order(1);
                     column.Width(2);
                     column.HeaderCell("Id");
                 });

                 columns.AddColumn(column =>
                 {
                     column.PropertyName<User>(x => x.Name);
                     column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                     column.IsVisible(true);
                     column.Order(2);
                     column.Width(3);
                     column.HeaderCell("Name");
                 });

                 columns.AddColumn(column =>
                 {
                     column.PropertyName<User>(x => x.LastName);
                     column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                     column.IsVisible(true);
                     column.Order(3);
                     column.Width(3);
                     column.HeaderCell("Last Name");
                 });

                 columns.AddColumn(column =>
                 {
                     column.PropertyName<User>(x => x.Balance);
                     column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                     column.IsVisible(true);
                     column.Order(4);
                     column.Width(2);
                     column.HeaderCell("Balance");
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
                 events.DocumentClosing(args =>
                 {
                     // close the document without closing the underlying stream
                     args.PdfWriter.CloseStream = false;
                     args.PdfDoc.Close();

                     args.PdfStreamOutput.Position = 0;

                     // sending in-memory pdf file as an email attachment
                     if (MailInfo == null) return;
                     using (var attachment = new Attachment(args.PdfStreamOutput, "test.pdf", MediaTypeNames.Application.Pdf))
                     {
                         using (var mailMessage = new MailMessage(MailInfo.From, MailInfo.To, MailInfo.Subject, MailInfo.Body))
                         {
                             mailMessage.Attachments.Add(attachment);

                             mailMessage.BodyEncoding = Encoding.UTF8;
                             mailMessage.SubjectEncoding = Encoding.UTF8;

                             var smtp = new SmtpClient
                             {
                                 //just for the local tests (it will produce an .eml file, which can be opened by outlook)
                                 DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory,
                                 PickupDirectoryLocation = Path.Combine(AppPath.ApplicationPath, "pdf")
                             };
                             smtp.Send(mailMessage);
                         }
                     }
                 });
             })
             .Export(export =>
             {
                 export.ToExcel();
                 export.ToXml();
             })
             .Generate(data => data.AsPdfStream(new MemoryStream())); // in-memory pdf file
        }
    }
}
