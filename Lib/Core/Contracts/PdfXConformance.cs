using iTextSharp.text.pdf;

namespace PdfRpt.Core.Contracts
{
    /// <summary>     
    /// Sets subsets of the PDF specification (ISO 15930-1 to ISO 15930-8) that promise 
    /// predictable and consistent output for press printing.
    /// The PDF/A specification (ISO 19005-1:2005): Document Management—Electronic Document File Format 
    /// for Long-Term Preservation.
    /// You can check the PDF/A conformance with the Preflight tool of Adobe Acrobat Pro for instance. 
    /// In Acrobat X, Preflight is an option under the Print Production Tools panel.
    /// More info: http://en.wikipedia.org/wiki/PDF/A
    /// </summary>
    public enum PdfXConformance
    {
        /// <summary>
        /// Default value.
        /// </summary>
        PDFXNONE = PdfWriter.PDFXNONE,

        /// <summary>
        /// The main goal of PDF/X-1a is to support blind exchange of PDF documents. 
        /// Blind exchange means you can deliver PDF documents to a print service provider 
        /// with hardly any technical discussion.
        /// </summary>
        PDFX1A2001 = PdfWriter.PDFX1A2001,

        /// <summary>
        /// Sets the conformance to PDF/X-3.
        /// A PDF/X-3 file can also contain color-managed data.
        /// </summary>
        PDFX32002 = PdfWriter.PDFX32002,

        /// <summary>
        /// A synonym for ISO 19005-1 Level A conformance.
        /// FOR ARCHIVING and the file can be viewed without the originating application.
        /// Transparent objects and layers (Optional Content Groups) are forbidden in PDF/A-1.
        /// LZW and JPEG2000 image compressions are forbidden in PDF/A-1.
        /// Embedded files are forbidden in PDF/A-1.
        /// Encryption is forbidden.
        /// </summary>        
        PDFA1A = 3 + PdfAConformanceLevel.PDF_A_1A,

        /// <summary>
        /// A synonym for ISO 19005-1 Level B conformance.
        /// In order to meet level-B conformance, all fonts must be embedded, 
        /// encryption isn’t allowed, audio and video content are forbidden, 
        /// JavaScript and executable file launches are not permitted, and so forth.
        /// </summary>        
        PDFA1B = 4 + PdfAConformanceLevel.PDF_A_1B,

        /// <summary>
        /// Level A conformance (PDF/A-2a) indicates complete compliance with the ISO 19005-2 requirements, including those related to structural and semantic properties of documents.
        /// </summary>
        PDFA2A = 5 + PdfAConformanceLevel.PDF_A_2A,

        /// <summary>
        /// Level B conformance (PDF/A-2b) indicates minimal compliance to ensure that the rendered visual appearance of a conforming file is preservable over the long term.
        /// </summary>
        PDFA2B = 6 + PdfAConformanceLevel.PDF_A_2B,

        /// <summary>
        /// Level U conformance represents Level B conformance with the additional requirement that all text in the document have Unicode equivalents. 
        /// </summary>
        PDFA2U = 7 + PdfAConformanceLevel.PDF_A_2U,

        /// <summary>
        /// PDF/A-3a
        /// </summary>
        PDFA3A = 8 + PdfAConformanceLevel.PDF_A_3A,

        /// <summary>
        /// PDF/A-3b
        /// </summary>
        PDFA3B = 9 + PdfAConformanceLevel.PDF_A_3B,

        /// <summary>
        /// PDF/A-3u
        /// </summary>
        PDFA3U = 10 + PdfAConformanceLevel.PDF_A_3U
    }
}
