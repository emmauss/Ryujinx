using ImGuiNET;
using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics;
using Ryujinx.Audio;
using Ryujinx.Audio.OpenAL;
using Ryujinx.Graphics.Gal;
using Ryujinx.Graphics.Gal.OpenGL;
using Ryujinx.HLE;
using Ryujinx.HLE.Input;
using System;
using System.Threading;

using Stopwatch = System.Diagnostics.Stopwatch;

namespace Ryujinx.UI
{
    partial class EmulationWindow : WindowHelper
    {
        public static EmulationController EmulationController;

        //toggles
        private bool isRunning = false;
        private bool IsRunning
        {
            get => isRunning;
            set
            {
                isRunning = value;
                UIActive = !value;
            }
        }

        private bool showMainUI = true;
        private bool ShowMainUI
        {
            get => showMainUI;
            set
            {
                showMainUI = value;
                if (value)
                {
                    CurrentPage = Page.GameList;
                    UIActive    = value;
                }
                else if (!ShowPauseUI)
                    UIActive = false;
            }
        }

        private bool showPauseUI;
        private bool ShowPauseUI
        {
            get => showPauseUI;
            set
            {
                showPauseUI = value;
                if (value)
                {
                    CurrentPage = Page.Emulation;
                    UIActive    = value;
                }
                else if (!ShowMainUI)
                    UIActive = false;
            }
        }        

        private Page CurrentPage = Page.GameList;

        private bool   EscapePressed;
        private string CurrentPath;
        private string PackagePath;

        private const int TouchScreenWidth = 1280;
        private const int TouchScreenHeight = 720;

        private const float TouchScreenRatioX = (float)TouchScreenWidth / TouchScreenHeight;
        private const float TouchScreenRatioY = (float)TouchScreenHeight / TouchScreenWidth;

        private const int TargetFPS = 60;

        FilePicker FileDialog;

        IGalRenderer Renderer;

        public static Switch Ns;

        private Thread RenderThread;

        private bool ResizeEvent;

        private bool TitleEvent;

        private string NewTitle;

        public EmulationWindow() : base("Ryujinx")
        {
            CurrentPath = Environment.CurrentDirectory;
            PackagePath = string.Empty;
            FileDialog  = FilePicker.GetFilePicker("rom",null);

            InitializeSwitch();

            ResizeEvent = false;

            TitleEvent = false;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            VSync = VSyncMode.On;            
        }

        private void RenderLoop()
        {
            MakeCurrent();

            PrepareTexture();

            Stopwatch Chrono = new Stopwatch();

            Chrono.Start();

            long TicksPerFrame = Stopwatch.Frequency / TargetFPS;

            long Ticks = 0;

            while (Exists && !IsExiting)
            {
                if (Ns.WaitFifo())
                {
                    Ns.ProcessFrame();
                }

                Renderer.RunActions();

                if (ResizeEvent)
                {
                    ResizeEvent = false;

                    Renderer.FrameBuffer.SetWindowSize(Width, Height);
                }

                Ticks += Chrono.ElapsedTicks;

                DeltaTime = (float)Chrono.Elapsed.TotalSeconds;

                Chrono.Restart();

                if (Ticks >= TicksPerFrame)
                {
                    RenderFrame();

                    //Queue max. 1 vsync
                    Ticks = Math.Min(Ticks - TicksPerFrame, TicksPerFrame);
                }
            }
        }

        public void MainLoop()
        {
            VSync = VSyncMode.Off;

            Visible = true;

            Renderer.FrameBuffer.SetWindowSize(Width, Height);

            Context.MakeCurrent(null);

            //OpenTK doesn't like sleeps in its thread, to avoid this a renderer thread is created
            RenderThread = new Thread(RenderLoop);

            RenderThread.Start();

            while (Exists && !IsExiting)
            {
                ProcessEvents();

                if (!IsExiting)
                {
                    UpdateFrame();

                    if (TitleEvent)
                    {
                        TitleEvent = false;

                        Title = NewTitle;
                    }
                }
            }
        }

        private new void RenderFrame()
        {
            if (UIActive)
            {
                StartFrame();

                isRunning = false;

                if (ShowMainUI)
                {
                    showPauseUI = false;

                    RenderMainUI();
                }
                else if (ShowPauseUI)
                {
                    showMainUI = false;

                    RenderPauseUI();
                }

                EndFrame();
            }
            else
            {
                Renderer.FrameBuffer.Render();

                Ns.Statistics.RecordSystemFrameTime();

                double HostFps = Ns.Statistics.GetSystemFrameRate();
                double GameFps = Ns.Statistics.GetGameFrameRate();

                NewTitle = $"Ryujinx | Host FPS: {HostFps:0.0} | Game FPS: {GameFps:0.0}";

                TitleEvent = true;

                SwapBuffers();

                Ns.Os.SignalVsync();
            }
        }

        public void InitializeSwitch()
        {
            MainContext.MakeCurrent(WindowInfo);

            Renderer = new OGLRenderer();

            Renderer.FrameBuffer.SetWindowSize(Width, Height);

            IAalOutput AudioOut = new OpenALAudioOut();

            Ns = new Switch(Renderer, AudioOut);

            Config.Read(Ns.Log);

            Ns.Log.Updated += ConsoleLog.PrintLog;
        }

        private new void UpdateFrame()
        {
            KeyboardState Keyboard = this.Keyboard ?? new KeyboardState();

            if (!UIActive)
            {
                if (Keyboard[Key.Escape] && !EscapePressed)
                {
                    EscapePressed = true;

                    IsRunning = false;

                    EmulationController.Pause();

                    if (EmulationController.IsLoaded)
                        ShowPauseUI = true;

                    UIActive = true;

                    return;
                }
                else if (Keyboard[Key.Escape])
                    EscapePressed = true;
                else
                    EscapePressed = false;

                HidControllerButtons CurrentButton = 0;
                HidJoystickPosition  LeftJoystick;
                HidJoystickPosition  RightJoystick;

                int LeftJoystickDX        = 0;
                int LeftJoystickDY        = 0;
                int RightJoystickDX       = 0;
                int RightJoystickDY       = 0;
                float AnalogStickDeadzone = Config.GamePadDeadzone;

                //LeftJoystick
                if (Keyboard[(Key)Config.JoyConKeyboard.Left.StickUp])    LeftJoystickDY = short.MaxValue;
                if (Keyboard[(Key)Config.JoyConKeyboard.Left.StickDown])  LeftJoystickDY = -short.MaxValue;
                if (Keyboard[(Key)Config.JoyConKeyboard.Left.StickLeft])  LeftJoystickDX = -short.MaxValue;
                if (Keyboard[(Key)Config.JoyConKeyboard.Left.StickRight]) LeftJoystickDX = short.MaxValue;

                //LeftButtons
                if (Keyboard[(Key)Config.JoyConKeyboard.Left.StickButton]) CurrentButton |= HidControllerButtons.KEY_LSTICK;
                if (Keyboard[(Key)Config.JoyConKeyboard.Left.DPadUp])      CurrentButton |= HidControllerButtons.KEY_DUP;
                if (Keyboard[(Key)Config.JoyConKeyboard.Left.DPadDown])    CurrentButton |= HidControllerButtons.KEY_DDOWN;
                if (Keyboard[(Key)Config.JoyConKeyboard.Left.DPadLeft])    CurrentButton |= HidControllerButtons.KEY_DLEFT;
                if (Keyboard[(Key)Config.JoyConKeyboard.Left.DPadRight])   CurrentButton |= HidControllerButtons.KEY_DRIGHT;
                if (Keyboard[(Key)Config.JoyConKeyboard.Left.ButtonMinus]) CurrentButton |= HidControllerButtons.KEY_MINUS;
                if (Keyboard[(Key)Config.JoyConKeyboard.Left.ButtonL])     CurrentButton |= HidControllerButtons.KEY_L;
                if (Keyboard[(Key)Config.JoyConKeyboard.Left.ButtonZL])    CurrentButton |= HidControllerButtons.KEY_ZL;

                //RightJoystick
                if (Keyboard[(Key)Config.JoyConKeyboard.Right.StickUp])    RightJoystickDY = short.MaxValue;
                if (Keyboard[(Key)Config.JoyConKeyboard.Right.StickDown])  RightJoystickDY = -short.MaxValue;
                if (Keyboard[(Key)Config.JoyConKeyboard.Right.StickLeft])  RightJoystickDX = -short.MaxValue;
                if (Keyboard[(Key)Config.JoyConKeyboard.Right.StickRight]) RightJoystickDX = short.MaxValue;

                //RightButtons
                if (Keyboard[(Key)Config.JoyConKeyboard.Right.StickButton]) CurrentButton |= HidControllerButtons.KEY_RSTICK;
                if (Keyboard[(Key)Config.JoyConKeyboard.Right.ButtonA])     CurrentButton |= HidControllerButtons.KEY_A;
                if (Keyboard[(Key)Config.JoyConKeyboard.Right.ButtonB])     CurrentButton |= HidControllerButtons.KEY_B;
                if (Keyboard[(Key)Config.JoyConKeyboard.Right.ButtonX])     CurrentButton |= HidControllerButtons.KEY_X;
                if (Keyboard[(Key)Config.JoyConKeyboard.Right.ButtonY])     CurrentButton |= HidControllerButtons.KEY_Y;
                if (Keyboard[(Key)Config.JoyConKeyboard.Right.ButtonPlus])  CurrentButton |= HidControllerButtons.KEY_PLUS;
                if (Keyboard[(Key)Config.JoyConKeyboard.Right.ButtonR])     CurrentButton |= HidControllerButtons.KEY_R;
                if (Keyboard[(Key)Config.JoyConKeyboard.Right.ButtonZR])    CurrentButton |= HidControllerButtons.KEY_ZR;

                //Controller Input
                if (Config.GamePadEnable)
                {
                    GamePadState GamePad = OpenTK.Input.GamePad.GetState(Config.GamePadIndex);
                    //LeftButtons
                    if (IsGamePadButtonPressed(GamePad, Config.JoyConController.Left.DPadUp)) CurrentButton      |= HidControllerButtons.KEY_DUP;
                    if (IsGamePadButtonPressed(GamePad, Config.JoyConController.Left.DPadDown)) CurrentButton    |= HidControllerButtons.KEY_DDOWN;
                    if (IsGamePadButtonPressed(GamePad, Config.JoyConController.Left.DPadLeft)) CurrentButton    |= HidControllerButtons.KEY_DLEFT;
                    if (IsGamePadButtonPressed(GamePad, Config.JoyConController.Left.DPadRight)) CurrentButton   |= HidControllerButtons.KEY_DRIGHT;
                    if (IsGamePadButtonPressed(GamePad, Config.JoyConController.Left.StickButton)) CurrentButton |= HidControllerButtons.KEY_LSTICK;
                    if (IsGamePadButtonPressed(GamePad, Config.JoyConController.Left.ButtonMinus)) CurrentButton |= HidControllerButtons.KEY_MINUS;
                    if (IsGamePadButtonPressed(GamePad, Config.JoyConController.Left.ButtonL)) CurrentButton     |= HidControllerButtons.KEY_L;
                    if (IsGamePadButtonPressed(GamePad, Config.JoyConController.Left.ButtonZL)) CurrentButton    |= HidControllerButtons.KEY_ZL;

                    //RightButtons
                    if (IsGamePadButtonPressed(GamePad, Config.JoyConController.Right.ButtonA)) CurrentButton     |= HidControllerButtons.KEY_A;
                    if (IsGamePadButtonPressed(GamePad, Config.JoyConController.Right.ButtonB)) CurrentButton     |= HidControllerButtons.KEY_B;
                    if (IsGamePadButtonPressed(GamePad, Config.JoyConController.Right.ButtonX)) CurrentButton     |= HidControllerButtons.KEY_X;
                    if (IsGamePadButtonPressed(GamePad, Config.JoyConController.Right.ButtonY)) CurrentButton     |= HidControllerButtons.KEY_Y;
                    if (IsGamePadButtonPressed(GamePad, Config.JoyConController.Right.StickButton)) CurrentButton |= HidControllerButtons.KEY_RSTICK;
                    if (IsGamePadButtonPressed(GamePad, Config.JoyConController.Right.ButtonPlus)) CurrentButton  |= HidControllerButtons.KEY_PLUS;
                    if (IsGamePadButtonPressed(GamePad, Config.JoyConController.Right.ButtonR)) CurrentButton     |= HidControllerButtons.KEY_R;
                    if (IsGamePadButtonPressed(GamePad, Config.JoyConController.Right.ButtonZR)) CurrentButton    |= HidControllerButtons.KEY_ZR;

                    //LeftJoystick
                    if (GetJoystickAxis(GamePad, Config.JoyConController.Left.Stick).X >= AnalogStickDeadzone
                     || GetJoystickAxis(GamePad, Config.JoyConController.Left.Stick).X <= -AnalogStickDeadzone)
                        LeftJoystickDX = (int)(GetJoystickAxis(GamePad, Config.JoyConController.Left.Stick).X * short.MaxValue);

                    if (GetJoystickAxis(GamePad, Config.JoyConController.Left.Stick).Y >= AnalogStickDeadzone
                     || GetJoystickAxis(GamePad, Config.JoyConController.Left.Stick).Y <= -AnalogStickDeadzone)
                        LeftJoystickDY = (int)(GetJoystickAxis(GamePad, Config.JoyConController.Left.Stick).Y * short.MaxValue);

                    //RightJoystick
                    if (GetJoystickAxis(GamePad, Config.JoyConController.Right.Stick).X >= AnalogStickDeadzone
                     || GetJoystickAxis(GamePad, Config.JoyConController.Right.Stick).X <= -AnalogStickDeadzone)
                        RightJoystickDX = (int)(GetJoystickAxis(GamePad, Config.JoyConController.Right.Stick).X * short.MaxValue);

                    if (GetJoystickAxis(GamePad, Config.JoyConController.Right.Stick).Y >= AnalogStickDeadzone
                     || GetJoystickAxis(GamePad, Config.JoyConController.Right.Stick).Y <= -AnalogStickDeadzone)
                        RightJoystickDY = (int)(GetJoystickAxis(GamePad, Config.JoyConController.Right.Stick).Y * short.MaxValue);
                }

                LeftJoystick = new HidJoystickPosition
                {
                    DX = LeftJoystickDX,
                    DY = LeftJoystickDY
                };

                RightJoystick = new HidJoystickPosition
                {
                    DX = RightJoystickDX,
                    DY = RightJoystickDY
                };

                bool HasTouch = false;

                //Get screen touch position from left mouse click
                //OpenTK always captures mouse events, even if out of focus, so check if window is focused.
                if (Focused && Mouse?.LeftButton == ButtonState.Pressed)
                {
                    MouseState Mouse = this.Mouse.Value;

                    int ScrnWidth  = Width;
                    int ScrnHeight = Height;

                    if (Width > Height * TouchScreenRatioX)
                    {
                        ScrnWidth = (int)(Height * TouchScreenRatioX);
                    }
                    else
                    {
                        ScrnHeight = (int)(Width * TouchScreenRatioY);
                    }

                    int StartX = (Width - ScrnWidth) >> 1;
                    int StartY = (Height - ScrnHeight) >> 1;

                    int EndX = StartX + ScrnWidth;
                    int EndY = StartY + ScrnHeight;

                    if (Mouse.X >= StartX &&
                        Mouse.Y >= StartY &&
                        Mouse.X < EndX &&
                        Mouse.Y < EndY)
                    {
                        int ScrnMouseX = Mouse.X - StartX;
                        int ScrnMouseY = Mouse.Y - StartY;

                        int MX = (int)(((float)ScrnMouseX / ScrnWidth) * TouchScreenWidth);
                        int MY = (int)(((float)ScrnMouseY / ScrnHeight) * TouchScreenHeight);

                        HidTouchPoint CurrentPoint = new HidTouchPoint
                        {
                            X = MX,
                            Y = MY,

                            //Placeholder values till more data is acquired
                            DiameterX = 10,
                            DiameterY = 10,
                            Angle     = 90
                        };

                        HasTouch = true;

                        Ns.Hid.SetTouchPoints(CurrentPoint);
                    }
                }

                if (!HasTouch)
                {
                    Ns.Hid.SetTouchPoints();
                }

                Ns.Hid.SetJoyconButton(
                    HidControllerId.CONTROLLER_HANDHELD,
                    HidControllerLayouts.Handheld_Joined,
                    CurrentButton,
                    LeftJoystick,
                    RightJoystick);

                Ns.Hid.SetJoyconButton(
                    HidControllerId.CONTROLLER_HANDHELD,
                    HidControllerLayouts.Main,
                    CurrentButton,
                    LeftJoystick,
                    RightJoystick);
            }
            else if (EmulationController != null)
                if (EmulationController.IsLoaded)
                {
                    if (Keyboard[Key.Escape] && !EscapePressed)
                    {
                        EscapePressed = true;

                        EmulationController.Resume();

                        if (ShowPauseUI & EmulationController.IsLoaded)
                            showPauseUI = false;

                        UIActive  = false;
                        IsRunning = true;
                    }
                    else if (Keyboard[Key.Escape])
                        EscapePressed = true;
                    else
                        EscapePressed = false;
                }
        }

        protected override void OnUnload(EventArgs e)
        {
            RenderThread.Join();

            base.OnUnload(e);
        }

        protected override void OnResize(EventArgs e)
        {
            ResizeEvent = true;
        }

        public void LoadPackage(string path)
        {           

            MainContext.MakeCurrent(WindowInfo);

            if (Ns == null)
                InitializeSwitch();

            if (EmulationController == null)
                EmulationController = new EmulationController(Ns);

            EmulationController.IsShutDown += EmulationController_IsShutDown;

            EmulationController.Load(path);

            IsRunning = true;

            ShowMainUI = false;
        }

        private void EmulationController_IsShutDown(object sender, EventArgs e)
        {
            EmulationController = null;

            Ns = null;
        }

        enum Page
        {
            Configuration,
            Emulation,
            GameList,
            PackageLoader
        }

    }
}
