using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using PdfRpt.Core.Contracts;
using PdfRpt.Core.Helper;

namespace PdfReportSamples.MergePdfFilesInMemory
{
    public class MergeInMemoryPdfFilesPdfReport
    {
        public string CreatePdfReport()
        {
            return mergeMultipleReports();
        }

        private string mergeMultipleReports()
        {
            // It's an in-memory PDF report
            var file1ContentBytes = new PdfReportToArray().CreatePdfReport();

            var file1Path = Path.Combine(AppPath.ApplicationPath, "Pdf\\PdfReportToArray.pdf");
            File.WriteAllBytes(file1Path, file1ContentBytes);

            using (var mergedFileStream = new MemoryStream())
            {
                new MergePdfDocuments
                {
                    DocumentMetadata =
                        new DocumentMetadata
                        {
                            Author = "Vahid",
                            Application = "PdfRpt",
                            Keywords = "Test",
                            Subject = "MergePdfFiles Rpt.",
                            Title = "Test"
                        },
                    InputFileStreams = new Stream[]
                    {
                        // Using the input in-memory PDF report(s)
                        new MemoryStream(file1ContentBytes),
                        new MemoryStream(file1ContentBytes)
                    },
                    OutputFileStream = mergedFileStream,
                    AttachmentsBookmarkLabel = "Attachment(s) ",
                    WriterCustomizer = importedPageInfo =>
                    {
                        addNewPageNumbersToFinalMergedFile(importedPageInfo);
                    }
                }.PerformMerge();

                // It's still an in-memory PDF file. Save it to a file or flush it in the browser.
                var mergedFileContentBytes = mergedFileStream.ToArray();

                // Save it to a file.
                var finalMergedFile = Path.Combine(AppPath.ApplicationPath, "Pdf\\mergedFile.pdf");
                File.WriteAllBytes(finalMergedFile, mergedFileContentBytes);
                return finalMergedFile;
            }
        }

        private void addNewPageNumbersToFinalMergedFile(ImportedPageInfo importedPageInfo)
        {
            var bottomMargin = importedPageInfo.PdfDocument.BottomMargin;
            var pageSize = importedPageInfo.PageSize;
            var contentByte = importedPageInfo.Stamp.GetOverContent();

            // hide the old footer
            contentByte.SaveState();
            contentByte.SetColorFill(BaseColor.WHITE);
            contentByte.Rectangle(0, 0, pageSize.Width, bottomMargin);
            contentByte.Fill();
            contentByte.RestoreState();

            // write the new page numbers
            var center = (pageSize.Left + pageSize.Right) / 2;
            ColumnText.ShowTextAligned(
                canvas: contentByte,
                alignment: Element.ALIGN_CENTER,
                phrase: new Phrase("Page " + importedPageInfo.CurrentPageNumber + "/" + importedPageInfo.TotalNumberOfPages),
                x: center,
                y: pageSize.GetBottom(25),
                rotation: 0,
                runDirection: PdfWriter.RUN_DIRECTION_LTR,
                arabicOptions: 0);
        }
    }
}