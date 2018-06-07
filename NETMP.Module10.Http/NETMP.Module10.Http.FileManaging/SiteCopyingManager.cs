using System;
using System.Collections.Generic;
using System.Linq;
using NETMP.Module10.Http.CopyingManager.Helpers;
using NETMP.Module10.Http.Data;
using NETMP.Module10.Http.HttpManager.Interfaces;

namespace NETMP.Module10.Http.CopyingManager
{
    public class SiteCopyingManager
    {
        private readonly IHttpResponseProvider _httpResponseProvider;
        private readonly IHtmlCrawler _htmlCrawler;

        public event EventHandler<string> SiteNodeFounded;
        public event EventHandler<SiteNodeCopiedToFileSystemEventArgs> SiteNodeCopiedToFileSystem;

        private readonly bool _verboseOn;
        private string _rootUri;

        public SiteCopyingManager(IHttpResponseProvider httpResponseProvider, IHtmlCrawler htmlCrawler, bool turnVerboseOn = true)
        {
            _httpResponseProvider = httpResponseProvider ?? throw new ArgumentNullException(nameof(httpResponseProvider));
            _htmlCrawler = htmlCrawler ?? throw new ArgumentNullException(nameof(_htmlCrawler));

            _verboseOn = turnVerboseOn;
        }

        public void CopySite(string uri, string outPath, int depthLevel,
                             TransitionToOtherDomainsLimits transactionLimits = TransitionToOtherDomainsLimits.NoLimits,
                             List<string> excludeFileExtensions = null)
        {
            if (string.IsNullOrEmpty(uri))
            {
                throw new ArgumentException("Null or empty uri.");
            }

            if (depthLevel < 0)
            {
                throw new ArgumentException("Negative depth level value.");
            }

            if (excludeFileExtensions != null)
            {
                ImageExtensionsHelper.ExcludeImageExtensions(excludeFileExtensions);
            }

            _rootUri = uri;

            var siteNodes = GetAllSiteNodes(uri, depthLevel, transactionLimits).ToList();

            WriteSiteNodesToFileSystem(siteNodes, outPath);
        }

        #region Private methods

        private IEnumerable<SiteNode> GetAllSiteNodes(string rootUri, int depthLevel, TransitionToOtherDomainsLimits transactionLimits)
        {
            while (depthLevel >= 0)
            {
                depthLevel--;

                var rootNode = SiteNodeHelper.GetFilledSiteNode(rootUri, _httpResponseProvider);

                OnSiteNodeFounded(this, rootNode.Uri);

                yield return rootNode;

                var links = SiteNodeHelper.GetSiteNodeLinks(_rootUri, rootNode, transactionLimits, _htmlCrawler);

                foreach (var link in links)
                {
                    var absoluteLink = UriHelper.GetAbsoluteLink(link, rootNode.Uri);

                    var linkNodes = GetAllSiteNodes(absoluteLink, depthLevel, transactionLimits);

                    foreach (var node in linkNodes)
                    {
                        yield return node;
                    }
                }
            }
        }

        private void WriteSiteNodesToFileSystem(IEnumerable<SiteNode> siteNodes, string outPath)
        {
            FileSystemHelper.CreateDirectory(outPath);

            var totalNodesCount = siteNodes.Count();
            var nodeNumber = 1;

            foreach (var node in siteNodes)
            {
                try
                {
                    var uriParts = SiteNodeHelper.GetSiteNodeUriParts(node);

                    var writePath = SiteNodeHelper.CreateAndGetWritePathForAUri(uriParts, outPath);

                    SiteNodeHelper.WriteSiteNodeToFileSystem(node, writePath);

                    OnSiteNodeCopiedToFileSystem(this, new SiteNodeCopiedToFileSystemEventArgs(node.Uri, nodeNumber, totalNodesCount));
                }
                catch(InvalidOperationException exc)
                {
                    OnSiteNodeCopiedToFileSystem(this, new SiteNodeCopiedToFileSystemEventArgs(exc.Message, nodeNumber, totalNodesCount));
                }
                finally
                {
                    nodeNumber++;
                }
            }
        }

        private void OnSiteNodeFounded(object sender, string e)
        {
            if (_verboseOn)
            {
                var handler = SiteNodeFounded;

                handler?.Invoke(sender, e);
            }
        }

        private void OnSiteNodeCopiedToFileSystem(object sender, SiteNodeCopiedToFileSystemEventArgs e)
        {
            if (_verboseOn)
            {
                var handler = SiteNodeCopiedToFileSystem;

                handler?.Invoke(sender, e);
            }
        }

        #endregion
    }
}
