using System;
using System.Collections.Generic;
using System.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;
using PdfReportSamples.Models;
using PdfRpt.Core.Contracts;
using PdfRpt.Core.Helper;

namespace PdfReportSamples.QuestionsForm
{
    public class EntryTemplate : IColumnItemsTemplate
    {
        readonly PdfRunDirection _pdfRunDirection;
        public EntryTemplate(PdfRunDirection pdfRunDirection)
        {
            _pdfRunDirection = pdfRunDirection;
        }

        /// <summary>
        /// This method is called at the end of the cell's rendering.
        /// </summary>
        /// <param name="cell">The current cell</param>
        /// <param name="position">The coordinates of the cell</param>
        /// <param name="canvases"></param>
        /// <param name="attributes">Current cell's custom attributes</param>
        public void CellRendered(PdfPCell cell, Rectangle position, PdfContentByte[] canvases, CellAttributes attributes)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public CellBasicProperties BasicProperties { set; get; }

        /// <summary>
        /// Defines the current cell's properties, based on the other cells values. 
        /// Here IList contains actual row's cells values.
        /// It can be null.
        /// </summary>
        public Func<IList<CellData>, CellBasicProperties> ConditionalFormatFormula { set; get; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public PdfPCell RenderingCell(CellAttributes attributes)
        {
            var data = attributes.RowData.TableRowData;
            var id = data.GetSafeStringValueOf<Question>(x => x.Id);
            var questionText = data.GetSafeStringValueOf<Question>(x => x.QuestionText);
            var answer1 = data.GetSafeStringValueOf<Question>(x => x.Answer1);
            var answer2 = data.GetSafeStringValueOf<Question>(x => x.Answer2);
            var answer3 = data.GetSafeStringValueOf<Question>(x => x.Answer3);
            var answer4 = data.GetSafeStringValueOf<Question>(x => x.Answer4);
            var picturePath = data.GetSafeStringValueOf<Question>(x => x.PicturePath);

            var font = attributes.BasicProperties.PdfFont;

            var relativeWidths = getRelativeWidths();

            var mainTable = new PdfGrid(relativeWidths)
            {
                RunDirection = (int)_pdfRunDirection,
                WidthPercentage = 100,
                SpacingBefore = 5,
                SpacingAfter = 5
            };

            addQuestionText(id, questionText, font, mainTable);
            addOptions(answer1, answer2, answer3, answer4, font, mainTable);
            addImageCell(picturePath, mainTable);

            return new PdfPCell(mainTable);
        }

        private float[] getRelativeWidths()
        {
            var relativeWidths = new float[] { 1, 5 };
            if (_pdfRunDirection == PdfRunDirection.LeftToRight)
            {
                relativeWidths = relativeWidths.Reverse().ToArray();
            }
            return relativeWidths;
        }

        private void addOptions(string answer1, string answer2, string answer3, string answer4, IPdfFont font, PdfGrid mainTable)
        {
            var optionsTable = new PdfGrid(numColumns: 2)
            {
                RunDirection = (int)_pdfRunDirection,
                WidthPercentage = 100,
            };

            //---------------- row - 1
            optionsTable.AddCell(new PdfPCell(font.FontSelector.Process("a) " + answer1))
            {
                Border = 0,
                Padding = 5
            });
            optionsTable.AddCell(new PdfPCell(font.FontSelector.Process("b) " + answer2))
            {
                Border = 0,
                Padding = 5
            });

            //---------------- row - 2            
            optionsTable.AddCell(new PdfPCell(font.FontSelector.Process("c) " + answer3))
            {
                Border = 0,
                Padding = 5
            });
            optionsTable.AddCell(new PdfPCell(font.FontSelector.Process("d) " + answer4))
            {
                Border = 0,
                Padding = 5
            });
            mainTable.AddCell(new PdfPCell(optionsTable) { Border = 0 });
        }

        private static void addQuestionText(string id, string questionText, IPdfFont font, PdfGrid mainTable)
        {
            mainTable.AddCell(new PdfPCell(font.FontSelector.Process(id + ") " + questionText))
            {
                Border = 0,
                Padding = 5,
                Colspan = 2
            });
        }

        private static void addImageCell(string picturePath, PdfGrid mainTable)
        {
            mainTable.AddCell(new PdfPCell(PdfImageHelper.GetITextSharpImageFromImageFile(picturePath))
            {
                Border = 0,
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE
            });
        }
    }
}
