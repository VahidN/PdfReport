using System.Windows.Forms;

namespace WpfPdfViewer.Controls
{
    public partial class AcroReader : UserControl
    {
        public AcroReader(string fileName)
        {
            InitializeComponent();
            ShowPdf(fileName);
        }

        public void ShowPdf(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return;
            axAcroPDF1.LoadFile(fileName);
            axAcroPDF1.setShowToolbar(true);
            axAcroPDF1.Show();
        }
    }
}
