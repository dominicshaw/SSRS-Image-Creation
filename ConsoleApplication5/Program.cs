using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Microsoft.Reporting.WinForms;

namespace ConsoleApplication5
{
    class Program
    {
        static void Main(string[] args)
        {
            SqlServerTypes.Utilities.LoadNativeAssemblies(AppDomain.CurrentDomain.BaseDirectory);

            var rvm = new ReportListViewModel();

            rvm.Add(new ReportViewModel() { Name = "Dom", Score = 1 });
            rvm.Add(new ReportViewModel() { Name = "Ryada", Score = 2 });

            ReportFactory.RunReport(rvm);

            Console.ReadKey();
        }
    }

    public class ReportListViewModel : List<ReportViewModel>
    {
    }

    public class ReportViewModel
    {
        public string Name { get; set; }
        public int Score { get; set; }
    }

    public static class ReportFactory
    {
        private static object _model;

        private static string _mimeType;
        private static string _encoding;
        private static string _fileNameExtension;
        private static Warning[] _warnings;
        private static string[] _streams;

        public static string RunReport(ReportListViewModel model, string format = "BMP", bool show = true)
        {
            _model = model;

            using (var lr = new LocalReport())
            {
                lr.ReportEmbeddedResource = "ConsoleApplication5.Reports.TestReport.rdlc";
                lr.EnableExternalImages = true;

                lr.DataSources.Add(new ReportDataSource("DataSource", _model));

                var renderedBytes = lr.Render
                (
                    "Image",
                    string.Format("<DeviceInfo><OutputFormat>{0}</OutputFormat></DeviceInfo>", format.ToUpper()),
                    out _mimeType,
                    out _encoding,
                    out _fileNameExtension,
                    out _streams,
                    out _warnings
                );

                var nm = "Test Report Image";

                var saveAs = Path.Combine(TempPath, nm + "." + format.ToLower());

                var idx = 0;
                while (File.Exists(saveAs))
                {
                    idx++;
                    saveAs = Path.Combine(TempPath, string.Format("{0}.{1}.{2}", nm, idx, format.ToLower()));
                }

                var stream = new FileStream(saveAs, FileMode.Create, FileAccess.Write);
                stream.Write(renderedBytes, 0, renderedBytes.Length);
                stream.Close();
                stream.Dispose();

                saveAs = Crop(saveAs);

                Process.Start(saveAs);

                return saveAs;
            }
        }

        private static string Crop(string saveAs)
        {
            var fi = new FileInfo(saveAs);

            var newLocation = fi.FullName.Substring(0, fi.FullName.Length - fi.Extension.Length) + ".png";

            var replacement = Crop(new Bitmap(Image.FromFile(saveAs)));
            replacement.Save(newLocation, ImageFormat.Png);

            return newLocation;
        }

        public static Bitmap Crop(Bitmap bmp)
        {
            var w = bmp.Width;
            var h = bmp.Height;

            Func<int, bool> allWhiteRow = row =>
            {
                for (var i = 0; i < w; ++i)
                    if (bmp.GetPixel(i, row).R != 255)
                        return false;
                return true;
            };

            Func<int, bool> allWhiteColumn = col =>
            {
                for (var i = 0; i < h; ++i)
                    if (bmp.GetPixel(col, i).R != 255)
                        return false;
                return true;
            };

            var topmost = 0;
            for (var row = 0; row < h; ++row)
            {
                if (allWhiteRow(row))
                    topmost = row;
                else break;
            }

            var bottommost = 0;
            for (var row = h - 1; row >= 0; --row)
            {
                if (allWhiteRow(row))
                    bottommost = row;
                else break;
            }

            int leftmost = 0, rightmost = 0;
            for (var col = 0; col < w; ++col)
            {
                if (allWhiteColumn(col))
                    leftmost = col;
                else
                    break;
            }

            for (var col = w - 1; col >= 0; --col)
            {
                if (allWhiteColumn(col))
                    rightmost = col;
                else
                    break;
            }

            if (rightmost == 0) rightmost = w; // As reached left
            if (bottommost == 0) bottommost = h; // As reached top.

            var croppedWidth = rightmost - leftmost;
            var croppedHeight = bottommost - topmost;

            if (croppedWidth == 0) // No border on left or right
            {
                leftmost = 0;
                croppedWidth = w;
            }

            if (croppedHeight == 0) // No border on top or bottom
            {
                topmost = 0;
                croppedHeight = h;
            }

            try
            {
                var target = new Bitmap(croppedWidth, croppedHeight);
                using (var g = Graphics.FromImage(target))
                {
                    g.DrawImage(bmp,
                      new RectangleF(0, 0, croppedWidth, croppedHeight),
                      new RectangleF(leftmost, topmost, croppedWidth, croppedHeight),
                      GraphicsUnit.Pixel);
                }
                return target;
            }
            catch (Exception ex)
            {
                throw new Exception(
                  string.Format("Values are topmost={0} btm={1} left={2} right={3} croppedWidth={4} croppedHeight={5}", topmost, bottommost, leftmost, rightmost, croppedWidth, croppedHeight),
                  ex);
            }
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
