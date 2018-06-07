using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;

namespace NETMP.Module10.Http.CopyingManager.Helpers
{
    public static class ImageExtensionsHelper
    {
        private static List<string> _supportedImageExtensions;

        public static List<string> SupportedImageExtensions => _supportedImageExtensions;

        static ImageExtensionsHelper()
        {
            _supportedImageExtensions = new List<string> { ".jpg", ".jpeg", ".tif", ".tiff", ".png", ".gif",".bmp", ".ico" };
        }

        public static void ExcludeImageExtensions(List<string> excludeExtensions)
        {
            _supportedImageExtensions = _supportedImageExtensions.Except(excludeExtensions).ToList();
        }

        public static ImageFormat GetImageFormat(string extension)
        {
            if (!SupportedImageExtensions.Contains(extension))
            {
                throw new ArgumentException("Not supported extension");
            }

            switch (extension)
            {
                case ".jpg":
                case ".jpeg":return ImageFormat.Jpeg;
                case ".tif":
                case ".tiff":return ImageFormat.Tiff;
                case ".png": return ImageFormat.Png;
                case ".gif": return ImageFormat.Gif;
                case ".bmp": return ImageFormat.Bmp;
                case ".ico": return ImageFormat.Icon;
                default: return null;
            }
        }
    }
}
