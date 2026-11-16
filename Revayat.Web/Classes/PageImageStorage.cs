using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web;

namespace Revayat.Web.Classes
{
    public static class PageImageStorage
    {
        private const int MaximumBytes = 5 * 1024 * 1024;
        private const long MaximumPixels = 20000000;

        public static bool TrySave(HttpPostedFileBase upload, string directory, out string fileName, out string error)
        {
            fileName = null;
            error = null;
            if (upload == null || upload.ContentLength <= 0 || upload.ContentLength > MaximumBytes)
            {
                error = "تصویر باید بین ۱ بایت و ۵ مگابایت باشد.";
                return false;
            }

            var extension = (Path.GetExtension(upload.FileName ?? "") ?? "").ToLowerInvariant();
            if (extension != ".jpg" && extension != ".jpeg" && extension != ".png")
            {
                error = "فقط فایل JPEG یا PNG پذیرفته می‌شود.";
                return false;
            }

            try
            {
                using (var image = Image.FromStream(upload.InputStream, true, true))
                {
                    var isJpeg = image.RawFormat.Guid == ImageFormat.Jpeg.Guid;
                    var isPng = image.RawFormat.Guid == ImageFormat.Png.Guid;
                    if ((!isJpeg && !isPng) || (isJpeg && extension == ".png") ||
                        (isPng && extension != ".png") || image.Width == 0 || image.Height == 0 ||
                        (long)image.Width * image.Height > MaximumPixels)
                    {
                        error = "محتوای تصویر معتبر نیست یا ابعاد آن بیش از حد بزرگ است.";
                        return false;
                    }

                    fileName = Guid.NewGuid().ToString("N") + (isJpeg ? ".jpg" : ".png");
                    Directory.CreateDirectory(directory);
                    try
                    {
                        image.Save(Path.Combine(directory, fileName), isJpeg ? ImageFormat.Jpeg : ImageFormat.Png);
                    }
                    catch
                    {
                        DeleteIfOwned(directory, fileName);
                        throw;
                    }
                    return true;
                }
            }
            catch (ArgumentException)
            {
                error = "فایل انتخاب‌شده تصویر معتبر نیست.";
            }
            catch (OutOfMemoryException)
            {
                error = "فایل انتخاب‌شده تصویر معتبر نیست.";
            }

            fileName = null;
            return false;
        }

        public static void DeleteIfOwned(string directory, string fileName)
        {
            if (string.IsNullOrEmpty(fileName) || Path.GetFileName(fileName) != fileName)
                return;

            Guid identifier;
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            if (!Guid.TryParse(Path.GetFileNameWithoutExtension(fileName), out identifier) ||
                (extension != ".jpg" && extension != ".jpeg" && extension != ".png" && extension != ".gif"))
                return;

            var path = Path.Combine(directory, fileName);
            if (File.Exists(path))
                File.Delete(path);
        }
    }
}
