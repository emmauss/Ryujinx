using Gtk;
using Ryujinx.Audio;
using Ryujinx.Common.Logging;
using Ryujinx.Configuration;
using Ryujinx.Debugger.Profiler;
using Ryujinx.Graphics.GAL;
using Ryujinx.Graphics.OpenGL;
using Ryujinx.HLE.FileSystem;
using Ryujinx.HLE.FileSystem.Content;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using GUI = Gtk.Builder.ObjectAttribute;

namespace Ryujinx.Ui
{
    public class MainWindow : Window
    {
        private static VirtualFileSystem _virtualFileSystem;
        private static ContentManager    _contentManager;

        private static HLE.Switch _emulationContext;

        private static GLRenderer _gLWidget;

        private static ApplicationList _applicationList;

        private static AutoResetEvent _deviceExitStatus = new AutoResetEvent(false);

        private static bool _updatingGameTable;
        private static bool _gameLoaded;
        private static bool _ending;
        private static bool _debuggerOpened;

        private static Ryujinx.Debugger.Debugger _debugger;

#pragma warning disable CS0649
#pragma warning disable IDE0044

        [GUI] Window   _mainWin;
        [GUI] MenuBar  _menuBar;
        [GUI] Box      _footerBox;
        [GUI] MenuItem _fullScreen;
        [GUI] Box      _statusBar;
        [GUI] MenuItem _stopEmulation;
        [GUI] MenuItem _firmwareInstallFile;
        [GUI] MenuItem _firmwareInstallDirectory;
        [GUI] Label    _hostStatus;
        [GUI] MenuItem _openDebugger;
        [GUI] Label    _gameStatus;
        [GUI] Label    _progressLabel;
        [GUI] Label    _firmwareVersionLabel;
        [GUI] LevelBar _progressBar;
        [GUI] Box      _viewBox;
        [GUI] Label    _vSyncStatus;
        [GUI] Box      _listStatusBox;

#pragma warning restore CS0649
#pragma warning restore IDE0044

        public MainWindow() : this(new Builder("Ryujinx.Ui.MainWindow.glade")) { }

        private MainWindow(Builder builder) : base(builder.GetObject("_mainWin").Handle)
        {
            builder.Autoconnect(this);

            this.DeleteEvent      += Window_Close;
            _fullScreen.Activated += FullScreen_Toggled;

            ApplicationLibrary.ApplicationAdded        += Application_Added;
            ApplicationLibrary.ApplicationCountUpdated += ApplicationCount_Updated;
            GLRenderer.StatusUpdatedEvent              += Update_StatusBar;

            // First we check that a migration isn't needed. (because VirtualFileSystem will create the new directory otherwise)
            bool continueWithStartup = Migration.PromptIfMigrationNeededForStartup(this, out bool migrationNeeded);
            if (!continueWithStartup)
            {
                End(null);
            }

            _virtualFileSystem = VirtualFileSystem.CreateInstance();
            _contentManager    = new ContentManager(_virtualFileSystem);

            if (migrationNeeded)
            {
                bool migrationSuccessful = Migration.DoMigrationForStartup(this, _virtualFileSystem);

                if (!migrationSuccessful)
                {
                    End(null);
                }
            }

            // Make sure that everything is loaded.
            _virtualFileSystem.Reload();

            ApplyTheme();

            _mainWin.Icon            = new Gdk.Pixbuf(Assembly.GetExecutingAssembly(), "Ryujinx.Ui.assets.Icon.png");
            _mainWin.Title           = $"Ryujinx {Program.Version}";
            _stopEmulation.Sensitive = false;

            _applicationList = new ApplicationList();
            _applicationList.ButtonPressEvent += ApplicationLibrary_Button_Pressed;

            _viewBox.Add(_applicationList);
            _viewBox.ShowAll();
#if USE_DEBUGGING
            _debugger = new Debugger.Debugger();
            _openDebugger.Activated += _openDebugger_Opened;
#else
            _openDebugger.Hide();
#endif

            UpdateGameTable();

            Task.Run(RefreshFirmwareLabel);

            _statusBar.Hide();
        }

#if USE_DEBUGGING
        private void _openDebugger_Opened(object sender, EventArgs e)
        {
            if (_debuggerOpened)
            {
                return;
            }

            Window debugWindow = new Window("Debugger");
            
            debugWindow.SetSizeRequest(1280, 640);
            debugWindow.Add(_debugger.Widget);
            debugWindow.DeleteEvent += DebugWindow_DeleteEvent;
            debugWindow.ShowAll();

            _debugger.Enable();

            _debuggerOpened = true;
        }

        private void DebugWindow_DeleteEvent(object o, DeleteEventArgs args)
        {
            _debuggerOpened = false;

            _debugger.Disable();

            (_debugger.Widget.Parent as Window)?.Remove(_debugger.Widget);
        }
#endif

        internal static void ApplyTheme()
        {
            if (!ConfigurationState.Instance.Ui.EnableCustomTheme)
            {
                return;
            }

            if (File.Exists(ConfigurationState.Instance.Ui.CustomThemePath) && (System.IO.Path.GetExtension(ConfigurationState.Instance.Ui.CustomThemePath) == ".css"))
            {
                CssProvider cssProvider = new CssProvider();

                cssProvider.LoadFromPath(ConfigurationState.Instance.Ui.CustomThemePath);

                StyleContext.AddProviderForScreen(Gdk.Screen.Default, cssProvider, 800);
            }
            else
            {
                Logger.PrintWarning(LogClass.Application, $"The \"custom_theme_path\" section in \"Config.json\" contains an invalid path: \"{ConfigurationState.Instance.Ui.CustomThemePath}\".");
            }
        }

        private HLE.Switch InitializeSwitchInstance()
        {
            _virtualFileSystem.Reload();

            HLE.Switch instance = new HLE.Switch(_virtualFileSystem, _contentManager, InitializeRenderer(), InitializeAudioEngine());

            instance.Initialize();

            return instance;
        }

        internal static void UpdateGameTable()
        {
            if (_updatingGameTable)
            {
                return;
            }

            _updatingGameTable = true;

            _applicationList.ClearItems();

            Thread applicationLibraryThread = new Thread(() =>
            {
                ApplicationLibrary.LoadApplications(ConfigurationState.Instance.Ui.GameDirs,
                    _virtualFileSystem, ConfigurationState.Instance.System.Language);

                _updatingGameTable = false;
            });
            applicationLibraryThread.Name = "GUI.ApplicationLibraryThread";
            applicationLibraryThread.IsBackground = true;
            applicationLibraryThread.Start();
        }

        internal void LoadApplication(string path)
        {
            if (_gameLoaded)
            {
                GtkDialog.CreateDialog("Ryujinx", "A game has already been loaded", "Please close it first and try again.");
            }
            else
            {
                Logger.RestartTime();

                HLE.Switch device = InitializeSwitchInstance();

                // TODO: Move this somewhere else + reloadable?
                Graphics.Gpu.GraphicsConfig.ShadersDumpPath = ConfigurationState.Instance.Graphics.ShadersDumpPath;

                if (Directory.Exists(path))
                {
                    string[] romFsFiles = Directory.GetFiles(path, "*.istorage");

                    if (romFsFiles.Length == 0)
                    {
                        romFsFiles = Directory.GetFiles(path, "*.romfs");
                    }

                    if (romFsFiles.Length > 0)
                    {
                        Logger.PrintInfo(LogClass.Application, "Loading as cart with RomFS.");
                        device.LoadCart(path, romFsFiles[0]);
                    }
                    else
                    {
                        Logger.PrintInfo(LogClass.Application, "Loading as cart WITHOUT RomFS.");
                        device.LoadCart(path);
                    }
                }
                else if (File.Exists(path))
                {
                    switch (System.IO.Path.GetExtension(path).ToLowerInvariant())
                    {
                        case ".xci":
                            Logger.PrintInfo(LogClass.Application, "Loading as XCI.");
                            device.LoadXci(path);
                            break;
                        case ".nca":
                            Logger.PrintInfo(LogClass.Application, "Loading as NCA.");
                            device.LoadNca(path);
                            break;
                        case ".nsp":
                        case ".pfs0":
                            Logger.PrintInfo(LogClass.Application, "Loading as NSP.");
                            device.LoadNsp(path);
                            break;
                        default:
                            Logger.PrintInfo(LogClass.Application, "Loading as homebrew.");
                            try
                            {
                                device.LoadProgram(path);
                            }
                            catch (ArgumentOutOfRangeException)
                            {
                                Logger.PrintError(LogClass.Application, "The file which you have specified is unsupported by Ryujinx.");
                            }
                            break;
                    }
                }
                else
                {
                    Logger.PrintWarning(LogClass.Application, "Please specify a valid XCI/NCA/NSP/PFS0/NRO file.");
                    End(device);
                }

                _emulationContext = device;

                _deviceExitStatus.Reset();

#if MACOS_BUILD
                CreateGameWindow(device);
#else
                Thread windowThread = new Thread(() =>
                {
                    CreateGameWindow(device);
                })
                {
                    Name = "GUI.WindowThread"
                };

                windowThread.Start();
#endif

                _gameLoaded              = true;
                _stopEmulation.Sensitive = true;

                _firmwareInstallFile.Sensitive      = false;
                _firmwareInstallDirectory.Sensitive = false;

                DiscordIntegrationModule.SwitchToPlayingState(device.System.TitleIdText, device.System.TitleName);

                ApplicationLibrary.LoadAndSaveMetaData(device.System.TitleIdText, appMetadata =>
                {
                    appMetadata.LastPlayed = DateTime.UtcNow.ToString();
                });
            }
        }

        private void CreateGameWindow(HLE.Switch device)
        {
            device.Hid.InitializePrimaryController(ConfigurationState.Instance.Hid.ControllerType);

            _gLWidget = new GLRenderer(_emulationContext);

            Application.Invoke(delegate
            {
                _viewBox.Remove(_applicationList);
                _viewBox.Add(_gLWidget);

                _gLWidget.ShowAll();
                EditFooterForGameRender();
            });

            _gLWidget.WaitEvent.WaitOne();

            _gLWidget.Start();

            device.Dispose();
            _deviceExitStatus.Set();

            // NOTE: Everything that is here will not be executed when you close the UI.
            Application.Invoke(delegate
            {
                _viewBox.Remove(_gLWidget);
                _gLWidget.Exit();

                if (_gLWidget.Window != this.Window && _gLWidget.Window != null)
                {
                    _gLWidget.Window.Dispose();
                }

                _gLWidget.Dispose();

                _viewBox.Add(_applicationList);

                this.Window.Title = $"Ryujinx {Program.Version}";

                _emulationContext = null;
                _gameLoaded       = false;
                _gLWidget         = null;

                DiscordIntegrationModule.SwitchToMainMenu();

                RecreateFooterForMenu();

                UpdateGameTable();

                Task.Run(RefreshFirmwareLabel);

                _stopEmulation.Sensitive            = false;
                _firmwareInstallFile.Sensitive      = true;
                _firmwareInstallDirectory.Sensitive = true;
            });
        }

        private void RecreateFooterForMenu()
        {
            _listStatusBox.Show();
            _statusBar.Hide();
        }

        private void EditFooterForGameRender()
        {
            _listStatusBox.Hide();
            _statusBar.Show();
        }

        public void ToggleExtraWidgets(bool show)
        {
            if (_gLWidget != null)
            {
                if (show)
                {
                    _menuBar.ShowAll();
                    _footerBox.Show();
                    _statusBar.Show();
                }
                else
                {
                    _menuBar.Hide();
                    _footerBox.Hide();
                }
            }

            bool fullScreenToggled = this.Window.State.HasFlag(Gdk.WindowState.Fullscreen);

            _fullScreen.Label = fullScreenToggled ? "Exit Fullscreen" : "Enter Fullscreen";
        }

        private static void UpdateGameMetadata(string titleId)
        {
            if (_gameLoaded)
            {
                ApplicationLibrary.LoadAndSaveMetaData(titleId, appMetadata =>
                {
                    DateTime lastPlayedDateTime = DateTime.Parse(appMetadata.LastPlayed);
                    double   sessionTimePlayed  = DateTime.UtcNow.Subtract(lastPlayedDateTime).TotalSeconds;

                    appMetadata.TimePlayed += Math.Round(sessionTimePlayed, MidpointRounding.AwayFromZero);
                });
            }
        }

        private void End(HLE.Switch device)
        {

#if USE_DEBUGGING
            _debugger.Dispose();
#endif

            if (_ending)
            {
                return;
            }

            _ending = true;

            if (device != null)
            {
                UpdateGameMetadata(device.System.TitleIdText);

                if (_gLWidget != null)
                {
                    // We tell the widget that we are exiting
                    _gLWidget.Exit();

                    // Wait for the other thread to dispose the HLE context before exiting.
                    _deviceExitStatus.WaitOne();
                }
            }

            Dispose();

            _applicationList.Dispose();

            Profile.FinishProfiling();
            DiscordIntegrationModule.Exit();
            Logger.Shutdown();
            Application.Quit();
        }

        private static IRenderer InitializeRenderer()
        {
            return new Renderer();
        }

        /// <summary>
        /// Picks an <see cref="IAalOutput"/> audio output renderer supported on this machine
        /// </summary>
        /// <returns>An <see cref="IAalOutput"/> supported by this machine</returns>
        private static IAalOutput InitializeAudioEngine()
        {
            if (OpenALAudioOut.IsSupported)
            {
                return new OpenALAudioOut();
            }
            else if (SoundIoAudioOut.IsSupported)
            {
                return new SoundIoAudioOut();
            }
            else
            {
                return new DummyAudioOut();
            }
        }

        //Events
        private void Application_Added(object sender, ApplicationAddedEventArgs args)
        {
            Application.Invoke(delegate
            {
                _applicationList.AddItem(args.AppData);
            });
        }

        private void ApplicationCount_Updated(object sender, ApplicationCountUpdatedEventArgs args)
        {
            Application.Invoke(delegate
            {
                _progressLabel.Text = $"{args.NumAppsLoaded}/{args.NumAppsFound} Games Loaded";
                float barValue      = 0;

                if (args.NumAppsFound != 0)
                {
                    barValue = (float)args.NumAppsLoaded / args.NumAppsFound;
                }

                _progressBar.Value = barValue;
            });
        }

        private void Update_StatusBar(object sender, StatusUpdatedEventArgs args)
        {
            Application.Invoke(delegate
            {
                _hostStatus.Text = args.HostStatus;
                _gameStatus.Text = args.GameStatus;

                if (args.VSyncEnabled)
                {
                    _vSyncStatus.Attributes = new Pango.AttrList();
                    _vSyncStatus.Attributes.Insert(new Pango.AttrForeground(11822, 60138, 51657));
                }
                else
                {
                    _vSyncStatus.Attributes = new Pango.AttrList();
                    _vSyncStatus.Attributes.Insert(new Pango.AttrForeground(ushort.MaxValue, 17733, 21588));
                }
            });
        }

        private void ApplicationLibrary_Button_Pressed(object sender, ButtonPressEventArgs args)
        {
            if (args.Event.Type != Gdk.EventType.DoubleButtonPress)
                return;

            foreach (ApplicationListItem item in _applicationList.ListItems)
            {
                if (item.Selected)
                {
                    LoadApplication(item.Data.Path);
                }
            }
        }

        private void Load_Application_File(object sender, EventArgs args)
        {
            FileChooserDialog fileChooser = new FileChooserDialog("Choose the file to open", this, FileChooserAction.Open, "Cancel", ResponseType.Cancel, "Open", ResponseType.Accept);

            fileChooser.Filter = new FileFilter();
            fileChooser.Filter.AddPattern("*.nsp" );
            fileChooser.Filter.AddPattern("*.pfs0");
            fileChooser.Filter.AddPattern("*.xci" );
            fileChooser.Filter.AddPattern("*.nca" );
            fileChooser.Filter.AddPattern("*.nro" );
            fileChooser.Filter.AddPattern("*.nso" );

            if (fileChooser.Run() == (int)ResponseType.Accept)
            {
                LoadApplication(fileChooser.Filename);
            }

            fileChooser.Dispose();
        }

        private void Load_Application_Folder(object sender, EventArgs args)
        {
            FileChooserDialog fileChooser = new FileChooserDialog("Choose the folder to open", this, FileChooserAction.SelectFolder, "Cancel", ResponseType.Cancel, "Open", ResponseType.Accept);

            if (fileChooser.Run() == (int)ResponseType.Accept)
            {
                LoadApplication(fileChooser.Filename);
            }

            fileChooser.Dispose();
        }

        private void Open_Ryu_Folder(object sender, EventArgs args)
        {
            Process.Start(new ProcessStartInfo()
            {
                FileName        = _virtualFileSystem.GetBasePath(),
                UseShellExecute = true,
                Verb            = "open"
            });
        }

        private void Exit_Pressed(object sender, EventArgs args)
        {
            End(_emulationContext);
        }

        private void Window_Close(object sender, DeleteEventArgs args)
        {
            End(_emulationContext);
        }

        private void StopEmulation_Pressed(object sender, EventArgs args)
        {
            _gLWidget?.Exit();
        }

        private void Installer_File_Pressed(object o, EventArgs args)
        {
            FileChooserDialog fileChooser = new FileChooserDialog("Choose the firmware file to open",
                                                                  this,
                                                                  FileChooserAction.Open,
                                                                  "Cancel",
                                                                  ResponseType.Cancel,
                                                                  "Open",
                                                                  ResponseType.Accept);

            fileChooser.Filter = new FileFilter();
            fileChooser.Filter.AddPattern("*.zip");
            fileChooser.Filter.AddPattern("*.xci");

            HandleInstallerDialog(fileChooser);
        }

        private void Installer_Directory_Pressed(object o, EventArgs args)
        {
            FileChooserDialog directoryChooser = new FileChooserDialog("Choose the firmware directory to open",
                                                                       this,
                                                                       FileChooserAction.SelectFolder,
                                                                       "Cancel",
                                                                       ResponseType.Cancel,
                                                                       "Open",
                                                                       ResponseType.Accept);

            HandleInstallerDialog(directoryChooser);
        }

        private void RefreshFirmwareLabel()
        {
            var currentFirmware = _contentManager.GetCurrentFirmwareVersion();

            GLib.Idle.Add(new GLib.IdleHandler(() =>
            {
                _firmwareVersionLabel.Text = currentFirmware != null ? currentFirmware.VersionString : "0.0.0";

                return false;
            }));
        }

        private void HandleInstallerDialog(FileChooserDialog fileChooser)
        {
            if (fileChooser.Run() == (int)ResponseType.Accept)
            {
                MessageDialog dialog = null;

                try
                {
                    string filename = fileChooser.Filename;

                    fileChooser.Dispose();

                    var firmwareVersion = _contentManager.VerifyFirmwarePackage(filename);

                    if (firmwareVersion == null)
                    {
                        dialog = new MessageDialog(this, DialogFlags.Modal, MessageType.Info, ButtonsType.Ok, false, "");

                        dialog.Text = "Firmware not found.";

                        dialog.SecondaryText = $"A valid system firmware was not found in {filename}.";

                        Logger.PrintError(LogClass.Application, $"A valid system firmware was not found in {filename}.");

                        dialog.Run();
                        dialog.Hide();
                        dialog.Dispose();

                        return;
                    }

                    var currentVersion = _contentManager.GetCurrentFirmwareVersion();

                    string dialogMessage = $"System version {firmwareVersion.VersionString} will be installed.";

                    if (currentVersion != null)
                    {
                        dialogMessage += $"This will replace the current system version {currentVersion.VersionString}. ";
                    }

                    dialogMessage += "Do you want to continue?";

                    dialog = new MessageDialog(this, DialogFlags.Modal, MessageType.Question, ButtonsType.YesNo, false, "");

                    dialog.Text = $"Install Firmware {firmwareVersion.VersionString}";
                    dialog.SecondaryText = dialogMessage;

                    int response = dialog.Run();

                    dialog.Dispose();

                    dialog = new MessageDialog(this, DialogFlags.Modal, MessageType.Info, ButtonsType.None, false, "");

                    dialog.Text = $"Install Firmware {firmwareVersion.VersionString}";

                    dialog.SecondaryText = "Installing firmware...";

                    if (response == (int)ResponseType.Yes)
                    {
                        Logger.PrintInfo(LogClass.Application, $"Installing firmware {firmwareVersion.VersionString}");
                        
                        Thread thread = new Thread(() =>
                        {
                            GLib.Idle.Add(new GLib.IdleHandler(() =>
                            {
                                dialog.Run();
                                return false;
                            }));

                            try
                            {
                                _contentManager.InstallFirmware(filename);

                                GLib.Idle.Add(new GLib.IdleHandler(() =>
                                {
                                    dialog.Dispose();

                                    dialog = new MessageDialog(this, DialogFlags.Modal, MessageType.Info, ButtonsType.Ok, false, "");

                                    dialog.Text = $"Install Firmware {firmwareVersion.VersionString}";

                                    dialog.SecondaryText = $"System version {firmwareVersion.VersionString} successfully installed.";

                                    Logger.PrintInfo(LogClass.Application, $"System version {firmwareVersion.VersionString} successfully installed.");

                                    dialog.Run();
                                    dialog.Dispose();

                                    return false;
                                }));
                            }
                            catch (Exception ex)
                            {
                                GLib.Idle.Add(new GLib.IdleHandler(() =>
                                {
                                    dialog.Dispose();

                                    dialog = new MessageDialog(this, DialogFlags.Modal, MessageType.Info, ButtonsType.Ok, false, "");

                                    dialog.Text = $"Install Firmware {firmwareVersion.VersionString} Failed.";

                                    dialog.SecondaryText = $"An error occured while installing system version {firmwareVersion.VersionString}." +
                                     " Please check logs for more info.";

                                    Logger.PrintError(LogClass.Application, ex.Message);

                                    dialog.Run();
                                    dialog.Dispose();

                                    return false;
                                }));
                            }
                            finally
                            {
                                RefreshFirmwareLabel();
                            }
                        });

                        thread.Name = "GUI.FirmwareInstallerThread";
                        thread.Start();
                    }
                    else
                    {
                        dialog.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    if (dialog != null)
                    {
                        dialog.Dispose();
                    }

                    dialog = new MessageDialog(this, DialogFlags.Modal, MessageType.Info, ButtonsType.Ok, false, "");

                    dialog.Text = "Parsing Firmware Failed.";

                    dialog.SecondaryText = "An error occured while parsing firmware. Please check the logs for more info.";

                    Logger.PrintError(LogClass.Application, ex.Message);

                    dialog.Run();
                    dialog.Dispose();
                }
            }
            else
            {
                fileChooser.Dispose();
            }
        }

        private void FullScreen_Toggled(object o, EventArgs args)
        {
            bool fullScreenToggled = this.Window.State.HasFlag(Gdk.WindowState.Fullscreen);

            if (!fullScreenToggled)
            {
                Fullscreen();

                ToggleExtraWidgets(false);
            }
            else
            {
                Unfullscreen();

                ToggleExtraWidgets(true);
            }
        }

        private void Settings_Pressed(object sender, EventArgs args)
        {
            SwitchSettings settingsWin = new SwitchSettings();
            settingsWin.Show();
        }

        private void Update_Pressed(object sender, EventArgs args)
        {
            string ryuUpdater = System.IO.Path.Combine(_virtualFileSystem.GetBasePath(), "RyuUpdater.exe");

            try
            {
                Process.Start(new ProcessStartInfo(ryuUpdater, "/U") { UseShellExecute = true });
            }
            catch(System.ComponentModel.Win32Exception)
            {
                GtkDialog.CreateErrorDialog("Update canceled by user or updater was not found");
            }
        }

        private void About_Pressed(object sender, EventArgs args)
        {
            AboutWindow aboutWin = new AboutWindow();
            aboutWin.Show();
        }

        private void RefreshList_Pressed(object sender, ButtonReleaseEventArgs args)
        {
            UpdateGameTable();
        }

        public static void SaveConfig()
        {
            ConfigurationState.Instance.ToFileFormat().SaveConfig(Program.ConfigurationPath);
        }
    }
}
