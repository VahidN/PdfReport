using System;
using System.Globalization;
using PdfRpt;
using PdfRpt.Core.Contracts;
using PdfRpt.Core.Helper;
using PdfRpt.FluentInterface;

namespace PdfReportSamples.InjectCustomRows
{
    public class InjectCustomRowsPdfReport
    {
        public IPdfReportData CreatePdfReport()
        {
            int row = 0;
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
                    defaultHeader.Message("Our new rpt.");
                    defaultHeader.RunDirection(PdfRunDirection.LeftToRight);
                });
            })
            .MainTableTemplate(template =>
            {
                template.BasicTemplate(BasicTemplate.SilverTemplate);
            })
            .MainTablePreferences(table =>
            {
                table.ColumnsWidthsType(TableColumnWidthType.Relative);
                table.NumberOfDataRowsPerPage(5);
            })
            .MainTableDataSource(dataSource =>
            {
                var listOfRows = InjectCustomRowsDataSource.MyTransactions();
                dataSource.AnonymousTypeList(listOfRows);
            })
            .MainTableSummarySettings(summarySettings =>
            {
                summarySettings.OverallSummarySettings("Total");
                summarySettings.PreviousPageSummarySettings("Pervious Page Summary");
                summarySettings.PageSummarySettings("Page Summary");
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
                    column.PropertyName("Id");
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(1);
                    column.Width(2);
                    column.HeaderCell("Id");
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName("Date");
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(2);
                    column.Width(3);
                    column.HeaderCell("Date");
                    column.ColumnItemsTemplate(template =>
                    {
                        template.TextBlock();
                        template.DisplayFormatFormula(obj => obj == null || string.IsNullOrEmpty(obj.ToString())
                                                            ? string.Empty : ((DateTime)obj).ToString("MM/dd/yyyy"));
                    });
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName("Description");
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(3);
                    column.Width(3);
                    column.HeaderCell("Description");
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName("Income");
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(4);
                    column.Width(2);
                    column.HeaderCell("Income");
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

                columns.AddColumn(column =>
                {
                    column.PropertyName("Payment");
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(5);
                    column.Width(2);
                    column.HeaderCell("Payment");
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

                columns.AddColumn(column =>
                {
                    column.PropertyName("Sign");
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(6);
                    column.Width(2);
                    column.HeaderCell("Sign");
                    column.ColumnItemsTemplate(template =>
                    {
                        template.TextBlock();
                        template.BasicProperties(new CellBasicProperties
                        {
                            PdfFont = new GenericFontProvider(
                                System.IO.Path.Combine(Environment.GetEnvironmentVariable("SystemRoot"), "fonts\\WINGDING.TTF"),
                                System.IO.Path.Combine(Environment.GetEnvironmentVariable("SystemRoot"), "fonts\\verdana.ttf"))
                        });
                    });
                    column.CalculatedField(
                    args =>
                    {
                        var residue = args.GetValueOf("Residue", 0);
                        if ((int)residue > 0)
                            return "J"; /*:)*/
                        return "L"; /*:(*/
                    });
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName("Residue");
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(7);
                    column.Width(2);
                    column.HeaderCell("Residue");
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
                events.RowStartedInjectCustomRows(customRows =>
                {
                    if (row > 0)
                    {
                        return; //stop adding new rows.
                    }

                    row++;
                    customRows.AddRow(newRow =>
                    {
                        newRow.AddCell("rowNo", 1);
                        newRow.AddCell("Id", 1);
                        newRow.AddCell("Date", DateTime.Parse("3/17/2012", CultureInfo.CreateSpecificCulture("en-US")));
                        newRow.AddCell("Description", "before 3/17/2012");
                        newRow.AddCell("Income", 100);
                        newRow.AddCell("Payment", 70);
                        newRow.AddCell("Sign", "J");
                        newRow.AddCell("Residue", 30);
                    });
                });
            })
            .Export(export =>
            {
                export.ToExcel();
            })
            .Generate(data => data.AsPdfFile(string.Format("{0}\\Pdf\\InjectCustomRowsSample-{1}.pdf", AppPath.ApplicationPath, Guid.NewGuid().ToString("N"))), debugMode: true);
        }
    }
}
