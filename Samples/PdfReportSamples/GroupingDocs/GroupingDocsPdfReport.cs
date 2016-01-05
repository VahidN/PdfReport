using System;
using System.Collections.Generic;
using iTextSharp.text.pdf;
using PdfReportSamples.HexDump;
using PdfRpt;
using PdfRpt.Core.Contracts;
using PdfRpt.Core.Helper;
using PdfRpt.FluentInterface;

namespace PdfReportSamples.GroupingDocs
{
    public class GroupingDocsPdfReport
    {
        private IPdfFont getWatermarkFont()
        {
            var watermarkFont = new GenericFontProvider(
                                        System.IO.Path.Combine(Environment.GetEnvironmentVariable("SystemRoot"), "fonts\\arial.ttf"),
                                        System.IO.Path.Combine(Environment.GetEnvironmentVariable("SystemRoot"), "fonts\\verdana.ttf"));
            watermarkFont.Color = new GrayColor(0.75f);
            watermarkFont.Size = 50;
            return watermarkFont;
        }

        public IPdfReportData CreatePdfReport()
        {
           return new PdfReport().DocumentPreferences(doc =>
            {
                doc.RunDirection(PdfRunDirection.RightToLeft);
                doc.Orientation(PageOrientation.Portrait);
                doc.PageSize(PdfPageSize.A4);
                doc.DocumentMetadata(new DocumentMetadata { Author = "وحيد", Application = "نرم افزار ", Keywords = "حساب تفصیلی ", Subject = "حساب تفصیلی " , Title = "حساب تفصیلی "  });
                doc.DiagonalWatermark(new DiagonalWatermark
                {
                    Text = "Diagonal Watermark\nLine 2\nLine 3",
                    RunDirection = PdfRunDirection.LeftToRight,
                    Font = getWatermarkFont(),
                    FillOpacity = 0.6f,
                    StrokeOpacity = 1
                });
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
                fonts.Size(8);
            })
            .PagesFooter(footer =>
            {
                footer.DefaultFooter(string.Concat("کاربر : ", "وحيد",
                                               " | ", "تاریخ تهیه گزارش : ", PersianDate.ToPersianDateTime(DateTime.Now, "/", true).FixWeakCharacters()));
            })
            .PagesHeader(header =>
            {
                header.CacheHeader(cache: true); // It's a default setting to improve the performance.
                header.DefaultHeader(defaultHeader =>
                {
                    defaultHeader.Message("دفتر فرضي");
                    defaultHeader.ImagePath(System.IO.Path.Combine(AppPath.ApplicationPath, "Images\\01.png"));
                });
            })
            .MainTableTemplate(template =>
            {
                template.CustomTemplate(new GrayTemplate());
            })
            .MainTablePreferences(table =>
            {
                table.ColumnsWidthsType(TableColumnWidthType.Relative);
                table.GroupsPreferences(new GroupsPreferences
                {
                    GroupType = GroupType.HideGroupingColumns,
                    RepeatHeaderRowPerGroup = true,
                    ShowOneGroupPerPage = true,
                    SpacingBeforeAllGroupsSummary = 5f,
                    NewGroupAvailableSpacingThreshold = 5f
                });
            })
            .MainTableDataSource(dataSource =>
            {
                var rows = new List<VoucherRowPrintViewModel>();
                var rnd = new Random();
                for (int i = 0; i < 10; i++)
                {
                    rows.Add(new VoucherRowPrintViewModel
                    {
                        Title ="عنوان "+ i,
                        VoucherNumber =i,
                        VoucherDate = DateTime.Now.AddDays(-i),
                        Description = "توضيحات "+i,
                        Debtor = i%2==0? 0: rnd.Next(1,100),
                        Creditor= i%2!=0? 0: rnd.Next(1,100)
                    });
                }
                dataSource.StronglyTypedList(rows);
            })
            .MainTableColumns(columns =>
            {
                columns.AddColumn(column =>
                {
                    column.PropertyName<VoucherRowPrintViewModel>(x => x.Title);
                    column.CellsHorizontalAlignment(PdfRpt.Core.Contracts.HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.IsRowNumber(true);
                    column.Order(0);
                    column.Width(0.7f);
                    column.Group(true,
                       (val1, val2) =>
                       {
                           return val1.ToString() == val2.ToString();
                       });
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName("rowNumber");
                    column.CellsHorizontalAlignment(PdfRpt.Core.Contracts.HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.IsRowNumber(true);
                    column.Order(0);
                    column.Width(0.7f);
                    column.HeaderCell("ردیف");
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName<VoucherRowPrintViewModel>(x => x.VoucherNumber);
                    column.CellsHorizontalAlignment(PdfRpt.Core.Contracts.HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(0);
                    column.Width(1);
                    column.HeaderCell("سند");
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName<VoucherRowPrintViewModel>(x => x.VoucherDate);
                    column.CellsHorizontalAlignment(PdfRpt.Core.Contracts.HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(1);
                    column.Width(1.5f);
                    column.ColumnItemsTemplate(template =>
                    {
                        template.TextBlock();
                        template.DisplayFormatFormula(obj =>
                        {
                            if (obj == null || string.IsNullOrEmpty(obj.ToString()))
                                return string.Empty;
                            return PersianDate.ToPersianDateTime((DateTime) obj);
                        });
                    });
                    column.HeaderCell("تاریخ");
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName<VoucherRowPrintViewModel>(x => x.Description);
                    column.CellsHorizontalAlignment(PdfRpt.Core.Contracts.HorizontalAlignment.Left);
                    column.IsVisible(true);
                    column.Order(0);
                    column.Width(4);
                    column.HeaderCell("شرح");
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName<VoucherRowPrintViewModel>(x => x.Debtor);
                    column.CellsHorizontalAlignment(PdfRpt.Core.Contracts.HorizontalAlignment.Right);
                    column.IsVisible(true);
                    column.Order(2);
                    column.Width(1.5f);
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
                    column.HeaderCell("بدهکار");
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName<VoucherRowPrintViewModel>(x => x.Creditor);
                    column.CellsHorizontalAlignment(PdfRpt.Core.Contracts.HorizontalAlignment.Right);
                    column.IsVisible(true);
                    column.Order(3);
                    column.Width(1.5f);
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
                    column.HeaderCell("بستانکار");
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName<VoucherRowPrintViewModel>(x => x.CaclulatedDetection);
                    column.CellsHorizontalAlignment(PdfRpt.Core.Contracts.HorizontalAlignment.Left);
                    column.IsVisible(true);
                    column.Order(4);
                    column.Width(1);
                    column.HeaderCell("تشخیص");
                });
                columns.AddColumn(column =>
                {
                    column.PropertyName<VoucherRowPrintViewModel>(x => x.CaclulatedRemains);
                    column.CellsHorizontalAlignment(PdfRpt.Core.Contracts.HorizontalAlignment.Right);
                    column.IsVisible(true);
                    column.Order(5);
                    column.Width(1.5f);
                    column.ColumnItemsTemplate(template =>
                    {
                        template.TextBlock();
                        template.DisplayFormatFormula(obj => obj == null || string.IsNullOrEmpty(obj.ToString())
                                                            ? string.Empty : string.Format("{0:n0}", obj));
                    });
                    column.HeaderCell("مانده");
                });

            })
            .MainTableSummarySettings(summarySettings =>
            {
                summarySettings.OverallSummarySettings("جمع کل");
                summarySettings.PreviousPageSummarySettings("نقل از صفحه قبل");
                //summarySettings.AllGroupsSummarySettings("جمع نهايي");
            })
            .MainTableEvents(events =>
            {
                events.DataSourceIsEmpty(message: "داده ای جهت نمایش وجود ندارد.");
                events.CellCreated(args =>
                {
                    args.Cell.BasicProperties.CellPadding = 4f;
                });
                events.MainTableAdded(args =>
                {
                    var taxTable = new PdfGrid(3);  // Create a clone of the MainTable's structure  
                    taxTable.RunDirection = 3;
                    taxTable.SetWidths(new float[] { 3, 3, 3 });
                    taxTable.WidthPercentage = 100f;
                    taxTable.SpacingBefore = 10f;

                    taxTable.AddSimpleRow(
                        (data, cellProperties) =>
                        {
                            data.Value = "امضاء تنظیم کننده";
                            cellProperties.ShowBorder = true;
                            cellProperties.PdfFont = args.PdfFont;
                        },
                        (data, cellProperties) =>
                        {
                            data.Value = "امضاء حسابدار";
                            cellProperties.ShowBorder = true;
                            cellProperties.PdfFont = args.PdfFont;
                        },
                        (data, cellProperties) =>
                        {
                            data.Value = "امضاء مدیرعامل";
                            cellProperties.ShowBorder = true;
                            cellProperties.PdfFont = args.PdfFont;
                        });
                    args.PdfDoc.Add(taxTable);
                });
            })
            .Export(export =>
            {
                export.ToExcel("خروجی اکسل");
                export.ToCsv("خروجی CSV");
                export.ToXml("خروجی XML");
            })
            .Generate(data => data.AsPdfFile(string.Format("{0}\\Pdf\\RptIListSample-{1}.pdf", AppPath.ApplicationPath, Guid.NewGuid().ToString("N"))));
        }
    }
}