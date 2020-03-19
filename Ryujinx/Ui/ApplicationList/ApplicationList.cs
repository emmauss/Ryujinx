using Gtk;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using SkiaTextRenderer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using Color = System.Drawing.Color;

namespace Ryujinx.Ui
{
    public class ApplicationList : Box
    {
        public event EventHandler<UIActionEventArgs> ActionTriggered;

        public ApplicationListItem[] ListItems = Array.Empty<ApplicationListItem>();

        private readonly SKColor PrimaryColor = SKColor.Parse("36393f");
        private readonly SKColor SelectionBorderColor = SKColor.Parse("8AF6F8");
        private readonly SKColor ScrollRegionColor = SKColor.Parse("696b6e");
        private readonly SKColor ScrollBarColor = SKColor.Parse("576780");
        private readonly SKColor ScrollBarActiveColor = SKColor.Parse("6c8ab8");

        private const int ListPadding          = 75;
        private const int ItemPadding          = 20;
        private const int ItemLimit            = 6;
        private const int TransitionFrameLimit = 10;
        private const int ScrollbarWidth       = 10;
        private const int ScrollRegionMargin   = 5;
        private const int ScrollStep           = 20;

        private SkRenderer _renderer;

        private bool _overlayActive = false;

        public bool IsCanvasFaded { get; set; }
        public bool IsActionTriggered { get; set; }

        public bool OverlayActive => _currentFrame < TransitionFrameLimit || _overlayActive;

        public bool TransitionActive => _currentFrame != 0 && _currentFrame != TransitionFrameLimit;
        public bool ScrollActive => _mouseX >= _viewBounds.Right - ScrollbarWidth - ScrollRegionMargin 
                                 && _mouseX < _viewBounds.Right - ScrollRegionMargin
                                 && _canvasHeight > _viewBounds.Height;

        private bool _queueDraw;
        private bool _isFadeIn;
        private bool IsScrolling { get; set; }

        private AutoResetEvent _waitRenderingEvent;
        private int _mouseX, _mouseY, _currentFrame, _drawCount;

        private float _scrollPos;
        private float _viewPos;
        private float _canvasHeight;
        private float _fadeLevel;

        private SKRect _viewBounds;

        private Dictionary<UIAction, UIElement> _elements;
        
        public SKRect CurrentOverlayBounds { get; set; }
        public ApplicationListItem SelectedItem { get; set; }

        private bool _transitionIn = false;

        public ApplicationList() : this(new Builder("Ryujinx.Ui.ApplicationList.ApplicationList.glade")) { }

        private ApplicationList(Builder builder) : base(builder.GetObject("_applicationList").Handle)
        {
            builder.Autoconnect(this);

            this.Expand = true;

            _renderer = new SkRenderer(Color.Transparent.ToSKColor());

            _renderer.DrawObjects += Renderer_Draw;

            _renderer.Expand = true;

            _currentFrame = TransitionFrameLimit;

            AddEvents((int)Gdk.EventMask.AllEventsMask);

            MotionNotifyEvent += Cursor_Moved;
            ScrollEvent += Mouse_Scroll;
            SizeAllocated += Size_Allocated;

            Destroyed += ApplicationList_Destroyed;

            _elements = new Dictionary<UIAction, UIElement>();

            foreach(UIAction action in Enum.GetValues(typeof(UIAction)))
            {
                _elements.Add(action, new UIElement());
            }

            //Vadjustment.ValueChanged += List_VScrolled;

            _viewBounds = new SKRect();

            Add(_renderer);

            _waitRenderingEvent = new AutoResetEvent(false);
        }

        private void ApplicationList_Destroyed(object sender, EventArgs e)
        {
            _renderer.CleanUp();
        }

        System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
        private int _regionHeight;

        private void Renderer_Draw(object sender, EventArgs e)
        {
            _viewBounds = new SKRect(0, 0, _renderer.AllocatedWidth, _renderer.AllocatedHeight);
            watch.Restart();
            if (e is DrawEventArgs de)
            {
                var canvas = de.Canvas;
                canvas.Clear(PrimaryColor);

                lock (this)
                {
                    if (!(_fadeLevel == 0 && IsCanvasFaded))
                    {
                        Draw(canvas);
                        DrawScrollbar(canvas);

                        if (_currentFrame == 0)
                        {
                            watch.Restart();
                        }

                        if (_currentFrame >= 60)
                        {
                            var passed = watch.ElapsedMilliseconds;
                        }

                        using (SKPaint paint = new SKPaint())
                        {
                            if (OverlayActive)
                            {
                                DrawOverlay(canvas);
                            }

                            if (_currentFrame < TransitionFrameLimit)
                            {
                                _queueDraw = true;
                            }
                        }

                        if (_queueDraw)
                        {
                            de.QueueRender = _queueDraw;
                            _queueDraw = false;
                            _drawCount = 0;
                        }
                    }

                    if (IsCanvasFaded)
                    {
                        _fadeLevel += _isFadeIn ? 1 : -1;
                        float fadeAlpha = (float)Math.Round(_fadeLevel / 60 * 255, MidpointRounding.AwayFromZero);
                        fadeAlpha = Math.Clamp(fadeAlpha, 0, 255);
                        using (SKPaint fade = new SKPaint())
                        {
                            fade.Style = SKPaintStyle.StrokeAndFill;
                            fade.Color = Color.Black.ToSKColor().WithAlpha((byte)fadeAlpha);
                            canvas.DrawRect(_viewBounds, fade);
                        }
                        _renderer.QueueRender();

                        if (fadeAlpha == 255)
                        {
                            Disable();
                        }
                        else if (fadeAlpha == 0)
                        {
                            IsCanvasFaded = false;
                        }
                    }
                }
            }

           // var elapsed = watch.ElapsedMilliseconds;
           // Console.WriteLine(elapsed);
        }

        public void Draw(SKCanvas canvas)
        {
            if (_drawCount <= 2)
            {
                _queueDraw = false;
            }

            _drawCount++;

            SKPaint itemPaint = new SKPaint
            {
                ImageFilter = SKImageFilter.CreateDropShadow(4, 4, 4, 4, new SKColor(0, 0, 0, 128), SKDropShadowImageFilterShadowMode.DrawShadowAndForeground)
            };

            int itemSize = ((Window.Width - ListPadding) / ItemLimit) - ItemPadding;

            int x = (Window.Width - ((itemSize + ItemPadding) * ItemLimit)) / 2;
            int y = ItemPadding - (int)_viewPos;

            int itemCount = 0;
            int rowHeight = 0;

            canvas.Clear(PrimaryColor);

            float blurFilter = GetFrameValue(-1, 10);

            if (blurFilter > 0)
            {
                itemPaint.ImageFilter = SKImageFilter.CreateBlur(blurFilter, blurFilter);
                itemPaint.MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, blurFilter);
            }
            itemPaint.FilterQuality = SKFilterQuality.Low;

            foreach (ApplicationListItem item in ListItems)
            {
                SKRect imageBounds = new SKRect(x, y, x + item.Image.Width, y + item.Image.Height);
                if (IsInBounds(imageBounds))
                {
                    if (item.ResizedImage == null || item.ResizedImage.Width != itemSize)
                    {
                        item.ResizedImage?.Dispose();
                        item.ResizedImage = new SKBitmap(itemSize, itemSize);
                        item.Image.ScalePixels(item.ResizedImage, SKFilterQuality.High);
                    }
                    canvas.DrawBitmap(item.ResizedImage, x, y, itemPaint);
                }
                else
                {

                }
                item.Coords.X = x;
                item.Coords.Y = y;
                item.ResizedSize.Width = itemSize;
                item.ResizedSize.Height = itemSize;

                itemCount++;

                rowHeight = Math.Max(rowHeight, itemSize + ItemPadding);

                if (itemCount == ItemLimit)
                {
                    itemCount = 0;

                    x = (Window.Width - ((itemSize + ItemPadding) * ItemLimit)) / 2;
                    y += rowHeight;

                    rowHeight = 0;
                }
                else
                {
                    x += itemSize + ItemPadding;
                }
            }

            if (SelectedItem != null)
            {
                // Draw selection rectangle
                SKRect selectRect = SKRect.Create(SelectedItem.Coords.X - 5, SelectedItem.Coords.Y - 5, SelectedItem.ResizedSize.Width + 10, SelectedItem.ResizedSize.Height + 10);

                SKPaint selectRectPaint = new SKPaint
                {
                    Style = SKPaintStyle.Stroke,
                    StrokeWidth = 4,
                    Color = SelectionBorderColor,
                    IsAntialias = true
                };

                canvas.DrawRoundRect(selectRect, 2, 2, selectRectPaint);

                // Draw tooltip triangle
                SKPath tooltipTrianglePath = new SKPath();

                tooltipTrianglePath.MoveTo((SelectedItem.Coords.X + SelectedItem.ResizedSize.Width / 2) - 10, SelectedItem.Coords.Y + SelectedItem.ResizedSize.Width + 30);
                tooltipTrianglePath.LineTo(SelectedItem.Coords.X + SelectedItem.ResizedSize.Width / 2, SelectedItem.Coords.Y + SelectedItem.ResizedSize.Width + 13);
                tooltipTrianglePath.LineTo((SelectedItem.Coords.X + SelectedItem.ResizedSize.Width / 2) + 10, SelectedItem.Coords.Y + SelectedItem.ResizedSize.Width + 30);
                tooltipTrianglePath.Close();

                SKPaint tooltipTrianglePaint = new SKPaint()
                {
                    Style = SKPaintStyle.StrokeAndFill,
                    Color = SKColor.Parse("505050").WithAlpha(250),
                    StrokeWidth = 10,
                    IsAntialias = true
                };

                canvas.DrawPath(tooltipTrianglePath, tooltipTrianglePaint);

                // Draw tooltip rectangle
                SKPaint tooltipRectPaint = new SKPaint
                {
                    Style = SKPaintStyle.Fill,
                    StrokeWidth = 4,
                    Color = SKColor.Parse("505050").WithAlpha(250),
                    ImageFilter = SKImageFilter.CreateDropShadow(4, 4, 4, 4, new SKColor(0, 0, 0, 128), SKDropShadowImageFilterShadowMode.DrawShadowAndForeground)
                };

                SKPaint tooltipTextPaint = new SKPaint
                {
                    TextSize = 24,
                    Color = SKColor.Parse("39AEDD"),
                    Typeface = SKTypeface.FromFamilyName("Calibri"),
                    IsAntialias = true
                };

                SKRect tooltipTextBounds = new SKRect();
                tooltipTextPaint.MeasureText(SelectedItem.Data.TitleName, ref tooltipTextBounds);

                int tooltipWidth = (int)tooltipTextBounds.Width + 20;

                int midpoint = (int)(SelectedItem.Coords.X + SelectedItem.ResizedSize.Width / 2);

                SKRect tooltipRect = SKRect.Create(midpoint - tooltipWidth / 2, SelectedItem.Coords.Y + SelectedItem.ResizedSize.Width + 30, tooltipWidth, tooltipTextBounds.Height + 20);

                canvas.DrawRect(tooltipRect, tooltipRectPaint);
                canvas.DrawText(SelectedItem.Data.TitleName, midpoint - (tooltipWidth / 2) + 10, SelectedItem.Coords.Y + SelectedItem.ResizedSize.Width + 58, tooltipTextPaint);

                selectRectPaint.Dispose();
                tooltipRectPaint.Dispose();
                tooltipTextPaint.Dispose();
                tooltipTrianglePaint.Dispose();
            }

           _canvasHeight = y + rowHeight + _viewPos;

            itemPaint.Dispose();

            bool IsInBounds(SKRect bounds)
            {
                return _viewBounds.Contains(bounds.Left, bounds.Top) ||
                    _viewBounds.Contains(bounds.Right, bounds.Top) ||
                    _viewBounds.Contains(bounds.Left, bounds.Bottom) ||
                    _viewBounds.Contains(bounds.Right, bounds.Bottom);
            }
        }

        public void FadeIn()
        {
            _waitRenderingEvent.Reset();
            IsActionTriggered = true;
            _fadeLevel = 1;
            _isFadeIn = true;
            IsCanvasFaded = true;
        }

        public void ClearFade()
        {
            IsActionTriggered = false;
            _fadeLevel = 59;
            _isFadeIn = false;
            IsCanvasFaded = true;
            Enable();
        }

        public void DrawOverlay(SKCanvas canvas)
        {
            Step();

            using (SKPaint paint = new SKPaint())
            {
                float blur = GetFrameValue(10, 0);
                byte opacity = (byte)GetFrameValue(0, 255);
                paint.Color = PrimaryColor.WithAlpha(opacity);
                paint.Style = SKPaintStyle.StrokeAndFill;
                paint.MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, blur);

                if (opacity == 0)
                {
                    _overlayActive = false;
                }

                CurrentOverlayBounds = new SKRect()
                {
                    Left = GetFrameValue(_viewBounds.Left + (ListPadding + 10), _viewBounds.Left + ListPadding),
                    Right = GetFrameValue(_viewBounds.Right - (ListPadding + 10), _viewBounds.Right - ListPadding),
                    Top = GetFrameValue(_viewBounds.Top + (ListPadding + 10), _viewBounds.Top + ListPadding),
                    Bottom = GetFrameValue(_viewBounds.Bottom - (ListPadding + 10), _viewBounds.Bottom - ListPadding),
                };

                SKRect fullOverlayBounds = new SKRect()
                {
                    Left = _viewBounds.Left + ListPadding,
                    Right = _viewBounds.Right - ListPadding,
                    Top = _viewBounds.Top + ListPadding,
                    Bottom = _viewBounds.Bottom - ListPadding,
                };

                canvas.DrawRect(CurrentOverlayBounds, paint);

                float x = CurrentOverlayBounds.Left + ItemPadding;
                float y = CurrentOverlayBounds.Top + 50;

                if (SelectedItem != null)
                {
                    using (SKPaint textPaint = new SKPaint())
                    {
                        textPaint.Color = Color.White.ToSKColor().WithAlpha(opacity);
                        textPaint.Typeface = SKTypeface.FromFamilyName("Calibri");
                        textPaint.TextSize = 40;

                        canvas.DrawText(SelectedItem.Data.TitleName, x, y, textPaint);
                        y += 20;

                        canvas.DrawLine(x, y, CurrentOverlayBounds.Right - ItemPadding, y, textPaint);
                        canvas.DrawLine(x, CurrentOverlayBounds.Bottom - 70, CurrentOverlayBounds.Right - ItemPadding, CurrentOverlayBounds.Bottom - 70, textPaint);

                        y += 20;

                        x += 20;

                        textPaint.TextSize = 20;

                        canvas.DrawBitmap(SelectedItem.Image, x, y, paint);

                        x += SelectedItem.Image.Width + 100;

                        y += 20;

                        float headerWidth = textPaint.MeasureText("Save Location");

                        float textX = x + headerWidth + ItemPadding;

                        float maxInfoWidth = fullOverlayBounds.Width - textX;

                        Font font = new Font(textPaint.Typeface, textPaint.TextSize);
                        
                        canvas.DrawText("Title Id", x + 5, y, textPaint);
                        SKSize size = DrawText(canvas, SelectedItem.Data.TitleId, y, textPaint, textX, maxInfoWidth, font);
                        y += size.Height + ItemPadding / 2;

                        canvas.DrawText("Developer", x + 5, y, textPaint);
                        size = DrawText(canvas, SelectedItem.Data.Developer, y, textPaint, textX, maxInfoWidth, font);
                        y += size.Height + ItemPadding / 2;

                        canvas.DrawText("Version", x + 5, y, textPaint);
                        size = DrawText(canvas, SelectedItem.Data.Version, y, textPaint, textX, maxInfoWidth, font);
                        y += size.Height + ItemPadding / 2;

                        canvas.DrawText("Time Played", x + 5, y, textPaint);
                        size = DrawText(canvas, SelectedItem.Data.TimePlayed, y, textPaint, textX, maxInfoWidth, font);
                        y += size.Height + ItemPadding / 2;

                        canvas.DrawText("Last Played", x + 5, y, textPaint);
                        size = DrawText(canvas, SelectedItem.Data.LastPlayed, y, textPaint, textX, maxInfoWidth, font);
                        y += size.Height + ItemPadding / 2;

                        canvas.DrawText("File Type", x + 5, y, textPaint);
                        size = DrawText(canvas, SelectedItem.Data.FileExtension, y, textPaint, textX, maxInfoWidth, font);
                        y += size.Height + ItemPadding / 2;

                        canvas.DrawText("Size", x + 5, y, textPaint);
                        size = DrawText(canvas, SelectedItem.Data.FileSize, y, textPaint, textX, maxInfoWidth, font);
                        y += size.Height + ItemPadding / 2;

                        canvas.DrawText("Location", x + 5, y, textPaint);
                        size = DrawText(canvas, SelectedItem.Data.Path, y, textPaint, textX, maxInfoWidth, font);

                        y = CurrentOverlayBounds.Bottom - 60;
                        x = CurrentOverlayBounds.Left + ItemPadding;

                        int buttonHeight = 40;
                        textPaint.TextSize = 30;


                        using (SKPaint selectionPaint = new SKPaint())
                        {
                            selectionPaint.Color = SelectionBorderColor;
                            selectionPaint.StrokeWidth = 5;
                            selectionPaint.Style = SKPaintStyle.Stroke;

                            UIElement element = _elements[UIAction.Launch];
                            DrawElement(element, "Launch");

                            x += element.Rect.Width + ItemPadding;
                            element = _elements[UIAction.OpenSaveLocation];
                            DrawElement(element, "Open Save Location");

                            x += element.Rect.Width + ItemPadding;
                            element = _elements[UIAction.OpenGameDirectory];
                            DrawElement(element, "Open Game Loation");

                            void DrawElement(UIElement uiElement, string content)
                            {
                                uiElement.IsOverlayElement = true;
                                float buttonWidth = textPaint.MeasureText(content) + 20;
                                element.Rect = new SKRect(x, y, x + buttonWidth, y + buttonHeight);
                                textPaint.Style = SKPaintStyle.Stroke;
                                canvas.DrawRect(uiElement.Rect, uiElement.IsSelected ? selectionPaint : textPaint);
                                textPaint.Style = SKPaintStyle.Fill;
                                canvas.DrawText(content, x + 10, y + 30, textPaint);
                            }
                        }

                    }
                }
            }

            if (TransitionActive)
            {
                _queueDraw = true;
            }

            SKSize DrawText(SKCanvas canvas, string text, float y, SKPaint textPaint, float textX, float maxInfoWidth, Font font)
            {
                var size = TextRendererSk.MeasureText(text, font, maxInfoWidth, TextFormatFlags.Top);

                y -= 18;

                TextRendererSk.DrawText(canvas,
                                        text,
                                        font,
                                        new SKRect(textX, y, textX + size.Width, y + size.Height),
                                        textPaint.Color,
                                        TextFormatFlags.Top);
                return size;
            }
        }

        public void DrawScrollbar(SKCanvas canvas)
        {
            if (_canvasHeight > _viewBounds.Height)
            {
                int x = (int)_viewBounds.Right - ScrollbarWidth - ScrollRegionMargin;
                float y = 0 + ScrollRegionMargin;
                int barRadius = ScrollbarWidth / 2;
                _regionHeight = (int)_viewBounds.Height - ScrollRegionMargin * 2;

                using (SKPaint paint = new SKPaint())
                {
                    paint.Color = ScrollRegionColor;
                    paint.Style = SKPaintStyle.StrokeAndFill;

                    canvas.DrawRoundRect(x, y, ScrollbarWidth, _regionHeight, barRadius, barRadius, paint);

                    // calculate bar height
                    float barHeight = _viewBounds.Height / _canvasHeight * _regionHeight;

                    paint.Color = ScrollActive || IsScrolling ? ScrollBarActiveColor : ScrollBarColor;

                    y = _scrollPos;
                    y = Math.Clamp(y - barHeight / 2, 0, _regionHeight - barHeight);
                    _viewPos = y / _regionHeight * _canvasHeight;

                    canvas.DrawRoundRect(x, y + ScrollRegionMargin, ScrollbarWidth, barHeight, barRadius, barRadius, paint);
                }
            }
        }

        public bool HandleClick(Gdk.EventType eventType, uint button)
        {
            if (!IsActionTriggered)
            {
                if (!OverlayActive)
                {
                    if (eventType == Gdk.EventType.ButtonRelease)
                    {
                        if (IsScrolling)
                        {
                            IsScrolling = false;
                            _scrollPos = Math.Clamp((float)_mouseY / _viewBounds.Height * _regionHeight, 0, _regionHeight);
                        }
                    }
                    else
                    {
                        if (ScrollActive || IsScrolling)
                        {
                            if (_canvasHeight > _viewBounds.Height)
                            {
                                _scrollPos = Math.Clamp((float)_mouseY / _viewBounds.Height * _regionHeight, 0, _regionHeight);

                                if (button == 1)
                                {
                                    if (eventType == Gdk.EventType.ButtonPress)
                                    {
                                        IsScrolling = true;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (!TransitionActive && SelectedItem != null)
                            {
                                if (!OverlayActive)
                                {
                                    _overlayActive = true;
                                    ShowOverlay();

                                    _currentFrame = 0;
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (eventType == Gdk.EventType.ButtonPress)
                    {
                        if (_mouseX < CurrentOverlayBounds.Left ||
                                                            _mouseY < CurrentOverlayBounds.Top ||
                                                            _mouseX > CurrentOverlayBounds.Right ||
                                                            _mouseY > CurrentOverlayBounds.Bottom)
                        {
                            DismissOverlay();


                            _currentFrame = 0;
                        }
                        else
                        {
                            var elementPair = _elements.Where(x => (x.Value.IsOverlayElement && x.Value.IsSelected)).FirstOrDefault();

                            if (elementPair.Value != null && SelectedItem != null)
                            {
                                UIActionResult result = UIActionResult.Succcess;
                                IsActionTriggered = true;
                                switch (elementPair.Key)
                                {
                                    case UIAction.OpenSaveLocation:
                                       result = UIActions.OpenSaveDirectory(SelectedItem.Data);

                                        IsActionTriggered = false;
                                        break;
                                    case UIAction.OpenGameDirectory:
                                        result = UIActions.OpenGameDirectory(SelectedItem.Data);
                                        IsActionTriggered = false;
                                        break;
                                    default:
                                        ActionTriggered?.Invoke(this, new UIActionEventArgs() { UIAction = elementPair.Key, Item = SelectedItem });
                                        break;
                                }
                            }
                        }
                    }
                }
            }

            CheckBounds();

            return true;
        }

        public void ShowOverlay()
        {
            _transitionIn = true;
        }

        public void DismissOverlay()
        {
            _transitionIn = false;
        }

        public float GetFrameValue(float start, float limit)
        {
            if (!_transitionIn)
            {
                float temp = start;
                start = limit;
                limit = temp;
            }

            float delta = MathF.Round(((limit - start) / (TransitionFrameLimit) * _currentFrame), MidpointRounding.ToZero);

            return start + delta;
        }

        public void Step()
        {
            if (_currentFrame + 1 <= TransitionFrameLimit)
            {
                _queueDraw = true;
            }
            _currentFrame++;
            _currentFrame = _currentFrame >= TransitionFrameLimit ? TransitionFrameLimit : _currentFrame;
        }

        private void Cursor_Moved(object sender, MotionNotifyEventArgs args)
        {
            args.Event.Window.GetPointer(out _mouseX, out _mouseY, out _);

            if (!OverlayActive)
            {
                if (IsScrolling)
                {
                    _scrollPos = Math.Clamp((float)_mouseY / _viewBounds.Height * _regionHeight, 0, _regionHeight);
                }

            }

            CheckBounds();
        }

        private void Mouse_Scroll(object sender, ScrollEventArgs args)
        {
            if (!IsScrolling && !OverlayActive)
            {
                float barHeight = _viewBounds.Height / _canvasHeight * _regionHeight;

                float halfBar = barHeight / 2;

                float val = _scrollPos * _viewBounds.Height / _regionHeight;
                
                val += (float)args.Event.DeltaY * ScrollStep;

                _scrollPos = Math.Clamp((float)val / _viewBounds.Height * _regionHeight, 0 + halfBar, _regionHeight - halfBar);

                CheckBounds();
            }
        }

        private void Size_Allocated(object sender, SizeAllocatedArgs args)
        {
            CheckBounds();
        }

        private void List_VScrolled(object sender, EventArgs args)
        {
            if (OverlayActive)
            {
               // Vadjustment.Value = _scrollPos;
                return;
            }

            CheckBounds();
        }

        private void CheckBounds()
        {
            lock (this)
            {
                ApplicationListItem _old = SelectedItem;

                if (!IsActionTriggered)
                {
                    if (!OverlayActive)
                    {
                        SelectedItem = null;

                        foreach (ApplicationListItem item in ListItems)
                        {
                            if (_mouseX > item.Coords.X && _mouseX < item.Coords.X + item.ResizedSize.Width &&
                                _mouseY > item.Coords.Y && _mouseY < item.Coords.Y + item.ResizedSize.Height)
                            {
                                SelectedItem = item;
                            }
                        }
                    }
                    else
                    {
                        for (int i = (int)UIAction.Launch; i <= (int)UIAction.OpenSaveLocation; i++)
                        {
                            UIElement element = _elements[(UIAction)i];
                            element.IsSelected = false;

                            if (_mouseX > element.Rect.Left && _mouseX < element.Rect.Right &&
                                _mouseY > element.Rect.Top && _mouseY < element.Rect.Bottom)
                            {
                                element.IsSelected = true;
                            }
                        }
                    }
                }
            }

            _renderer.QueueRender();
        }

        public void AddItem(ApplicationData applicationData)
        {
            SKBitmap itemImage = SKBitmap.Decode(applicationData.Icon);

            Array.Resize(ref ListItems, ListItems.Length + 1);

            ListItems[^1] = new ApplicationListItem
            {
                Data        = applicationData,
                Image       = itemImage,
                ResizedSize = new SKSize(itemImage.Width, itemImage.Height),
                Coords      = new SKPoint()
            };

            _renderer.QueueRender();
        }

        public void ClearItems()
        {
            if(ListItems.Length > 0)
            {
                ListItems.All((x) =>
                {
                    x.Dispose();
                    return true;
                });
            }

            ListItems = Array.Empty<ApplicationListItem>();
        }

        public void Wait()
        {
            _waitRenderingEvent.WaitOne();
        }

        public void Disable()
        {
            _renderer.IsRendering = false;
            var timer = new System.Timers.Timer(1000);
            timer.Elapsed += Timer_Elapsed;
            timer.AutoReset = false;
            timer.Start();
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _waitRenderingEvent.Set();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            _waitRenderingEvent.Set();
            _renderer.Dispose();
            SelectedItem = null;
            _elements.Clear();
            ListItems = null;
            _renderer = null;
        }

        public void Enable()
        {
            _waitRenderingEvent.Set();
            _renderer.IsRendering = true;
            _renderer.QueueRender();
        }
    }
}