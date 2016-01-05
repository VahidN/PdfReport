using System;
using System.Web;
using PdfReportSamples.IList;
using PdfReportSamples.InMemory;

namespace WebAppTests
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        { }

        protected void btnBrowse_Click(object sender, EventArgs e)
        {
            // To view its implementation, right click on the method and then select `go to implementation` 
            var rpt = new IListPdfReport().CreatePdfReport();
            Response.Redirect(rpt.FileName.Replace(HttpRuntime.AppDomainAppPath, string.Empty));
        }

        protected void btnInMemory_Click(object sender, EventArgs e)
        {
            new InMemoryPdfReport().CreatePdfReport();
        }        
    }
}