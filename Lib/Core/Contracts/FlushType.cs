
namespace PdfRpt.Core.Contracts
{
    /// <summary>
    /// How to flush an in memory PDF file.
    /// </summary>
    public enum FlushType
    {
        /// <summary>
        /// Content-Disposition: attachment. Force download and display download popup.
        /// </summary>
        Attachment,

        /// <summary>
        /// Content-Disposition: inline. Display PDF in the browser.
        /// </summary>
        Inline
    }
}