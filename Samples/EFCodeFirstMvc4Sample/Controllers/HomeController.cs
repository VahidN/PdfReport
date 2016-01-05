using System.Web;
using System.Web.Mvc;
using EFCodeFirstSample;

namespace EFCodeFirstMvc4Sample.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            using (var context = new MyContext())
            {
                var user = context.UserProfiles.Find(1);
                if (user == null)
                    return Content("");

                var lazyLoadedDocs = user.Docs;
                // To view its implementation, right click on the method and then select `go to implementation` 
                var rpt = DocsPdfReport.CreatePdfReport(lazyLoadedDocs);

                var outputFilePath = rpt.FileName.Replace(HttpRuntime.AppDomainAppPath, string.Empty);
                return File(outputFilePath, "application/pdf", "pdfRpt.pdf");
            }
        }
    }
}