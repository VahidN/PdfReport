using System;
using iTextSharp.text;
using PdfRpt.Core.Contracts;

namespace PdfRpt.Core.Helper
{
    /// <summary>
    /// This class converts the most common paper sizes to their iTextSharp.text.Rectangle equivalents.
    /// </summary>
    public static class PdfPageSizeToRectangle
    {
        /// <summary>
        /// Converts the most common paper sizes to their iTextSharp.text.Rectangle equivalents.
        /// </summary>
        /// <param name="pageSize">page size</param>
        /// <returns>Rectangle</returns>
        public static Rectangle ToRectangle(this PdfPageSize pageSize)
        {
            switch (pageSize)
            {
                case PdfPageSize.Letter:
                    return PageSize.LETTER;

                case PdfPageSize.Note:
                    return PageSize.NOTE;

                case PdfPageSize.Legal:
                    return PageSize.LEGAL;

                case PdfPageSize.Tabloid:
                    return PageSize.TABLOID;

                case PdfPageSize.Executive:
                    return PageSize.EXECUTIVE;

                case PdfPageSize.Postcard:
                    return PageSize.POSTCARD;

                case PdfPageSize.A0:
                    return PageSize.A0;

                case PdfPageSize.A1:
                    return PageSize.A1;

                case PdfPageSize.A2:
                    return PageSize.A2;

                case PdfPageSize.A3:
                    return PageSize.A3;

                case PdfPageSize.A4:
                    return PageSize.A4;

                case PdfPageSize.A5:
                    return PageSize.A5;

                case PdfPageSize.A6:
                    return PageSize.A6;

                case PdfPageSize.A7:
                    return PageSize.A7;

                case PdfPageSize.A8:
                    return PageSize.A8;

                case PdfPageSize.A9:
                    return PageSize.A9;

                case PdfPageSize.A10:
                    return PageSize.A10;

                case PdfPageSize.B0:
                    return PageSize.B0;

                case PdfPageSize.B1:
                    return PageSize.B1;

                case PdfPageSize.B2:
                    return PageSize.B2;

                case PdfPageSize.B3:
                    return PageSize.B3;

                case PdfPageSize.B4:
                    return PageSize.B4;

                case PdfPageSize.B5:
                    return PageSize.B5;

                case PdfPageSize.B6:
                    return PageSize.B6;

                case PdfPageSize.B7:
                    return PageSize.B7;

                case PdfPageSize.B8:
                    return PageSize.B8;

                case PdfPageSize.B9:
                    return PageSize.B9;

                case PdfPageSize.B10:
                    return PageSize.B10;

                case PdfPageSize.ArchE:
                    return PageSize.ARCH_E;

                case PdfPageSize.ArchD:
                    return PageSize.ARCH_D;

                case PdfPageSize.ArchC:
                    return PageSize.ARCH_C;

                case PdfPageSize.ArchB:
                    return PageSize.ARCH_B;

                case PdfPageSize.ArchA:
                    return PageSize.ARCH_A;

                case PdfPageSize.AmericanFoolscap:
                    return PageSize.FLSA;

                case PdfPageSize.EuropeanFoolscap:
                    return PageSize.FLSE;

                case PdfPageSize.HalfLetter:
                    return PageSize.HALFLETTER;

                case PdfPageSize.Size11X17:
                    return PageSize._11X17;

                case PdfPageSize.ID1:
                    return PageSize.ID_1;

                case PdfPageSize.ID2:
                    return PageSize.ID_2;

                case PdfPageSize.ID3:
                    return PageSize.ID_3;

                case PdfPageSize.Ledger:
                    return PageSize.LEDGER;

                case PdfPageSize.CrownQuarto:
                    return PageSize.CROWN_QUARTO;

                case PdfPageSize.LargeCrownQuarto:
                    return PageSize.LARGE_CROWN_QUARTO;

                case PdfPageSize.DemyQuarto:
                    return PageSize.DEMY_QUARTO;

                case PdfPageSize.RoyalQuarto:
                    return PageSize.ROYAL_QUARTO;

                case PdfPageSize.CrownOctavo:
                    return PageSize.CROWN_OCTAVO;

                case PdfPageSize.LargeCrownOctavo:
                    return PageSize.LARGE_CROWN_OCTAVO;

                case PdfPageSize.DemyOctavo:
                    return PageSize.DEMY_OCTAVO;

                case PdfPageSize.RoyalOctavo:
                    return PageSize.ROYAL_OCTAVO;

                case PdfPageSize.SmallPaperback:
                    return PageSize.SMALL_PAPERBACK;

                case PdfPageSize.PengiunSmallPaperback:
                    return PageSize.PENGUIN_SMALL_PAPERBACK;

                case PdfPageSize.PenguinLargePaparback:
                    return PageSize.PENGUIN_LARGE_PAPERBACK;

                default:
                    throw new InvalidOperationException(pageSize + " is undefined. please use the CustomPageSize to specify it.");
            }
        }
    }
}
