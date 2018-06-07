using System.Text.RegularExpressions;

namespace NETMP.Module10.Http.CopyingManager.Helpers
{
    public static class UriHelper
    {
        private const string Telephone = "tel";
        private const string Protocol = "http";
        private const string DoubleBackSlash = @"//";

        private static readonly Regex _hostRegex;

        static UriHelper()
        {
            _hostRegex = new Regex(@"^(([^:\/?#]+):)?(\/\/([^\/?#]*))?", RegexOptions.Compiled);
        }

        public static bool HasExtension(string uri)
        {
            return FileSystemHelper.HasExtension(uri);
        }

        public static bool IsMediaFile(string uri)
        {
            return HasExtension(uri) && ImageExtensionsHelper.SupportedImageExtensions.Contains(GetExtension(uri));
        }

        public static bool IsValidLink(string link)
        {
            return !string.IsNullOrEmpty(link) && !link.Contains(Telephone);
        }

        public static string GetExtension(string uri)
        {
            return FileSystemHelper.GetExtension(uri);
        }

        public static string GetHost(string uri)
        {
            return _hostRegex.Match(uri).Value;
        }

        public static string GetAbsoluteLink(string link, string rootLink)
        {
            if (!link.Contains(Protocol))
            {
                if (link.StartsWith(DoubleBackSlash))
                {
                    return link.Replace(DoubleBackSlash, $"{Protocol}://");
                }

                var host = GetHost(rootLink);

                return string.Concat(host, link);
            }

            return link;
        }
    }
}
