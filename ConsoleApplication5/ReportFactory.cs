using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using log4net;
using Microsoft.Reporting.WinForms;

namespace ConsoleApplication5
{
    public static class ReportFactory
    {
        private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static ReportListViewModel _model;

        public static string RunReport(ReportListViewModel model)
        {
            _model = model;

            using (var lr = new LocalReport())
            {
                lr.ReportEmbeddedResource = "ConsoleApplication5.Reports.TestReport.rdlc";
                lr.EnableExternalImages = true;

                lr.SubreportProcessing += lr_SubreportProcessing;

                lr.DataSources.Add(new ReportDataSource("DataSource", _model));
                
                var renderedBytes = lr.Render
                (
                    "Image",
                    "<DeviceInfo><OutputFormat>BMP</OutputFormat></DeviceInfo>",
                    out string _,
                    out string _,
                    out string _,
                    out string[] _,
                    out var warnings
                );

                foreach (var w in warnings)
                    _log.Warn($"{w.Code}: {w.Message}");

                const string nm = "Test Report Image";

                var path = Path.Combine(TempPath, nm + ".bmp");

                var idx = 0;
                while (File.Exists(path))
                {
                    idx++;
                    path = Path.Combine(TempPath, string.Format("{0}.{1}.bmp", nm, idx));
                }

                using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    stream.Write(renderedBytes, 0, renderedBytes.Length);
                    stream.Close();
                }

                path = CropAndSaveAsPng(path); // normally could just show it but if we want to remove all the white space we can do this

                Process.Start(path);
                _log.Info("Generated to " + path);

                return path;
            }
        }

        private static void lr_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            var userId = int.Parse(e.Parameters[0].Values[0]);
            e.DataSources.Add(new ReportDataSource("SubDataSet", _model.First(x => x.UserID == userId).Items));
        }

        private static string CropAndSaveAsPng(string saveAs)
        {
            var fi = new FileInfo(saveAs);

            var newLocation = fi.FullName.Substring(0, fi.FullName.Length - fi.Extension.Length) + ".png";

            var bmp = new Bitmap(Image.FromFile(fi.FullName));

            var replacement = bmp.Crop();
            replacement.Save(newLocation, ImageFormat.Png);

            return newLocation;
        }

        private static string TempPath
        {
            get
            {
                var dir = Path.Combine(Path.GetTempPath(), "Test");

                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                return dir;
            }
        }
    }
}