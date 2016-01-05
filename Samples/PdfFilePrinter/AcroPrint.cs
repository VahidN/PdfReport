using System;
using System.Diagnostics;
using System.IO;
using System.Management;
using Microsoft.Win32;

namespace PdfFilePrinter
{
    /// <summary>
    /// Executes the Adobe Reader and prints a file while suppressing the Acrobat print
    /// dialog box, then terminating the Reader.
    /// </summary>
    public class AcroPrint
    {
        /// <summary>
        /// The Adobe Reader or Adobe Acrobat path such as 'C:\Program Files\Adobe\Adobe Reader X\AcroRd32.exe'.
        /// If it's not specified, the InstalledAdobeReaderPath property value will be used.
        /// </summary>
        public string AdobeReaderPath { set; get; }

        /// <summary>
        /// Returns the default printer name.
        /// </summary>
        public string DefaultPrinterName
        {
            get
            {
                var query = new ObjectQuery("SELECT * FROM Win32_Printer");
                using (var searcher = new ManagementObjectSearcher(query))
                {
                    foreach (var mo in searcher.Get())
                    {
                        if (((bool?)mo["Default"]) ?? false)
                            return mo["Name"] as string;
                    }
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// The name and path of the PDF file to print.
        /// </summary>
        public string PdfFilePath { set; get; }

        /// <summary>
        /// Name of the printer such as '\\PrintServer\HP LaserJet'.
        /// If it's not specified, the DefaultPrinterName property value will be used.
        /// </summary>
        public string PrinterName { set; get; }

        /// <summary>
        /// Returns the HKEY_CLASSES_ROOT\Software\Adobe\Acrobat\Exe value.
        /// If AcroRd32.exe does not exist, returns string.Empty
        /// </summary>
        public string InstalledAdobeReaderPath
        {
            get
            {
                var acroRd32Exe = Registry.ClassesRoot.OpenSubKey(@"Software\Adobe\Acrobat\Exe", writable: false);
                if (acroRd32Exe == null)
                    return string.Empty;

                var exePath = acroRd32Exe.GetValue(string.Empty) as string;
                if (string.IsNullOrEmpty(exePath))
                    return string.Empty;

                exePath = exePath.Trim(new[] { '"' });
                return File.Exists(exePath) ? exePath : string.Empty;
            }
        }

        /// <summary>
        /// Executes the Adobe Reader and prints a file while suppressing the Acrobat print
        /// dialog box, then terminating the Reader.
        /// </summary>
        /// <param name="timeout">The amount of time, in milliseconds, to wait for the associated process to exit. The maximum is the largest possible value of a 32-bit integer, which represents infinity to the operating system.</param>
        public void PrintPdfFile(int timeout = Int32.MaxValue)
        {
            if (!File.Exists(PdfFilePath))
                throw new ArgumentException(PdfFilePath + " does not exist.");

            var args = string.Format("/N /T \"{0}\" \"{1}\"", PdfFilePath, getPrinterName());
            var process = startAdobeProcess(args);
            if (!process.WaitForExit(timeout))
                process.Kill();
        }

        private Process startAdobeProcess(string arguments = "")
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = this.getExePath(),
                Arguments = arguments,
                CreateNoWindow = true,
                ErrorDialog = false,
                UseShellExecute = false,
                Verb = "print"
            };

            return Process.Start(startInfo);
        }

        private string getPrinterName()
        {
            var printer = PrinterName;
            if (string.IsNullOrEmpty(printer))
                printer = DefaultPrinterName;

            if (string.IsNullOrEmpty(printer))
                throw new ArgumentException("Please set the PrinterName.");

            return printer;
        }

        private string getExePath()
        {
            var exePath = AdobeReaderPath;
            if (string.IsNullOrEmpty(exePath) || !File.Exists(exePath))
                exePath = InstalledAdobeReaderPath;

            if (string.IsNullOrEmpty(exePath))
                throw new ArgumentException("Please set the full path of the AcroRd32.exe or Acrobat.exe.");

            return exePath;
        }
    }
}