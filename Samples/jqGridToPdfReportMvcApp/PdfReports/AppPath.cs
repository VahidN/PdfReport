using System.IO;
using System.Reflection;
using System.Web;

namespace jqGridToPdfReportMvcApp.PdfReports
{
    public static class AppPath
    {
        public static string ApplicationPath
        {
            get
            {
                if (isInWeb)
                    return HttpRuntime.AppDomainAppPath;

                return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            }
        }

        private static bool isInWeb
        {
            get { return HttpContext.Current != null; }
        }
    }
}