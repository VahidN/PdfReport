using System.ComponentModel;

namespace PdfReportSamples.Models
{
    public enum OrderType
    {
        Ordinary,

        [Description("From Company A")]
        FromCompanyA,

        [Description("From Company B")]
        FromCompanyB
    }
}
