using System;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Ryujinx.Graphics.GAL;
using Ryujinx.Skia.Ui.Skia.Scene;
using SkiaSharp;

namespace Ryujinx.Skia.Ui
{
    public unsafe class OpenGlBackend : IUIBackend
    {
        private readonly float[] _vertices =
        {
             1f,  1f, 0.0f, 1.0f, 1.0f,
             1f, -1f, 0.0f, 1.0f, 0.0f,
            -1f, -1f, 0.0f, 0.0f, 0.0f,
            -1f, 1f, 0.0f, 0.0f, 1.0f
        };

        private readonly uint[] _indices =
         {
            0, 1, 3,
            1, 2, 3
        };

        private int _vertexShader;
        private int _fragmentShader;
        private int _shaderProgram;

        private readonly string _vertexShaderSource = @"#version 330 core

                    layout(location = 0) in vec3 aPosition;
                    layout(location = 1) in vec2 aTexCoord;
                    out vec2 texCoord;

                    void main(void)
                    {
                        texCoord = aTexCoord;

                        gl_Position = vec4(aPosition, 1.0);
                    }";

        private readonly string _fragmentShaderSource = @"#version 330

                out vec4 outputColor;

                in vec2 texCoord;
                uniform sampler2D texture0;

                void main()
                {
                    vec4 frag_color = texture(texture0, texCoord);
                    outputColor = vec4(frag_color.rgba);
                }";

        private int _vertexBufferObject;
        private int _vertexArrayObject;
        private int _elementBufferObject;

        private int _skiaFrameBuffer;
        private int _skiaTexture;
        private int _skiaRenderBuffer;
        private GRGlInterface _interface;
        private int _gameTexture;
        private unsafe Window* _rendererContext;
        private Window* _window;
        private Window* _mainContext;
        private SKSize size;
        private bool _needBinding;
        private int _gameFramebuffer;

        public OpenGlBackend(int majorVersion, int minorVersion)
        {
            MajorVersion = majorVersion;
            MinorVersion = minorVersion;
        }

        public GRContext Context { get; private set; }

        public GRBackendRenderTarget GRBackendRenderTarget { get; private set; }
        public int MajorVersion { get; }
        public int MinorVersion { get; }

        public SKImageInfo SKImageInfo { get; private set; }
        public SKSize Size
        {
            get => size; set
            {
                size = value;

                if (_rendererContext != null)
                {
                    GLFW.SetWindowSize(_rendererContext, (int)size.Width, (int)size.Height);
                }
            }
        }

        public void CopyGameFramebuffer()
        {
            int currentFbo = GL.GetInteger(GetPName.FramebufferBinding);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.BindTexture(TextureTarget.Texture2D, _gameTexture);
            GL.ReadBuffer(ReadBufferMode.Back);

            GL.CopyTexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba8, 0, 0, (int)Size.Width, (int)Size.Height, 0);

            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, currentFbo);
        }

        public GRBackendRenderTarget CreateRenderTarget()
        {
            GL.Enable(EnableCap.Multisample);
            _skiaFrameBuffer = GL.GenFramebuffer();
            _skiaTexture = GL.GenTexture();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _skiaFrameBuffer);
            GL.BindTexture(TextureTarget.Texture2D, _skiaTexture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, SKImageInfo.Width, SKImageInfo.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBaseLevel, 0);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMaxLevel, 0);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, _skiaTexture, 0);

            _skiaRenderBuffer = GL.GenRenderbuffer();
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, _skiaRenderBuffer);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.Depth24Stencil8, SKImageInfo.Width, SKImageInfo.Height);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);

            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, _skiaRenderBuffer);

            var maxSamples = Context.GetMaxSurfaceSampleCount(SKWindow.ColorType);
            GL.GetInteger(GetPName.FramebufferBinding, out var framebuffer);
            GL.GetInteger(GetPName.Samples, out var samples);
            samples = samples > maxSamples ? maxSamples : samples;
            GRGlFramebufferInfo glInfo = new GRGlFramebufferInfo((uint)framebuffer, SKWindow.ColorType.ToGlSizedFormat());
            GL.GetInteger(GetPName.StencilBits, out var stencil);

            stencil = stencil == 0 ? 8 : stencil;

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            return new GRBackendRenderTarget(SKImageInfo.Width, SKImageInfo.Height, samples, stencil, glInfo);
        }

        public void Dispose()
        {
            Context?.Dispose();
            GRBackendRenderTarget?.Dispose();
            _interface?.Dispose();
            FreeResources();
            DeleteGameBuffers();
            DisposeShaderOjects();
            GLFW.DestroyWindow(_rendererContext);
        }

        private void DeleteGameBuffers()
        {
            if (_gameTexture != 0)
            {
                GL.DeleteTexture(_gameTexture);
            }
        }

        private void DisposeShaderOjects()
        {
            GL.DeleteBuffer(_vertexBufferObject);
            GL.DeleteVertexArray(_vertexArrayObject);
            GL.DeleteBuffer(_elementBufferObject);
            GL.DeleteShader(_vertexShader);
            GL.DeleteShader(_fragmentShader);
            GL.DeleteProgram(_shaderProgram);
        }

        public void Draw()
        {
            DrawTextures();
        }

        public void InitializeSkiaSurface()
        {
            GRBackendRenderTarget?.Dispose();

            if (_interface == null)
            {
                _interface = GRGlInterface.Create();
                Context = GRContext.CreateGl(_interface);

                var limits = Context.GetResourceCacheLimit();

                Context.SetResourceCacheLimit(limits * 2);
            }

            SKImageInfo = new SKImageInfo((int)Size.Width, (int)Size.Height, SKWindow.ColorType);

            FreeResources();

            GRBackendRenderTarget = CreateRenderTarget();

            GLFW.SwapInterval(1);
        }

        public unsafe void Initilize(Window* window)
        {
            _window = window;

            GLFW.MakeContextCurrent(_window);

            CreateContexts();

            GL.LoadBindings(new GLFWBindingsContext());

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.ClearColor(Color4.Black);

            GenerateShaderObjects();
        }

        private unsafe void CreateContexts()
        {
            GLFW.WindowHint(WindowHintBool.Visible, false);
            GLFW.WindowHint(WindowHintInt.ContextVersionMajor, 4);
            GLFW.WindowHint(WindowHintInt.ContextVersionMinor, 0);
            GLFW.WindowHint(WindowHintContextApi.ContextCreationApi, ContextApi.NativeContextApi);
            GLFW.WindowHint(WindowHintClientApi.ClientApi, ClientApi.OpenGlApi);
            GLFW.WindowHint(WindowHintOpenGlProfile.OpenGlProfile, OpenGlProfile.Core);
            GLFW.WindowHint(WindowHintBool.Focused, false);
            _rendererContext = GLFW.CreateWindow((int)Size.Width, (int)Size.Height, "", null, _window);
        }

        private void GenerateShaderObjects()
        {
            _vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(_vertexShader, _vertexShaderSource);
            GL.CompileShader(_vertexShader);

            _fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(_fragmentShader, _fragmentShaderSource);
            GL.CompileShader(_fragmentShader);

            _shaderProgram = GL.CreateProgram();
            GL.AttachShader(_shaderProgram, _vertexShader);
            GL.AttachShader(_shaderProgram, _fragmentShader);

            GL.LinkProgram(_shaderProgram);

            GL.DetachShader(_shaderProgram, _vertexShader);
            GL.DetachShader(_shaderProgram, _fragmentShader);
            GL.DeleteShader(_vertexShader);
            GL.DeleteShader(_fragmentShader);

            GL.UseProgram(_shaderProgram);

            _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

            _elementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);

            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);

            var vertexLocation = GL.GetAttribLocation(_shaderProgram, "aPosition");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

            var texCoordLocation = GL.GetAttribLocation(_shaderProgram, "aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

            GL.UseProgram(0);
            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public void FreeResources()
        {
            if (_skiaFrameBuffer != 0)
            {
                GL.DeleteFramebuffer(_skiaFrameBuffer);
                GL.DeleteTexture(_skiaTexture);
                GL.DeleteRenderbuffer(_skiaRenderBuffer);
            }
        }

        public void BindGameTexture()
        {
            if (_needBinding)
            {
                _needBinding = false;

                if(_gameFramebuffer == 0)
                {
                    GL.DeleteFramebuffer(_gameFramebuffer);
                }

                _gameFramebuffer = GL.GenFramebuffer();
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, _gameFramebuffer);
                GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, _gameTexture, 0);

                GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            }
        }

        public void CreateGameResources()
        {
            _gameTexture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, _gameTexture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, SKImageInfo.Width, SKImageInfo.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBaseLevel, 0);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMaxLevel, 0);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            _needBinding = true;
        }

        public SKSurface GetSurface()
        {
            SKImageInfo = new SKImageInfo((int)Size.Width, (int)Size.Height, SKWindow.ColorType, SKAlphaType.Premul);

            return SKSurface.Create(Context, GRBackendRenderTarget, GRSurfaceOrigin.BottomLeft, SKWindow.ColorType);
        }

        public void ResetSkiaContext()
        {
            Context.ResetContext();

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _skiaFrameBuffer);
            GL.BindTexture(TextureTarget.Texture2D, _skiaTexture);
        }

        private void RebindVbo()
        {
            GL.UseProgram(_shaderProgram);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);

            GL.BindVertexArray(_vertexArrayObject);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);

            var vertexLocation = GL.GetAttribLocation(_shaderProgram, "aPosition");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

            var texCoordLocation = GL.GetAttribLocation(_shaderProgram, "aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

            GL.UseProgram(0);
            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        private void DrawTextures()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, 0);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.DrawBuffer(DrawBufferMode.ColorAttachment0);

            var scene = (IManager.Instance as SKWindow).ActiveScene;

            RebindVbo();

            GL.UseProgram(_shaderProgram);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.Viewport(0, 0, (int)Size.Width, (int)Size.Height);
            GL.BindVertexArray(_vertexArrayObject);

            if (scene is GameScene)
            {
                BindGameTexture();

                GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, 0);
                GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, _gameFramebuffer);

                GL.BlitFramebuffer(0,
                    0,
                    (int)size.Width,
                    (int)size.Height,
                    0,
                    0,
                    (int)size.Width,
                    (int)size.Height,
                    ClearBufferMask.ColorBufferBit,
                    BlitFramebufferFilter.Linear);

                GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, 0);
            }

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.BindTexture(TextureTarget.Texture2D, _skiaTexture);
            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);

            GL.UseProgram(0);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public void SwitchContext(ContextType context)
        {
            switch (context)
            {
                case ContextType.Main:
                    GLFW.MakeContextCurrent(_window);
                    break;
                case ContextType.Game:
                    GLFW.MakeContextCurrent(_rendererContext);
                    break;
                case ContextType.None:
                    GLFW.MakeContextCurrent(null);
                    break;
            }
        }

        public void Present()
        {
            GLFW.SwapBuffers(_window);
        }

        public void SwapInterval(int value)
        {
            GLFW.SwapInterval(value);
        }

        public IRenderer CreateRenderer()
        {
            var renderer = new Graphics.OpenGL.Renderer();

            renderer.InitializeBackgroundContext(_rendererContext);

            return renderer;
        }

        public void ReleaseRenderer()
        {
            // not used
        }
    }
}