using System;
using System.IO;
using iTextSharp.tool.xml.pipeline.html;

namespace PdfRpt.Core.Helper.HtmlToPdf
{
    /// <summary>
    /// XmlWorker's Images Path Provider class.
    /// </summary>
    public class ImageProvider : AbstractImageProvider
    {
        /// <summary>
        /// Images directory path.
        /// </summary>
        public string ImagesPath { set; get; }

        /// <summary>
        /// returns images directory path.
        /// </summary>        
        public override string GetImageRootPath()
        {
            checkPath();
            return ImagesPath;
        }

        private void checkPath()
        {
            if (string.IsNullOrEmpty(ImagesPath))
                throw new NullReferenceException("Please specify the image's location Path.");

            if (!Directory.Exists(ImagesPath))
                throw new DirectoryNotFoundException(string.Format("{0} dir not found.", ImagesPath));

            if (!ImagesPath.EndsWith("\\"))
                ImagesPath += "\\";
        }
    }
}