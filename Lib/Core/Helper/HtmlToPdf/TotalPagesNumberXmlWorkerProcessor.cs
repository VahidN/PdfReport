using System.Collections.Generic;
using iTextSharp.text;
using iTextSharp.tool.xml;
using iTextSharp.tool.xml.html;

namespace PdfRpt.Core.Helper.HtmlToPdf
{
    /// <summary>
    /// Custom TotalPagesNumber tag processor.
    /// </summary>
    public class TotalPagesNumberXmlWorkerProcessor : AbstractTagProcessor
    {
        readonly iTextSharp.text.Image _image;
        /// <summary>
        /// ctor.
        /// </summary>        
        public TotalPagesNumberXmlWorkerProcessor(iTextSharp.text.Image image)
        {
            _image = image;
        }

        /// <summary>
        /// This method is called when a closing tag has been encountered of the
        /// ITagProcessor implementation that is mapped to the tag.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="tag">the tag encountered</param>
        /// <param name="currentContent">
        /// a list of content possibly created by TagProcessing of inner tags, and by startElement and 
        /// content methods of this ITagProcessor
        /// </param>
        /// <returns>the resulting element to add to the document or a content stack.</returns>
        public override IList<IElement> End(IWorkerContext ctx, Tag tag, IList<IElement> currentContent)
        {
            IList<IElement> list = new List<IElement>();
            var htmlPipelineContext = GetHtmlPipelineContext(ctx);
            list.Add(GetCssAppliers().Apply(new Chunk((iTextSharp.text.Image)GetCssAppliers().Apply(_image, tag, htmlPipelineContext), 0, 0, true), tag, htmlPipelineContext));
            return list;
        }

        /// <summary>
        /// true if the tag implementation must keep it's own currentContent stack.
        /// </summary>        
        public override bool IsStackOwner()
        {
            return false;
        }
    }
}