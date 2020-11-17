using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SkiaSharp;
using Topten.RichTextKit;

namespace Ryujinx.Skia.Ui.Skia.Widget
{
    public class Spinner : Box
    {
        private ActionButton _upButton;
        private ActionButton _downButton;
        private Entry _entry;

        public float Minimum { get; set; } = 0;
        public float Maximum { get; set; } = 100;
        public float Step { get; set; } = 1;

        public SpinnerType Type { get; set; } = SpinnerType.Numeric;

        private int _selectedIndex = -1;

        public List<string> Options { get; private set; }

        public bool WrapAround{ get; set; }

        public string Value
        {
            get
            {
                return _entry.Text;
            }
            set
            {
                switch (Type)
                {
                    case SpinnerType.Numeric:
                        if (float.TryParse(value, out float result))
                        {
                            result = Math.Clamp(result, Minimum, Maximum);

                            _entry.Text = result.ToString();
                        }
                        else
                        {
                            _entry.Text = Minimum.ToString();
                        }
                        break;
                    case SpinnerType.List:
                        if (Options.Count > 0)
                        {
                            int index = Options.FindIndex(x => x == value);
                            if (index > -1)
                            {
                                _selectedIndex = index;

                                _entry.Text = value;
                            }
                            else
                            {
                                _selectedIndex = 0;

                                _entry.Text = Options.FirstOrDefault();
                            }
                        }
                        else
                        {
                            _selectedIndex = -1;

                            _entry.Text = string.Empty;
                        }
                        break;
                }
            }
        }

        public Spinner(SKRect bounds) : base(bounds)
        {
            Orientation = Orientation.Vertical;

            ScrollEnabled = false;

            _upButton = new ActionButton("chevron-up-outline")
            {
                HorizontalAlignment = LayoutOptions.Stretch
            };

            _downButton = new ActionButton("chevron-down-outline")
            {
                HorizontalAlignment = LayoutOptions.Stretch
            };

            Options = new List<string>();

            _entry = new Entry();
            _entry.Measure(default);
            _entry.Bounds = SKRect.Create(0, 0, 60, 40);

            _upButton.Activate += Up_Button_Activate;
            _downButton.Activate += Down_Button_Activate;

            _entry.Text = "0";
        }

        public void SetOptions(List<string> options)
        {
            Options.Clear();

            if(options != null && options.Count > 0)
            {
                Options.AddRange(options);

                Type = SpinnerType.List;

                Value = Options.First();

                var longest = Options.OrderByDescending(x => x.Length).FirstOrDefault();

                var renderer = new RichString(longest);
                renderer.FontSize(_entry.FontSize);
                renderer.FontFamily(_entry.FontFamily);

                var length = renderer.MeasuredWidth + _entry.Padding.Left + _entry.Padding.Right;

                _entry.Bounds = SKRect.Create(0, 0, length, 40);

                renderer.DiscardLayout();
            }
            else 
            {
                Type = SpinnerType.Numeric;
            } 

        }

        private void Up_Button_Activate(object sender, EventArgs e)
        {
            switch (Type)
            {
                case SpinnerType.Numeric:
                    if (float.TryParse(Value, out float newValue))
                    {
                        newValue += Step;

                        if (WrapAround && newValue > Maximum)
                        {
                            newValue = Minimum;
                        }

                        newValue = Math.Clamp(newValue, Minimum, Maximum);

                        _entry.Text = newValue.ToString();
                    }
                    else
                    {
                        _entry.Text = Minimum.ToString();
                    }
                    break;
                case SpinnerType.List:
                    if (Options.Count > 0)
                    {
                        _selectedIndex++;

                        if (WrapAround && _selectedIndex >= Options.Count)
                        {
                            _selectedIndex = 0;
                        }

                        _selectedIndex = Math.Clamp(_selectedIndex, 0, Options.Count - 1);

                        Value = Options[_selectedIndex];
                    }
                    else
                    {
                        _selectedIndex = -1;

                        _entry.Text = string.Empty;
                    }
                    break;
            }
        }

        private void Down_Button_Activate(object sender, EventArgs e)
        {
            switch (Type)
            {
                case SpinnerType.Numeric:
                    if (float.TryParse(Value, out float newValue))
                    {
                        newValue -= Step;

                        if (WrapAround && newValue < Minimum)
                        {
                            newValue = Maximum;
                        }

                        newValue = Math.Clamp(newValue, Minimum, Maximum);

                        _entry.Text = newValue.ToString();
                    }
                    else
                    {
                        _entry.Text = Minimum.ToString();
                    }
                    break;
                case SpinnerType.List:
                    if (Options.Count > 0)
                    {
                        _selectedIndex--;

                        if (WrapAround && _selectedIndex < 0)
                        {
                            _selectedIndex = Options.Count - 1;
                        }

                        _selectedIndex = Math.Clamp(_selectedIndex, 0, Options.Count - 1);

                        Value = Options[_selectedIndex];
                    }
                    else
                    {
                        _selectedIndex = -1;

                        _entry.Text = string.Empty;
                    }
                    break;
            }
        }

        public override void Measure()
        {
            base.Measure();

            var width = _entry.Width;

            _upButton.Measure(SKRect.Create(_upButton.Location, new SKSize(width, _upButton.Height)));
            _downButton.Measure(SKRect.Create(_downButton.Location, new SKSize(width, _downButton.Height)));

            Width = width + Padding.Left + Padding.Right;
        }

        public override void AttachTo(Scene.Scene parent)
        {
            base.AttachTo(parent);

            Elements.Clear();

            AddElement(_upButton);
            AddElement(_entry);
            AddElement(_downButton);
        }
    }
}