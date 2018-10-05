using Qml.Net;
using Ryujinx.UI.Emulation;

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

                    QmlEngine.Load("UI/MainWindow.qml");

                    return Application.Exec();
                }
            }
        }
    }
}
