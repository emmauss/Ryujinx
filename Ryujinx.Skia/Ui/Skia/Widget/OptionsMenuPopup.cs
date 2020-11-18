using OpenTK.Windowing.Common.Input;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Ryujinx.Skia.Ui.Skia.Scene;
using SkiaSharp;
using SkiaSharp.Elements;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ryujinx.Skia.Ui.Skia.Widget
{
    public class OptionsMenuPopup : UIElement, IPopup
    {
        public bool DismissOnFocusOut { get; set; } = true;
        public bool IsInputGrabbed { get; set; }
        public bool IsDismissed { get; set; }

        private readonly Rectangle _boundingRectangle;

        private Box _optionBox;
        private UIElement _attachedElement;
        private UIElement _activeOption;

        public event EventHandler<IInput.InputEventArgs> Input;

        private IHoverable _hoveredElement;

        public UIElement Content
        {
            get => _attachedElement; set
            {
                _attachedElement = value;

                Measure();
            }
        }

        public OptionsMenuPopup()
        {
            _optionBox = new Box(default)
            {
                Orientation = Orientation.Vertical,
                LayoutOptions = LayoutOptions.Stretch,
                ScrollEnabled =  false
            };
            _optionBox.Padding = default;
            _optionBox.Margin = default;

            _boundingRectangle = new Rectangle(default);
        }

        public void AddWidget(UIElement widget)
        {
            _optionBox.AddElement(widget);

            _optionBox.Size = default;

            _optionBox.Measure();
        }

        public override void AttachTo(Scene.Scene parent)
        {
            base.AttachTo(parent);

            _optionBox.AttachTo(parent);
        }

        public override void Draw(SKCanvas canvas)
        {
            base.Draw(canvas);

            if (!DrawElement)
            {
                return;
            }

            if (Content != null)
            {
                _boundingRectangle.Bounds = Bounds;

                _boundingRectangle.Size = Bounds.Size - new SKSize(Padding.Left + Padding.Right, Padding.Top + Padding.Bottom);
                _boundingRectangle.Location += new SKPoint(Padding.Left, Padding.Top);

                _boundingRectangle.FillColor = ParentScene.Theme.BackgroundColor;
                _boundingRectangle.BorderColor = SKColors.Gray;
                _boundingRectangle.CornerRadius = new SKPoint(10, 10);

                _boundingRectangle.Draw(canvas);

                canvas.Save();

                canvas.ClipRoundRect(new SKRoundRect(_boundingRectangle.Bounds, 10));

                _optionBox.Draw(canvas);

                canvas.Restore();
            }
        }

        public void Dismiss()
        {
            if (IsDismissed)
            {
                return;
            }

            IsDismissed = true;
        }

        public override void Measure()
        {
            if (Content != null)
            {
                SKRect bounds = IManager.Instance.Bounds;
                
                _optionBox.Size = default;

                _optionBox.Measure();

                Size = _optionBox.Size + new SKSize(Padding.Left + Padding.Right, Padding.Top + Padding.Bottom);

                if (Bottom >= bounds.Bottom - 20)
                {
                    Location = new SKPoint(Location.X, bounds.Bottom - Height - 20);
                }

                if (Right >= bounds.Right - 20)
                {
                    Location = new SKPoint(bounds.Right - Width - 20, Location.Y);
                }

                _optionBox.Location = new SKPoint(Location.X + Padding.Left, Location.Y + Padding.Top);

                _optionBox.Measure();
            }
        }

        public override void Measure(SKRect bounds)
        {
            Measure();
        }

        public void Show(SKPoint location)
        {
            IsDismissed = false;

            Location = location;

            Measure();

            ParentScene.ShowPopup(this);
        }

        public void OnGrabInput()
        {
            throw new NotImplementedException();
        }

        public void OnLeaveInput()
        {

        }

        public override void ResetState()
        {
            base.ResetState();

            _activeOption?.ResetState();
        }

        public void HandleMouse(SKPoint position, InputMode inputMode)
        {
            var element = _optionBox.GetElementAtPosition(position);

            if (_hoveredElement != element && _hoveredElement != null)
            {
                _hoveredElement.IsHovered = false;
            }

            if (element != _activeOption)
            {
                _activeOption?.ResetState();
            }

            _activeOption = element as UIElement;

            if(element is IHoverable hoverable)
            {
                hoverable.OnHover();

                _hoveredElement = hoverable;
            }

            if(element is ISelectable selectable && inputMode != InputMode.None)
            {
                selectable.OnSelect();
            }

            if(element is IAction action && inputMode == InputMode.MouseUp)
            {
                action.OnActivate();
            }

            if (element is IInput input)
            {
                input.HandleMouse(position, inputMode);
            }
        }

        public void HandleKeyboard(Keys key, KeyModifiers modifiers, InputMode inputMode)
        {
        }

        public void HandleText(string text)
        {
        }

        public class ContextMenuOptionSelectedEventArgs : EventArgs
        {
            public string SelectedOption { get; set; }
        }
    }
}
