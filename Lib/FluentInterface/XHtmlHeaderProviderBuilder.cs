using System;
using PdfRpt.Core.Contracts;
using PdfRpt.HeaderTemplates;

namespace PdfRpt.FluentInterface
{
    /// <summary>
    /// Defines dynamic headers for pages and individual groups by using iTextSharp's HTML to PDF capabilities (XmlWorker class).
    /// </summary>
    public class XHtmlHeaderProviderBuilder
    {
        private readonly XHtmlHeaderProvider _builder = new XHtmlHeaderProvider();

        internal XHtmlHeaderProvider HtmlHeaderProvider
        {
            get { return _builder; }
        }

        /// <summary>
        /// Properties of pages headerds. 
        /// </summary>
        public void PageHeaderProperties(XHeaderBasicProperties data)
        {
            _builder.PageHeaderProperties = data;
        }

        /// <summary>
        /// Properties of groups headerds. 
        /// </summary>
        public void GroupHeaderProperties(XHeaderBasicProperties data)
        {
            _builder.GroupHeaderProperties = data;
        }

        /// <summary>
        /// Returns dynamic HTML content of the group header.
        /// </summary>
        public void AddGroupHeader(Func<HeaderData, string> header)
        {
            _builder.AddGroupHeader = header;
        }

        /// <summary>
        /// Returns dynamic HTML content of the page header.
        /// </summary>
        public void AddPageHeader(Func<HeaderData, string> header)
        {
            _builder.AddPageHeader = header;
        }
    }
}