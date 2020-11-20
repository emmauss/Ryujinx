using OpenTK.Windowing.Common.Input;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SkiaSharp;
using SkiaSharp.Elements;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ryujinx.Skia.Ui.Skia.Widget
{
    public class ContextMenu : UIElement, IPopup
    {
        public event EventHandler<OptionSelectedEventArgs> OptionSelected;
        public event EventHandler<IInput.InputEventArgs> Input;

        public Dictionary<string, string> Options { get; set; }

        public bool DismissOnFocusOut { get; set; }
        public bool IsInputGrabbed { get; set; }
        public bool IsDismissed { get; set; }

        private Rectangle _boundingRectangle;

        private Box _optionBox;
        private UIElement _attachedElement;
        private UIElement _activeOption;

        public UIElement AttachedElement
        {
            get => _attachedElement; set
            {
                _attachedElement = value;

                Measure();
            }
        }

        public ContextMenu()
        {
            Options = new Dictionary<string, string>();

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

        public override void AttachTo(Scene.Scene parent)
        {
            base.AttachTo(parent);

            _optionBox.AttachTo(parent);

            RecreateOptions();
        }

        public void SetOptions(IDictionary<string, string> options)
        {
            Options.Clear();

            foreach(var option in options)
            {
                Options.Add(option.Key, option.Value);
            }

            RecreateOptions();
        }

        public void RecreateOptions()
        {
            if (ParentScene != null)
            {
                _optionBox.Elements.Clear();

                foreach (var option in Options)
                {
                    OptionLabel label = new OptionLabel(option.Value, option.Key)
                    {
                        Margin = default,
                        HorizontalAlignment = LayoutOptions.Stretch
                    };

                    label.Activate += Label_Activate;

                    _optionBox.AddElement(label);
                }

                _optionBox.Bounds = default;
                _optionBox.BackgroundColor = SKColors.Transparent;
                _optionBox.BorderColor = SKColors.Transparent;

                Measure();
            }
        }

        private void Label_Activate(object sender, EventArgs e)
        {
            if (sender is OptionLabel label)
            {
                OnOptionSelect(label.Tag);
            }
        }

        public override void Draw(SKCanvas canvas)
        {
            base.Draw(canvas);

            if (AttachedElement != null && DrawElement)
            {
                _boundingRectangle.Bounds = Bounds;

                _boundingRectangle.Size = Bounds.Size - new SKSize(Padding.Left + Padding.Right, Padding.Top + Padding.Bottom);
                _boundingRectangle.Location += new SKPoint(Padding.Left, Padding.Top);

                _boundingRectangle.FillColor = SKColors.Transparent;
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
            if (AttachedElement != null)
            {
                _optionBox.Size = default;

                _optionBox.Measure();

                Size = _optionBox.Size + new SKSize(Padding.Left + Padding.Right, Padding.Top + Padding.Bottom);

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

            ParentScene.ShowPopup(this);
        }

        public void OnGrabInput()
        {
        }

        public void OnLeaveInput()
        {

        }

        public void OnOptionSelect(string option)
        {
            ParentScene.DismissPopup();

            OptionSelected?.Invoke(this, new OptionSelectedEventArgs() { SelectedOption = option });
        }

        public override void ResetState()
        {
            base.ResetState();

            _activeOption?.ResetState();
        }

        public void HandleMouse(SKPoint position, InputMode inputMode)
        {
            var element = _optionBox.GetElementAtPosition(position);

            if (element != _activeOption)
            {
                _activeOption?.ResetState();
            }

            _activeOption = element as UIElement;

            if(element is IHoverable hoverable)
            {
                hoverable.OnHover();
            }

            if(element is ISelectable selectable && inputMode != InputMode.None)
            {
                selectable.OnSelect();
            }

            if(element is IAction action && inputMode == InputMode.MouseUp)
            {
                action.OnActivate();
            }
        }

        public void HandleKeyboard(Keys key, KeyModifiers modifiers, InputMode inputMode)
        {
        }

        public void HandleText(string text)
        {
        }

        public class OptionSelectedEventArgs : EventArgs
        {
            public string SelectedOption { get; set; }
        }
    }
}
