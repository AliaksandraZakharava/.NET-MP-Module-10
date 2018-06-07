using System;
using NETMP.Module10.Http.DialogManager.Interfaces;

namespace NETMP.Module10.Http.DialogManager.Console
{
    public class InputOutputManager : IInputOutputManager
    {
        public string ReadMessage()
        {
            return System.Console.ReadLine();
        }

        public void DisplayMessage(string message)
        {
            System.Console.WriteLine($"{message}");
        }

        public void DisplayException(Exception exception)
        {
            System.Console.WriteLine($"{Environment.NewLine}Finished with exception: {exception.Message}");

            while (exception.InnerException != null)
            {
                System.Console.WriteLine(exception.InnerException.Message);

                exception = exception.InnerException;
            }
        }

        public void WaitForInput()
        {
            System.Console.ReadKey();
        }
    }
}
