using System.Text.RegularExpressions;
using System.Windows.Documents;

namespace DemosBrowser.Controls.SourceCodeEditor
{
    public interface IParagraphProcessor
    {
        Regex SplitWordsRegex { get; }
        int GetWordTypeID(string word);
        Inline FormatInlineForID(Inline inline, int id);
    }
}
