using System;
using System.Collections.Generic;
using NETMP.Module10.Http.Data;

namespace NETMP.Module10.Http.DialogManager.Interfaces
{
    public interface IUserDialogManager
    {
        string GetInputDirectoryPath();

        string GetSiteUri();

        int GetDepthNumber();

        TransitionToOtherDomainsLimits GetTransitionToOtherDomainsLimits();

        List<string> GetFileExtensionsToExclude();

        void DisplayOperationStartedMessage();

        void DisplayOperationFinishedMessage();

        void DisplayWaitMessage();

        void DisplayExceptionMessage(Exception exception);

        void DisplaySiteNodeFoundMessage(object sender, string uri);

        void DisplaySiteNodeCopiedToFileSystemStepCompletedMessage(object sender, SiteNodeCopiedToFileSystemEventArgs args);
    }
}
