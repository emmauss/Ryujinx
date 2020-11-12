using SkiaSharp;

namespace Ryujinx.Skia.Ui.Skia
{
    public class Theme
    {
        public string Name{ get; set; }
        
        public SKColor SceneBackgroundColor { get; set; }
        public SKColor BackgroundColor { get; set; }
        public SKColor LightPrimaryColor { get; set; }
        public SKColor PrimaryColor { get; set; }
        public SKColor DarkPrimaryColor { get; set; }
        public SKColor SecondaryColor { get; set; }
        public SKColor ForegroundColor { get; set; }
        public SKColor HoverBackgroundColor { get; set; }
        public SKColor HoverOutlineColor { get; set; }
        public SKColor HoverForegroundColor { get; set; }
        public SKColor SelectBackgroundColor { get; set; }
        public SKColor SelectForegroundColor { get; set; }
        public SKColor ContextHoverBackgroundColor { get; set; }
        public SKColor ContextHoverOutlineColor { get; set; }
        public SKColor ContextHoverForegroundColor { get; set; }
        public SKColor ContextSelectBackgroundColor { get; set; }
        public SKColor ContextSelectForegroundColor { get; set; }
        public SKColor SelectOutlineColor { get; set; }
        public SKColor ModalBackgroundColor { get; set; }
        public SKColor ModalBackdropColor { get; set; }

        public string FontFamily { get; set; } = "Calibri";
    }
}