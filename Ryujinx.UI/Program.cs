using Qml.Net;
using Ryujinx.UI.Emulation;
using Ryujinx.UI.UI.Models;
using OpenTK.Platform;

using static Qml.Net.Qml;

namespace Ryujinx.UI
{
    class Program
    {
        static int Main(string[] args)
        {
            QQuickStyle.SetStyle("Fusion");

            using (var Application = new QGuiApplication(args))
            {
                using (var QmlEngine = new QQmlApplicationEngine())
                {
                    RegisterType<EmulationController>("Ryujinx");
                    RegisterType<ConfigurationModel>("Ryujinx");

                    QmlEngine.Load("UI/MainWindow.qml");

                    OpenTK.Toolkit.Init();

                    return Application.Exec();
                }
            }
        }
    }
}
