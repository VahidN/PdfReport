using System.Windows.Forms;
using PdfReportSamples.CalculatedFields;

namespace WindowsFormsAppTests
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.Load += Form1_Load;
            this.FormClosing += Form1_FormClosing;
        }

        void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            axAcroPDF1.Dispose();
            axAcroPDF1 = null;
        }

        /*
         * Add "Adobe PDF Reader" COM Component to your Toolbox.
         * Then just drag it into your Windows Form from the Toolbox.
         */
        void Form1_Load(object sender, System.EventArgs e)
        {
            // To view its implementation, right click on the method and then select `go to implementation` 
            var rpt = new CalculatedFieldsPdfReport().CreatePdfReport();
            axAcroPDF1.LoadFile(rpt.FileName);            
            axAcroPDF1.setShowToolbar(true);
            axAcroPDF1.Show();
        }
    }
}
