using System;
using PdfRpt.Core.Contracts;
using PdfRpt.FluentInterface;

namespace PdfReportSamples.MasterDetails
{
    public class MasterDetailsPdfReport
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
            .PagesHeader(header =>
            {
                header.CacheHeader(cache: true); // It's a default setting to improve the performance.
                header.CustomHeader(new MasterDetailsHeaders { PdfRptFont = header.PdfFont });
            })
            .PagesFooter(footer =>
            {
                footer.DefaultFooter(DateTime.Now.ToString("MM/dd/yyyy"));
            })
            .MainTableTemplate(t => t.BasicTemplate(BasicTemplate.SilverTemplate))
            .MainTablePreferences(table =>
            {
                table.ColumnsWidthsType(TableColumnWidthType.Relative);
                table.GroupsPreferences(new GroupsPreferences
                {
                    GroupType = GroupType.HideGroupingColumns,
                    RepeatHeaderRowPerGroup = true,
                    ShowOneGroupPerPage = false,
                    SpacingBeforeAllGroupsSummary = 5f,
                    NewGroupAvailableSpacingThreshold = 170
                });
            })
            .MainTableDataSource(dataSource =>
            {
                dataSource.GenericDataReader(
                   providerName: "System.Data.SQLite",
                   connectionString: "Data Source=" + System.IO.Path.Combine(AppPath.ApplicationPath, "data\\blogs.sqlite"),
                   sql: @"select 
                            tblParents.BirthDate as ParentBirthDate,
                            tblParents.Name as ParentName,
                            tblParents.LastName as ParentLastName,
                            tblKids.Name as KidName,
                            tblKids.BirthDate as KidBirthDate
                            from tblParents
                                left outer join tblKids
                                     on tblKids.ParentId = tblParents.Id
                            order by 
                                tblParents.Name,
                                tblParents.LastName,
                                tblKids.Name"
               );
            })
            .MainTableColumns(columns =>
            {
                columns.AddColumn(column =>
                {
                    column.PropertyName("rowNo");
                    column.IsRowNumber(true);
                    column.CellsHorizontalAlignment(HorizontalAlignment.Left);
                    column.IsVisible(true);
                    column.Order(0);
                    column.Width(1);
                    column.HeaderCell("#");
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName("ParentBirthDate");
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.Order(1);
                    column.Width(2);
                    column.HeaderCell("ParentBirthDate");
                    column.Group(
                    (val1, val2) =>
                    {
                        var date1 = (DateTime)val1;
                        var date2 = (DateTime)val2;
                        return date1.Year == date2.Year && date1.Month == date2.Month && date1.Day == date2.Day;
                    });
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName("ParentName");
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.Order(2);
                    column.Width(2);
                    column.HeaderCell("ParentName");
                    column.Group(
                    (val1, val2) =>
                    {
                        return val1.ToString() == val2.ToString();
                    });
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName("ParentLastName");
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.Order(3);
                    column.Width(2);
                    column.HeaderCell("ParentLastName");
                    column.Group(
                    (val1, val2) =>
                    {
                        return val1.ToString() == val2.ToString();
                    });
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName("KidName");
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.Order(4);
                    column.Width(2);
                    column.HeaderCell("Child Name");
                    column.IsVisible(true);
                });

                columns.AddColumn(column =>
                {
                    column.PropertyName("KidBirthDate");
                    column.CellsHorizontalAlignment(HorizontalAlignment.Center);
                    column.Order(5);
                    column.Width(2);
                    column.HeaderCell("BirthDate");
                    column.IsVisible(true);
                });
            })
            .MainTableEvents(events =>
            {
                events.DataSourceIsEmpty(message: "There is no data available to display.");
            })
            .Export(e => e.ToExcel())
            .Generate(data => data.AsPdfFile(string.Format("{0}\\Pdf\\RptMasterDetailsSample-{1}.pdf", AppPath.ApplicationPath, Guid.NewGuid().ToString("N"))));
        }
    }
}
