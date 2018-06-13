using ImGuiNET;
using OpenTK;
using Ryujinx.Audio;
using Ryujinx.Audio.OpenAL;
using Ryujinx.Graphics.Gal;
using Ryujinx.Graphics.Gal.OpenGL;
using Ryujinx.HLE;
using Ryujinx.HLE.Input;
using OpenTK.Graphics;
using OpenTK.Input;
using System;
using System.IO;

namespace Ryujinx.UI
{
    partial class EmulationWindow : WindowHelper
    {
        //toggles     
        private bool showMainUI = true;
        private bool showPauseUI;
        private bool isRunning = false;
        private bool IsRunning
        {
            get => isRunning;
            set
            {
                isRunning = value;
                UIActive = !value;
                if (!value)
                {
                    //ShowMainUI = true;
                }
            }
        }

        private bool ShowMainUI
        {
            get => showMainUI;
            set
            {
                showMainUI = value;
                showPauseUI = !value && !isRunning;
                UIActive = value;
            }
        }

        private bool ShowPauseUI
        {
            get => showPauseUI;
            set
            {
                showPauseUI = value;
                UIActive = value;
                showMainUI = !value;
            }
        }

        private EmulationController EmulationController;

        private Page CurrentPage = Page.PackageLoader;

        private bool EscapePressed;

        private string CurrentPath = Environment.CurrentDirectory;
        private string PackagePath = string.Empty;

        private const int TouchScreenWidth = 1280;
        private const int TouchScreenHeight = 720;

        private const float TouchScreenRatioX = (float)TouchScreenWidth / TouchScreenHeight;
        private const float TouchScreenRatioY = (float)TouchScreenHeight / TouchScreenWidth;

        FilePicker FileDialog;

        IGalRenderer Renderer;
        IAalOutput AudioOut;
        Switch Ns;

        public EmulationWindow() : base("Test")
        {
            FileDialog = FilePicker.GetFilePicker("rom",null);

            Renderer = new OGLRenderer();

            AudioOut = new OpenALAudioOut();

            Ns = new Switch(Renderer, AudioOut);

            Config.Read(Ns.Log);

            EmulationController = new EmulationController(Ns);

            Ns.Log.Updated += ConsoleLog.PrintLog;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            VSync = VSyncMode.On;

            Renderer.FrameBuffer.SetWindowSize(Width, Height);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            _deltaTime = (float)e.Time;
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

                Title = $"Ryujinx | Host FPS: {HostFps:0.0} | Game FPS: {GameFps:0.0}";

                SwapBuffers();

                Ns.Os.SignalVsync();
            }
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            if(!UIActive)
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
                HidJoystickPosition LeftJoystick;
                HidJoystickPosition RightJoystick;

                int LeftJoystickDX = 0;
                int LeftJoystickDY = 0;
                int RightJoystickDX = 0;
                int RightJoystickDY = 0;

                //RightJoystick
                if (Keyboard[(Key)Config.FakeJoyCon.Left.StickUp]) LeftJoystickDY = short.MaxValue;
                if (Keyboard[(Key)Config.FakeJoyCon.Left.StickDown]) LeftJoystickDY = -short.MaxValue;
                if (Keyboard[(Key)Config.FakeJoyCon.Left.StickLeft]) LeftJoystickDX = -short.MaxValue;
                if (Keyboard[(Key)Config.FakeJoyCon.Left.StickRight]) LeftJoystickDX = short.MaxValue;

                //LeftButtons
                if (Keyboard[(Key)Config.FakeJoyCon.Left.StickButton]) CurrentButton |= HidControllerButtons.KEY_LSTICK;
                if (Keyboard[(Key)Config.FakeJoyCon.Left.DPadUp]) CurrentButton |= HidControllerButtons.KEY_DUP;
                if (Keyboard[(Key)Config.FakeJoyCon.Left.DPadDown]) CurrentButton |= HidControllerButtons.KEY_DDOWN;
                if (Keyboard[(Key)Config.FakeJoyCon.Left.DPadLeft]) CurrentButton |= HidControllerButtons.KEY_DLEFT;
                if (Keyboard[(Key)Config.FakeJoyCon.Left.DPadRight]) CurrentButton |= HidControllerButtons.KEY_DRIGHT;
                if (Keyboard[(Key)Config.FakeJoyCon.Left.ButtonMinus]) CurrentButton |= HidControllerButtons.KEY_MINUS;
                if (Keyboard[(Key)Config.FakeJoyCon.Left.ButtonL]) CurrentButton |= HidControllerButtons.KEY_L;
                if (Keyboard[(Key)Config.FakeJoyCon.Left.ButtonZL]) CurrentButton |= HidControllerButtons.KEY_ZL;

                //RightJoystick
                if (Keyboard[(Key)Config.FakeJoyCon.Right.StickUp]) RightJoystickDY = short.MaxValue;
                if (Keyboard[(Key)Config.FakeJoyCon.Right.StickDown]) RightJoystickDY = -short.MaxValue;
                if (Keyboard[(Key)Config.FakeJoyCon.Right.StickLeft]) RightJoystickDX = -short.MaxValue;
                if (Keyboard[(Key)Config.FakeJoyCon.Right.StickRight]) RightJoystickDX = short.MaxValue;

                //RightButtons
                if (Keyboard[(Key)Config.FakeJoyCon.Right.StickButton]) CurrentButton |= HidControllerButtons.KEY_RSTICK;
                if (Keyboard[(Key)Config.FakeJoyCon.Right.ButtonA]) CurrentButton |= HidControllerButtons.KEY_A;
                if (Keyboard[(Key)Config.FakeJoyCon.Right.ButtonB]) CurrentButton |= HidControllerButtons.KEY_B;
                if (Keyboard[(Key)Config.FakeJoyCon.Right.ButtonX]) CurrentButton |= HidControllerButtons.KEY_X;
                if (Keyboard[(Key)Config.FakeJoyCon.Right.ButtonY]) CurrentButton |= HidControllerButtons.KEY_Y;
                if (Keyboard[(Key)Config.FakeJoyCon.Right.ButtonPlus]) CurrentButton |= HidControllerButtons.KEY_PLUS;
                if (Keyboard[(Key)Config.FakeJoyCon.Right.ButtonR]) CurrentButton |= HidControllerButtons.KEY_R;
                if (Keyboard[(Key)Config.FakeJoyCon.Right.ButtonZR]) CurrentButton |= HidControllerButtons.KEY_ZR;

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
                if (Focused && Mouse?.GetState().LeftButton == ButtonState.Pressed)
                {
                    int ScrnWidth = Width;
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
                            Angle = 90
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

                Ns.ProcessFrame();

                Renderer.RunActions();
            }
            else if(EmulationController.IsLoaded)
            {
                if (Keyboard[Key.Escape] && !EscapePressed)
                {
                    EscapePressed = true;
                    EmulationController.Resume();

                    if (ShowPauseUI & EmulationController.IsLoaded)
                        showPauseUI = false;
                    UIActive = false;
                    IsRunning = true;
                }
                else if (Keyboard[Key.Escape])
                    EscapePressed = true;
                else
                    EscapePressed = false;
            }
        }       

        public void LoadPackage(string path)
        {
            ShowMainUI = false;
            MainContext.MakeCurrent(WindowInfo);
            if (Directory.Exists(path))
            {
                string[] RomFsFiles = Directory.GetFiles(path, "*.istorage");

                if (RomFsFiles.Length == 0)
                {
                    RomFsFiles = Directory.GetFiles(path, "*.romfs");
                }

                if (RomFsFiles.Length > 0)
                {
                    Console.WriteLine("Loading as cart with RomFS.");

                    Ns.LoadCart(path, RomFsFiles[0]);
                }
                else
                {
                    Console.WriteLine("Loading as cart WITHOUT RomFS.");

                    Ns.LoadCart(path);
                }
            }
            else if (File.Exists(path))
            {
                Console.WriteLine("Loading as homebrew.");

                Ns.LoadProgram(path);
            }
            IsRunning = true;
            EmulationController.IsLoaded = true;
        }

        enum Page
        {
            Configuration,
            Emulation,
            PackageLoader
        }

    }
}
