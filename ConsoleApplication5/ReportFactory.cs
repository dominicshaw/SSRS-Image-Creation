﻿using System.Diagnostics;
using System.IO;
using Microsoft.Reporting.WinForms;

namespace ConsoleApplication5
{
    public static class ReportFactory
    {
        private static object _model;

        private static string _mimeType;
        private static string _encoding;
        private static string _fileNameExtension;
        private static Warning[] _warnings;
        private static string[] _streams;

        public static string RunReport(ReportListViewModel model)
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
                    "<DeviceInfo><OutputFormat>BMP</OutputFormat></DeviceInfo>",
                    out _mimeType,
                    out _encoding,
                    out _fileNameExtension,
                    out _streams,
                    out _warnings
                );

                var nm = "Test Report Image";

                var saveAs = Path.Combine(TempPath, nm + ".bmp");

                var idx = 0;
                while (File.Exists(saveAs))
                {
                    idx++;
                    saveAs = Path.Combine(TempPath, string.Format("{0}.{1}.bmp", nm, idx));
                }

                var stream = new FileStream(saveAs, FileMode.Create, FileAccess.Write);
                stream.Write(renderedBytes, 0, renderedBytes.Length);
                stream.Close();
                stream.Dispose();

                saveAs = saveAs.CropAndSaveAsPng(); // normally could just show it but if we want to remove all the white space we can do this

                Process.Start(saveAs);

                return saveAs;
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