using Qml.Net;
using System;

namespace Ryujinx.UI
{
    class Program
    {
        static int Main(string[] args)
        {
            QQuickStyle.SetStyle("Material");

            using (var Application = new QGuiApplication(args))
            {
                using (var QmlEngine = new QQmlApplicationEngine())
                {
                    QmlEngine.Load("UI/MainWindow.qml");

                    return Application.Exec();
                }
            }
        }
    }
}
