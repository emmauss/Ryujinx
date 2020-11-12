using ARMeilleure.Translation.PTC;
using Ryujinx.Common.Logging;
using Ryujinx.Common.SystemInfo;
using Ryujinx.Configuration;
using OpenTK;
using System;
using System.IO;
using System.Reflection;
using Ryujinx.HLE.FileSystem;
using Ryujinx.HLE.FileSystem.Content;
using Ryujinx.Skia.Ui;
using System.Threading.Tasks;
using Ryujinx.Skia.Ui.Skia.Scene;
using Ryujinx.Skia.App;
using System.Threading;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Topten.RichTextKit;
using Ryujinx.Common.Configuration;

namespace Ryujinx.Skia
{
    class Program
    {
        public static string Version { get; private set; }

        public static string ConfigurationPath { get; set; }

        static void Main(string[] args)
        {
            // Parse Arguments
            string launchPath = null;
            string baseDirPath = null;
            for (int i = 0; i < args.Length; ++i)
            {
                string arg = args[i];

                if (arg == "-r" || arg == "--root-data-dir")
                {
                    if (i + 1 >= args.Length)
                    {
                        Logger.Error?.Print(LogClass.Application, $"Invalid option '{arg}'");
                        continue;
                    }

                    baseDirPath = args[++i];
                }
                else if (launchPath == null)
                {
                    launchPath = arg;
                }
            }

            Version = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;

            Console.Title = $"Ryujinx Console {Version}";

            string systemPath = Environment.GetEnvironmentVariable("Path", EnvironmentVariableTarget.Machine);
            Environment.SetEnvironmentVariable("Path", $"{Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin")};{systemPath}");

            AppDataManager.Initialize(baseDirPath);

            // Initialize the configuration
            ConfigurationState.Initialize();

            // Initialize the logger system
            LoggerModule.Initialize();

            // Initialize Discord integration
            DiscordIntegrationModule.Initialize();

            string localConfigurationPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config.json");
            string appDataConfigurationPath = Path.Combine(AppDataManager.BaseDirPath, "Config.json");

            // Now load the configuration as the other subsystems are now registered
            if (File.Exists(localConfigurationPath))
            {
                ConfigurationPath = localConfigurationPath;

                ConfigurationFileFormat configurationFileFormat = ConfigurationFileFormat.Load(localConfigurationPath);

                ConfigurationState.Instance.Load(configurationFileFormat, ConfigurationPath);
            }
            else if (File.Exists(appDataConfigurationPath))
            {
                ConfigurationPath = appDataConfigurationPath;

                ConfigurationFileFormat configurationFileFormat = ConfigurationFileFormat.Load(appDataConfigurationPath);

                ConfigurationState.Instance.Load(configurationFileFormat, ConfigurationPath);
            }
            else
            {
                // No configuration, we load the default values and save it on disk
                ConfigurationPath = appDataConfigurationPath;

                ConfigurationState.Instance.LoadDefault();
                ConfigurationState.Instance.ToFileFormat().SaveConfig(appDataConfigurationPath);
            }

            PrintSystemInfo();

            FontMapper.Default = new Skia.Ui.Skia.Widget.FontMapper();

            RenderWindow window = new RenderWindow(1280, 720);

            string game = "";

            if (launchPath != null)
            {
                game = launchPath;
            }

            window.NavigateTo(new TestScene());

            window.StartApp(game); 
        }

        private static void PrintSystemInfo()
        {
            Logger.Notice.Print(LogClass.Application, $"Ryujinx Version: {Version}");

            Logger.Notice.Print(LogClass.Application, $"Operating System: {SystemInfo.Instance.OsDescription}");
            Logger.Notice.Print(LogClass.Application, $"CPU: {SystemInfo.Instance.CpuName}");
            Logger.Notice.Print(LogClass.Application, $"Total RAM: {SystemInfo.Instance.RamSizeInMB}");

            var enabledLogs = Logger.GetEnabledLevels();
            Logger.Notice.Print(LogClass.Application, $"Logs Enabled: {(enabledLogs.Count == 0 ? "<None>" : string.Join(", ", enabledLogs))}");
        }
    }
}
