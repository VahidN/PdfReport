using System;
using System.Linq;
using System.Net.Mime;
using System.Globalization;
using PdfRpt.Core.Contracts;

namespace PdfRpt.Core.Helper
{
    /// <summary>
    /// This class has not a hard reference to the System.Web assembly. 
    /// So it can be called from the `.NET client profile` library.
    /// </summary>
    public static class SoftHttpContext
    {
        const string Error = "Please call this method in an ASP.NET application.";

        /// <summary>
        /// Flushes the fileData into the user's browser.
        /// It's designed for the ASP.NET Applications.
        /// </summary>
        /// <param name="fileName">name of the file</param>
        /// <param name="fileData">byte array containing the file's data</param>
        /// <param name="flushType">How to flush an in memory PDF file</param>
        public static void FlushInBrowser(string fileName, byte[] fileData, FlushType flushType = FlushType.Attachment)
        {
            var fileLength = fileData.Length;

            var context = getCurrentHttpContext();
            var response = getCurrentResponse(context);
            setNoCache(response);
            setContentType(response);
            addHeaders(fileName, fileLength, response, flushType);
            setBufferTrue(response);
            clearResponse(response);
            writeToOutputStream(fileData, fileLength, response);
            responseEnd(response);
        }

        private static void responseEnd(object response)
        {
            var responseEnd = response.GetType().GetMethod("End");
            responseEnd.Invoke(response, new object[] { });
        }

        private static void writeToOutputStream(byte[] fileData, int fileLength, object response)
        {
            var outputStreamProperty = response.GetType().GetProperty("OutputStream");
            var outputStream = outputStreamProperty.GetValue(response, null);

            var write = outputStream.GetType().GetMethod("Write");
            write.Invoke(outputStream, new object[] { fileData, 0, fileLength });

            var flush = outputStream.GetType().GetMethod("Flush");
            flush.Invoke(outputStream, new object[] { });

            var close = outputStream.GetType().GetMethod("Close");
            close.Invoke(outputStream, new object[] { });
        }

        private static void clearResponse(object response)
        {
            var clear = response.GetType().GetMethod("Clear");
            clear.Invoke(response, new object[] { });
        }

        private static void setBufferTrue(object response)
        {
            var bufferProperty = response.GetType().GetProperty("Buffer");
            bufferProperty.SetValue(response, true, null);
        }

        private static void addHeaders(string fileName, long fileLength, object response, FlushType flushType)
        {
            var addHeader = response.GetType().GetMethod("AddHeader");
            addHeader.Invoke(response, new object[] { "Content-Length", fileLength.ToString(CultureInfo.InvariantCulture) });
            var contentDisposition = flushType == FlushType.Inline ? "inline;filename=" : "attachment;filename=";
            addHeader.Invoke(response, new object[] { "content-disposition", contentDisposition + fileName });
        }

        private static void setContentType(object response)
        {
            var contentTypeProperty = response.GetType().GetProperty("ContentType");
            contentTypeProperty.SetValue(response, MediaTypeNames.Application.Pdf, null);
        }

        private static void setNoCache(object response)
        {
            var cacheProperty = response.GetType().GetProperty("Cache");
            var cache = cacheProperty.GetValue(response, null);
            if (cache == null)
                throw new InvalidOperationException(Error);

            var setCacheability = cache.GetType().GetMethods().FirstOrDefault(x => x.Name == "SetCacheability");
            if (setCacheability == null)
                throw new InvalidOperationException(Error);

            setCacheability.Invoke(cache, new object[] { 1 /* HttpCacheability.NoCache */ });
        }

        private static object getCurrentResponse(object context)
        {
            var responseProperty = context.GetType().GetProperty("Response");
            var response = responseProperty.GetValue(context, null);
            if (response == null)
                throw new InvalidOperationException(Error);

            return response;
        }

        private static object getCurrentHttpContext()
        {
            var systemWeb = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => x.FullName.Contains("System.Web"));
            if (systemWeb == null)
                throw new InvalidOperationException(Error);

            var typ = Type.GetType("System.Web.HttpContext" + ", " + systemWeb.FullName, true, true);
            var prop = typ.GetProperty("Current");

            //get a reference to the current HTTP context
            var context = prop.GetValue(null, null);
            if (context == null)
                throw new InvalidOperationException(Error);

            return context;
        }
    }
}