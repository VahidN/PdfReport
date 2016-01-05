using System;
using PdfRpt.Core.Helper;
using PdfRpt.Core.Contracts;
using PdfRpt.FluentInterface;

namespace PdfReportSamples.ZapfDingbatsSymbols
{
    public class ZapfDingbatsSymbolsPdfReport
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
                fonts.Path(System.IO.Path.Combine(Environment.GetEnvironmentVariable("SystemRoot"), "fonts\\Tahoma.ttf"),
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
                    defaultHeader.Message("Adobe Zapf Dingbats Symbols");
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
                var dataTable = new System.Data.DataTable();
                dataTable.Columns.Add("Adobe Zapf Dingbats", typeof(string));
                dataTable.Columns.Add("Value", typeof(string));
                dataTable.Columns.Add("Symbol", typeof(AdobeZapfDingbats));

                foreach (var item in Enum.GetNames(typeof(AdobeZapfDingbats)))
                {
                    var value = (AdobeZapfDingbats)Enum.Parse(typeof(AdobeZapfDingbats), item);
                    dataTable.Rows.Add(item, string.Format("{0}/0x{1:X2}", (int)value, (int)value), value);
                }

                dataSource.DataTable(dataTable);
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
                    column.PropertyName("Adobe Zapf Dingbats");
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(1);
                    column.Width(4);
                    column.HeaderCell("Adobe Zapf Dingbats");
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName("Value");
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(2);
                    column.Width(1);
                    column.HeaderCell("Value");
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName("Symbol");
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(3);
                    column.Width(1);
                    column.HeaderCell("Symbol");
                    column.ColumnItemsTemplate(template =>
                    {
                        template.Symbol(list =>
                        {
                            var data = list.GetValueOf("Symbol");
                            if (Enum.IsDefined(typeof(AdobeZapfDingbats), data))
                            {
                                return (AdobeZapfDingbats)data;
                            }
                            return AdobeZapfDingbats.BallotX;
                        });
                    });
                });
            })
            .MainTableEvents(events =>
            {
                events.DataSourceIsEmpty(message: "There is no data available to display.");
            })
            .Generate(data => data.AsPdfFile(string.Format("{0}\\Pdf\\ZapfDingbatsSymbols-{1}.pdf", AppPath.ApplicationPath, Guid.NewGuid().ToString("N"))));
        }
    }
}