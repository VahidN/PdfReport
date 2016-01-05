using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using iTextSharp.tool.xml.html.pdfelement;
using iTextSharp.tool.xml.pipeline;

namespace PdfRpt.Core.Helper.HtmlToPdf
{
    /// <summary>
    /// XMLWorker does not support RTL by default. so we need to collect the parsed elements first and
    /// then wrap them with a RTL table.
    /// </summary>
    public class RtlElementsCollector : IElementHandler
    {
        private readonly Paragraph _paragraph;

        /// <summary>
        /// XMLWorker does not support RTL by default. so we need to collect the parsed elements first and
        /// then wrap them with a RTL table.
        /// </summary>
        public RtlElementsCollector()
        {
            _paragraph = new Paragraph
            {
                Alignment = Element.ALIGN_LEFT
            };
        }

        /// <summary>
        /// This Paragraph contains all of the parsed elements.
        /// </summary>
        public Paragraph Paragraph
        {
            get { return _paragraph; }
        }

        /// <summary>
        /// Intercepting the XMLWorker's parser and collecting its interpreted pdf elements.
        /// </summary>        
        public void Add(IWritable htmlElement)
        {
            var writableElement = htmlElement as WritableElement;
            if (writableElement == null)
                return;

            foreach (var element in writableElement.Elements())
            {
                if (element is NoNewLineParagraph)
                {
                    var noNewLineParagraph = element as NoNewLineParagraph;
                    foreach (var item in noNewLineParagraph)
                    {
                        addElement(item);
                    }
                }
                else if (element is PdfDiv)
                {
                    var div = element as PdfDiv;
                    foreach (var divChildElement in div.Content)
                    {
                        addElement(divChildElement);
                    }
                }
                else if (element is Paragraph)
                {
                    var paragraph = element as Paragraph;
                    paragraph.Alignment = Element.ALIGN_LEFT;
                    _paragraph.Add(element);
                }
                else
                {
                    addElement(element);
                }
            }
        }

        private void addElement(IElement divChildElement)
        {
            fixNestedTablesRunDirection(divChildElement);
            _paragraph.Add(divChildElement);
        }

        /// <summary>
        /// It's necessary to fix the RTL of the final PdfPTables.
        /// </summary>        
        private static void fixNestedTablesRunDirection(IElement element)
        {
            var table = element as PdfPTable;
            if (table == null)
                return;

            table.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
            foreach (var row in table.Rows)
            {
                foreach (var cell in row.GetCells())
                {
                    cell.RunDirection = PdfWriter.RUN_DIRECTION_RTL;

                    if (cell.CompositeElements == null)
                        continue;

                    foreach (var item in cell.CompositeElements)
                    {
                        fixNestedTablesRunDirection(item);
                    }
                }
            }
        }
    }
}