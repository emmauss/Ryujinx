using ARMeilleure.Translation.PTC;
using OpenTK.Windowing.Common.Input;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Ryujinx.Audio;
using Ryujinx.Common.Configuration;
using Ryujinx.Common.Configuration.Hid;
using Ryujinx.Common.Logging;
using Ryujinx.Configuration;
using Ryujinx.Graphics.GAL;
using Ryujinx.HLE;
using Ryujinx.HLE.HOS.Services.Hid;
using Ryujinx.Skia.Ui.Skia.Widget;
using Ryujinx.Ui;
using Ryujinx.Motion;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using static Ryujinx.Skia.Ui.RenderWindow;
using Rectangle = Ryujinx.Skia.Ui.Skia.Widget.Rectangle;

namespace Ryujinx.Skia.Ui.Skia.Scene
{
    public class GameScene : Scene
    {
        private const int SwitchPanelWidth = 1280;
        private const int SwitchPanelHeight = 720;

        public static event EventHandler<StatusUpdatedEventArgs> StatusUpdatedEvent;

        private Switch _device;

        public readonly string Path;

        private IRenderer _renderer;

        private bool _initialize;

        private bool _resized;

        private Client _dsuClient;

        private readonly GraphicsDebugLevel _glLogLevel;

        public bool Closed { get; set; }

        public bool EnableOverlay { get; set; }

        //overlays
        private readonly Label _statusText;

        //private LottieWidget _loadAnimation;
        private ProgressBar _ptcProgressBar;
        private readonly Rectangle _loadingRect;
        private bool _gameStarted;
        private bool _loaded;

        private HotkeyButtons _prevHotkeyButtons;
        private bool _isDisposing;

        private IHostUiHandler _uiHandler;

        public GameScene(string path)
        {
            Path = path;

            IManager.Instance.Resized += Instance_Resized;

            _glLogLevel = ConfigurationState.Instance.Logger.GraphicsDebugLevel;

            StatusUpdatedEvent += GameScene_StatusUpdatedEvent;

            _statusText = new Label("Loading Game")
            {
                BackgroundColor = SKColors.Transparent,
                ForegroundColor = Theme.ForegroundColor,
                FontStyle = SKFontStyle.Bold
            };

            _loadingRect = new Rectangle(default)
            {
                FillColor = Theme.BackgroundColor
            };

            AddElement(_statusText);

            IsSelfRendering = true;
            EnableOverlay = true;

           /* string resourceID = "Ryujinx.Skia.Ui.Assets.gray.json";
            Assembly assembly = GetType().GetTypeInfo().Assembly;

            using (Stream stream = assembly.GetManifestResourceStream(resourceID))
            {
                SKRect bounds = SKRect.Create(0, 0, 200, 200);
                _loadAnimation = new LottieWidget(bounds);
                _loadAnimation.Load(stream);
            }*/

            SKWindow.TargetFps = 60;

            _uiHandler = new SkiaHostUiHandler(this);
            
            _dsuClient = new Client();

            DrawUi = true;
        }

        public override void Draw(SKCanvas canvas)
        {
            if (_loadingRect.FillColor.Alpha != 0)
            {
                using SKPaint paint = new SKPaint()
                {
                    Color = _loadingRect.FillColor,
                    Style = SKPaintStyle.Fill
                };

                canvas.DrawRect(_loadingRect.Bounds, paint);
            }

            base.Draw(canvas);
        }

        public void Load()
        {
            /*_loadAnimation.StartDelay(500);
            AddElement(_loadAnimation);*/
            _device = InitializeSwitchInstance();

            Initialize();

            UpdateGraphicsConfig();

            Logger.Notice.Print(LogClass.Application, $"Using Firmware Version: {ContentManager.GetCurrentFirmwareVersion()?.VersionString}");

            if (Directory.Exists(Path))
            {
                string[] romFsFiles = Directory.GetFiles(Path, "*.istorage");

                if (romFsFiles.Length == 0)
                {
                    romFsFiles = Directory.GetFiles(Path, "*.romfs");
                }

                if (romFsFiles.Length > 0)
                {
                    Logger.Info?.Print(LogClass.Application, "Loading as cart with RomFS.");
                    _device.LoadCart(Path, romFsFiles[0]);
                }
                else
                {
                    Logger.Info?.Print(LogClass.Application, "Loading as cart WITHOUT RomFS.");
                    _device.LoadCart(Path);
                }
            }
            else if (File.Exists(Path))
            {
                switch (System.IO.Path.GetExtension(Path).ToLowerInvariant())
                {
                    case ".xci":
                        Logger.Info?.Print(LogClass.Application, "Loading as XCI.");
                        _device.LoadXci(Path);
                        break;
                    case ".nca":
                        Logger.Info?.Print(LogClass.Application, "Loading as NCA.");
                        _device.LoadNca(Path);
                        break;
                    case ".nsp":
                    case ".pfs0":
                        Logger.Info?.Print(LogClass.Application, "Loading as NSP.");
                        _device.LoadNsp(Path);
                        break;
                    default:
                        Logger.Info?.Print(LogClass.Application, "Loading as homebrew.");
                        try
                        {
                            _device.LoadProgram(Path);
                        }
                        catch (ArgumentOutOfRangeException)
                        {
                            Logger.Error?.Print(LogClass.Application, "The file which you have specified is unsupported by Ryujinx.");

                            Exit();

                            return;
                        }
                        break;
                }
            }
            else
            {
                Logger.Warning?.Print(LogClass.Application, "Please specify a valid XCI/NCA/NSP/PFS0/NRO file.");

                Exit();

                return;
            }

            _loaded = true;

            if (ConfigurationState.Instance.System.EnablePtc)
            {
                _ptcProgressBar = new ProgressBar(default)
                {
                    ForegroundColor = Theme.ForegroundColor,
                    BackgroundColor = Theme.BackgroundColor
                };

                AddElement(_ptcProgressBar);
                if (_device.System.EnablePtc)
                {
                    Ptc.TranslationProgress += PtcTranslationProgress;
                }
            }

            IManager.Instance.StartGame();
        }

        private void PtcTranslationProgress(object sender, PtcTranslationProgressEvent e)
        {
            _ptcProgressBar.Maximum = e.FunctionCount;
            _ptcProgressBar.Minimum = 0;
            _ptcProgressBar.Progress = e.Translated;

            _statusText.Text = $"PTC:  {e.Translated} Of {e.FunctionCount} Translated. {e.Rejitted} Rejitted.";
        }

        private HLE.Switch InitializeSwitchInstance()
        {
            VirtualFileSystem.Reload();

            HLE.Switch instance = new HLE.Switch(VirtualFileSystem, ContentManager, RenderWindow.UserChannelPersistence, InitializeRenderer(), InitializeAudioEngine())
            {
                UiHandler = _uiHandler
            };

            instance.Initialize();

            return instance;
        }

        public static void UpdateGraphicsConfig()
        {
            int resScale = ConfigurationState.Instance.Graphics.ResScale;
            float resScaleCustom = ConfigurationState.Instance.Graphics.ResScaleCustom;
            Graphics.Gpu.GraphicsConfig.ResScale = (resScale == -1) ? resScaleCustom : resScale;
            Graphics.Gpu.GraphicsConfig.MaxAnisotropy = ConfigurationState.Instance.Graphics.MaxAnisotropy;
            Graphics.Gpu.GraphicsConfig.ShadersDumpPath = ConfigurationState.Instance.Graphics.ShadersDumpPath;
        }

        private static IRenderer InitializeRenderer()
        {
            return SKWindow.UiBackend.CreateRenderer();
        }

        private static IAalOutput InitializeAudioEngine()
        {
            if (ConfigurationState.Instance.System.AudioBackend.Value == AudioBackend.SoundIo)
            {
                if (SoundIoAudioOut.IsSupported)
                {
                    return new SoundIoAudioOut();
                }
                else
                {
                    Logger.Warning?.Print(LogClass.Audio, "SoundIO is not supported, falling back to dummy audio out.");
                }
            }
            else if (ConfigurationState.Instance.System.AudioBackend.Value == AudioBackend.OpenAl)
            {
                if (OpenALAudioOut.IsSupported)
                {
                    return new OpenALAudioOut();
                }
                else
                {
                    Logger.Warning?.Print(LogClass.Audio, "OpenAL is not supported, trying to fall back to SoundIO.");

                    if (SoundIoAudioOut.IsSupported)
                    {
                        Logger.Warning?.Print(LogClass.Audio, "Found SoundIO, changing configuration.");

                        ConfigurationState.Instance.System.AudioBackend.Value = AudioBackend.SoundIo;
                        SaveConfig();

                        return new SoundIoAudioOut();
                    }
                    else
                    {
                        Logger.Warning?.Print(LogClass.Audio, "SoundIO is not supported, falling back to dummy audio out.");
                    }
                }
            }

            return new DummyAudioOut();
        }

        public static void SaveConfig()
        {
            ConfigurationState.Instance.ToFileFormat().SaveConfig(Program.ConfigurationPath);
        }

        private void GameScene_StatusUpdatedEvent(object sender, StatusUpdatedEventArgs e)
        {
            if (_gameStarted)
            {
                _statusText.Text = $"{e.FifoStatus:00.00} {e.GameStatus:00.00}  {e.GpuName}";
            }
        }

        public void Exit()
        {
            if(_isDisposing)
            {
                return;
            }

            _isDisposing = true;

            IManager.Instance.Back();

            Dispose();

            if (_device.System.EnablePtc)
            {
                Ptc.TranslationProgress -= PtcTranslationProgress;
            }
           // _loadAnimation.FadeOut();
        }

        public void Initialize()
        {
            _renderer = _device.Gpu.Renderer;
        }

        private void Instance_Resized(object sender, EventArgs e)
        {
            _resized = true;
        }

        public void Start()
        {
            var manager = IManager.Instance;
            _renderer.Initialize(_glLogLevel);
            _renderer.Window.SetSize((int)manager.Bounds.Width, (int)manager.Bounds.Height);
            _initialize = true;

            SKWindow.UiBackend.SwapInterval(0);
        }

        public unsafe void ProcessFrame(SKWindow gameWindow)
        {
            if (_device != null && !Closed)
            {
                SKWindow.UiBackend.SwitchContext(ContextType.Game);

                if (_resized)
                {
                    var manager = IManager.Instance;

                    SKWindow.UiBackend.CreateGameResources();

                    _renderer.Window.SetSize((int)manager.Bounds.Width, (int)manager.Bounds.Height);

                    _resized = false;
                }

                lock (_device)
                {
                    if (_device.WaitFifo() && !Closed)
                    {
                        _device.Statistics.RecordFifoStart();
                        _device.ProcessFrame();
                        _device.Statistics.RecordFifoEnd();
                    }
                }
            }
        }

        public unsafe void Render(SKWindow gameWindow)
        {
            if (Closed)
            {
                SKWindow.UiBackend.SwapInterval(0);

                Exit();

                return;
            }
            if (!_loaded)
            {
                return;
            }

            if (!_initialize)
            {
                Start();
            }

            string dockedMode = ConfigurationState.Instance.System.EnableDockedMode ? "Docked" : "Handheld";
            float scale = Graphics.Gpu.GraphicsConfig.ResScale;

            if (scale != 1)
            {
                dockedMode += $" ({scale}x)";
            }

            lock (_device)
            {
                if (!Closed)
                {
                    _device.PresentFrame(() =>
                    {
                        if (!_gameStarted)
                        {
                            FadeOutOverlayRect();

                            Theme.SceneBackgroundColor = SKColors.Transparent;
                        }

                        gameWindow.CopyGameFramebuffer();

                        _gameStarted = true;
                    });
                }
            }

            StatusUpdatedEvent?.Invoke(this, new StatusUpdatedEventArgs(
                _device.EnableDeviceVsync,
                dockedMode,
                $"Game: {_device.Statistics.GetGameFrameRate():00.00} FPS",
                $"FIFO: {_device.Statistics.GetFifoPercent():0.00} %",
                $"GPU:  {_renderer.GpuVendor}"));
        }

        public void FadeOutOverlayRect()
        {
            Animator?.Stop();
            _ptcProgressBar?.FadeOut();
            Animator = new Animation();
            Animator.With(255, 0, 1000, SetRectAlpha, endCallback: HideOverlayWidgets);
            Animator.Play();
        }

        public void FadeInOverlayRect()
        {
            Animator?.Stop();
           // _loadAnimation.Play();
            Animator = new Animation();
            Animator.With(0, 255, 200, SetRectAlpha);
            Animator.Play();
        }

        public void SetRectAlpha(double alpha)
        {
            _loadingRect.FillColor = _loadingRect.FillColor.WithAlpha((byte)alpha);

            IManager.Instance?.InvalidateMeasure();
        }

        public void HideOverlayWidgets()
        {
           // Elements.Remove(_loadAnimation);
            if (_ptcProgressBar != null)
            {
                Elements.Remove(_ptcProgressBar);
                _ptcProgressBar.ParentScene = null;
                _ptcProgressBar.Dispose();
                _ptcProgressBar = null;
            }
        }

        public void ClearOverlayRectWidgets()
        {
            lock (this)
            {
              //  Elements.Remove(_loadAnimation);
                if (_ptcProgressBar != null)
                {
                    Elements.Remove(_ptcProgressBar);
                    _ptcProgressBar.ParentScene = null;
                    _ptcProgressBar.Dispose();
                    _ptcProgressBar = null;
                }
                /*_loadAnimation.Stop();
                _loadAnimation.Dispose();
                _loadAnimation.ParentScene = null;
                _loadAnimation = null;
                */
                Elements.Remove(_loadingRect);
            }
        }

        public void Update(SKWindow gameWindow)
        {
            if (!gameWindow.IsFocused || Closed)
            {
                return;
            }

            KeyboardState keyboard = gameWindow.KeyboardState;
            MouseState mouse = gameWindow.MouseState;
            var joysticks = gameWindow.JoystickStates;

            if (Modals.Count == 0)
            {
                if (!keyboard.IsKeyDown(Keys.Escape) && keyboard.WasKeyDown(Keys.Escape))
                {
                    if (!_gameStarted && _device.System.EnablePtc)
                    {
                        Ptc.TranslationProgress -= PtcTranslationProgress;
                        Ptc.Continue();

                        _ptcProgressBar.Progress = _ptcProgressBar.Maximum;

                        return;
                    }

                    MessageDialog dialog = new MessageDialog(this, "Close Emulation", "Do you want to end this emulation session?", "", DialogButtons.Yes | DialogButtons.No);

                    Task.Run(() =>
                    {
                        dialog.Run();

                        if (dialog.DialogResult == "Yes")
                        {
                            lock (_device)
                            {
                                Closed = true;

                                return;
                            }
                        }
                    });
                }
            }
            else
            {
                return;
            }

            if (!gameWindow.IsActive || !_gameStarted)
            {
                return;
            }

            List<GamepadInput> gamepadInputs = new List<GamepadInput>(NpadDevices.MaxControllers);

            List<SixAxisInput> motionInputs = new List<SixAxisInput>(NpadDevices.MaxControllers);

            MotionDevice motionDevice = new MotionDevice(_dsuClient);

            foreach (InputConfig inputConfig in ConfigurationState.Instance.Hid.InputConfig.Value)
            {
                ControllerKeys currentButton = 0;
                JoystickPosition leftJoystick = new JoystickPosition();
                JoystickPosition rightJoystick = new JoystickPosition();
                KeyboardInput? hidKeyboard = null;

                int leftJoystickDx = 0;
                int leftJoystickDy = 0;
                int rightJoystickDx = 0;
                int rightJoystickDy = 0;

                if (inputConfig.EnableMotion)
                {
                    motionDevice.RegisterController(inputConfig.PlayerIndex);
                }

                if (inputConfig is KeyboardConfig keyboardConfig)
                {
                    if (gameWindow.IsFocused)
                    {
                        // Keyboard Input
                        KeyboardController keyboardController = new KeyboardController(keyboardConfig, keyboard);

                        currentButton = keyboardController.GetButtons();

                        (leftJoystickDx, leftJoystickDy) = keyboardController.GetLeftStick();
                        (rightJoystickDx, rightJoystickDy) = keyboardController.GetRightStick();

                        leftJoystick = new JoystickPosition
                        {
                            Dx = leftJoystickDx,
                            Dy = leftJoystickDy
                        };

                        rightJoystick = new JoystickPosition
                        {
                            Dx = rightJoystickDx,
                            Dy = rightJoystickDy
                        };

                        if (ConfigurationState.Instance.Hid.EnableKeyboard)
                        {
                            hidKeyboard = keyboardController.GetKeysDown();
                        }

                        if (!hidKeyboard.HasValue)
                        {
                            hidKeyboard = new KeyboardInput
                            {
                                Modifier = 0,
                                Keys = new int[0x8]
                            };
                        }

                        if (ConfigurationState.Instance.Hid.EnableKeyboard)
                        {
                            _device.Hid.Keyboard.Update(hidKeyboard.Value);
                        }
                    }
                }
                else if (inputConfig is Common.Configuration.Hid.ControllerConfig controllerConfig)
                {
                    JoystickState joystick = joysticks[controllerConfig.Index];
                    // Controller Input
                    JoystickController joystickController = new JoystickController(controllerConfig, joystick);

                    currentButton |= joystickController.GetButtons();

                    (leftJoystickDx, leftJoystickDy) = joystickController.GetLeftStick();
                    (rightJoystickDx, rightJoystickDy) = joystickController.GetRightStick();

                    leftJoystick = new JoystickPosition
                    {
                        Dx = controllerConfig.LeftJoycon.InvertStickX ? -leftJoystickDx : leftJoystickDx,
                        Dy = controllerConfig.LeftJoycon.InvertStickY ? -leftJoystickDy : leftJoystickDy
                    };

                    rightJoystick = new JoystickPosition
                    {
                        Dx = controllerConfig.RightJoycon.InvertStickX ? -rightJoystickDx : rightJoystickDx,
                        Dy = controllerConfig.RightJoycon.InvertStickY ? -rightJoystickDy : rightJoystickDy
                    };
                }

                currentButton |= _device.Hid.UpdateStickButtons(leftJoystick, rightJoystick);

                motionDevice.Poll(inputConfig.PlayerIndex, inputConfig.Slot);

                SixAxisInput sixAxisInput = new SixAxisInput()
                {
                    PlayerId = (HLE.HOS.Services.Hid.PlayerIndex)inputConfig.PlayerIndex,
                    Accelerometer = motionDevice.Accelerometer,
                    Gyroscope = motionDevice.Gyroscope,
                    Rotation = motionDevice.Rotation,
                    Orientation = motionDevice.Orientation
                };

                motionInputs.Add(sixAxisInput);

                gamepadInputs.Add(new GamepadInput
                {
                    PlayerId = (HLE.HOS.Services.Hid.PlayerIndex)inputConfig.PlayerIndex,
                    Buttons = currentButton,
                    LStick = leftJoystick,
                    RStick = rightJoystick
                });

                if (inputConfig.ControllerType == Common.Configuration.Hid.ControllerType.JoyconPair)
                {
                    if (!inputConfig.MirrorInput)
                    {
                        motionDevice.Poll(inputConfig.PlayerIndex, inputConfig.AltSlot);

                        sixAxisInput = new SixAxisInput()
                        {
                            PlayerId = (HLE.HOS.Services.Hid.PlayerIndex)inputConfig.PlayerIndex,
                            Accelerometer = motionDevice.Accelerometer,
                            Gyroscope = motionDevice.Gyroscope,
                            Rotation = motionDevice.Rotation,
                            Orientation = motionDevice.Orientation
                        };
                    }

                    motionInputs.Add(sixAxisInput);
                }
            }

            _device.Hid.Npads.Update(gamepadInputs);
            _device.Hid.Npads.UpdateSixAxis(motionInputs);

            if (gameWindow.IsFocused)
            {
                // Hotkeys
                HotkeyButtons currentHotkeyButtons = KeyboardController.GetHotkeyButtons(keyboard);

                if (currentHotkeyButtons.HasFlag(HotkeyButtons.ToggleVSync) &&
                    !_prevHotkeyButtons.HasFlag(HotkeyButtons.ToggleVSync))
                {
                    _device.EnableDeviceVsync = !_device.EnableDeviceVsync;
                }

                _prevHotkeyButtons = currentHotkeyButtons;
            }

            //Touchscreen
            bool hasTouch = false;

            // Get screen touch position from left mouse click
            // OpenTK always captures mouse events, even if out of focus, so check if window is focused.
            if (gameWindow.IsFocused && mouse.IsAnyButtonDown)
            {
                SKRect bounds = IManager.Instance.Bounds;

                int AllocatedWidth = (int)bounds.Width;
                int AllocatedHeight = (int)bounds.Height;

                int screenWidth = AllocatedWidth;
                int screenHeight = AllocatedHeight;

                if (AllocatedWidth > (AllocatedHeight * SwitchPanelWidth) / SwitchPanelHeight)
                {
                    screenWidth = (AllocatedHeight * SwitchPanelWidth) / SwitchPanelHeight;
                }
                else
                {
                    screenHeight = (AllocatedWidth * SwitchPanelHeight) / SwitchPanelWidth;
                }

                int startX = (AllocatedWidth - screenWidth) >> 1;
                int startY = (AllocatedHeight - screenHeight) >> 1;

                int endX = startX + screenWidth;
                int endY = startY + screenHeight;


                if (mouse.X >= startX &&
                    mouse.Y >= startY &&
                    mouse.X < endX &&
                    mouse.Y < endY)
                {
                    int screenMouseX = (int)mouse.X - startX;
                    int screenMouseY = (int)mouse.Y - startY;

                    int mX = (screenMouseX * SwitchPanelWidth) / screenWidth;
                    int mY = (screenMouseY * SwitchPanelHeight) / screenHeight;

                    TouchPoint currentPoint = new TouchPoint
                    {
                        X = (uint)mX,
                        Y = (uint)mY,

                        // Placeholder values till more data is acquired
                        DiameterX = 10,
                        DiameterY = 10,
                        Angle = 90
                    };

                    hasTouch = true;

                    _device.Hid.Touchscreen.Update(currentPoint);
                }
            }

            if (!hasTouch)
            {
                _device.Hid.Touchscreen.Update();
            }

            _device.Hid.DebugPad.Update();
        }

        public override void Measure()
        {
            lock (this)
            {
                base.Measure();

                var manager = IManager.Instance;
                _statusText.Location = new SKPoint(manager.Bounds.Left + 50, manager.Bounds.Bottom - 50);
                _statusText.Measure();

                /*if (_loadAnimation != null)
                {
                    _loadingRect.Bounds = manager.Bounds;

                    var size = _loadAnimation.Size;
                    _loadAnimation.Measure(SKRect.Create(new SKPoint(manager.Bounds.MidX - size.Width / 2, manager.Bounds.MidY - size.Height / 2), size));


                }*/

                if (_ptcProgressBar != null)
                {
                    float margin = manager.Bounds.Width * 0.1f;
                    float progressBarWidth = manager.Bounds.Width - (margin * 2);
                    SKPoint location = new SKPoint(manager.Bounds.Left + margin, manager.Bounds.Bottom - 100);
                    var  size = new SKSize(progressBarWidth, 20);
                    _ptcProgressBar.Measure(SKRect.Create(location, size));
                }
            }
        }

        public override void Dispose()
        {
            _dsuClient?.Dispose();

            Theme = Themes.Dark;
            SKWindow.UiBackend.ReleaseRenderer();

            Ptc.Close();

            PtcProfiler.Stop();

            _device.DisposeGpu();

            _device.Dispose();

            base.Dispose();

        }
    }
}
