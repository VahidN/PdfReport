using System;
using System.Diagnostics;
using System.IO;
using DemosBrowser.Core;
using PdfReportSamples;
using PdfReportSamples.DigitalSignature;
using PdfThumbnailComparer.Lib;

namespace PdfThumbnailComparer
{
    /// <summary>
    /// I need to test all of samples after updating the iTextSharp to `see`
    /// if anything has changed in that library which affects the final produced PDF file's images.
    /// It's not possible to compare the md5 hashes of final PDF files, 
    /// because each new PDF file will have a new embedded ID.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            var path = Path.Combine(AppPath.ApplicationPath, "Thumbs");
            var samples = SamplesList.LoadSamplesList();
            foreach (var sample in samples)
            {
                runSample(path, sample);
            }
            Process.Start(path);
        }

        private static void runSample(string path, Type sample)
        {
            Console.WriteLine("GeneratePdf({0});", sample.FullName);
            var report = SamplesList.GeneratePdf(sample);

            if (report == null)
                return;

            if (!File.Exists(report.FileName))
                return;

            if (sample == typeof(DigitalSignaturePdfReport))
                return;

            var imageData = new PdfToImage
            {
                PdfFilePath = report.FileName
            }.PdfPageToPng();
            File.WriteAllBytes(Path.Combine(path, sample.FullName.Replace("PdfReportSamples.", string.Empty) + ".png"), imageData);
        }
    }
}