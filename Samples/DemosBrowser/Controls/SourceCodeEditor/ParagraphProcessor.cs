using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Documents;
using System.Windows.Media;
using DemosBrowser.Core;

namespace DemosBrowser.Controls.SourceCodeEditor
{
    public class ParagraphProcessor : IParagraphProcessor
    {
        private static readonly Regex _splitRegex = new Regex(@"(\s|\(|\)|\+|\-|\%|\*|\[|\]|/)", RegexOptions.Compiled);
        private static readonly HashSet<string> _keyWords =
            new HashSet<string>(StringComparer.CurrentCultureIgnoreCase)
                {
                    "abstract", "base", "break", "byte", "case", "catch", "checked", "class", "const", "continue", 
				    "default", "delegate", "do", "value",
				    "else", "enum", "event", "exdouble", "explicit", "extern",
				    "false", "finally", "fixed", "for", "foreach", 
				    "get", "goto",
				    "if", "implicit", "in", "interface", "internal", "is",
				    "lock",
				    "namespace", "new", "null",
				    "object", "operator", "out", "override",
				    "private", "protected", "public", 
				    "readonly", "ref", "return",
				    "sealed", "set", "sizeof", "static", "struct", "switch",
				    "this", "throw", "true", "try", "typeof",
				    "unchecked", "unsafe", "using",
				    "virtual", "while", "partial", "var"
                };

        private static readonly HashSet<string> _dataTypes =
                new HashSet<string>(StringComparer.CurrentCultureIgnoreCase)
        {
            "string", "exfloat", "float", "int",
            "long", "sbyte", "short", "uint", "ulong",
            "void", "double", "decimal", "bool", "char", "ushort"
        };

        public ParagraphProcessor()
        {
            var list = SamplesList.LoadPdfRptPublicTypes();
            if (list == null || !list.Any()) return;
            foreach (var item in list)
                _dataTypes.Add(item);
        }

        public Regex SplitWordsRegex { get { return _splitRegex; } }
        
        public int GetWordTypeID(string word)
        {
            if (_keyWords.Contains(word))            
                return 1;
            
            if (_dataTypes.Contains(word))            
                return 2;
            
            return 0;
        }

        public Inline FormatInlineForID(Inline inline, int id)
        {
            if (id == 1)
            {
                inline.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 255, 127, 53));
            }      
            else if (id == 2)
            {
                inline.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 255, 170, 53));
            }
            return inline;
        }
    }
}
