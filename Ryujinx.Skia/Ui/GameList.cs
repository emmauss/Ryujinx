using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Ryujinx.Skia.Ui.Skia.Widget;
using SkiaSharp;

namespace Ryujinx.Skia.Ui
{
    public class GameList : WrapLayout, ISelection
    {
        private LottieWidget _loadingWidget;

        public bool Loading
        {
            get => loading; set
            {
                loading = value;

                if (value)
                {
                    _loadingWidget.Play();
                }
                else
                {
                    _loadingWidget.Stop();
                }
            }
        }

        private int _selectedIndex = -1;
        private UIElement _selectedItem;
        private bool loading;

        public GameList(SKRect bounds) : base(bounds)
        {
            string resourceID = "Ryujinx.Skia.Ui.Assets.listloading.json";
            Assembly assembly = GetType().GetTypeInfo().Assembly;

            using (Stream stream = assembly.GetManifestResourceStream(resourceID))
            {
                var loadingBounds = SKRect.Create(0, 0, 200, 200);
                _loadingWidget = new LottieWidget(loadingBounds);
                _loadingWidget.Load(stream);
                _loadingWidget.Stop();
            }
        }

        public bool SelectionEnabled => SelectionMode != SelectionMode.None;
        public List<UIElement> SelectedItems { get; set; }

        // TODO :check if item exists in collection
        public UIElement SelectedItem
        {
            get => _selectedItem; set
            {
                int oldIndex = _selectedIndex;

                _selectedItem = value;

                SelectionChanged?.Invoke(this, new SelectionChangedEventArgs()
                {
                    PreviousIndex = oldIndex,
                    CurrentIndex = SelectedIndex,
                    SelectedItem = value
                });
            }
        }

        public override void AttachTo(Skia.Scene.Scene parent)
        {
            base.AttachTo(parent);
            _loadingWidget.AttachTo(parent);
        }

        public override void Draw(SKCanvas canvas)
        {
            SKColor gameListColor = ParentScene.Theme.BackgroundColor;

            if (ParentScene.Theme.Name == "Light")
            {
                gameListColor = SKColor.Parse("#fafafa");
            }
            else if (ParentScene.Theme.Name == "Dark")
            {
                gameListColor = SKColor.Parse("#2e2e2e");
            }

            using SKPaint paint = new SKPaint()
            {
                Color = gameListColor,
                Style = SKPaintStyle.StrokeAndFill
            };

            canvas.DrawRect(Bounds, paint);
            base.Draw(canvas);

            if (Loading)
            {
                float width = Width / 3;
                float height = Height / 3;
                float size = MathF.Min(width, height);

                SKSize loadingIndicatorSize = new SKSize(size, size);

                SKPoint loadingIndicatorLocation = new SKPoint(Bounds.MidX - size / 2, Bounds.MidY - size / 2);

                _loadingWidget.Location = loadingIndicatorLocation;
                _loadingWidget.Size = loadingIndicatorSize;

                _loadingWidget.Draw(canvas);
            }
        }

        public int SelectedIndex
        {
            get => _selectedIndex; set
            {
                int oldIndex = _selectedIndex;

                _selectedIndex = value;

                SelectionChanged?.Invoke(this, new SelectionChangedEventArgs()
                {
                    PreviousIndex = oldIndex,
                    CurrentIndex = value,
                    SelectedItem = SelectedItem
                });
            }
        }
        public SelectionMode SelectionMode { get; set; }

        public event EventHandler<SelectionChangedEventArgs> SelectionChanged;
    }
}