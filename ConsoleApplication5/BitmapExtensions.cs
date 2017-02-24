using System;
using System.Drawing;

namespace ConsoleApplication5
{
    public static class BitmapExtensions
    {
        public static Bitmap Crop(this Bitmap bmp)
        {
            int topmost, bottommost, leftmost, rightmost, croppedWidth, croppedHeight;

            GetMargins(bmp, out topmost, out bottommost, out leftmost, out rightmost, out croppedWidth, out croppedHeight);

            try
            {
                var target = new Bitmap(croppedWidth - 2, croppedHeight - 2);

                using (var g = Graphics.FromImage(target))
                {
                    g.DrawImage(bmp,
                        new RectangleF(-1, -1, croppedWidth - 1, croppedHeight - 1),
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

        private static void GetMargins(Bitmap bmp, out int topmost, out int bottommost, out int leftmost, out int rightmost, out int croppedWidth, out int croppedHeight)
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

            topmost = 0;
            for (var row = 0; row < h; ++row)
            {
                if (allWhiteRow(row))
                    topmost = row;
                else break;
            }

            bottommost = 0;
            for (var row = h - 1; row >= 0; --row)
            {
                if (allWhiteRow(row))
                    bottommost = row;
                else break;
            }

            leftmost = 0;
            rightmost = 0;
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

            croppedWidth = rightmost - leftmost;
            croppedHeight = bottommost - topmost;

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
        }
    }
}