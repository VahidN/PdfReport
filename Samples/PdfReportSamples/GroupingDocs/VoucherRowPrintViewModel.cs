using System;

namespace PdfReportSamples.GroupingDocs
{
    public class VoucherRowPrintViewModel
    {
        public string Title { set; get; }
        public int VoucherNumber { set; get; }
        public DateTime VoucherDate { set; get; }
        public string Description { set; get; }
        public int Debtor { set; get; }
        public int Creditor { set; get; }

        public string CaclulatedDetection 
        {
            get { return Debtor > 0 ? "بد" : "بس"; }
        }

        public int CaclulatedRemains 
        {
            get { return Debtor - Creditor; }
        }
    }
}