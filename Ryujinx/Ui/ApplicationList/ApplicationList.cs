using Gtk;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using GUI = Gtk.Builder.ObjectAttribute;

namespace Ryujinx.Ui
{
    public class ApplicationList : ScrolledWindow
    {
        public event EventHandler<ItemActivatedArgs> ItemActivated;
        private const int LIST_PADDING = 200;
        private const int ITEM_PADDING = 20;
        private const int ITEM_LIMIT = 6;

        private bool _lockScroll;

        public ApplicationListItem[] _listItems = new ApplicationListItem[0];

        private SkRenderer _renderer;

        private SKBitmap _listBitmap;

        private SKBitmap _overlayBitmap;

        private int _draw_counts;

        public AutoResetEvent _waitRenderingEvent;

        private int _mouseX, _mouseY;

        private float _scrollPos;

        private string testText = "";

        int animationValue;
        bool animationRequested;

        public ApplicationList() : this(new Builder("Ryujinx.Ui.ApplicationList.ApplicationList.glade")) { }

        public ApplicationList(Builder builder) : base(builder.GetObject("_applicationList").Handle)
        {
            builder.Autoconnect(this);

            _renderer = new SkRenderer(Color.Transparent.ToSKColor());

            _renderer.DrawGraphs += Renderer_Draw;

            AddEvents((int)Gdk.EventMask.AllEventsMask);

            ButtonReleaseEvent += Item_Clicked;
            MotionNotifyEvent += Cursor_Moved;
            SizeAllocated += Size_Allocated;

            Vadjustment.ValueChanged += List_VScrolled;

            Add(_renderer);

            _waitRenderingEvent = new AutoResetEvent(false);

            //AnimationLoop(300);
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
        async Task AnimationLoop(int maxValue)
        {
            if (!animationRequested)
            {
                animationRequested = true;
                animationValue = 0;

                while (animationValue < maxValue)
                {
                    animationValue += 15;

                    _renderer.QueueDraw();

                    await Task.Delay(10);
                }

                animationRequested = false;

                ClearBitmap();
            }
        }

        public void Draw(SKImageInfo info)
        {
            if (_listBitmap != null && _draw_counts == 2)
            {
                return;
            }

            _draw_counts++;

            SKPaint itemPaint = new SKPaint
            {
                ImageFilter = SKImageFilter.CreateDropShadow(4, 4, 4, 4, new SKColor(0, 0, 0, 128), SKDropShadowImageFilterShadowMode.DrawShadowAndForeground)
            };

            int itemSize = ((Window.Width - LIST_PADDING) / ITEM_LIMIT) - ITEM_PADDING;

            int x = (Window.Width - ((itemSize + ITEM_PADDING) * ITEM_LIMIT)) / 2;
            int y = ITEM_PADDING;

            int itemCount = 0;
            int rowHeight = 0;

            SKBitmap itemResizedBitmap = new SKBitmap(itemSize, itemSize);

            ApplicationListItem selectedItem = null;

            _listBitmap?.Dispose();

            _listBitmap = new SKBitmap(info);

            using (SKCanvas canvas = new SKCanvas(_listBitmap))
            {
                canvas.Clear();

                for (int i = 0; i < _listItems.Length; i++)
                {
                    _listItems[i].Image.ScalePixels(itemResizedBitmap, SKFilterQuality.High);

                    canvas.DrawBitmap(itemResizedBitmap, x, y, itemPaint);

                    _listItems[i].Coords.X = x;
                    _listItems[i].Coords.Y = y;
                    _listItems[i].ResizedSize.Width = itemSize;
                    _listItems[i].ResizedSize.Height = itemSize;

                    itemCount++;

                    rowHeight = Math.Max(rowHeight, itemResizedBitmap.Height + ITEM_PADDING);

                    if (itemCount == ITEM_LIMIT)
                    {
                        itemCount = 0;

                        x = (Window.Width - ((itemSize + ITEM_PADDING) * ITEM_LIMIT)) / 2;
                        y += rowHeight;

                        rowHeight = 0;
                    }
                    else
                    {
                        x += itemResizedBitmap.Width + ITEM_PADDING;
                    }

                    if (_listItems[i].Selected)
                    {
                        selectedItem = _listItems[i];
                    }
                }

                if (selectedItem != null)
                {
                    // Draw selection rectangle
                    SKRect selectRect = SKRect.Create(selectedItem.Coords.X - 5, selectedItem.Coords.Y - 5, selectedItem.ResizedSize.Width + 10, selectedItem.ResizedSize.Height + 10);

                    var selectRectPaint = new SKPaint
                    {
                        Style = SKPaintStyle.Stroke,
                        StrokeWidth = 4,
                        Color = SKColor.Parse("8AF6F8"),
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
                        Typeface = SKTypeface.FromFamilyName("MS Gothic"),
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

            /*if (animationRequested)
            {
                var rect = SKRect.Create(20, 256 + 20 + 20, Window.Width - 41, animationValue);

                // the brush (fill with blue)
                var paint = new SKPaint
                {
                    Style = SKPaintStyle.Fill,
                    Color = Color.Gray.ToSKColor()
                };

                // draw fill
                canvas.DrawRect(rect, paint);

                // change the brush (stroke with red)
                paint.Style = SKPaintStyle.Stroke;
                paint.Color = Color.DarkGray.ToSKColor();

                // draw stroke
                canvas.DrawRect(rect, paint);
            }
            else
            {
                var rect = SKRect.Create(20, 256 + 20 + 20, Window.Width - 41, 300);

                // the brush (fill with blue)
                var paint = new SKPaint
                {
                    Style = SKPaintStyle.Fill,
                    Color = Color.Gray.ToSKColor()
                };

                // draw fill
                canvas.DrawRect(rect, paint);

                // change the brush (stroke with red)
                paint.Style = SKPaintStyle.Stroke;
                paint.Color = Color.DarkGray.ToSKColor();

                // draw stroke
                canvas.DrawRect(rect, paint);

                var painttext = new SKPaint
                {
                    TextSize = 24,
                    Color = Color.Black.ToSKColor(),
                    Typeface = SKTypeface.FromFamilyName("Arial"),
                    IsAntialias = true
                };

                canvas.DrawText(_listItems[0].Data.TitleName, 40, 256 + 20 + 20 + 20 + 14, painttext);
            }*/

            /*int x = 20, y = 20;
            int itemCounter = 0;

            int limit = (Window.Width / 256) - 1;
            int padding = (Window.Width - (limit * 256) - 20) / limit;

            int rowHeight = 0;

            for (int i = 0; i < _listItems.Length; i++)
            {
                float ratio = 1.0f;

                if (i != 2)
                {
                    ratio = 1.5f;
                }

                if (i == 2)
                {
                    y = 100;
                }
                else
                {
                    y = (_listItems[i].Image.Height + 100) - (int)(_listItems[i].Image.Height / ratio);
                }

                SKBitmap test = new SKBitmap((int)(_listItems[i].Image.Width / ratio), (int)(_listItems[i].Image.Height / ratio));

                _listItems[i].Image.ScalePixels(test, SKFilterQuality.High);

                SKPaint paint = new SKPaint();
                SKColor shadowColor = new SKColor(0, 0, 0, 90);

                paint.ImageFilter = SKImageFilter.CreateDropShadow(2, 2, 4, 4, shadowColor, SKDropShadowImageFilterShadowMode.DrawShadowAndForeground);

                if (i != 2)
                {
                    for (int j = 0; j < 100; j++)
                    {
                        canvas.DrawBitmap(test, x, y + j, paint);
                    }
                }
                else
                {
                    canvas.DrawBitmap(test, x, y, paint);
                }

                _listItems[i].Coords.X = x;
                _listItems[i].Coords.Y = y;


                itemCounter++;

                rowHeight = Math.Max(rowHeight, _listItems[i].Image.Height + 20);

                x += test.Width + 20;

                if (itemCounter == limit)
                {
                    itemCounter = 0;

                    x = 20;
                    y += rowHeight;

                    rowHeight = 0;
                }
                else
                {
                    x += (int)_listItems[i].Size.Height + padding;
                }
            }

            //_renderer.HeightRequest = y + rowHeight + 10;

            var painttext = new SKPaint
            {
                TextSize = 24,
                Color = SKColors.Red,
                Typeface = SKTypeface.FromFamilyName("Arial")
            };

            canvas.DrawText(testText, 30, 30, painttext);
            */
            _waitRenderingEvent.Set();
        }

        public void DrawOverlay(SKImageInfo info)
        {

        }

        private void ClearBitmap()
        {
            _listBitmap?.Dispose();
            _listBitmap = null;

            _draw_counts = 0;

            _renderer.QueueDraw();
        }

        private void Item_Clicked(object sender, ButtonReleaseEventArgs args)
        {
            _lockScroll = !_lockScroll;
            
            CheckBounds();

            //AnimationLoop(300);
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
            /*if (args.Event.DeltaY == 1)
            {
                _listItems = shiftLeft(_listItems);
            }
            else
            {
                _listItems = shiftRight(_listItems);
            }*/
            _scrollPos = (int)((Adjustment)sender).Value;

            CheckBounds();

            //AnimationLoop(300);
        }

        public ApplicationListItem[] shiftLeft(ApplicationListItem[] arr)
        {
            ApplicationListItem[] demo = new ApplicationListItem[arr.Length];

            for (int i = 0; i < arr.Length - 1; i++)
            {
                demo[i] = arr[i + 1];
            }

            demo[demo.Length - 1] = arr[0];

            return demo;
        }

        public ApplicationListItem[] shiftRight(ApplicationListItem[] arr)
        {
            ApplicationListItem[] demo = new ApplicationListItem[arr.Length];

            for (int i = 1; i < arr.Length; i++)
            {
                demo[i] = arr[i - 1];
            }

            demo[0] = arr[demo.Length - 1];

            return demo;
        }

        private void CheckBounds()
        {
            for (int i = 0; i < _listItems.Length; i++)
            {
                _listItems[i].Selected = false;
            }

            for (int i = 0; i < _listItems.Length; i++)
            {
                if (_mouseX > _listItems[i].Coords.X && _mouseX < _listItems[i].Coords.X + _listItems[i].ResizedSize.Width &&
                    _mouseY + _scrollPos > _listItems[i].Coords.Y && _mouseY + _scrollPos < _listItems[i].Coords.Y + _listItems[i].ResizedSize.Height)
                {
                    _listItems[i].Selected = true;
                }
            }

            ClearBitmap();
        }

        private static byte[] GetResourceBytes(string resourceName)
        {
            Stream resourceStream = Assembly.GetCallingAssembly().GetManifestResourceStream(resourceName);
            byte[] resourceByteArray = new byte[resourceStream.Length];

            resourceStream.Read(resourceByteArray);

            return resourceByteArray;
        }

        public void AddItem(ApplicationData applicationData)
        {
            SKBitmap itemImage = SKBitmap.Decode(applicationData.Icon);

            Array.Resize(ref _listItems, _listItems.Length + 1);

            _listItems[_listItems.Length - 1] = new ApplicationListItem()
            {
                Data = applicationData,
                Image = itemImage,
                ResizedSize = new SKSize(itemImage.Width, itemImage.Height),
                Coords = new SKPoint()
            };

            ClearBitmap();
        }

        public void ClearItems()
        {
            _listItems = new ApplicationListItem[0];

            ClearBitmap();
        }
    }
}