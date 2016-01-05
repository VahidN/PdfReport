using System.IO;
using System.Windows.Forms;

namespace DemosBrowser.Toolkit.AcrobatReader
{
    public partial class AcroPdf : UserControl
    {
        public AcroPdf(string fileName)
        {
            InitializeComponent();
            ShowPdf(fileName);
        }

        public void ShowPdf(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return;
            if (!File.Exists(fileName))
                throw new FileNotFoundException(fileName + " file does not exist.");

            axAcroPDF1.LoadFile(fileName);
            axAcroPDF1.setShowToolbar(true);
            axAcroPDF1.Show();
        }

        protected void DisposeCore()
        {
            if (axAcroPDF1 == null || axAcroPDF1.IsDisposed) return;
            axAcroPDF1.Dispose();
            axAcroPDF1 = null;
        }
    }
}