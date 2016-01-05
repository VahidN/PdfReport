using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using iTextSharp.tool.xml.css;
using iTextSharp.tool.xml.html;
using iTextSharp.tool.xml.parser;
using iTextSharp.tool.xml.pipeline.css;
using iTextSharp.tool.xml.pipeline.end;
using iTextSharp.tool.xml.pipeline.html;
using PdfRpt.Core.Contracts;

namespace PdfRpt.Core.Helper.HtmlToPdf
{
    /// <summary>
    /// Using iTextSharp's HTML to PDF capabilities.
    /// </summary>
    public class XmlWorkerHelper
    {
        /// <summary>
        /// The HTML to show.
        /// </summary>
        public string Html { set; get; }

        /// <summary>
        /// Run direction, left-to-right or right-to-left.
        /// </summary>
        public PdfRunDirection RunDirection { set; get; }

        /// <summary>
        /// Optional external CSS files.
        /// </summary>
        public IList<string> CssFilesPath { set; get; }

        /// <summary>
        /// Optional inline CSS content.
        /// </summary>
        public string InlineCss { set; get; }

        /// <summary>
        /// Optional images directory path.
        /// </summary>
        public string ImagesPath { set; get; }

        /// <summary>
        /// Custom HTML Element.
        /// </summary>
        public iTextSharp.text.Image PdfElement { set; get; }

        /// <summary>
        /// Html document's default font.
        /// </summary>
        public Font DefaultFont { set; get; }

        /// <summary>
        /// Using iTextSharp's HTML to PDF capabilities.
        /// </summary>
        public PdfPCell RenderHtml()
        {
            IElementHandler elementsHandler;
            if (RunDirection == PdfRunDirection.RightToLeft)
            {
                elementsHandler = new RtlElementsCollector();
            }
            else
            {
                elementsHandler = new SimpleElementsCollector();
            }

            processHtml(elementsHandler);

            var runDirection = RunDirection == PdfRunDirection.RightToLeft ? PdfWriter.RUN_DIRECTION_RTL : PdfWriter.RUN_DIRECTION_LTR;
            var cell = new PdfPCell
            {                
                RunDirection = runDirection,
                HorizontalAlignment = Element.ALIGN_LEFT,
                UseAscender = true,
                UseDescender = true
            };

            var paragraph = RunDirection == PdfRunDirection.RightToLeft ?
                                                        ((RtlElementsCollector)elementsHandler).Paragraph :
                                                        ((SimpleElementsCollector)elementsHandler).Paragraph;

            cell.AddElement(paragraph);
            return cell;
        }

        private void processHtml(IElementHandler elementsHandler)
        {
            var cssResolver = new StyleAttrCSSResolver();

            if (CssFilesPath != null && CssFilesPath.Any())
            {
                foreach (var cssFile in CssFilesPath)
                {
                    cssResolver.AddCss(XmlWorkerUtils.GetCssFile(cssFile));
                }
            }

            if (!string.IsNullOrEmpty(InlineCss))
            {
                cssResolver.AddCss(InlineCss, "utf-8", true);
            }

            var htmlContext = new HtmlPipelineContext(new CssAppliersImpl(new UnicodeFontProvider(DefaultFont)));
            if (!string.IsNullOrEmpty(ImagesPath))
            {
                htmlContext.SetImageProvider(new ImageProvider { ImagesPath = ImagesPath });
            }
            htmlContext.CharSet(Encoding.UTF8);

            var tagsProcessorFactory = (DefaultTagProcessorFactory)Tags.GetHtmlTagProcessorFactory();
            if (PdfElement != null)
            {
                tagsProcessorFactory.AddProcessor("totalpagesnumber", new TotalPagesNumberXmlWorkerProcessor(PdfElement));
            }

            htmlContext.SetAcceptUnknown(true).AutoBookmark(true).SetTagFactory(tagsProcessorFactory);
            var pipeline = new CssResolverPipeline(cssResolver,
                                                   new HtmlPipeline(htmlContext, new ElementHandlerPipeline(elementsHandler, null)));
            var worker = new XMLWorker(pipeline, parseHtml: true);
            var parser = new XMLParser();
            parser.AddListener(worker);
            parser.Parse(new StringReader(Html));
        }
    }
}