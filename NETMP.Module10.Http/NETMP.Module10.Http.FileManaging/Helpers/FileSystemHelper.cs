using System.Drawing;
using System.IO;

namespace NETMP.Module10.Http.CopyingManager.Helpers
{
    public static class FileSystemHelper
    {
        public static bool HasExtension(string uri)
        {
            return !string.IsNullOrEmpty(GetExtension(uri));
        }

        public static string GetExtension(string uri)
        {
            return Path.GetExtension(uri);
        }

        public static void CreateDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public static void CreateTextFile(string writePath, string text)
        {
            File.Create(writePath).Close();

            File.WriteAllText(writePath, text);
        }

        public static void CreateImageFile(string writePath, byte[] imageBytes)
        {
            File.Create(writePath).Close();

            using (var ms = new MemoryStream(imageBytes))
            {
                var image = Image.FromStream(ms);

                image.Save(writePath, ImageExtensionsHelper.GetImageFormat(GetExtension(writePath)));
            }
        }
    }
}
