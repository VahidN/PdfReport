using System.IO;
using System.Linq;
using PdfRpt.Core.Contracts;
using PdfRpt.Core.Helper;

namespace PdfReportSamples.DynamicCompile
{
    public class DynamicCompilePdfReport
    {
        public IPdfReportData CreatePdfReport()
        {
            var dir = @"..\Samples\PdfReportSamples";
            var sourceCode = File.ReadAllText(dir + @"\IList\IListPdfReport.cs");
            var fullyQualifiedClassName = "PdfReportSamples.IList.IListPdfReport";

            using (var compiler = new CompileAsIPdfReportData
            {
                CompilerType = CompilerType.CSharp,
                CompilerVersion = "v4.0", //if you want to use v4.0, you need to change the target framework of the running asm to v4.0 as well.
                FullyQualifiedClassName = fullyQualifiedClassName,
                ReferencedAssemblies = new[]
                {
                    "itextsharp.dll",
                    "PdfRpt.dll",
                    "EPPlus.dll",
                    "PdfReportSamples.dll" //for the used Models
                },
                SourceCode = sourceCode
            })
            {
                var rptClassInstance = compiler.DynamicCompile();
                var createPdfReportMethod = rptClassInstance.GetType()
                                                            .GetMethods()
                                                            .First(x => x.Name == "CreatePdfReport");
                var rptObject = createPdfReportMethod.Invoke(rptClassInstance, null);
                return rptObject as IPdfReportData;
            }
        }
    }
}
