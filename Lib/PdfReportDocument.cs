using System.Collections.Generic;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using PdfRpt.Core.Contracts;
using PdfRpt.Core.Helper;
using PdfRpt.Core.PdfTable;

namespace PdfRpt
{
    /// <summary>
    /// PDF report generator class
    /// </summary>
    public class PdfReportDocument
    {
        #region Fields (3)

        IPdfReportData _pdfRptData;
        RenderMainTable _pdfRptRenderMainTable;
        DocumentSettings _pdfDocumentSettings;
        PdfConformance _pdfConformance;

        #endregion Fields

        #region Constructors (1)

        /// <summary>
        /// ctor
        /// </summary>
        public PdfReportDocument()
        {
            LastRenderedRowData = new LastRenderedRowData();
        }

        #endregion Constructors

        #region Properties (6)

        /// <summary>
        /// It's designed for the ASP.NET Applications.
        /// </summary>
        public bool FlushInBrowser { set; get; }

        /// <summary>
        /// How to flush an in memory PDF file.
        /// </summary>
        public FlushType FlushType { set; get; }

        /// <summary>
        /// Summary cells data of the main table's columns
        /// </summary>
        public IList<SummaryCellData> ColumnSummaryCellsData { private set; get; }

        /// <summary>
        /// Holds last result of the actual rendering engine of iTextSharp during its processes.
        /// </summary>
        public LastRenderedRowData LastRenderedRowData { private set; get; }

        /// <summary>
        /// PDF Document object
        /// </summary>
        public Document PdfDoc { get; private set; }

        /// <summary>
        /// Reports' definition data
        /// </summary>
        public IPdfReportData PdfRptData
        {
            get { return _pdfRptData; }
            set { _pdfRptData = value; }
        }

        /// <summary>
        /// PdfWriter object
        /// </summary>
        public PdfWriter PdfWriter { get; private set; }

        #endregion Properties

        #region Methods (9)

        // Public Methods (1) 

        /// <summary>
        /// Start generating the report based on the PdfRptData 
        /// </summary>
        public void GeneratePdf(bool debugMode = false)
        {
            checkNullValues();

            if (debugMode)
            {
                runInDebugMode();
            }
            else
            {
                runInReleaseMode();
            }
        }

        private void runInDebugMode()
        {
            PdfDoc = new Document(DocumentSettings.GetPageSizeAndColor(_pdfRptData.DocumentPreferences),
                        _pdfRptData.DocumentPreferences.PagePreferences.Margins.Left,
                        _pdfRptData.DocumentPreferences.PagePreferences.Margins.Right,
                        _pdfRptData.DocumentPreferences.PagePreferences.Margins.Top,
                        _pdfRptData.DocumentPreferences.PagePreferences.Margins.Bottom);

            createPdf();
            PdfDoc.Dispose();
        }

        private void runInReleaseMode()
        {
            new Document(DocumentSettings.GetPageSizeAndColor(_pdfRptData.DocumentPreferences),
                        _pdfRptData.DocumentPreferences.PagePreferences.Margins.Left,
                        _pdfRptData.DocumentPreferences.PagePreferences.Margins.Right,
                        _pdfRptData.DocumentPreferences.PagePreferences.Margins.Top,
                        _pdfRptData.DocumentPreferences.PagePreferences.Margins.Bottom)
                        .SafeUsingBlock(pdfDisposable =>
                        {
                            PdfDoc = pdfDisposable;
                            createPdf();
                        });
        }

        private void createPdf()
        {
            if (FlushInBrowser)
                _pdfRptData.PdfStreamOutput = new MemoryStream();

            var stream = _pdfRptData.PdfStreamOutput;
            initPdfWriter(stream);
            initSettings();
            _pdfDocumentSettings.ApplyBeforePdfDocOpenSettings();
            _pdfDocumentSettings.SetEncryption();
            PdfDoc.Open();
            _pdfConformance.SetColorProfile();

            if (_pdfRptData.MainTableEvents != null)
                _pdfRptData.MainTableEvents.DocumentOpened(new EventsArguments { PdfDoc = PdfDoc, PdfWriter = PdfWriter, ColumnCellsSummaryData = ColumnSummaryCellsData, PageSetup = _pdfRptData.DocumentPreferences, PdfFont = _pdfRptData.PdfFont, PdfColumnsAttributes = _pdfRptData.PdfColumnsAttributes });

            _pdfDocumentSettings.ApplySettings();
            _pdfDocumentSettings.AddFileAttachments();
            addMainTable();
            _pdfDocumentSettings.ApplySignature(stream);

            if (_pdfRptData.MainTableEvents != null)
                _pdfRptData.MainTableEvents.DocumentClosing(new EventsArguments { PdfDoc = PdfDoc, PdfWriter = PdfWriter, PdfStreamOutput = stream, ColumnCellsSummaryData = ColumnSummaryCellsData, PageSetup = _pdfRptData.DocumentPreferences, PdfFont = _pdfRptData.PdfFont, PdfColumnsAttributes = _pdfRptData.PdfColumnsAttributes });

            flushFileInBrowser();
        }

        private void flushFileInBrowser()
        {
            if (!FlushInBrowser)
                return;

            // close the document without closing the underlying stream
            PdfWriter.CloseStream = false;
            PdfDoc.Close();
            _pdfRptData.PdfStreamOutput.Position = 0;

            // write pdf bytes to output stream
            var pdf = ((MemoryStream)_pdfRptData.PdfStreamOutput).ToArray();
            SoftHttpContext.FlushInBrowser(_pdfRptData.FileName, pdf, FlushType);
        }

        private void initSettings()
        {
            _pdfDocumentSettings = new DocumentSettings
            {
                DocumentSecurity = _pdfRptData.DocumentSecurity,
                PageSetup = _pdfRptData.DocumentPreferences,
                PdfDoc = PdfDoc,
                PdfWriter = PdfWriter,
                DocumentProperties = _pdfRptData.DocumentPreferences.DocumentMetadata
            };
        }
        // Private Methods (8) 

        private void addMainTable()
        {
            _pdfRptRenderMainTable = new RenderMainTable
            {
                PdfRptData = _pdfRptData,
                ColumnSummaryCellsData = ColumnSummaryCellsData,
                PdfDoc = PdfDoc,
                PdfWriter = PdfWriter,
                CurrentRowInfoData = LastRenderedRowData
            };
            _pdfRptRenderMainTable.AddToDocument();
        }

        private void checkNullValues()
        {
            if (_pdfRptData.DocumentPreferences.PagePreferences.Margins == null)
            {
                _pdfRptData.DocumentPreferences.PagePreferences.Margins = new DocumentMargins
                {
                    Bottom = 50,
                    Left = 36,
                    Right = 36,
                    Top = 36
                };
            }

            if (_pdfRptData.DocumentPreferences.PagePreferences.Size == null)
            {
                _pdfRptData.DocumentPreferences.PagePreferences.Size = PageSize.A4;
            }
        }

        private void initPdfWriter(Stream stream)
        {
            if ((int)_pdfRptData.DocumentPreferences.ConformanceLevel > (int)PdfXConformance.PDFX32002)
            {
                PdfWriter = PdfAWriter.GetInstance(PdfDoc, stream, PdfConformance.PdfXToPdfA[_pdfRptData.DocumentPreferences.ConformanceLevel]);
            }
            else
            {
                PdfWriter = PdfWriter.GetInstance(PdfDoc, stream);
            }

            var pageEvents = new PageEvents
            {
                PdfRptHeader = _pdfRptData.Header,
                PageSetup = _pdfRptData.DocumentPreferences,
                PdfRptFooter = _pdfRptData.Footer,
                CurrentRowInfoData = LastRenderedRowData,
                ColumnSummaryCellsData = ColumnSummaryCellsData,
                MainTableEvents = _pdfRptData.MainTableEvents,
                PdfFont = _pdfRptData.PdfFont,
                PdfColumnsAttributes = _pdfRptData.PdfColumnsAttributes
            };
            PdfWriter.PageEvent = pageEvents;
            _pdfConformance = new PdfConformance { PdfWriter = PdfWriter, PageSetup = _pdfRptData.DocumentPreferences };
            _pdfConformance.SetConformanceLevel();
        }

        #endregion Methods
    }
}