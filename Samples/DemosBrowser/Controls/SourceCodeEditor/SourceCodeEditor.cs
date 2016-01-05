using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace DemosBrowser.Controls.SourceCodeEditor
{
    public class SourceCodeEditor : RichTextBox
    {
        private bool _disableTextChangedEvent;
        private IParagraphProcessor _paragraphProcessor = new ParagraphProcessor();

        public SourceCodeEditor()
        {
            this.Document.PageWidth = 1000;
            //this.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
            //this.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;            

            var style = new Style(typeof(Paragraph));
            style.Setters.Add(new Setter(Block.MarginProperty, new Thickness(0)));
            this.Resources.Add(typeof(Paragraph), style);
            this.AcceptsTab = true;

            this.TextChanged += this.SourceCodeEditor_TextChanged;
        }

        public double PageWidth
        {
            get { return this.Document.PageWidth; }
            set { this.Document.PageWidth = value; }
        }

        public bool DisableAddingWhiteSpacesOnEnter { get; set; }

        public string ParagraphProcessorType
        {
            get { return this._paragraphProcessor.GetType().FullName; }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("ParagraphProcessorType cannot be null or empty string");
                }
                var type = System.Type.GetType(value);
                if (type == null)
                {
                    throw new ArgumentException(
                        String.Format("Type '{0}' passed to ParagraphProcessorType cannot be recognized", value));
                }
                if (typeof(IParagraphProcessor).IsAssignableFrom(type))
                {
                    throw new ArgumentException(
                        String.Format("Type '{0}' passed to ParagraphProcessorType must implement IParagraphProcessor", type));
                }
                this._paragraphProcessor = (IParagraphProcessor)Activator.CreateInstance(type);
            }
        }

        private void SetLongText(string text)
        {
            this.Document.Blocks.Clear();
            this._disableTextChangedEvent = true;

            var lines = text.Split('\n');

            foreach (var line in lines)
            {
                var p = new Paragraph();
                this.Document.Blocks.Add(p);
                this.EvaluateParagraph(p, new List<string>(this._paragraphProcessor.SplitWordsRegex.Split(line.TrimEnd())));
            }

            this._disableTextChangedEvent = false;
        }


        private void SourceCodeEditor_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this._disableTextChangedEvent)
            {
                return;
            }

            var changedParagraphs = new HashSet<Paragraph>();
            // prepare a list of paragraphs that need to be changed
            foreach (var change in e.Changes)
            {
                if (change.Offset < (change.Offset + change.AddedLength))
                {
                    var start = this.Document.ContentStart.GetPositionAtOffset(change.Offset);
                    var end = this.Document.ContentStart.GetPositionAtOffset(change.Offset + change.AddedLength);
                    if (start == null || end == null) continue;
                    var p = start.Paragraph;
                    if (p == null)
                    {
                        var nearest = start.GetAdjacentElement(LogicalDirection.Forward) as Paragraph;
                        if (nearest != null) p = nearest;
                    }
                    while (p != null)
                    {
                        if (p.ContentStart.CompareTo(end) >= 0)
                        {
                            break;
                        }
                        changedParagraphs.Add(p);
                        p = p.NextBlock as Paragraph;
                    }
                }
            }

            this.ReEvaluateParagraphs(changedParagraphs);

        }

        private void ReEvaluateParagraphs(IEnumerable<Paragraph> paragraphs)
        {
            this._disableTextChangedEvent = true;

            foreach (var p in paragraphs)
            {
                this.EvaluateParagraph(p);
            }

            this._disableTextChangedEvent = false;

        }

        private void EvaluateParagraph(Paragraph paragraph)
        {
            this.EvaluateParagraph(paragraph, null);
        }

        private void EvaluateParagraph(Paragraph paragraph, List<string> list)
        {
            if (list == null) list = this.ExtractListOfWords(paragraph);
            if (list == null) return;

            Inline cursorNeighboutingElement;
            LinkedList<Inline> inlines = GetListOfInlines(list, out cursorNeighboutingElement);

            this.ReAssignInlinesInParagraph(paragraph, inlines, cursorNeighboutingElement);
        }

        private LinkedList<Inline> GetListOfInlines(List<string> list, out Inline cursorNeighboutingElement)
        {
            var sb = new StringBuilder();
            var inlines = new LinkedList<Inline>();
            cursorNeighboutingElement = null;
            var idKeyWordAroundCaret = 0;
            for (var i = 0; i < list.Count; i++)
            {
                var curr = list[i];
                // if this is the caret
                if (curr == null)
                {
                    // if the caret is in the first character - add an empty element to serve as pointer to the caret
                    if (i == 0)
                    {
                        var run = new Run("");
                        inlines.AddLast(run);
                    }
                    // if the caret is not at the very beginning of the row make sure to add a pointer to it
                    else // if (i > 0)
                    {
                        if (sb.Length > 0)
                        {
                            var run = new Run(sb.ToString());
                            inlines.AddLast(run);
                            sb = new StringBuilder();
                        }

                    }
                    cursorNeighboutingElement = inlines.Last.Value;
                    continue;
                }

                // check if upcomming is the caret pointer (null value) and it is not at the end
                if (i < (list.Count - 2) && list[i + 1] == null)
                {
                    idKeyWordAroundCaret = this._paragraphProcessor.GetWordTypeID(curr + list[i + 2]);
                    if (idKeyWordAroundCaret > 0)
                    {
                        if (sb.Length > 0)
                        {
                            inlines.AddLast(new Run(sb.ToString()));
                            sb = new StringBuilder();
                        }
                        inlines.AddLast(this._paragraphProcessor.FormatInlineForID(new Run(curr), idKeyWordAroundCaret));
                        continue;
                    }
                }

                var id = idKeyWordAroundCaret > 0
                             ? idKeyWordAroundCaret
                             : this._paragraphProcessor.GetWordTypeID(curr);
                if (id > 0)
                {
                    idKeyWordAroundCaret = 0;
                    if (sb.Length > 0)
                    {
                        inlines.AddLast(new Run(sb.ToString()));
                        sb = new StringBuilder();
                    }
                    inlines.AddLast(this._paragraphProcessor.FormatInlineForID(new Run(curr), id));
                }
                else
                {
                    sb.Append(curr);
                }

            }

            if (sb.Length > 0)
            {
                inlines.AddLast(new Run(sb.ToString()));
            }
            return inlines;
        }

        private void ReAssignInlinesInParagraph(Paragraph paragraph, LinkedList<Inline> inlines, TextElement cursorNeighboutingElement)
        {
            this.BeginChange();
            paragraph.Inlines.Clear();
            paragraph.Inlines.AddRange(inlines);
            if (cursorNeighboutingElement != null)
            {
                this.CaretPosition = cursorNeighboutingElement.ContentEnd;
            }
            this.EndChange();
        }

        private List<string> ExtractListOfWords(Paragraph paragraph)
        {
            var caretIsWithin = (this.CaretPosition != null &&
                                 this.CaretPosition.CompareTo(paragraph.ContentStart) >= 0 &&
                                 this.CaretPosition.CompareTo(paragraph.ContentEnd) <= 0);


            var list = new List<string>();

            if (caretIsWithin)
            {
                var beforeText = paragraph.GetTextBefore(this.CaretPosition);
                var afterText = paragraph.GetTextAfter(this.CaretPosition);
                // enter key was pressed
                if (beforeText == "" && afterText == "")
                {

                    if (this.DisableAddingWhiteSpacesOnEnter)
                    {
                        return null;
                    }
                    // add white space to the new line - same as the previous line
                    var prev = paragraph.PreviousBlock as Paragraph;
                    if (prev != null)
                    {
                        var previousLine = prev.GetText();
                        var spaces = new StringBuilder();
                        foreach (var ch in previousLine)
                        {
                            if (ch == ' ' || ch == '\t')
                            {
                                spaces.Append(ch);
                            }
                            else
                            {
                                break;
                            }
                        }
                        if (spaces.Length > 0)
                        {
                            beforeText = spaces.ToString();
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
                // else

                if (beforeText != "") list.AddRange(this._paragraphProcessor.SplitWordsRegex.Split(beforeText));
                list.Add(null);
                if (afterText != "") list.AddRange(this._paragraphProcessor.SplitWordsRegex.Split(afterText));
            }
            else
            {
                var text = paragraph.GetText();
                var array = this._paragraphProcessor.SplitWordsRegex.Split(text);
                list.AddRange(array);
            }
            return list;
        }

        /// <summary>
        /// If you want to pass long text please use SetLongText method instead
        /// </summary>
        public string Text
        {
            get
            {
                var range = new TextRange(this.Document.ContentStart, this.Document.ContentEnd);
                return range.Text;
            }
            set
            {                
                if (String.IsNullOrEmpty(value))
                {
                    new TextRange(this.Document.ContentStart, this.Document.ContentEnd) { Text = value };
                }
                else
                {
                    this.SetLongText(value);
                }
            }
        }


        public static readonly DependencyProperty BoundTextProperty =
            DependencyProperty.Register("BoundText",
                            typeof(string), typeof(SourceCodeEditor),
                            new PropertyMetadata(
                                string.Empty,
                                OnBoundTextChanged));

        public string BoundText
        {
            get { return (string)GetValue(BoundTextProperty); }
            set { SetValue(BoundTextProperty, value); }
        }

        private static void OnBoundTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var box = d as SourceCodeEditor;
            if (box != null)
                box.onTextPropertyChanged(e);
        }

        void onTextPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            var val = e.NewValue as string;
            if (string.IsNullOrEmpty(val))
            {
                this.Document.Blocks.Clear();
                return;
            }
            Text = val;
        }
    }
}
