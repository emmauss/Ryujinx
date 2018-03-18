using System;
using Eto.Forms;

namespace Ryujinx.EUI.Core
{
    class Program
    {
        static void Main(string[] args)
        {
            var platform = new Eto.GtkSharp.Platform();
            new Application(platform).Run(new MainForm());
        }
    }
}
