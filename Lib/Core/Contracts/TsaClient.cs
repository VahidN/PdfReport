
namespace PdfRpt.Core.Contracts
{
    /// <summary>
    /// Timestamp Authority Client's info
    /// </summary>
    public class TsaClient
    {
        /// <summary>
        /// URL of the Time Stamp Authority
        /// </summary>
        public string Url { set; get; }

        /// <summary>
        /// TSA Username
        /// </summary>
        public string UserName { set; get; }

        /// <summary>
        /// TSA password
        /// </summary>
        public string Password { set; get; }
    }
}
