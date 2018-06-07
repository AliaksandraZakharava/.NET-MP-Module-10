using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NETMP.Module10.Http.Data;
using NETMP.Module10.Http.DialogManager.Interfaces;

namespace NETMP.Module10.Http.DialogManager.Console
{
    public class UserDialogManager : IUserDialogManager
    {
        private readonly IInputOutputManager _inputOutputManager;

        public UserDialogManager(IInputOutputManager inputOutputManager)
        {
            _inputOutputManager = inputOutputManager ?? throw new ArgumentNullException(nameof(inputOutputManager));
        }

        public string GetInputDirectoryPath()
        {
            Func<string, bool> limitationsOnValue = value =>string.IsNullOrEmpty(Path.GetFileName(value)) || string.IsNullOrEmpty(Path.GetExtension(value));

            return GetInputValue(limitationsOnValue, 
                                "Please, input output path.", 
                                "Input value is not a file path or file name. Please, try again.");
        }

        public string GetSiteUri()
        {
            Func<string, bool> limitationsOnValue = value => string.IsNullOrEmpty(value);

            return GetInputValue(limitationsOnValue, 
                                "Please, input site uri.", 
                                "Please, input site uri.");
        }

        public int GetDepthNumber()
        {
            Func<string, bool> limitationsOnValue = value => Int32.TryParse(value, out var number);

            var result = GetInputValue(limitationsOnValue, 
                                        "Please, input depth number.", 
                                        "Input value is not a number. Please, try again.");

            return Int32.Parse(result);
        }

        public TransitionToOtherDomainsLimits GetTransitionToOtherDomainsLimits()
        {
            var validValues = new List<string> { "-n", "-cd", "-cu"};

            Func<string, bool> limitationsOnValue = value => string.IsNullOrEmpty(value) || !validValues.Contains(value);

            var result =  GetInputValue(limitationsOnValue, 
                                        "Please, input -n for 'no limits, -cd for 'current domain', -cu for 'current uri:", 
                                        "Please, input valid limit value.");

            switch (result.Trim())
            {
                case "-n": return TransitionToOtherDomainsLimits.NoLimits;
                case "-cd": return TransitionToOtherDomainsLimits.OnlyInsideCurrentDomain;
                case "-cu": return TransitionToOtherDomainsLimits.NotHigherThenPassedUri;
                default: return TransitionToOtherDomainsLimits.NoLimits;
            }
        }

        public List<string> GetFileExtensionsToExclude()
        {
            Func<string, bool> limitationsOnValue = value => string.IsNullOrEmpty(value) || !value.Split(',').Any();

            var result = GetInputValue(limitationsOnValue, 
                                       "Please, input file extensions to exclude separated by commas in the format '.extension'. Press any button to skip.", 
                                       "Can not parse values. Please, input file extensions to exclude", true);

            return result.Split(',').Select(item => item.Trim()).ToList();
        }

        public void DisplayOperationStartedMessage()
        {
            _inputOutputManager.DisplayMessage("Starting processing...");
        }

        public void DisplayOperationFinishedMessage()
        {
            _inputOutputManager.DisplayMessage("Finished processing.");
        }

        public void DisplayWaitMessage()
        {
            _inputOutputManager.DisplayMessage("Press any key to exit...");
            _inputOutputManager.WaitForInput();
        }

        public void DisplayExceptionMessage(Exception exception)
        {
            _inputOutputManager.DisplayException(exception);
        }

        public void DisplaySiteNodeFoundMessage(object sender, string uri)
        {
            _inputOutputManager.DisplayMessage($"Source found: {uri}");
        }

        public void DisplaySiteNodeCopiedToFileSystemStepCompletedMessage(object sender,
            SiteNodeCopiedToFileSystemEventArgs args)
        {
            var uri = FormatOutputUri(args.SiteNodeUri);
            var percent = args.SiteNodeNumber / args.TotalSiteNodeNumbers * 100;

            _inputOutputManager.DisplayMessage($"{uri}________________________________________copied ({percent:0.00}% completed)");
        }

        #region Private methods

            public string GetInputValue(Func<string, bool> limitationsOnValue, string askMessage, string retryMessage, bool canBeEmpty = false)
        {
            _inputOutputManager.DisplayMessage(askMessage);

            var value = _inputOutputManager.ReadMessage().Trim();

            if (canBeEmpty && string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            while (limitationsOnValue(value))
            {
                _inputOutputManager.DisplayMessage(retryMessage);

                value = _inputOutputManager.ReadMessage();
            }

            return value;
        }

        private string FormatOutputUri(string uri)
        {
            if (uri.Length >= 50)
            {
                return $"{uri.Substring(0, 47)}...";
            }

            var sb = new StringBuilder(uri);

            while (sb.Length < 50) sb.Append("_");

            return sb.ToString();
        }

        #endregion
    }
}
