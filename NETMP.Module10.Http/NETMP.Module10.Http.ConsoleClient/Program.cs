using NETMP.Module10.Http.DialogManager.Interfaces;
using System;
using NETMP.Module10.Http.CopyingManager;
using NETMP.Module10.Http.DialogManager.Console;
using NETMP.Module10.Http.HttpManager;
using NETMP.Module10.Http.HttpManager.Interfaces;

namespace NETMP.Module10.Http.ConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            IInputOutputManager inputOutputManger = new InputOutputManager();
            IUserDialogManager dialogManager = new UserDialogManager(inputOutputManger);

            IHttpResponseProvider httpResponseProvider = new HttpResponseProvider();
            IHtmlCrawler htmlCrawler = new HtmlCrawler();

            SiteCopyingManager siteCopyingManager = new SiteCopyingManager(httpResponseProvider, htmlCrawler);

            try
            {
                var outputPath = dialogManager.GetInputDirectoryPath();
                var siteUri = dialogManager.GetSiteUri();
                var depth = dialogManager.GetDepthNumber();
                var limits = dialogManager.GetTransitionToOtherDomainsLimits();
                var extensionsToExclude = dialogManager.GetFileExtensionsToExclude();

                siteCopyingManager.SiteNodeFounded += dialogManager.DisplaySiteNodeFoundMessage;
                siteCopyingManager.SiteNodeCopiedToFileSystem += dialogManager.DisplaySiteNodeCopiedToFileSystemStepCompletedMessage;

                siteCopyingManager.CopySite(siteUri, outputPath, depth, limits, extensionsToExclude);
            }
            catch (Exception exc)
            {
                dialogManager.DisplayExceptionMessage(exc);
            }
            finally
            {
                dialogManager.DisplayOperationFinishedMessage();
                dialogManager.DisplayWaitMessage();
            }
        }
    }
}
