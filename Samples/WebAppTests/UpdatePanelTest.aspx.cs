using System;
using PdfReportSamples.InMemory;

namespace WebAppTests
{
    public partial class UpdatePanelTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        { }

        protected void btnInMemory_Click(object sender, EventArgs e)
        {
            new InMemoryPdfReport().CreatePdfReport();
        }
    }
}