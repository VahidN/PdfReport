using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PdfReportSamples;
using PdfRpt;
using PdfRpt.Core.Contracts;
using PdfRpt.FluentInterface;

namespace DemosBrowser.Core
{
    public static class SamplesList
    {
        public static IList<Type> LoadSamplesList()
        {
            var justLoadBaseTestClassesInTheAppDomain = AppPath.ApplicationPath;
            var asm = AppDomain.CurrentDomain.GetAssemblies().First(x => x.FullName.Contains("PdfReportSamples"));

            var samples = new List<Type>();
            foreach (var type in asm.GetTypes())
            {
                if (type.Name.EndsWith("PdfReport"))
                    samples.Add(type);
            }
            return samples.OrderBy(x => x.FullName).ToList();
        }

        public static void WarmupTheSystem(IList<Type> samples)
        {
            if (samples == null || !samples.Any()) return;
            GeneratePdf(samples[0]);
        }

        public static IPdfReportData GeneratePdf(Type sample)
        {
            var instance = Activator.CreateInstance(sample);
            var createPdfReportMethod = instance.GetType().GetMethods().FirstOrDefault(x => x.Name == "CreatePdfReport");
            if (createPdfReportMethod == null)
                throw new InvalidOperationException("Couldn't find CreatePdfReport Method in " + sample.Name + " class");

            var rptObject = createPdfReportMethod.Invoke(instance, null);
            if (rptObject == null)
                return null;

            var fileName = rptObject as string;
            if (fileName != null)
            {
               var rptData = new DataBuilder();
               rptData.SetFileName(fileName);
               return rptData;
            }

            var rpt = rptObject as IPdfReportData;
            if (rpt == null)
                throw new InvalidOperationException("CreatePdfReport Method does not return IPdfReportData");

            return rpt;
        }

        public static IPdfReportData GeneratePdfFromIPdfReportData(Type sample)
        {
            var rpt = sample.ActivateSample();
            new PdfReportDocument { PdfRptData = rpt }.GeneratePdf();
            return rpt;
        }

        public static IPdfReportData GeneratePdf(IPdfReportData sampleInstance)
        {
            new PdfReportDocument { PdfRptData = sampleInstance }.GeneratePdf();
            return sampleInstance;
        }

        public static IPdfReportData ActivateSample(this Type sample)
        {
            return Activator.CreateInstance(sample) as IPdfReportData;
        }

        public static bool IsImplementingIPdfReportData(this Type t)
        {
            return t.GetInterfaces().Contains(typeof(IPdfReportData)) && t.GetConstructor(Type.EmptyTypes) != null;
        }

        public static IList<string> SamplesPath(Type selectedSample)
        {
            var dir = @"..\Samples\PdfReportSamples";
            if (!Directory.Exists(dir) || selectedSample == null) return new List<string>();

            var parts = selectedSample.ToString().Split('.');
            var sampleFolder = parts[1];

            dir += @"\" + sampleFolder;
            return Directory.GetFiles(dir, "*.cs").ToList();
        }

        public static IList<string> LoadPdfRptPublicTypes()
        {
            var justLoadBaseTestClassesInTheAppDomain = AppPath.ApplicationPath;
            var asms = AppDomain.CurrentDomain.GetAssemblies();
            var asm = asms.First(x => x.FullName.Contains("PdfRpt"));
            return asm.GetTypes().Where(x => x.IsPublic).Select(x => x.Name).ToList();
        }
    }
}