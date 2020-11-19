using System;
using System.Collections.Generic;
using OpenTK.Windowing.Common.Input;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Ryujinx.Skia.Ui.Skia.Scene;
using SkiaSharp;

using static Ryujinx.Skia.Ui.Skia.Widget.ContextMenu;

namespace Ryujinx.Skia.Ui.Skia.Widget
{
    public class ToggleButton : UIElement, IInput
    {
        public event EventHandler<OptionSelectedEventArgs> StateChange;
        public List<string> _states;

        public string ActiveState{ get; private set; }

        private OptionLabel _activeLabel;

        private readonly Box _stateBox;

        private readonly Rectangle _boundingRectangle;
        private UIElement _activeOption;

        public bool IsInputGrabbed { get; set; }
        public OptionType OptionType { get; }

        public event EventHandler<IInput.InputEventArgs> Input;

        public ToggleButton(OptionType optionType)
        {
            _stateBox = new Box(default)
            {
                ContentSpacing = 0,
                Padding = default,
                Orientation = Orientation.Horizontal,
                ScrollEnabled = false
            };

            _boundingRectangle = new Rectangle(default);
            _states = new List<string>();
            OptionType = optionType;
        }
        
        public override void AttachTo(Scene.Scene parent)
        {
            base.AttachTo(parent);

            _stateBox.AttachTo(parent);

            RecreateStates();
        }

        public void SetStates(List<string> states)
        {
            _states.Clear();

            _states.AddRange(states);

            RecreateStates();
        }

        public void SetSelected(string selected)
        {
            if (_states.Contains(selected) && ParentScene != null)
            {
                int index = _states.FindIndex(x => x == selected);

                if (index > -1)
                {
                    _activeLabel = _stateBox.Elements[index] as OptionLabel;
                    
                    ActiveState = selected;
                }
            }
        }

        public override void Draw(SKCanvas canvas)
        {
            base.Draw(canvas);

            if (!DrawElement)
            {
                return;
            }

            _boundingRectangle.Bounds = Bounds;

            _boundingRectangle.Size = Bounds.Size - new SKSize(Padding.Left + Padding.Right, Padding.Top + Padding.Bottom);
            _boundingRectangle.Location += new SKPoint(Padding.Left, Padding.Top);

            _boundingRectangle.FillColor = SKColors.Transparent;
            _boundingRectangle.BorderColor = SKColors.Gray;
            _boundingRectangle.CornerRadius = new SKPoint(10, 10);

            _boundingRectangle.Draw(canvas);

            canvas.Save();

            canvas.ClipRoundRect(new SKRoundRect(_boundingRectangle.Bounds, 10));

            _activeLabel?.OnSelect();

            _stateBox.Draw(canvas);

            canvas.Restore();
        }

        public void RecreateStates()
        {
            if (ParentScene != null)
            {
                _stateBox.Elements.Clear();

                foreach (var option in _states)
                {
                    OptionLabel label = new OptionLabel(option, OptionType)
                    {
                        Margin = default,
                        HorizontalAlignment = LayoutOptions.Stretch
                    };

                    label.Activate += Label_Activate;

                    _stateBox.AddElement(label);
                }

                _stateBox.Bounds = default;
                _stateBox.BackgroundColor = SKColors.Transparent;
                _stateBox.BorderColor = SKColors.Transparent;

                Measure();

                if (_states.Count > 0)
                {
                    ActiveState = _states[0];
                    _activeLabel = _stateBox.Elements[0] as OptionLabel;
                }
                else
                {
                    ActiveState = string.Empty;
                    _activeLabel = null;

                    OptionLabel label = new OptionLabel("None")
                    {
                        Margin = default,
                        HorizontalAlignment = LayoutOptions.Stretch
                    };

                    label.Activate += Label_Activate;

                    _stateBox.AddElement(label);
                }
            }
        }

        private void Label_Activate(object sender, EventArgs e)
        {
            if (sender is OptionLabel label)
            {
                string newState = label.Label;

                _activeLabel?.ResetState();

                _activeLabel = label;

                if (newState != ActiveState)
                {
                    ActiveState = newState;

                    StateChange?.Invoke(this, new OptionSelectedEventArgs() { SelectedOption = newState });
                }

                IManager.Instance.InvalidateMeasure();
            }
        }

        public void HandleKeyboard(Keys key, KeyModifiers modifiers, InputMode inputMode)
        {
        }

        public void HandleMouse(SKPoint position, InputMode inputMode)
        {
            var element = _stateBox.GetElementAtPosition(position);

            if (element != _activeOption)
            {
                _activeOption?.ResetState();
            }

            _activeOption = element as UIElement;

            if (element is IHoverable hoverable)
            {
                hoverable.OnHover();
            }

            if (element is ISelectable selectable && inputMode != InputMode.None)
            {
                selectable.OnSelect();
            }

            if (element is IAction action && inputMode == InputMode.MouseUp)
            {
                action.OnActivate();
            }
        }

        public override void Measure()
        {
            _stateBox.Size = default;

            _stateBox.Measure();

            Size = _stateBox.Size + new SKSize(Padding.Left + Padding.Right, Padding.Top + Padding.Bottom);

            _stateBox.Location = new SKPoint(Location.X + Padding.Left, Location.Y + Padding.Top);

            _stateBox.Measure();
        }

        public override void Measure(SKRect bounds)
        {
            Measure();
        }

        public void OnGrabInput()
        {
        }

        public void OnLeaveInput()
        {
        }

        public void HandleText(string text)
        {
        }
    }
}