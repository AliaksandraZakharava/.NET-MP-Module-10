using System;

namespace NETMP.Module10.Http.DialogManager.Interfaces
{
    public interface IInputOutputManager
    {
        string ReadMessage();

        void DisplayMessage(string message);

        void DisplayException(Exception exception);

        void WaitForInput();
    }
}
