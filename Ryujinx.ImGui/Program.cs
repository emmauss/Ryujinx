using System;

namespace Ryujinx.UI
{
    class Program
    {
        static void Main(string[] args)
        {
            EmulationWindow mainUI = new EmulationWindow();
            mainUI.Run(60.0, 60.0);

            Environment.Exit(0);
        }
    }
}
