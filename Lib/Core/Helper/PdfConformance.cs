using System.Collections.Generic;
using iTextSharp.text.pdf;
using PdfRpt.Core.Contracts;

namespace PdfRpt.Core.Helper
{
    /// <summary>
    /// Applies PDF/A Conformance.
    /// </summary>
    public class PdfConformance
    {
        /// <summary>
        /// PdfX To PdfA Converter
        /// </summary>
        public static readonly IDictionary<PdfXConformance, PdfAConformanceLevel> PdfXToPdfA =
        new Dictionary<PdfXConformance, PdfAConformanceLevel>
        {
            { PdfXConformance.PDFA1A , PdfAConformanceLevel.PDF_A_1A },
            { PdfXConformance.PDFA1B , PdfAConformanceLevel.PDF_A_1B },
            { PdfXConformance.PDFA2A , PdfAConformanceLevel.PDF_A_2A },
            { PdfXConformance.PDFA2B , PdfAConformanceLevel.PDF_A_2B },
            { PdfXConformance.PDFA2U , PdfAConformanceLevel.PDF_A_2U },
            { PdfXConformance.PDFA3A , PdfAConformanceLevel.PDF_A_3A },
            { PdfXConformance.PDFA3B , PdfAConformanceLevel.PDF_A_3B },
            { PdfXConformance.PDFA3U , PdfAConformanceLevel.PDF_A_3U } 
        };

        /// <summary>
        /// PdfWriter Object.
        /// </summary>
        public PdfWriter PdfWriter { get; set; }
        /// <summary>
        /// Document settings.
        /// </summary>
        public DocumentPreferences PageSetup { get; set; }

        /// <summary>
        /// Sets PDF/A Conformance Level.
        /// </summary>
        public void SetConformanceLevel()
        {
            if (PageSetup.ConformanceLevel == PdfXConformance.PDFXNONE)
            {
                // Sets the transparency blending colorspace to RGB. 
                // The default blending colorspace is CMYK and will result in faded colors in the screen and in printing.
                PdfWriter.RgbTransparencyBlending = true;
                return;
            }

            if ((int)PageSetup.ConformanceLevel <= (int)PdfXConformance.PDFX32002)
            {
                PdfWriter.PDFXConformance = (int)PageSetup.ConformanceLevel;
            }
        }

        /// <summary>
        /// Sets PDF/A Conformance ColorProfile.
        /// </summary>
        public void SetColorProfile()
        {
            if (PageSetup.ConformanceLevel == PdfXConformance.PDFXNONE) return;

            var pdfDictionary = new PdfDictionary(PdfName.OUTPUTINTENT);
            pdfDictionary.Put(PdfName.OUTPUTCONDITIONIDENTIFIER, new PdfString("sRGB IEC61966-2.1"));
            pdfDictionary.Put(PdfName.INFO, new PdfString("sRGB IEC61966-2.1"));
            pdfDictionary.Put(PdfName.S, PdfName.GTS_PDFA1);

            var profileStream = StreamHelper.GetResourceByName("PdfRpt.Core.Helper.srgb.profile");
            var pdfICCBased = new PdfICCBased(ICC_Profile.GetInstance(profileStream));
            pdfICCBased.Remove(PdfName.ALTERNATE);
            pdfDictionary.Put(PdfName.DESTOUTPUTPROFILE, PdfWriter.AddToBody(pdfICCBased).IndirectReference);

            PdfWriter.ExtraCatalog.Put(PdfName.OUTPUTINTENTS, new PdfArray(pdfDictionary));
        }
    }
}
