using System.Drawing;
using System.IO;
//It's part of the .NET 4.0+ now.
using System.Windows.Forms.DataVisualization.Charting;
using iTextSharp.text;
using iTextSharp.text.pdf;
using PdfRpt.Core.Helper;
using PdfRpt.Core.Contracts;

namespace PdfReportSamples.ChartImage
{
    public class MSChartHelper
    {
        // MS Chart learning tutorials:
        // http://weblogs.asp.net/scottgu/archive/2010/02/07/built-in-charting-controls-vs-2010-and-net-4-series.aspx
        Chart _chart;

        public System.Drawing.Font AxisTitleFont { set; get; }

        public string AxisXTitle { set; get; }

        public string AxisYTitle { set; get; }

        public string ChartTitle { set; get; }

        public System.Drawing.Font ChartTitleFont { set; get; }

        public System.Drawing.Font LabelStyleFont { set; get; }

        public System.Drawing.Font LegendsFont { set; get; }

        public void AddChartToPage(Document pdfDoc,
                                   int scalePercent = 100,
                                   float spacingBefore = 50,
                                   float spacingAfter = 10,
                                   float widthPercentage = 80)
        {
            using (var chartimage = new MemoryStream())
            {
                _chart.SaveImage(chartimage, ChartImageFormat.Bmp); //BMP gives the best compression result

                var iTextSharpImage = PdfImageHelper.GetITextSharpImageFromByteArray(chartimage.ToArray());
                iTextSharpImage.ScalePercent(scalePercent);
                iTextSharpImage.Alignment = Element.ALIGN_CENTER;

                var table = new PdfGrid(1)
                {
                    WidthPercentage = widthPercentage,
                    SpacingBefore = spacingBefore,
                    SpacingAfter = spacingAfter
                };
                table.AddCell(iTextSharpImage);

                pdfDoc.Add(table);
            }
        }

        /// <summary>
        /// Do not pass an object to the AddXY method here.
        /// Extract its X and Y values and cast them to one of the following types directly, before using them:
        /// Double, Decimal, Single, int, long, uint, ulong, String, DateTime, short, ushort
        /// </summary>
        public void AddXY(object xValue, params object[] yValue)
        {
            _chart.Series[0].Points.AddXY(xValue, yValue);
        }

        public void ChartInit(int width, int height)
        {
            _chart = new Chart
            {
                Width = width,
                Height = height,
                AntiAliasing = AntiAliasingStyles.All,
                TextAntiAliasingQuality = TextAntiAliasingQuality.High,
                Palette = ChartColorPalette.BrightPastel,
                BackColor = ColorTranslator.FromHtml("#F3DFC1"),
                BackGradientStyle = GradientStyle.TopBottom
            };

            setBorder();
            setTitles();
            setChartAreas();
            setLegends();
            setSeries();
        }

        public void FreeResources()
        {
            if (_chart != null && !_chart.IsDisposed)
                _chart.Dispose();
        }        

        private void setBorder()
        {
            _chart.BorderSkin.SkinStyle = BorderSkinStyle.Emboss;
            _chart.BorderlineWidth = 2;
            _chart.BorderlineColor = Color.FromArgb(181, 64, 1);
            _chart.BorderlineDashStyle = ChartDashStyle.Solid;
        }

        private void setChartAreas()
        {
            _chart.ChartAreas.Add("ChartArea1");
            _chart.ChartAreas[0].AxisX.Title = AxisXTitle;
            _chart.ChartAreas[0].AxisY.Title = AxisYTitle;
            _chart.ChartAreas[0].AxisX.TitleFont = AxisTitleFont;
            _chart.ChartAreas[0].AxisY.TitleFont = AxisTitleFont;
            _chart.ChartAreas[0].AxisX.LabelStyle.Font = LabelStyleFont;
            _chart.ChartAreas[0].AxisX.LabelStyle.Angle = -90;
            _chart.ChartAreas[0].BackColor = Color.White;
            _chart.ChartAreas[0].AxisX.LineColor = Color.FromArgb(64, 64, 64);
            _chart.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.FromArgb(64, 64, 64);
            _chart.ChartAreas[0].AxisY.LineColor = Color.FromArgb(64, 64, 64);
            _chart.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.FromArgb(64, 64, 64);
        }

        private void setLegends()
        {
            _chart.Legends.Add("Default");
            _chart.Legends[0].LegendStyle = LegendStyle.Row;
            _chart.Legends[0].IsTextAutoFit = false;
            _chart.Legends[0].DockedToChartArea = "ChartArea1";
            _chart.Legends[0].Docking = Docking.Bottom;
            _chart.Legends[0].IsDockedInsideChartArea = false;
            _chart.Legends[0].BackColor = Color.Transparent;
            _chart.Legends[0].Font = LegendsFont;
        }

        private void setSeries()
        {
            _chart.Series.Add("");
            _chart.Series[0].ChartType = SeriesChartType.Column;
            _chart.Series[0].Palette = ChartColorPalette.EarthTones;
            _chart.Series[0].IsValueShownAsLabel = true;
            _chart.Series[0].IsVisibleInLegend = false;
        }

        private void setTitles()
        {
            _chart.Titles.Add(ChartTitle);
            _chart.Titles[0].Font = ChartTitleFont;
            _chart.Titles[0].TextStyle = TextStyle.Shadow;
            _chart.Titles[0].ShadowOffset = 3;
            _chart.Titles[0].ShadowColor = Color.FromArgb(32, 0, 0);
            _chart.Titles[0].Alignment = ContentAlignment.TopCenter;
            _chart.Titles[0].ForeColor = Color.FromArgb(26, 59, 105);
        }
    }
}
