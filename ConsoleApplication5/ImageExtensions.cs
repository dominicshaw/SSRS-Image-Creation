using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace ConsoleApplication5
{
    public static class ImageExtensions
    {
        public static string CropAndSaveAsPng(this string imageLocation)
        {
            var fi = new FileInfo(imageLocation);

            var newLocation = fi.FullName.Substring(0, fi.FullName.Length - fi.Extension.Length) + ".png";

            var replacement = Crop(new Bitmap(Image.FromFile(imageLocation)));
            replacement.Save(newLocation, ImageFormat.Png);

            return newLocation;
        }

        private static Bitmap Crop(Bitmap bmp)
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
    }
}