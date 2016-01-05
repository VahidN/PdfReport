using System;
using System.IO;
using iTextSharp.tool.xml;
using iTextSharp.tool.xml.css;

namespace PdfRpt.Core.Helper.HtmlToPdf
{
    /// <summary>
    /// XMLWorker's helper methods.
    /// </summary>
    public static class XmlWorkerUtils
    {
        /// <summary>
        /// returns a css file
        /// </summary>
        /// <param name="filePath">css file's path</param>
        /// <returns>XMLWorker CSS file</returns>
        public static ICssFile GetCssFile(string filePath)
        {
            checkCssFile(filePath);
            return XMLWorkerHelper.GetCSS(new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
        }

        private static void checkCssFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new NullReferenceException("Please specify the CSS file's location.");

            if (!File.Exists(filePath))
                throw new DirectoryNotFoundException(string.Format("{0} file not found.", filePath));
        }
    }
}