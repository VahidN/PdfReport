using System.Windows;
using System.Windows.Documents;

namespace DemosBrowser.Controls.SourceCodeEditor
{
    //from: http://blog.bodurov.com/Wpf-Source-Code-Editor/
    public static class ExtInline
    {
        public static Paragraph GetParagraph(this Inline inline)
        {
            var contentElement = ((ContentElement)inline.Parent);
            while (contentElement != null)
            {
                var p = contentElement as Paragraph;
                if (p != null)
                {
                    return p;
                }
                contentElement = ((ContentElement)inline.Parent);
            }
            return null;
        }
    }
}
