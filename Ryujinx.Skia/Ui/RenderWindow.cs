using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Ryujinx.Audio;
using Ryujinx.Common.Logging;
using Ryujinx.Configuration;
using Ryujinx.Graphics.GAL;
using Ryujinx.Graphics.OpenGL;
using Ryujinx.HLE.FileSystem;
using Ryujinx.HLE.FileSystem.Content;
using Ryujinx.HLE.HOS;
using Ryujinx.Skia.App;
using Ryujinx.Skia.Ui.Skia.Scene;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Ryujinx.Skia.Ui
{
    public partial class RenderWindow : SKWindow
    {
        public RenderWindow(int width, int height) : base(width, height)
        {
            VirtualFileSystem = VirtualFileSystem.CreateInstance();
            ContentManager = new ContentManager(VirtualFileSystem);
            VirtualFileSystem.Reload();

            UserChannelPersistence = new UserChannelPersistence();

            GamePath = null;

            ApplicationHelper.Initialize(VirtualFileSystem);
        }

        public void RefreshApplicationLibrary()
        {
            ApplicationLibrary.LoadApplications(ConfigurationState.Instance.Ui.GameDirs, VirtualFileSystem, ConfigurationState.Instance.System.Language);

            if(ActiveScene is SplashScene scene)
            {
                scene.End();
            }
        }

        public void StartApp(string gamePath = "")
        {
            if (!string.IsNullOrEmpty(gamePath))
            {
                LoadApplication(gamePath);
            }

            Start();
        }

        public SystemVersion FirmwareVersion => ContentManager.GetCurrentFirmwareVersion();

        public static ContentManager ContentManager { get; set; }
        public static VirtualFileSystem VirtualFileSystem { get; set; }

        public static UserChannelPersistence UserChannelPersistence;
        protected string GamePath;

        protected unsafe override void OnUpdateFrame(FrameEventArgs args)
        {
            GLFW.MakeContextCurrent(null);
        }

        public void LoadApplication(string path)
        {
            Scene currentScene = ActiveScene;

            if (currentScene is GameScene)
            {
                return;
            }
            else
            {
                Logger.RestartTime();

                VirtualFileSystem.Reload();

                GameScene scene = new GameScene(path);

                NavigateTo(scene);

                scene.Load();
            }
        }

        // TODO: handle
        private void HandleRelaunch()
        {
            // If the previous index isn't -1, that mean we are relaunching.
            if (UserChannelPersistence.PreviousIndex != -1)
            {
                LoadApplication(GamePath);
            }
            else
            {
                // otherwise, clear state.
                UserChannelPersistence = new UserChannelPersistence();
                GamePath = null;
            }
        }

        public override void StartGame()
        {
            base.StartGame();

            GamePath = (ActiveScene as GameScene).Path;
        }
    }
}