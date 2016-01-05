using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using iTextSharp.text;
using PdfReportSamples.Models;
using PdfRpt.Core.Contracts;
using PdfRpt.Core.Helper;
using PdfRpt.FluentInterface;

namespace PdfReportSamples.Events
{
    public class EventsPdfReport
    {
        public IPdfReportData CreatePdfReport()
        {
            return new PdfReport().DocumentPreferences(doc =>
            {
                doc.RunDirection(PdfRunDirection.LeftToRight);
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
                template.BasicTemplate(BasicTemplate.SilverTemplate);
            })
            .MainTablePreferences(table =>
            {
                table.ColumnsWidthsType(TableColumnWidthType.Relative);
            })
            .MainTableDataSource(dataSource =>
            {
                var listOfRows = new List<Order>();
                for (int i = 0; i < 60; i++)
                {
                    listOfRows.Add(new Order
                    {
                        Id = i,
                        Description = "Description Description ... " + i,
                        Price = 1000 + i
                    });
                }
                dataSource.StronglyTypedList(listOfRows);
            })
            .MainTableSummarySettings(summarySettings =>
            {
                summarySettings.OverallSummarySettings("Summary");
                summarySettings.PreviousPageSummarySettings("Previous Page Summary");
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
                    column.PropertyName<Order>(x => x.Description);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(1);
                    column.Width(3);
                    column.HeaderCell("Description");
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<Order>(x => x.Id);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(2);
                    column.Width(1);
                    column.HeaderCell("Id");
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName<Order>(x => x.Price);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(4);
                    column.Width(2);
                    column.HeaderCell("Price");
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

                events.CellCreated(args =>
                    {
                        if (args.CellType == CellType.PreviousPageSummaryCell ||
                            args.CellType == CellType.PageSummaryCell ||
                            args.CellType == CellType.SummaryRowCell)
                        {
                            if (!string.IsNullOrEmpty(args.Cell.RowData.FormattedValue) &&
                                args.Cell.RowData.PropertyName == "Price")
                            {
                                args.Cell.RowData.FormattedValue += " $";
                            }
                        }
                    });

                events.MainTableCreated(args =>
                {
                    var infoTable = new PdfGrid(numColumns: 1)
                    {
                        WidthPercentage = 100
                    };
                    infoTable.AddSimpleRow(
                         (cellData, properties) =>
                         {
                             cellData.Value = "Show data before the main table ...";
                             properties.PdfFont = events.PdfFont;
                             properties.RunDirection = PdfRunDirection.LeftToRight;
                         });
                    var table = infoTable.AddBorderToTable(borderColor: BaseColor.LIGHT_GRAY, spacingBefore: 10f);
                    table.SpacingAfter = 10f;
                    args.PdfDoc.Add(table);
                });

                events.ShouldSkipRow(args =>
                {
                    var rowData = args.TableRowData;
                    //var previousTableRowData = args.PreviousTableRowData;

                    var description = rowData.FirstOrDefault(x => x.PropertyName == "Description");
                    if (description != null &&
                        description.PropertyValue.ToSafeString() == "Description Description ... 1")
                    {
                        return true; // don't render this row.
                    }

                    return false;
                });

                var pageNumber = 0;
                events.ShouldSkipHeader(args =>
                {
                    pageNumber++;
                    if (pageNumber == 2)
                    {
                        return true; // don't render this header row.
                    }

                    return false;
                });

                events.ShouldSkipFooter(args =>
                {
                    if (pageNumber == 2)
                    {
                        return true; // don't render this footer row.
                    }

                    return false;
                });

                events.MainTableAdded(args =>
                {
                    /*var objData = args.ColumnCellsSummaryData.Where(x => x.CellData.PropertyName.Equals("Price"))
                        .OrderByDescending(x => x.OverallRowNumber)
                        .First()
                        .OverallAggregateValue;*/

                    var data = args.LastOverallAggregateValueOf<Order>(y => y.Price);
                    var msg = "Total: " + data + ", " + long.Parse(data, NumberStyles.AllowThousands, CultureInfo.InvariantCulture).NumberToText(Language.English);
                    var infoTable = new PdfGrid(numColumns: 1)
                    {
                        WidthPercentage = 100
                    };
                    infoTable.AddSimpleRow(
                         (cellData, properties) =>
                         {
                             cellData.Value = "Show data after the main table ...";
                             properties.PdfFont = events.PdfFont;
                             properties.RunDirection = PdfRunDirection.LeftToRight;
                         });
                    infoTable.AddSimpleRow(
                         (cellData, properties) =>
                         {
                             cellData.Value = msg;
                             properties.PdfFont = events.PdfFont;
                             properties.RunDirection = PdfRunDirection.LeftToRight;
                         });
                    args.PdfDoc.Add(infoTable.AddBorderToTable(borderColor: BaseColor.LIGHT_GRAY, spacingBefore: 10f));
                });
            })
            .Export(export =>
            {
                export.ToExcel();
            })
            .Generate(data => data.AsPdfFile(string.Format("{0}\\Pdf\\EventsPdfReport-{1}.pdf", AppPath.ApplicationPath, Guid.NewGuid().ToString("N"))));
        }
    }
}