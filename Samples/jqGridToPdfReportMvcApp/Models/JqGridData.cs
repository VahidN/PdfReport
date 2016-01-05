using System.Collections.Generic;

namespace jqGridToPdfReportMvcApp.Models
{
    public class JqGridData
    {
        public int Total { get; set; }

        public int Page { get; set; }

        public int Records { get; set; }

        public IList<JqGridRowData> Rows { get; set; }

        public object UserData { get; set; }
    }

    public class JqGridRowData
    {
        public int Id { set; get; }
        public IList<string> RowCells { set; get; }
    }
}