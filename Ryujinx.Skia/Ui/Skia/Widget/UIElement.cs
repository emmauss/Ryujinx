using OpenTK.Mathematics;
using Ryujinx.Skia.Ui.Skia.Scene;
using SkiaSharp;
using SkiaSharp.Elements;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ryujinx.Skia.Ui.Skia.Widget
{
    public abstract class UIElement : Element, IDisposable
    {
        private System.Timers.Timer _timer;

        public ContextMenu ContextMenu
        {
            get => _contextMenu; set
            {
                _contextMenu?.Dismiss();
                _contextMenu?.Dispose();

                _contextMenu = value;
            }
        }

        public SKPoint _contextLocation = default;

        public bool IsActive { get; set; } = true;
        public Animation Animator { get; set; }

        public LayoutOptions HorizontalAlignment { get; set; }
        public LayoutOptions VerticalAlignment { get; set; }

        public Margin Margin { get; set; } = new Margin(5);
        public Margin Padding { get; set; } = new Margin(5);

        public Scene.Scene ParentScene { get; set; }

        public virtual SKColor BackgroundColor { get; set; } = SKColors.Transparent;

        public virtual SKColor ForegroundColor { get; set; }

        public bool ClipChildren { get; set; } = true;

        public event EventHandler<EventArgs> Attached;

        public bool DrawElement { get; set; }

        private Rectangle _overlayRectangle;

        private ManualResetEvent _fadeEvent;
        private ContextMenu _contextMenu;

        public UIElement()
        {
            Animator = null;
            _overlayRectangle = new Rectangle(default)
            {
                BorderColor = SKColors.Transparent,
                FillColor = SKColors.Transparent
            };

            _fadeEvent = new ManualResetEvent(false);

            _timer = new System.Timers.Timer();
            _timer.Elapsed += _timer_Elapsed;
            _timer.Stop();
            _timer.AutoReset = false;
        }

        private void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            FadeIn();
        }

        public override void Draw(SKCanvas canvas)
        {
            base.Draw(canvas);

            var matrix = canvas.TotalMatrix;

            var tranformedRect = matrix.MapRect(Bounds);

            var windowBounds = IManager.Instance.Bounds;

            DrawElement =  windowBounds.Contains(tranformedRect.Left, tranformedRect.Top)
              || windowBounds.Contains(tranformedRect.Right, tranformedRect.Top)
              || windowBounds.Contains(tranformedRect.Left, tranformedRect.Bottom)
              || windowBounds.Contains(tranformedRect.Right, tranformedRect.Bottom);
        }

        public void DrawOverlay(SKCanvas canvas)
        {
            _overlayRectangle.Bounds = Bounds;
            _overlayRectangle.Draw(canvas);
        }

        public abstract void Measure();
        public abstract void Measure(SKRect bounds);

        public virtual void AttachTo(Scene.Scene parent)
        {
            ParentScene = parent;
            Attached?.Invoke(this, null);
            IManager.Instance.InvalidateMeasure();
        }

        public virtual void Dispose()
        {
            Animator?.Stop(false);
        }

        public void StartDelay(int delayMs)
        {
            SetOverlayAlpha(255);
            Animator?.Stop();
            Animator = new Animation();
            Animator.With(0, 1, delayMs, endCallback: FadeIn);
            Animator.Play();
        }

        public virtual void FadeOut()
        {
            _fadeEvent.Reset();
            Animator?.Stop();
            Animator = new Animation();
            Animator.With(0, 255, 1000, SetOverlayAlpha, () => _fadeEvent.Set());
            Animator.Play();

            IManager.Instance?.InvalidateMeasure();
        }

        public virtual void FadeIn()
        {
            _fadeEvent.Reset();
            Animator?.Stop(false);

            IManager.Instance?.InvalidateMeasure();

            Animator = new Animation();
            Animator.With(255, 0, 1000, SetOverlayAlpha, endCallback: () =>
            {
                IsActive = true;
                _fadeEvent.Set();
            });
            Animator.Play();

            IManager.Instance?.InvalidateMeasure();
        }

        public void SetOverlayAlpha(double alpha)
        {
            IManager.Instance?.InvalidateMeasure();

            _overlayRectangle.FillColor = BackgroundColor.WithAlpha((byte)alpha);

            if (alpha == 0)
            {
                IsActive = true;
            }
            else
            {
                IsActive = false;
            }
        }

        public void ShowContextMenu(SKPoint location)
        {
            _contextLocation = location;

            LayoutContextMenu();

            ContextMenu?.Show(location);
        }

        public virtual void LayoutContextMenu()
        {
            if(ContextMenu != null)
            {
                SKRect bounds = IManager.Instance.Bounds;

                ContextMenu.Measure();

                ContextMenu.Location = _contextLocation;

                if(ContextMenu.Bottom >= bounds.Bottom - 20)
                {
                    ContextMenu.Location = new SKPoint(ContextMenu.Location.X, bounds.Bottom - ContextMenu.Height - 20);
                }
                
                if(ContextMenu.Right >= bounds.Right - 20)
                {
                    ContextMenu.Location = new SKPoint(bounds.Right - ContextMenu.Width - 20, ContextMenu.Location.Y);
                }
            }
        }

        public virtual void ResetState()
        {
        }
    }
}
