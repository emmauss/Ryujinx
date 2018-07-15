using ImGuiNET;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using Ryujinx.Common.Input;

namespace Ryujinx.UI
{
    class WindowHelper : GameWindow
    {
        protected float DeltaTime;
        protected bool  UIActive;

        protected GraphicsContext MainContext;
        protected GraphicsContext UIContext;

        private bool  IsWindowOpened;
        private int   FontTexture;
        private float WheelPosition;

        protected KeyboardState? Keyboard = null;

        protected MouseState? Mouse = null;

        protected GamePadState? GamePad = null;

        public WindowHelper(string Title) : base(1280, 720, GraphicsMode.Default, Title, GameWindowFlags.Default
            , DisplayDevice.Default, 3, 3, GraphicsContextFlags.ForwardCompatible)
        {
            base.Title = Title;

            IsWindowOpened = true;

            Location = new Point(
                (DisplayDevice.Default.Width / 2) - (Width / 2),
                (DisplayDevice.Default.Height / 2) - (Height / 2));

            MainContext = new GraphicsContext(GraphicsMode.Default,
                WindowInfo, 3, 3, GraphicsContextFlags.ForwardCompatible);

            UIContext = new GraphicsContext(GraphicsMode.Default,
                WindowInfo, 3, 3, GraphicsContextFlags.ForwardCompatible);

            UIContext.MakeCurrent(WindowInfo);

            UIActive = true;
        }

        public void StartFrame()
        {
            UIContext.MakeCurrent(WindowInfo);

            IO IO = ImGui.GetIO();

            IO.DisplaySize             = new System.Numerics.Vector2(Width, Height);
            IO.DisplayFramebufferScale = new System.Numerics.Vector2(Values.CurrentWindowScale);
            IO.DeltaTime               = DeltaTime;

            ImGui.NewFrame();

            HandleInput(IO);
        }

        public unsafe void EndFrame()
        {
            ImGui.Render();

            DrawData* data = ImGui.GetDrawData();

            RenderImDrawData(data);

            MainContext?.MakeCurrent(WindowInfo);
        }

        protected unsafe void PrepareTexture()
        {
            ImGui.GetIO().FontAtlas.AddDefaultFont();

            IO IO = ImGui.GetIO();

            // Build texture atlas
            FontTextureData texData = IO.FontAtlas.GetTexDataAsAlpha8();

            // Create OpenGL texture
            FontTexture = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2D, FontTexture);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);

            GL.TexImage2D(
                TextureTarget.Texture2D,
                0,
                PixelInternalFormat.Alpha,
                texData.Width,
                texData.Height,
                0,
                PixelFormat.Alpha,
                PixelType.UnsignedByte,
                new IntPtr(texData.Pixels));

            // Store the texture identifier in the ImFontAtlas substructure.
            IO.FontAtlas.SetTexID(FontTexture);

            IO.FontAtlas.ClearTexData();

            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        unsafe void HandleInput(IO IO)
        {
            KeyboardState KeyboardState = default(KeyboardState);

            if (Keyboard != null)
                if (Keyboard.HasValue)
                    KeyboardState = Keyboard.Value;

            MouseState MouseState = default(MouseState);

            if (Mouse != null)
                if (Mouse.HasValue)
                    MouseState = Mouse.Value;

            if (Focused)
            {
                if (Mouse.HasValue)
                {
                    Point WindowPoint = new Point(MouseState.X, MouseState.Y);

                    IO.MousePosition = new System.Numerics.Vector2(WindowPoint.X,
                        WindowPoint.Y);
                }

                if (this.Keyboard.HasValue)
                    foreach (Key Key in Enum.GetValues(typeof(Key)))
                    {
                        IO.KeysDown[(int)Key] = KeyboardState[Key];

                        if (KeyboardState[Key])
                            continue;

                        ImGuiNative.igGetIO()->KeyAlt = (byte)((KeyboardState[Key.AltLeft]
                            || KeyboardState[Key.AltRight]) ? 1 : 0);

                        ImGuiNative.igGetIO()->KeyCtrl = (byte)((KeyboardState[Key.ControlLeft]
                            || KeyboardState[Key.ControlRight]) ? 1 : 0);

                        ImGuiNative.igGetIO()->KeyShift = (byte)((KeyboardState[Key.ShiftLeft]
                            || KeyboardState[Key.ShiftRight]) ? 1 : 0);

                        ImGuiNative.igGetIO()->KeySuper = (byte)((KeyboardState[Key.WinLeft]
                            || KeyboardState[Key.WinRight]) ? 1 : 0);
                    }
            }
            else
            {
                IO.MousePosition = new System.Numerics.Vector2(-1f, -1f);

                for (int i = 0; i <= 512; i++)
                {
                    IO.KeysDown[i] = false;
                }
            }

            if (Mouse.HasValue)
            {
                IO.MouseDown[0] = MouseState.LeftButton   == ButtonState.Pressed;
                IO.MouseDown[1] = MouseState.RightButton  == ButtonState.Pressed;
                IO.MouseDown[2] = MouseState.MiddleButton == ButtonState.Pressed;

                float NewWheelPos = MouseState.WheelPrecise;
                float Delta       = NewWheelPos - WheelPosition;
                WheelPosition     = NewWheelPos;
                IO.MouseWheel     = Delta;
            }
        }

        private unsafe void RenderImDrawData(DrawData* DrawData)
        {
            Vector4 ClearColor = new Vector4(114f / 255f, 144f / 255f, 154f / 255f, 1.0f);

            GL.Viewport(0, 0, Width, Height);

            GL.ClearColor(ClearColor.X, ClearColor.Y, ClearColor.Z, ClearColor.W);

            GL.Clear(ClearBufferMask.ColorBufferBit);

            GL.GetInteger(GetPName.TextureBinding2D, out int last_texture);

            GL.PushAttrib(AttribMask.EnableBit | AttribMask.ColorBufferBit | AttribMask.TransformBit);

            GL.Enable(EnableCap.Blend);

            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            GL.Disable(EnableCap.CullFace);

            GL.Disable(EnableCap.DepthTest);

            GL.Enable(EnableCap.ScissorTest);

            GL.EnableClientState(ArrayCap.VertexArray);

            GL.EnableClientState(ArrayCap.TextureCoordArray);

            GL.EnableClientState(ArrayCap.ColorArray);

            GL.Enable(EnableCap.Texture2D);

            GL.UseProgram(0);
            
            IO IO = ImGui.GetIO();

            ImGui.ScaleClipRects(DrawData, IO.DisplayFramebufferScale);
            
            GL.MatrixMode(MatrixMode.Projection);

            GL.PushMatrix();

            GL.LoadIdentity();

            GL.Ortho(
                0.0f,
                IO.DisplaySize.X / IO.DisplayFramebufferScale.X,
                IO.DisplaySize.Y / IO.DisplayFramebufferScale.Y,
                0.0f,
                -1.0f,
                1.0f);

            GL.MatrixMode(MatrixMode.Modelview);

            GL.PushMatrix();

            GL.LoadIdentity();

            // Render command lists
            for (int n = 0; n < DrawData->CmdListsCount; n++)
            {
                NativeDrawList* CmdList   = DrawData->CmdLists[n];
                byte*           VtxBuffer = (byte*)CmdList->VtxBuffer.Data;
                ushort*         IdxBuffer = (ushort*)CmdList->IdxBuffer.Data;

                GL.VertexPointer(2, VertexPointerType.Float, sizeof(DrawVert), new IntPtr(VtxBuffer + DrawVert.PosOffset));

                GL.TexCoordPointer(2, TexCoordPointerType.Float, sizeof(DrawVert), new IntPtr(VtxBuffer + DrawVert.UVOffset));

                GL.ColorPointer(4, ColorPointerType.UnsignedByte, sizeof(DrawVert), new IntPtr(VtxBuffer + DrawVert.ColOffset));

                for (int Cmd = 0; Cmd < CmdList->CmdBuffer.Size; Cmd++)
                {
                    DrawCmd* PCmd = &(((DrawCmd*)CmdList->CmdBuffer.Data)[Cmd]);

                    if (PCmd->UserCallback != IntPtr.Zero)
                    {
                        throw new NotImplementedException();
                    }
                    else
                    {
                        GL.BindTexture(TextureTarget.Texture2D, PCmd->TextureId.ToInt32());

                        GL.Scissor(
                            (int)PCmd->ClipRect.X,
                            (int)(IO.DisplaySize.Y - PCmd->ClipRect.W),
                            (int)(PCmd->ClipRect.Z - PCmd->ClipRect.X),
                            (int)(PCmd->ClipRect.W - PCmd->ClipRect.Y));

                        ushort[] indices = new ushort[PCmd->ElemCount];

                        for (int i = 0; i < indices.Length; i++)
                            indices[i] = IdxBuffer[i];

                        GL.DrawElements(PrimitiveType.Triangles, (int)PCmd->ElemCount, DrawElementsType.UnsignedShort, new IntPtr(IdxBuffer));
                    }

                    IdxBuffer += PCmd->ElemCount;
                }
            }

            // Restore modified state
            GL.DisableClientState(ArrayCap.ColorArray);

            GL.DisableClientState(ArrayCap.TextureCoordArray);

            GL.DisableClientState(ArrayCap.VertexArray);

            GL.BindTexture(TextureTarget.Texture2D, last_texture);

            GL.MatrixMode(MatrixMode.Modelview);

            GL.PopMatrix();

            GL.MatrixMode(MatrixMode.Projection);

            GL.PopMatrix();

            GL.PopAttrib();

            SwapBuffers();
        }

        internal static bool IsGamePadButtonPressed(GamePadState GamePad, GamePadButton Button)
        {
            if (Button == GamePadButton.LTrigger || Button == GamePadButton.RTrigger)
            {
                return GetGamePadTrigger(GamePad, Button) >= Config.GamePadTriggerThreshold;
            }
            else
            {
                return (GetGamePadButton(GamePad, Button) == ButtonState.Pressed);
            }
        }

        internal static ButtonState GetGamePadButton(GamePadState GamePad, GamePadButton Button)
        {
            switch (Button)
            {
                case GamePadButton.A:         return GamePad.Buttons.A;
                case GamePadButton.B:         return GamePad.Buttons.B;
                case GamePadButton.X:         return GamePad.Buttons.X;
                case GamePadButton.Y:         return GamePad.Buttons.Y;
                case GamePadButton.LStick:    return GamePad.Buttons.LeftStick;
                case GamePadButton.RStick:    return GamePad.Buttons.RightStick;
                case GamePadButton.LShoulder: return GamePad.Buttons.LeftShoulder;
                case GamePadButton.RShoulder: return GamePad.Buttons.RightShoulder;
                case GamePadButton.DPadUp:    return GamePad.DPad.Up;
                case GamePadButton.DPadDown:  return GamePad.DPad.Down;
                case GamePadButton.DPadLeft:  return GamePad.DPad.Left;
                case GamePadButton.DPadRight: return GamePad.DPad.Right;
                case GamePadButton.Start:     return GamePad.Buttons.Start;
                case GamePadButton.Back:      return GamePad.Buttons.Back;
                default:                      throw  new ArgumentException();
            }
        }

        internal static float GetGamePadTrigger(GamePadState GamePad, GamePadButton Trigger)
        {
            switch (Trigger)
            {
                case GamePadButton.LTrigger: return GamePad.Triggers.Left;
                case GamePadButton.RTrigger: return GamePad.Triggers.Right;
                default:                     throw new ArgumentException();
            }
        }

        internal static Vector2 GetJoystickAxis(GamePadState GamePad, GamePadStick Joystick)
        {
            switch (Joystick)
            {
                case GamePadStick.LJoystick: return GamePad.ThumbSticks.Left;
                case GamePadStick.RJoystick: return new Vector2(-GamePad.ThumbSticks.Right.Y, -GamePad.ThumbSticks.Right.X);
                default:                     throw new ArgumentException();
            }
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            Keyboard = e.Keyboard;
        }

        protected override void OnKeyUp(KeyboardKeyEventArgs e)
        {
            Keyboard = e.Keyboard;
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            Mouse = e.Mouse;
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            Mouse = e.Mouse;
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            Mouse = e.Mouse;
        }
    }
}
