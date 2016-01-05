namespace jqGridToPdfReportMvcApp.Models
{
    public class JqGridRequest
    {
        public string sidx { set; get; }
        public string sord { set; get; }
        public int page { set; get; }
        public int rows { set; get; }
        public bool _search { set; get; }
        public string searchField { set; get; }
        public string searchString { set; get; }
        public string searchOper { set; get; }
        public string filters { set; get; }
        public string oper { set; get; }
    }
}