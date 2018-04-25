using Gtk;
using Ryujinx.Core;
using System;
using System.Threading.Tasks;
using System.Globalization;
using GUI = Gtk.Builder.ObjectAttribute;

namespace Ryujinx.UI.UI
{
    public class InputPage : Notebook
    {
        public Widget Widget => Notebook;

        Notebook Notebook;
        //Buttons
        [GUI] Button LeftAnalogUp;
        [GUI] Button LeftAnalogDown;
        [GUI] Button LeftAnalogLeft;
        [GUI] Button LeftAnalogRight;
        [GUI] Button LeftAnalogStick;
        [GUI] Button RightAnalogUp;
        [GUI] Button RightAnalogDown;
        [GUI] Button RightAnalogLeft;
        [GUI] Button RightAnalogRight;
        [GUI] Button RightAnalogStick;
        [GUI] Button DPadUp;
        [GUI] Button DPadDown;
        [GUI] Button DPadLeft;
        [GUI] Button DPadRight;
        [GUI] Button ButtonA;
        [GUI] Button ButtonB;
        [GUI] Button ButtonX;
        [GUI] Button ButtonY;
        [GUI] Button ButtonL;
        [GUI] Button ButtonR;
        [GUI] Button ButtonZL;
        [GUI] Button ButtonZR;
        [GUI] Button ButtonMinus;
        [GUI] Button ButtonPlus;

        Gdk.Key CurrentKeyPressed;
        bool IsPressed;
        bool CancelCurrentEvent;

        Builder builder = new Builder("InputPage.glade");

        public InputPage()
        {
            builder.Autoconnect(this);

            Notebook = (Notebook)builder.GetObject("InputNotebook");

            //Register Events
            LeftAnalogUp.Clicked += LeftAnalogUp_Clicked;
            LeftAnalogDown.Clicked += LeftAnalogDown_Clicked;
            LeftAnalogLeft.Clicked += LeftAnalogLeft_Clicked;
            LeftAnalogRight.Clicked += LeftAnalogRight_Clicked;
            LeftAnalogStick.Clicked += LeftAnalogStick_Clicked;

            RightAnalogUp.Clicked += RightAnalogUp_Clicked;
            RightAnalogDown.Clicked += RightAnalogDown_Clicked;
            RightAnalogLeft.Clicked += RightAnalogLeft_Clicked;
            RightAnalogRight.Clicked += RightAnalogRight_Clicked;
            RightAnalogStick.Clicked += RightAnalogStick_Clicked;

            DPadUp.Clicked += DPadUp_Clicked;
            DPadDown.Clicked += DPadDown_Clicked;
            DPadLeft.Clicked += DPadLeft_Clicked;
            DPadRight.Clicked += DPadRight_Clicked;

            ButtonA.Clicked += ButtonA_Clicked;
            ButtonB.Clicked += ButtonB_Clicked;
            ButtonX.Clicked += ButtonX_Clicked;
            ButtonY.Clicked += ButtonY_Clicked;
            ButtonZL.Clicked += ButtonZL_Clicked;
            ButtonZR.Clicked += ButtonZR_Clicked;
            ButtonMinus.Clicked += ButtonMinus_Clicked;
            ButtonPlus.Clicked += ButtonPlus_Clicked;

            // Load Values
            LeftAnalogUp.Label = ((OpenTK.Input.Key)Config.FakeJoyCon.Left.StickUp).ToString();
            LeftAnalogDown.Label = ((OpenTK.Input.Key)Config.FakeJoyCon.Left.StickDown).ToString();
            LeftAnalogLeft.Label = ((OpenTK.Input.Key)Config.FakeJoyCon.Left.StickLeft).ToString();
            LeftAnalogRight.Label = ((OpenTK.Input.Key)Config.FakeJoyCon.Left.StickRight).ToString();
            LeftAnalogStick.Label = ((OpenTK.Input.Key)Config.FakeJoyCon.Left.StickButton).ToString();

            RightAnalogUp.Label = ((OpenTK.Input.Key)Config.FakeJoyCon.Right.StickUp).ToString();
            RightAnalogDown.Label = ((OpenTK.Input.Key)Config.FakeJoyCon.Right.StickDown).ToString();
            RightAnalogLeft.Label = ((OpenTK.Input.Key)Config.FakeJoyCon.Right.StickLeft).ToString();
            RightAnalogRight.Label = ((OpenTK.Input.Key)Config.FakeJoyCon.Right.StickRight).ToString();
            RightAnalogStick.Label = ((OpenTK.Input.Key)Config.FakeJoyCon.Right.StickButton).ToString();

            DPadUp.Label = ((OpenTK.Input.Key)Config.FakeJoyCon.Left.DPadUp).ToString();
            DPadDown.Label = ((OpenTK.Input.Key)Config.FakeJoyCon.Left.DPadDown).ToString();
            DPadLeft.Label = ((OpenTK.Input.Key)Config.FakeJoyCon.Left.DPadLeft).ToString();
            DPadRight.Label = ((OpenTK.Input.Key)Config.FakeJoyCon.Left.DPadRight).ToString();

            ButtonA.Label = ((OpenTK.Input.Key)Config.FakeJoyCon.Right.ButtonA).ToString();
            ButtonB.Label = ((OpenTK.Input.Key)Config.FakeJoyCon.Right.ButtonB).ToString();
            ButtonX.Label = ((OpenTK.Input.Key)Config.FakeJoyCon.Right.ButtonX).ToString();
            ButtonY.Label = ((OpenTK.Input.Key)Config.FakeJoyCon.Right.ButtonY).ToString();
            ButtonL.Label = ((OpenTK.Input.Key)Config.FakeJoyCon.Left.ButtonL).ToString();
            ButtonR.Label = ((OpenTK.Input.Key)Config.FakeJoyCon.Right.ButtonR).ToString();
            ButtonZL.Label = ((OpenTK.Input.Key)Config.FakeJoyCon.Left.ButtonZL).ToString();
            ButtonZR.Label = ((OpenTK.Input.Key)Config.FakeJoyCon.Right.ButtonZR).ToString();
            ButtonMinus.Label = ((OpenTK.Input.Key)Config.FakeJoyCon.Left.ButtonMinus).ToString();
            ButtonPlus.Label = ((OpenTK.Input.Key)Config.FakeJoyCon.Right.ButtonPlus).ToString();
        }

        private async void ButtonPlus_Clicked(object sender, EventArgs e)
        {
            if (sender is Button ClickedButton)
            {
                OpenTK.Input.Key key = await GetKeyPress(ClickedButton);
                if (key != default(OpenTK.Input.Key))
                {
                    var joycon = Config.FakeJoyCon;
                    joycon.Right.ButtonPlus = (int)key;
                    Config.FakeJoyCon = joycon;
                }
            }
        }

        private async void ButtonMinus_Clicked(object sender, EventArgs e)
        {
            if (sender is Button ClickedButton)
            {
                OpenTK.Input.Key key = await GetKeyPress(ClickedButton);
                if (key != default(OpenTK.Input.Key))
                {
                    var joycon = Config.FakeJoyCon;
                    joycon.Left.ButtonMinus = (int)key;
                    Config.FakeJoyCon = joycon;
                }
            }
        }

        private async void ButtonZR_Clicked(object sender, EventArgs e)
        {
            if (sender is Button ClickedButton)
            {
                OpenTK.Input.Key key = await GetKeyPress(ClickedButton);
                if (key != default(OpenTK.Input.Key))
                {
                    var joycon = Config.FakeJoyCon;
                    joycon.Right.ButtonZR = (int)key;
                    Config.FakeJoyCon = joycon;
                }
            }
        }

        private async void ButtonZL_Clicked(object sender, EventArgs e)
        {
            if (sender is Button ClickedButton)
            {
                OpenTK.Input.Key key = await GetKeyPress(ClickedButton);
                if (key != default(OpenTK.Input.Key))
                {
                    var joycon = Config.FakeJoyCon;
                    joycon.Left.ButtonZL = (int)key;
                    Config.FakeJoyCon = joycon;
                }
            }
        }

        private async void ButtonY_Clicked(object sender, EventArgs e)
        {
            if (sender is Button ClickedButton)
            {
                OpenTK.Input.Key key = await GetKeyPress(ClickedButton);
                if (key != default(OpenTK.Input.Key))
                {
                    var joycon = Config.FakeJoyCon;
                    joycon.Right.ButtonY = (int)key;
                    Config.FakeJoyCon = joycon;
                }
            }
        }

        private async void ButtonX_Clicked(object sender, EventArgs e)
        {
            if (sender is Button ClickedButton)
            {
                OpenTK.Input.Key key = await GetKeyPress(ClickedButton);
                if (key != default(OpenTK.Input.Key))
                {
                    var joycon = Config.FakeJoyCon;
                    joycon.Right.ButtonX = (int)key;
                    Config.FakeJoyCon = joycon;
                }
            }
        }

        private async void ButtonB_Clicked(object sender, EventArgs e)
        {
            if (sender is Button ClickedButton)
            {
                OpenTK.Input.Key key = await GetKeyPress(ClickedButton);
                if (key != default(OpenTK.Input.Key))
                {
                    var joycon = Config.FakeJoyCon;
                    joycon.Right.ButtonB = (int)key;
                    Config.FakeJoyCon = joycon;
                }
            }
        }

        private async void ButtonA_Clicked(object sender, EventArgs e)
        {
            if (sender is Button ClickedButton)
            {
                OpenTK.Input.Key key = await GetKeyPress(ClickedButton);
                if (key != default(OpenTK.Input.Key))
                {
                    var joycon = Config.FakeJoyCon;
                    joycon.Right.ButtonA = (int)key;
                    Config.FakeJoyCon = joycon;
                }
            }
        }

        private async void DPadRight_Clicked(object sender, EventArgs e)
        {
            if (sender is Button ClickedButton)
            {
                OpenTK.Input.Key key = await GetKeyPress(ClickedButton);
                if (key != default(OpenTK.Input.Key))
                {
                    var joycon = Config.FakeJoyCon;
                    joycon.Left.DPadRight = (int)key;
                    Config.FakeJoyCon = joycon;
                }
            }
        }

        private async void DPadLeft_Clicked(object sender, EventArgs e)
        {
            if (sender is Button ClickedButton)
            {
                OpenTK.Input.Key key = await GetKeyPress(ClickedButton);
                if (key != default(OpenTK.Input.Key))
                {
                    var joycon = Config.FakeJoyCon;
                    joycon.Left.DPadLeft = (int)key;
                    Config.FakeJoyCon = joycon;
                }
            }
        }

        private async void DPadDown_Clicked(object sender, EventArgs e)
        {
            if (sender is Button ClickedButton)
            {
                OpenTK.Input.Key key = await GetKeyPress(ClickedButton);
                if (key != default(OpenTK.Input.Key))
                {
                    var joycon = Config.FakeJoyCon;
                    joycon.Left.DPadDown = (int)key;
                    Config.FakeJoyCon = joycon;
                }
            }
        }

        private async void DPadUp_Clicked(object sender, EventArgs e)
        {
            if (sender is Button ClickedButton)
            {
                OpenTK.Input.Key key = await GetKeyPress(ClickedButton);
                if (key != default(OpenTK.Input.Key))
                {
                    var joycon = Config.FakeJoyCon;
                    joycon.Left.DPadUp = (int)key;
                    Config.FakeJoyCon = joycon;
                }
            }
        }

        private async void RightAnalogStick_Clicked(object sender, EventArgs e)
        {
            if (sender is Button ClickedButton)
            {
                OpenTK.Input.Key key = await GetKeyPress(ClickedButton);
                if (key != default(OpenTK.Input.Key))
                {
                    var joycon = Config.FakeJoyCon;
                    joycon.Right.StickButton = (int)key;
                    Config.FakeJoyCon = joycon;
                }
            }
        }

        private async void RightAnalogRight_Clicked(object sender, EventArgs e)
        {
            if (sender is Button ClickedButton)
            {
                OpenTK.Input.Key key = await GetKeyPress(ClickedButton);
                if (key != default(OpenTK.Input.Key))
                {
                    var joycon = Config.FakeJoyCon;
                    joycon.Right.StickRight = (int)key;
                    Config.FakeJoyCon = joycon;
                }
            }
        }

        private async void RightAnalogLeft_Clicked(object sender, EventArgs e)
        {
            if (sender is Button ClickedButton)
            {
                OpenTK.Input.Key key = await GetKeyPress(ClickedButton);
                if (key != default(OpenTK.Input.Key))
                {
                    var joycon = Config.FakeJoyCon;
                    joycon.Right.StickLeft = (int)key;
                    Config.FakeJoyCon = joycon;
                }
            }
        }

        private async void RightAnalogDown_Clicked(object sender, EventArgs e)
        {
            if (sender is Button ClickedButton)
            {
                OpenTK.Input.Key key = await GetKeyPress(ClickedButton);
                if (key != default(OpenTK.Input.Key))
                {
                    var joycon = Config.FakeJoyCon;
                    joycon.Right.StickDown = (int)key;
                    Config.FakeJoyCon = joycon;
                }
            }
        }

        private async void RightAnalogUp_Clicked(object sender, EventArgs e)
        {
            if (sender is Button ClickedButton)
            {
                OpenTK.Input.Key key = await GetKeyPress(ClickedButton);
                if (key != default(OpenTK.Input.Key))
                {
                    var joycon = Config.FakeJoyCon;
                    joycon.Right.StickUp = (int)key;
                    Config.FakeJoyCon = joycon;
                }
            }
        }

        private async void LeftAnalogStick_Clicked(object sender, EventArgs e)
        {
            if (sender is Button ClickedButton)
            {
                OpenTK.Input.Key key = await GetKeyPress(ClickedButton);
                if (key != default(OpenTK.Input.Key))
                {
                    var joycon = Config.FakeJoyCon;
                    joycon.Left.StickButton = (int)key;
                    Config.FakeJoyCon = joycon;
                }
            }
        }

        private async void LeftAnalogRight_Clicked(object sender, EventArgs e)
        {
            if (sender is Button ClickedButton)
            {
                OpenTK.Input.Key key = await GetKeyPress(ClickedButton);
                if (key != default(OpenTK.Input.Key))
                {
                    var joycon = Config.FakeJoyCon;
                    joycon.Left.StickRight = (int)key;
                    Config.FakeJoyCon = joycon;
                }
            }
        }

        private async void LeftAnalogLeft_Clicked(object sender, EventArgs e)
        {
            if (sender is Button ClickedButton)
            {
                OpenTK.Input.Key key = await GetKeyPress(ClickedButton);
                if (key != default(OpenTK.Input.Key))
                {
                    var joycon = Config.FakeJoyCon;
                    joycon.Left.StickLeft = (int)key;
                    Config.FakeJoyCon = joycon;
                }
            }
        }

        private async void LeftAnalogDown_Clicked(object sender, EventArgs e)
        {
            if (sender is Button ClickedButton)
            {
                OpenTK.Input.Key key = await GetKeyPress(ClickedButton);
                if (key != default(OpenTK.Input.Key))
                {
                    var joycon = Config.FakeJoyCon;
                    joycon.Left.StickDown = (int)key;
                    Config.FakeJoyCon = joycon;
                }
            }
        }

        private async void LeftAnalogUp_Clicked(object sender, EventArgs e)
        {
            if (sender is Button ClickedButton)
            {
                OpenTK.Input.Key key = await GetKeyPress(ClickedButton);
                if (key != default(OpenTK.Input.Key))
                {
                    var joycon = Config.FakeJoyCon;
                    joycon.Left.StickUp = (int)key;
                    Config.FakeJoyCon = joycon;
                }
            }
        }

        private void InputPage_KeyPressEvent(object o, KeyPressEventArgs args)
        {
            CurrentKeyPressed = args.Event.Key;
            IsPressed = true;
        }

        public async Task<OpenTK.Input.Key> GetKeyPress(Button ClickedButton)
        {
            string oldLabel = ClickedButton.Label;
            try
            {
                ClickedButton.IsFocus = true;
                ClickedButton.Label = "Enter Key";
                ClickedButton.KeyPressEvent += InputPage_KeyPressEvent;
                ClickedButton.FocusOutEvent += ClickedButton_FocusOutEvent;


                ClickedButton.KeyPressEvent -= InputPage_KeyPressEvent;
                while (!IsPressed)
                {
                    if (CancelCurrentEvent)
                        return default(OpenTK.Input.Key);
                    await Task.Delay(1);
                }

                IsPressed = false;
                string KeyCode = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(CurrentKeyPressed.ToString());

                //Convert certain GTK Keys to OpenTK
                switch (CurrentKeyPressed)
                {
                    case Gdk.Key.Shift_L:
                        KeyCode = "LShift";
                        break;
                    case Gdk.Key.Shift_R:
                        KeyCode = "RShift";
                        break;
                    case Gdk.Key.Alt_L:
                        KeyCode = "LAlt";
                        break;
                    case Gdk.Key.Alt_R:
                        KeyCode = "RAlt";
                        break;
                    case Gdk.Key.Control_L:
                        KeyCode = "LControl";
                        break;
                    case Gdk.Key.Control_R:
                        KeyCode = "RControl";
                        break;
                    case Gdk.Key.dead_tilde:
                        KeyCode = "Tilde";
                        break;
                }

                return (OpenTK.Input.Key)Enum.Parse(typeof(OpenTK.Input.Key), KeyCode, true);
            }
            finally
            {
                CancelCurrentEvent = false;
                Gtk.Application.Invoke(delegate
                {
                    ClickedButton.FocusOutEvent -= ClickedButton_FocusOutEvent;
                    ClickedButton.KeyPressEvent -= InputPage_KeyPressEvent;
                    ClickedButton.Label = ClickedButton.Label.Equals("Enter Key") ? oldLabel : ClickedButton.Label;
                });
            }
        }

        private void ClickedButton_FocusOutEvent(object o, FocusOutEventArgs args)
        {
            CancelCurrentEvent = true;
            IsPressed = false;
        }

        
    }
}
