using System.ComponentModel;

namespace PdfReportSamples.SingleEntity
{
    public class Document
    {
        public int Id { set; get; }

        [DisplayName("تاریخ")]
        public string Date { set; get; }

        [DisplayName("روز هفته")]
        public string Day { set; get; }
    }
}