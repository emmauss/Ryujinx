using Gtk;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using System;
using System.Threading;

using Color = System.Drawing.Color;

namespace Ryujinx.Ui
{
    public class ApplicationList : ScrolledWindow
    {
        public ApplicationListItem[] ListItems = Array.Empty<ApplicationListItem>();

        private const int ListPadding = 75;
        private const int ItemPadding = 20;
        private const int ItemLimit   = 6;

        private bool _lockScroll;

        private SkRenderer _renderer;

        private SKBitmap _listBitmap;

        private SKBitmap _overlayBitmap;

        private int _drawCounts;

        private AutoResetEvent _waitRenderingEvent;

        private int _mouseX, _mouseY;

        private float _scrollPos;

        public ApplicationList() : this(new Builder("Ryujinx.Ui.ApplicationList.ApplicationList.glade")) { }

        private ApplicationList(Builder builder) : base(builder.GetObject("_applicationList").Handle)
        {
            builder.Autoconnect(this);

            this.Expand = true;

            _renderer = new SkRenderer(Color.Transparent.ToSKColor());

            _renderer.DrawObjects += Renderer_Draw;

            AddEvents((int)Gdk.EventMask.AllEventsMask);

            MotionNotifyEvent += Cursor_Moved;
            SizeAllocated += Size_Allocated;

            Vadjustment.ValueChanged += List_VScrolled;

            Add(_renderer);

            _waitRenderingEvent = new AutoResetEvent(false);
        }

        private void Renderer_Draw(object sender, EventArgs e)
        {
            if (e is SKPaintSurfaceEventArgs se)
            {
                se.Surface.Canvas.Clear();

                Draw(se.Info);

                se.Surface.Canvas.DrawBitmap(_listBitmap, se.Info.Rect);
            }
        }

        public void Draw(SKImageInfo info)
        {
            if (_listBitmap != null && _drawCounts == 2)
            {
                return;
            }

            _drawCounts++;

            SKPaint itemPaint = new SKPaint
            {
                ImageFilter = SKImageFilter.CreateDropShadow(4, 4, 4, 4, new SKColor(0, 0, 0, 128), SKDropShadowImageFilterShadowMode.DrawShadowAndForeground)
            };

            int itemSize = ((Window.Width - ListPadding) / ItemLimit) - ItemPadding;

            int x = (Window.Width - ((itemSize + ItemPadding) * ItemLimit)) / 2;
            int y = ItemPadding;

            int itemCount = 0;
            int rowHeight = 0;

            SKBitmap itemResizedBitmap = new SKBitmap(itemSize, itemSize);

            ApplicationListItem selectedItem = null;

            _listBitmap?.Dispose();

            _listBitmap = new SKBitmap(info);

            using (SKCanvas canvas = new SKCanvas(_listBitmap))
            {
                canvas.Clear();

                foreach (ApplicationListItem item in ListItems)
                {
                    item.Image.ScalePixels(itemResizedBitmap, SKFilterQuality.High);

                    canvas.DrawBitmap(itemResizedBitmap, x, y, itemPaint);

                    item.Coords.X = x;
                    item.Coords.Y = y;
                    item.ResizedSize.Width  = itemSize;
                    item.ResizedSize.Height = itemSize;

                    itemCount++;

                    rowHeight = Math.Max(rowHeight, itemResizedBitmap.Height + ItemPadding);

                    if (itemCount == ItemLimit)
                    {
                        itemCount = 0;

                        x = (Window.Width - ((itemSize + ItemPadding) * ItemLimit)) / 2;
                        y += rowHeight;

                        rowHeight = 0;
                    }
                    else
                    {
                        x += itemResizedBitmap.Width + ItemPadding;
                    }

                    if (item.Selected)
                    {
                        selectedItem = item;
                    }
                }

                if (selectedItem != null)
                {
                    // Draw selection rectangle
                    SKRect selectRect = SKRect.Create(selectedItem.Coords.X - 5, selectedItem.Coords.Y - 5, selectedItem.ResizedSize.Width + 10, selectedItem.ResizedSize.Height + 10);

                    SKPaint selectRectPaint = new SKPaint
                    {
                        Style       = SKPaintStyle.Stroke,
                        StrokeWidth = 4,
                        Color       = SKColor.Parse("8AF6F8"),
                        IsAntialias = true
                    };

                    canvas.DrawRoundRect(selectRect, 2, 2, selectRectPaint);

                    // Draw tooltip triangle
                    SKPath tooltipTrianglePath = new SKPath();

                    tooltipTrianglePath.MoveTo((selectedItem.Coords.X + selectedItem.ResizedSize.Width / 2) - 10, selectedItem.Coords.Y + selectedItem.ResizedSize.Width + 30);
                    tooltipTrianglePath.LineTo(selectedItem.Coords.X + selectedItem.ResizedSize.Width / 2, selectedItem.Coords.Y + selectedItem.ResizedSize.Width + 13);
                    tooltipTrianglePath.LineTo((selectedItem.Coords.X + selectedItem.ResizedSize.Width / 2) + 10, selectedItem.Coords.Y + selectedItem.ResizedSize.Width + 30);
                    tooltipTrianglePath.Close();

                    SKPaint tooltipTrianglePaint = new SKPaint()
                    {
                        Style       = SKPaintStyle.StrokeAndFill,
                        Color       = SKColor.Parse("505050").WithAlpha(250),
                        StrokeWidth = 10,
                        IsAntialias = true
                    };

                    canvas.DrawPath(tooltipTrianglePath, tooltipTrianglePaint);

                    // Draw tooltip rectangle
                    SKPaint tooltipRectPaint = new SKPaint
                    {
                        Style       = SKPaintStyle.Fill,
                        StrokeWidth = 4,
                        Color       = SKColor.Parse("505050").WithAlpha(250),
                        ImageFilter = SKImageFilter.CreateDropShadow(4, 4, 4, 4, new SKColor(0, 0, 0, 128), SKDropShadowImageFilterShadowMode.DrawShadowAndForeground)
                    };

                    SKPaint tooltipTextPaint = new SKPaint
                    {
                        TextSize    = 24,
                        Color       = SKColor.Parse("39AEDD"),
                        Typeface    = SKTypeface.FromFamilyName("MS Gothic"),
                        IsAntialias = true
                    };

                    SKRect tooltipTextBounds = new SKRect();
                    tooltipTextPaint.MeasureText(selectedItem.Data.TitleName, ref tooltipTextBounds);

                    int tooltipWidth = (int)tooltipTextBounds.Width + 20;

                    int midpoint = (int)(selectedItem.Coords.X + selectedItem.ResizedSize.Width / 2);

                    SKRect tooltipRect = SKRect.Create(midpoint - tooltipWidth / 2, selectedItem.Coords.Y + selectedItem.ResizedSize.Width + 30, tooltipWidth, tooltipTextBounds.Height + 20);

                    canvas.DrawRect(tooltipRect, tooltipRectPaint);
                    canvas.DrawText(selectedItem.Data.TitleName, midpoint - (tooltipWidth/2) + 10, selectedItem.Coords.Y + selectedItem.ResizedSize.Width + 58, tooltipTextPaint);
                }

                _renderer.HeightRequest = y + rowHeight;

                itemPaint.Dispose();
                itemResizedBitmap.Dispose();
            }

            _waitRenderingEvent.Set();
        }

        public void DrawOverlay(SKImageInfo info)
        {

        }

        private void ClearBitmap()
        {
            _listBitmap?.Dispose();
            _listBitmap = null;

            _drawCounts = 0;

            _renderer.QueueDraw();
        }

        private void Cursor_Moved(object sender, MotionNotifyEventArgs args)
        {
            args.Event.Window.GetPointer(out _mouseX, out _mouseY, out _);

            CheckBounds();
        }

        private void Size_Allocated(object sender, SizeAllocatedArgs args)
        {
            CheckBounds();
        }

        private void List_VScrolled(object sender, EventArgs args)
        {
            _scrollPos = (int)((Adjustment)sender).Value;

            CheckBounds();
        }

        private void CheckBounds()
        {
            foreach (ApplicationListItem item in ListItems)
            {
                item.Selected = false;
            }

            foreach (ApplicationListItem item in ListItems)
            {
                if (_mouseX > item.Coords.X && _mouseX < item.Coords.X + item.ResizedSize.Width &&
                    _mouseY + _scrollPos > item.Coords.Y && _mouseY + _scrollPos < item.Coords.Y + item.ResizedSize.Height)
                {
                    item.Selected = true;
                }
            }

            ClearBitmap();
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

            ClearBitmap();
        }

        public void ClearItems()
        {
            ListItems = Array.Empty<ApplicationListItem>();

            ClearBitmap();
        }
    }
}