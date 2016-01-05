using System;
using System.Web.UI;

namespace SlPdf.Web
{
    public partial class ShowPdf : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request["file"] != null)
            {
                Response.Redirect(Request["file"]);
            }
        }
    }
}