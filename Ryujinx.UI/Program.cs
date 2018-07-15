using System;

namespace Ryujinx.UI
{
    class Program
    {
        static void Main(string[] args)
        {
            EmulationWindow MainUI = new EmulationWindow();

            MainUI.MainLoop();

            Environment.Exit(0);
        }
    }
}
