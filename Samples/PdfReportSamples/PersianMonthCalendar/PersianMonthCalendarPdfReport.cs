using System;
using PdfReportSamples.Models;
using PdfRpt.Calendar;
using PdfRpt.Core.Contracts;
using PdfRpt.FluentInterface;
using iTextSharp.text;

namespace PdfReportSamples.PersianMonthCalendar
{
    public class PersianMonthCalendarPdfReport
    {
        public IPdfReportData CreatePdfReport()
        {
            return new PdfReport().DocumentPreferences(doc =>
            {
                doc.RunDirection(PdfRunDirection.RightToLeft);
                doc.Orientation(PageOrientation.Portrait);
                doc.PageSize(PdfPageSize.A4);
                doc.DocumentMetadata(new DocumentMetadata { Author = "Vahid", Application = "PdfRpt", Keywords = "IList Rpt.", Subject = "Test Rpt", Title = "Test" });
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
                    defaultHeader.RunDirection(PdfRunDirection.RightToLeft);
                    defaultHeader.ImagePath(System.IO.Path.Combine(AppPath.ApplicationPath, "Images\\01.png"));
                    defaultHeader.Message("گزارش ساعات كاركرد كاركنان");
                });
            })
            .MainTableTemplate(template =>
            {
                template.BasicTemplate(BasicTemplate.ClassicTemplate);
            })
            .MainTablePreferences(table =>
            {
                table.ColumnsWidthsType(TableColumnWidthType.Relative);
                table.SplitRows(true);
            })
            .MainTableDataSource(dataSource =>
            {
                var listOfRows = PersianMonthCalendarDataSource.CreateDataSource();
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
                    column.Width(0.5f);
                    column.HeaderCell("#");
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<UserMonthCalendar>(x => x.Id);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(1);
                    column.Width(0.5f);
                    column.HeaderCell("شماره");
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<UserMonthCalendar>(x => x.Name);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(2);
                    column.Width(1);
                    column.HeaderCell("نام");
                });

                columns.AddColumn(column =>
                {
                    // Calendar's cell data type should be PdfRpt.Calendar.CalendarData
                    column.PropertyName<UserMonthCalendar>(x => x.MonthCalendarData);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(3);
                    column.Width(3);
                    column.HeaderCell("تقويم ماهيانه");
                    column.ColumnItemsTemplate(itemsTemplate =>
                    {
                        itemsTemplate.MonthCalendar(new CalendarAttributes
                        {
                            CalendarType = CalendarType.PersianCalendar,
                            UseLongDayNamesOfWeek = true,
                            Padding = 3,
                            DescriptionHorizontalAlignment = HorizontalAlignment.Center,
                            SplitRows = true,
                            CellsCustomizer = info =>
                            {
                                if (info.Year == 1391 && info.Month == 1 && info.DayNumber == 1)
                                {
                                    info.NumberCell.BackgroundColor = new BaseColor(System.Drawing.Color.LimeGreen.ToArgb());
                                    var phrase = info.NumberCell.Phrase;
                                    foreach (var chunk in phrase.Chunks)
                                        chunk.Font.Color = new BaseColor(System.Drawing.Color.Yellow.ToArgb());
                                }
                            }
                        });
                    });
                });

            })
            .MainTableEvents(events =>
            {
                events.DataSourceIsEmpty(message: "There is no data available to display.");
            })
            .Generate(data => data.AsPdfFile(string.Format("{0}\\Pdf\\PersianMonthCalendarPdfReport-{1}.pdf", AppPath.ApplicationPath, Guid.NewGuid().ToString("N"))));
        }
    }
}
