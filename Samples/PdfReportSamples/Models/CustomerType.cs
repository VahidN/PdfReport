using System.ComponentModel;

namespace PdfReportSamples.Models
{
    public enum CustomerType
    {
        [Description("Ordinary User")]
        Ordinary,

        [Description("Special User")]
        Special
    }
}
