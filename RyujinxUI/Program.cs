using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gtk;

namespace RyujinxUI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Application.Init();
            MainWindow mainWindow = MainWindow.CreateWindow();
            mainWindow.Show();
            Application.Run();
        }
    }
}
