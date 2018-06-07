using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NETMP.Module10.Http.Data;
using NETMP.Module10.Http.HttpManager.Interfaces;

namespace NETMP.Module10.Http.CopyingManager.Helpers
{
    public static class SiteNodeHelper
    {
        private const string UrlSchemeSeparator = @"//";
        private const string HtmlFileExtension = ".html";
        private const string UriWithoutParamsRegexString = @"[^\?]+\?";
        private const string UriWithoutTagsRegexString = @".+\#";
        private const string UriWithoutMailToRegexString = @".+(?=mail)";

        public static SiteNode GetFilledSiteNode(string rootUri, IHttpResponseProvider httpResponseProvider)
        {
            var rootNode = new SiteNode(rootUri);

            if (UriHelper.IsMediaFile(rootUri))
            {
                try
                {
                    rootNode.Media = httpResponseProvider.RequestLinkBytes(rootUri);
                }
                catch
                {
                    rootNode.Media = new byte[0];
                }
            }
            else
            {
                try
                {
                    rootNode.Html = httpResponseProvider.RequestHttpLayout(rootUri);
                }
                catch
                {
                    rootNode.Html = string.Empty;
                }

            }

            return rootNode;
        }

        public static IEnumerable<string> GetSiteNodeLinks(string rootUri, SiteNode node, TransitionToOtherDomainsLimits transactionLimits, IHtmlCrawler htmlCrawler)
        {
            var links = htmlCrawler.FindHtmlPageLinks(node.Html).Where(UriHelper.IsValidLink);
            return FilterLinksAccordingToTransitionToOtherDomainsLimits(rootUri, links, transactionLimits);
        }

        public static IEnumerable<string> GetSiteNodeUriParts(SiteNode node)
        {
            return node.Uri.Split(new[] { UrlSchemeSeparator }, StringSplitOptions.None).Last().Split('/')
                           .Where(part => !string.IsNullOrEmpty(part))
                           .Select(part => !part.Contains("?") ? part : Regex.Match(part, UriWithoutParamsRegexString).Value.TrimEnd('?'))
                           .Select(part => !part.Contains("#") ? part : Regex.Match(part, UriWithoutTagsRegexString).Value.TrimEnd('#'))
                           .Select(part => !part.Contains("mailto") ? part : Regex.Match(part, UriWithoutMailToRegexString).Value)
                           .ToList();
        }

        public static string CreateAndGetWritePathForAUri(IEnumerable<string> uriParts, string outPath)
        {
             var writePath = outPath;

            foreach (var uriPart in uriParts)
            {
                writePath = Path.Combine(writePath, uriPart);

                if (!uriPart.EndsWith(HtmlFileExtension))
                {
                    FileSystemHelper.CreateDirectory(writePath);
                }
            }

            if (!FileSystemHelper.HasExtension(writePath) ||
                 FileSystemHelper.GetExtension(writePath) != HtmlFileExtension)
            {
                if (!ImageExtensionsHelper.SupportedImageExtensions.Contains(FileSystemHelper.GetExtension(writePath)))
                {
                    writePath = Path.Combine(writePath, $"{uriParts.Last()}{HtmlFileExtension}");
                }
            }

            return writePath;
        }

        public static void WriteSiteNodeToFileSystem(SiteNode node, string writePath)
        {
            File.Create(writePath).Close();

            if (node.Media != null)
            {
                FileSystemHelper.CreateImageFile(writePath, node.Media);
            }
            else if (!string.IsNullOrEmpty(node.Html))
            {
                FileSystemHelper.CreateTextFile(writePath, node.Html);
            }
            else
            {
                throw new InvalidOperationException($"Strange link (-_-): {node.Uri} ");
            }
        }

        #region Private methods

        private static IEnumerable<string> FilterLinksAccordingToTransitionToOtherDomainsLimits(string rootUri, IEnumerable<string> links, TransitionToOtherDomainsLimits transactionLimits)
        {
            switch (transactionLimits)
            {
                case TransitionToOtherDomainsLimits.NoLimits:
                    return links;
                case TransitionToOtherDomainsLimits.OnlyInsideCurrentDomain:
                    return links.Where(link => link.Contains(UriHelper.GetHost(rootUri)));
                case TransitionToOtherDomainsLimits.NotHigherThenPassedUri:
                    return links.Where(link => link.Contains(rootUri));
                default:
                    return Enumerable.Empty<string>();
            }
        }

        #endregion
    }
}
