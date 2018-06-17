using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using ImGuiNET;

namespace Ryujinx.UI
{
    class WindowHelper : GameWindow
    {
        protected float _deltaTime;
        bool IsWindowOpened = false;
        int s_fontTexture;
        float _wheelPosition;
        protected GraphicsContext MainContext;
        protected GraphicsContext UIContext;
        protected bool UIActive;

        public WindowHelper(string title) : base(1280, 720, GraphicsMode.Default, title, GameWindowFlags.Default
            , DisplayDevice.Default, 3, 3, GraphicsContextFlags.ForwardCompatible)
        {
            Title = title;
            IsWindowOpened = true;

            Location = new Point(
                (DisplayDevice.Default.Width / 2) - (Width / 2),
                (DisplayDevice.Default.Height / 2) - (Height / 2));

            MainContext = (GraphicsContext)Context;

            UIContext = new GraphicsContext(GraphicsMode.Default,
                WindowInfo,4,5,GraphicsContextFlags.ForwardCompatible);
            UIContext.MakeCurrent(WindowInfo);
            UIActive = true;
        }

        public void ShowDemo()
        {
            ImGuiNative.igShowDemoWindow(ref IsWindowOpened);
        }

        public void StartFrame()
        {
            UIContext.MakeCurrent(WindowInfo);
            IO io = ImGui.GetIO();
            io.DisplaySize = new System.Numerics.Vector2(Width, Height);
            io.DisplayFramebufferScale = new System.Numerics.Vector2(Values.CurrentWindowScale);
            io.DeltaTime = _deltaTime;
            ImGui.NewFrame();
            HandleInput(io);
        }

        public unsafe void EndFrame()
        {
            ImGui.Render();
            DrawData* data = ImGui.GetDrawData();
            RenderImDrawData(data);

            MainContext.MakeCurrent(WindowInfo);
        }

        protected unsafe void PrepareTexture()
        {
            ImGui.GetIO().FontAtlas.AddDefaultFont();

            IO io = ImGui.GetIO();

            // Build texture atlas
            FontTextureData texData = io.FontAtlas.GetTexDataAsAlpha8();

            // Create OpenGL texture
            s_fontTexture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, s_fontTexture);
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
            io.FontAtlas.SetTexID(s_fontTexture);

            // Cleanup (don't clear the input data if you want to append new fonts later)
            //io.Fonts->ClearInputData();
            io.FontAtlas.ClearTexData();
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        protected unsafe override void OnLoad(EventArgs e)
        {
            PrepareTexture();
        }

        unsafe void HandleInput(IO io)
        {
            MouseState cursorState = Mouse.GetCursorState();
            MouseState mouseState = Mouse.GetState();
            if (Focused)
            {
                Point windowPoint = PointToClient(new Point(cursorState.X, cursorState.Y));
                io.MousePosition = new System.Numerics.Vector2(windowPoint.X / io.DisplayFramebufferScale.X,
                    windowPoint.Y / io.DisplayFramebufferScale.Y);

                foreach (Key key in Enum.GetValues(typeof(Key)))
                {
                    io.KeysDown[(int)key] = Keyboard[key];
                    if (Keyboard[key])
                        continue;
                    ImGuiNative.igGetIO()->KeyAlt = (byte)((Keyboard[Key.AltLeft]
                        || Keyboard[Key.AltRight]) ? 1 : 0);
                    ImGuiNative.igGetIO()->KeyCtrl = (byte)((Keyboard[Key.ControlLeft]
                        || Keyboard[Key.ControlRight]) ? 1 : 0);
                    ImGuiNative.igGetIO()->KeyShift = (byte)((Keyboard[Key.ShiftLeft]
                        || Keyboard[Key.ShiftRight]) ? 1 : 0);
                    ImGuiNative.igGetIO()->KeySuper = (byte)((Keyboard[Key.WinLeft]
                        || Keyboard[Key.WinRight]) ? 1 : 0);
                }
            }
            else
            {
                io.MousePosition = new System.Numerics.Vector2(-1f, -1f);
                for (int i = 0; i <= 512; i++)
                {
                    io.KeysDown[i] = false;
                }
            }

            io.MouseDown[0] = mouseState.LeftButton == ButtonState.Pressed;
            io.MouseDown[1] = mouseState.RightButton == ButtonState.Pressed;
            io.MouseDown[2] = mouseState.MiddleButton == ButtonState.Pressed;

            float newWheelPos = mouseState.WheelPrecise;
            float delta = newWheelPos - _wheelPosition;
            _wheelPosition = newWheelPos;
            io.MouseWheel = delta;
        }

        private unsafe void RenderImDrawData(DrawData* draw_data)
        {
            // Rendering
            int display_w, display_h;
            display_w = Width;
            display_h = Height;

            Vector4 clear_color = new Vector4(114f / 255f, 144f / 255f, 154f / 255f, 1.0f);
            GL.Viewport(0, 0, display_w, display_h);
            GL.ClearColor(clear_color.X, clear_color.Y, clear_color.Z, clear_color.W);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            // We are using the OpenGL fixed pipeline to make the example code simpler to read!
            // Setup render state: alpha-blending enabled, no face culling, no depth testing, scissor enabled, vertex/texcoord/color pointers.
            int last_texture;
            GL.GetInteger(GetPName.TextureBinding2D, out last_texture);
            GL.PushAttrib(AttribMask.EnableBit | AttribMask.ColorBufferBit | AttribMask.TransformBit);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.Disable(EnableCap.CullFace);
            GL.Disable(EnableCap.DepthTest);
            GL.Enable(EnableCap.ScissorTest);
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.TextureCoordArray);
            GL.EnableClientState(ArrayCap.ColorArray);
            GL.Enable(EnableCap.Texture2D);

            GL.UseProgram(0);

            // Handle cases of screen coordinates != from framebuffer coordinates (e.g. retina displays)
            IO io = ImGui.GetIO();
            ImGui.ScaleClipRects(draw_data, io.DisplayFramebufferScale);

            // Setup orthographic projection matrix
            GL.MatrixMode(MatrixMode.Projection);
            GL.PushMatrix();
            GL.LoadIdentity();
            GL.Ortho(
                0.0f,
                io.DisplaySize.X / io.DisplayFramebufferScale.X,
                io.DisplaySize.Y / io.DisplayFramebufferScale.Y,
                0.0f,
                -1.0f,
                1.0f);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.PushMatrix();
            GL.LoadIdentity();

            // Render command lists
            for (int n = 0; n < draw_data->CmdListsCount; n++)
            {
                NativeDrawList* cmd_list = draw_data->CmdLists[n];
                byte* vtx_buffer = (byte*)cmd_list->VtxBuffer.Data;
                ushort* idx_buffer = (ushort*)cmd_list->IdxBuffer.Data;

                DrawVert vert0 = *((DrawVert*)vtx_buffer);
                DrawVert vert1 = *(((DrawVert*)vtx_buffer) + 1);
                DrawVert vert2 = *(((DrawVert*)vtx_buffer) + 2);

                GL.VertexPointer(2, VertexPointerType.Float, sizeof(DrawVert), new IntPtr(vtx_buffer + DrawVert.PosOffset));
                GL.TexCoordPointer(2, TexCoordPointerType.Float, sizeof(DrawVert), new IntPtr(vtx_buffer + DrawVert.UVOffset));
                GL.ColorPointer(4, ColorPointerType.UnsignedByte, sizeof(DrawVert), new IntPtr(vtx_buffer + DrawVert.ColOffset));

                for (int cmd_i = 0; cmd_i < cmd_list->CmdBuffer.Size; cmd_i++)
                {
                    DrawCmd* pcmd = &(((DrawCmd*)cmd_list->CmdBuffer.Data)[cmd_i]);
                    if (pcmd->UserCallback != IntPtr.Zero)
                    {
                        throw new NotImplementedException();
                    }
                    else
                    {
                        GL.BindTexture(TextureTarget.Texture2D, pcmd->TextureId.ToInt32());
                        GL.Scissor(
                            (int)pcmd->ClipRect.X,
                            (int)(io.DisplaySize.Y - pcmd->ClipRect.W),
                            (int)(pcmd->ClipRect.Z - pcmd->ClipRect.X),
                            (int)(pcmd->ClipRect.W - pcmd->ClipRect.Y));
                        ushort[] indices = new ushort[pcmd->ElemCount];
                        for (int i = 0; i < indices.Length; i++) { indices[i] = idx_buffer[i]; }
                        GL.DrawElements(PrimitiveType.Triangles, (int)pcmd->ElemCount, DrawElementsType.UnsignedShort, new IntPtr(idx_buffer));
                    }
                    idx_buffer += pcmd->ElemCount;
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
    }
}
