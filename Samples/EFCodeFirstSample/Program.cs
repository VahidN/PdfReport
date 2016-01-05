using System.Data.Entity;
using System.Diagnostics;

namespace EFCodeFirstSample
{
    class Program
    {
        static void Main(string[] args)
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<MyContext, Configuration>());
            using (var context = new MyContext())
            {
                lazyLoadingAndDynamicProxies(context);
            }
        }

        private static void lazyLoadingAndDynamicProxies(MyContext context)
        {
            var user = context.UserProfiles.Find(1);
            if (user == null)
                return;

            var lazyLoadedDocs = user.Docs;
            // To view its implementation, right click on the method and then select `go to implementation` 
            var rpt = DocsPdfReport.CreatePdfReport(lazyLoadedDocs);
            Process.Start(rpt.FileName);
        }
    }
}