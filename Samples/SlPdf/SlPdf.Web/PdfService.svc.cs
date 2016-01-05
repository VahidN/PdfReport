using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Web;
using PdfReportSamples.IList;

namespace SlPdf.Web
{
    [ServiceContract(Namespace = "")]
    [SilverlightFaultBehavior]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class PdfService
    {
        [OperationContract]
        public string CreateReport(string name)
        {
            // To view its implementation, right click on the method and then select `go to implementation` 
            var rpt = new IListPdfReport().CreatePdfReport();
            //returns the created pdf file's path
            return rpt.FileName.Replace(HttpRuntime.AppDomainAppPath, string.Empty);
        }
    }
}
