using System;
using System.Collections.Generic;
using System.Windows.Media;
using iTextSharp.text;
using PdfReportSamples.Models;
using PdfRpt;
using PdfRpt.Core.Contracts;
using PdfRpt.FluentInterface;

namespace PdfReportSamples.CharacterMap
{
    public class CharacterMapPdfReport
    {

        public IPdfReportData CreatePdfReport()
        {
            var fontFamilyName = "Wingdings";
            var fontProvider = new GenericFontProvider(
                                System.IO.Path.Combine(Environment.GetEnvironmentVariable("SystemRoot"), "fonts\\WINGDING.TTF"),
                                System.IO.Path.Combine(Environment.GetEnvironmentVariable("SystemRoot"), "fonts\\verdana.ttf"))
                                {
                                    Size = 17,
                                    Color = new BaseColor(System.Drawing.Color.DarkRed.ToArgb())
                                };

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
                footer.DefaultFooter(DateTime.Now.ToString("MM/dd/yyyy"));
            })
            .PagesHeader(header =>
            {
                header.CacheHeader(cache: true); // It's a default setting to improve the performance.
                header.DefaultHeader(defaultHeader =>
                {
                    defaultHeader.RunDirection(PdfRunDirection.LeftToRight);
                    defaultHeader.Message("Characters Map");
                });
            })
            .MainTableTemplate(template =>
            {
                template.BasicTemplate(BasicTemplate.SilverTemplate);
            })
            .MainTablePreferences(table =>
            {
                table.ColumnsWidthsType(TableColumnWidthType.Relative);
                table.MainTableType(TableType.HorizontalStackPanel);
                table.HorizontalStackPanelPreferences(columnsPerRow: 10);
            })
            .MainTableDataSource(dataSource =>
            {
                // FontFamily is defined in System.Windows.Media of PresentationCore.dll assembly.
                var fontFamily = new FontFamily(fontFamilyName);

                var listOfRows = new List<CharacterInfo>();
                GlyphTypeface glyph = null;
                foreach (var typeface in fontFamily.GetTypefaces())
                {
                    if (typeface.TryGetGlyphTypeface(out glyph) && (glyph != null))
                        break;
                }

                if (glyph == null)
                    throw new InvalidOperationException("Couldn't find a GlyphTypeface.");

                foreach (var item in glyph.CharacterToGlyphMap)
                {
                    var index = item.Key;
                    listOfRows.Add(new CharacterInfo
                    {
                        Character = Convert.ToChar(index),
                        CharacterCode = string.Format("0x{0:X}", index)
                    });
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
                    column.PropertyName<CharacterInfo>(x => x.Character);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.IsVisible(true);
                    column.Order(1);
                    column.Width(1);
                    column.HeaderCell(fontFamilyName + " Characters", mergeHeaderCell: true);
                    column.ColumnItemsTemplate(itemsTemplate =>
                        {
                            itemsTemplate.CustomTemplate(new CellTemplate(fontProvider));
                        });
                });
            })
            .MainTableEvents(events =>
            {
                events.DataSourceIsEmpty(message: "There is no data available to display.");
            })
            .Generate(data => data.AsPdfFile(string.Format("{0}\\Pdf\\CharacterMapSample-{1}.pdf", AppPath.ApplicationPath, Guid.NewGuid().ToString("N"))));
        }
    }
}
