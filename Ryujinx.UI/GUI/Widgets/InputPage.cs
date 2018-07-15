using ImGuiNET;
using OpenTK.Input;
using OpenTK.Platform;
using OpenTK.Platform.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Ryujinx.Common.Input;


namespace Ryujinx.UI.Widgets
{
    public partial class ConfigurationWidget
    {
        static bool[]  Toggles = new bool[50];

        static float   ContentWidth;
        static Vector2 GroupSize;
        static Key     PressedKey;
        static string  PressedButton;
        static string  Axis;
        static bool    RequestPopup;

        static InputDevice CurrentSelectedDevice = default(InputDevice);

        static Dictionary<int, InputDevice> ConnectedHIDs;

        public static void DrawInputPage()
        {
            Vector2 AvailableSpace = ImGui.GetContentRegionAvailable();

            if (ConnectedHIDs == null)
                RefreshDevices();

            if (string.IsNullOrWhiteSpace(CurrentSelectedDevice.Name))
                CurrentSelectedDevice = ConnectedHIDs.First().Value;

            ImGui.Text("Connected Devices");

            if (ImGui.BeginCombo(string.Empty, CurrentSelectedDevice.Name,
                ComboFlags.HeightSmall))
            {
                foreach(InputDevice Device in ConnectedHIDs.Values)
                {
                    bool IsSelected = (CurrentSelectedDevice.Index == Device.Index);
                    if(ImGui.Selectable(Device.Name,IsSelected))
                    {
                        CurrentSelectedDevice = Device;
                    }
                }

                ImGui.EndCombo();
            }

            ImGui.SameLine();

            if (ImGui.Button("Refresh"))
                RefreshDevices();

            if (CurrentSelectedDevice.DeviceType == DeviceType.GamePad)
            {
                ImGui.Checkbox("Enable GamePad", ref CurrentGamePadEnable);

                ImGuiNative.igBeginGroup();

                ImGui.Text("Game Pad Deadzone");

                ImGui.SliderFloat(string.Empty, ref CurrentGamePadDeadzone, 0, 1, CurrentGamePadDeadzone.ToString(), 1f);

                ImGuiNative.igEndGroup();

                ImGui.SameLine();

                ImGuiNative.igBeginGroup();

                ImGui.Text("Game Pad Trigger Threshold");

                ImGui.SliderFloat(string.Empty, ref CurrentGamePadTriggerThreshold, 0, 1, CurrentGamePadTriggerThreshold.ToString(), 1f);

                ImGuiNative.igEndGroup();
            }

            GroupSize = new Vector2(AvailableSpace.X / 2, AvailableSpace.Y / 3);

            if (CurrentSelectedDevice.DeviceType == DeviceType.Keyboard)
                DrawKeyboardInputLayout();
            else
                DrawControllerInputLayout();

            if (Toggles.Contains(true))
            {
                ImGui.OpenPopup("Enter Key");
                RequestPopup = true;
            }
            else
                RequestPopup = false;

            if (ImGui.BeginPopupModal("Enter Key", WindowFlags.AlwaysAutoResize|
                WindowFlags.NoMove
                | WindowFlags.NoResize))
            {
                ImGui.Text("Please enter a key");

                if (!RequestPopup)
                    ImGui.CloseCurrentPopup();

                ImGui.EndPopup();
            }

            if (ImGui.Button("Reset", new Vector2(ContentWidth, 50)))
            {
                Reset();
            }

        }

        static void DrawLeftAnalog()
        {
            // Show the Left Analog bindings
            ImGui.Text("Left Analog");

            ImGuiNative.igBeginGroup();
            ImGuiNative.igBeginGroup();

            ImGui.Text("Up");

            if (ImGui.Button(((Key)KeyboardInputLayout.Left.StickUp).ToString(),
                new Vector2(ContentWidth, 50)))
            {
                Toggles[0] = true;
            }

            if (Toggles[0])
            {                
                if (GetKey(ref PressedKey))
                {
                    KeyboardInputLayout.Left.StickUp = (int)PressedKey;

                    Toggles[0] = false;
                }
            }

            ImGuiNative.igEndGroup();

            ImGui.SameLine();

            ImGuiNative.igBeginGroup();

            ImGui.Text("Down");

            if (ImGui.Button(((Key)KeyboardInputLayout.Left.StickDown).ToString(),
                new Vector2(ContentWidth, 50)))
            {
                Toggles[1] = true;
            }

            if (Toggles[1])
            {
                if (GetKey(ref PressedKey))
                {
                    KeyboardInputLayout.Left.StickDown = (int)PressedKey;

                    Toggles[1] = false;
                }
            }

            ImGuiNative.igEndGroup();

            ImGuiNative.igBeginGroup();

            ImGui.Text("Left");

            if (ImGui.Button(((Key)KeyboardInputLayout.Left.StickLeft).ToString(),
                new Vector2(ContentWidth, 50)))
            {
                Toggles[2] = true;
            }

            if (Toggles[2])
            {
                if (GetKey(ref PressedKey))
                {
                    KeyboardInputLayout.Left.StickLeft = (int)PressedKey;

                    Toggles[2] = false;
                }
            }

            ImGuiNative.igEndGroup();

            ImGui.SameLine();

            ImGuiNative.igBeginGroup();

            ImGui.Text("Right");

            if (ImGui.Button(((Key)KeyboardInputLayout.Left.StickRight).ToString(),
                new Vector2(ContentWidth, 50)))
            {
                Toggles[3] = true;
            }

            if (Toggles[3])
            {
                if (GetKey(ref PressedKey))
                {
                    KeyboardInputLayout.Left.StickRight = (int)PressedKey;

                    Toggles[3] = false;
                }
            }

            ImGuiNative.igEndGroup();

            ImGuiNative.igEndGroup();            
        }

        static void DrawRightAnalog()
        {
            //Show Right Analog Bindings
            ImGui.Text("Right Analog");

            ImGuiNative.igBeginGroup();

            ImGuiNative.igBeginGroup();

            ImGui.Text("Up");

            if (ImGui.Button(((Key)KeyboardInputLayout.Right.StickUp).ToString(),
                new Vector2(ContentWidth, 50)))
            {
                Toggles[4] = true;
            }

            if (Toggles[4])
            {
                if (GetKey(ref PressedKey))
                {
                    KeyboardInputLayout.Right.StickUp = (int)PressedKey;

                    Toggles[4] = false;
                }
            }
            ImGuiNative.igEndGroup();

            ImGui.SameLine();

            ImGuiNative.igBeginGroup();

            ImGui.Text("Down");

            if (ImGui.Button(((Key)KeyboardInputLayout.Right.StickDown).ToString(),
                new Vector2(ContentWidth, 50)))
            {
                Toggles[5] = true;
            }

            if (Toggles[5])
            {
                if (GetKey(ref PressedKey))
                {
                    KeyboardInputLayout.Right.StickDown = (int)PressedKey;

                    Toggles[5] = false;
                }
            }

            ImGuiNative.igEndGroup();

            ImGuiNative.igBeginGroup();

            ImGui.Text("Left");

            if (ImGui.Button(((Key)KeyboardInputLayout.Right.StickLeft).ToString(),
                new Vector2(ContentWidth, 50)))
            {
                Toggles[6] = true;
            }

            if (Toggles[6])
            {
                if (GetKey(ref PressedKey))
                {
                    KeyboardInputLayout.Right.StickLeft = (int)PressedKey;

                    Toggles[6] = false;
                }
            }
            ImGuiNative.igEndGroup();

            ImGui.SameLine();

            ImGuiNative.igBeginGroup();

            ImGui.Text("Right");

            if (ImGui.Button(((Key)KeyboardInputLayout.Right.StickRight).ToString(),
                new Vector2(ContentWidth, 50)))
            {
                Toggles[7] = true;
            }

            if (Toggles[7])
            {
                if (GetKey(ref PressedKey))
                {
                    KeyboardInputLayout.Right.StickRight = (int)PressedKey;

                    Toggles[7] = false;
                }
            }
            ImGuiNative.igEndGroup();

            ImGuiNative.igEndGroup();
        }

        static void DrawControllerLeftAnalog()
        {
            // Show the Left Analog bindings
            ImGui.Text("Left Analog");

            ImGuiNative.igBeginGroup();

            ImGuiNative.igBeginGroup();

            ImGui.Text("Stick");

            if (ImGui.Button(ControllerInputLayout.Left.Stick.ToString(),
                new Vector2(ContentWidth, 50)))
            {
                Toggles[0] = true;
            }

            if (Toggles[0])
            {
                if (GetAxis(ref Axis))
                {
                    ControllerInputLayout.Left.Stick =
                        (GamePadStick)Enum.Parse(typeof(GamePadStick), Axis);

                    Toggles[0] = false;
                }
            }

            ImGuiNative.igEndGroup();
            
            ImGui.SameLine();

            ImGuiNative.igBeginGroup();

            ImGui.Text("Button");

            if (ImGui.Button(ControllerInputLayout.Left.StickButton.ToString(),
                new Vector2(ContentWidth, 50)))
            {
                Toggles[3] = true;
            }

            if (Toggles[3])
            {
                if (GetButton(ref PressedButton))
                {
                    ControllerInputLayout.Left.StickButton =
                        (GamePadButton)Enum.Parse(typeof(GamePadButton),PressedButton);

                    Toggles[3] = false;
                }
            }

            ImGuiNative.igEndGroup();

            ImGuiNative.igEndGroup();
        }

        static void DrawControllerRightAnalog()
        {
            //Show Right Analog Bindings
            ImGui.Text("Right Analog");

            ImGuiNative.igBeginGroup();

            ImGuiNative.igBeginGroup();

            ImGui.Text("Stick");

            if (ImGui.Button(ControllerInputLayout.Right.Stick.ToString()
                , new Vector2(ContentWidth, 50)))
            {
                Toggles[4] = true;
            }

            if (Toggles[4])
            {
                if (GetAxis(ref Axis))
                {
                    ControllerInputLayout.Right.Stick = 
                        (GamePadStick)Enum.Parse(typeof(GamePadStick), Axis);

                    Toggles[4] = false;
                }
            }

            ImGuiNative.igEndGroup();            

            ImGui.SameLine();

            ImGuiNative.igBeginGroup();

            ImGui.Text("Button");

            if (ImGui.Button(ControllerInputLayout.Right.StickButton.ToString(),
                new Vector2(ContentWidth, 50)))
            {
                Toggles[7] = true;
            }

            if (Toggles[7])
            {
                if (GetButton(ref PressedButton))
                {
                    ControllerInputLayout.Right.StickButton =
                        (GamePadButton)Enum.Parse(typeof(GamePadButton),PressedButton);

                    Toggles[7] = false;
                }
            }
            ImGuiNative.igEndGroup();

            ImGuiNative.igEndGroup();
        }

        static void DrawDpad()
        {
            string ButtonHeader = string.Empty;

            //Show DPad Bindings
            ImGui.Text("D-Pad");

            ImGuiNative.igBeginGroup();

            ImGuiNative.igBeginGroup();

            ImGui.Text("Up");

            ButtonHeader = CurrentSelectedDevice.DeviceType == DeviceType.Keyboard ?
                ((Key)KeyboardInputLayout.Left.DPadUp).ToString()
                : ControllerInputLayout.Left.DPadUp.ToString();

            if (ImGui.Button(ButtonHeader, new Vector2(ContentWidth, 50)))
            {
                Toggles[8] = true;
            }

            if (Toggles[8])
            {
                switch(CurrentSelectedDevice.DeviceType)
                {
                    case DeviceType.GamePad:
                        if (GetButton(ref PressedButton))
                        {
                            ControllerInputLayout.Left.DPadUp =
                                (GamePadButton)Enum.Parse(typeof(GamePadButton),PressedButton);

                            Toggles[8] = false;
                        }
                        break;
                    case DeviceType.Keyboard:
                        if (GetKey(ref PressedKey))
                        {
                            KeyboardInputLayout.Left.DPadUp = (int)PressedKey;

                            Toggles[8] = false;
                        }
                        break;
                }
            }

            ImGuiNative.igEndGroup();

            ImGui.SameLine();

            ImGuiNative.igBeginGroup();

            ImGui.Text("Down");

            ButtonHeader = CurrentSelectedDevice.DeviceType == DeviceType.Keyboard ?
                ((Key)KeyboardInputLayout.Left.DPadDown).ToString()
                : ControllerInputLayout.Left.DPadDown.ToString();

            if (ImGui.Button(ButtonHeader, new Vector2(ContentWidth, 50)))
            {
                Toggles[9] = true;
            }

            if (Toggles[9])
            {
                switch (CurrentSelectedDevice.DeviceType)
                {
                    case DeviceType.GamePad:
                        if (GetButton(ref PressedButton))
                        {
                            ControllerInputLayout.Left.DPadDown =
                                (GamePadButton)Enum.Parse(typeof(GamePadButton),PressedButton);
                            Toggles[9] = false;
                        }
                        break;
                    case DeviceType.Keyboard:
                        if (GetKey(ref PressedKey))
                        {
                            KeyboardInputLayout.Left.DPadDown = (int)PressedKey;
                            Toggles[9] = false;
                        }
                        break;
                }
            }

            ImGuiNative.igEndGroup();

            ImGuiNative.igBeginGroup();

            ImGui.Text("Left");

            ButtonHeader = CurrentSelectedDevice.DeviceType == DeviceType.Keyboard ?
                ((Key)KeyboardInputLayout.Left.DPadLeft).ToString()
                : ControllerInputLayout.Left.DPadLeft.ToString();

            if (ImGui.Button(ButtonHeader, new Vector2(ContentWidth, 50)))
            {
                Toggles[10] = true;
            }

            if (Toggles[10])
            {
                switch (CurrentSelectedDevice.DeviceType)
                {
                    case DeviceType.GamePad:
                        if (GetButton(ref PressedButton))
                        {
                            ControllerInputLayout.Left.DPadLeft =
                                (GamePadButton)Enum.Parse(typeof(GamePadButton),PressedButton);

                            Toggles[10] = false;
                        }
                        break;
                    case DeviceType.Keyboard:
                        if (GetKey(ref PressedKey))
                        {
                            KeyboardInputLayout.Left.DPadLeft = (int)PressedKey;

                            Toggles[10] = false;
                        }
                        break;
                }
            }

            ImGuiNative.igEndGroup();

            ImGui.SameLine();

            ImGuiNative.igBeginGroup();

            ImGui.Text("Right");

            ButtonHeader = CurrentSelectedDevice.DeviceType == DeviceType.Keyboard ?
                ((Key)KeyboardInputLayout.Left.DPadRight).ToString()
                : ControllerInputLayout.Left.DPadRight.ToString();

            if (ImGui.Button(ButtonHeader, new Vector2(ContentWidth, 50)))
            {
                Toggles[11] = true;
            }

            if (Toggles[11])
            {
                switch (CurrentSelectedDevice.DeviceType)
                {
                    case DeviceType.GamePad:
                        if (GetButton(ref PressedButton))
                        {
                            ControllerInputLayout.Left.DPadRight =
                                (GamePadButton)Enum.Parse(typeof(GamePadButton),PressedButton);

                            Toggles[11] = false;
                        }
                        break;
                    case DeviceType.Keyboard:
                        if (GetKey(ref PressedKey))
                        {
                            KeyboardInputLayout.Left.DPadRight = (int)PressedKey;

                            Toggles[11] = false;
                        }
                        break;
                }
            }

            ImGuiNative.igEndGroup();

            ImGuiNative.igEndGroup();
        }

        static void DrawActionKeys()
        {
            string ButtonHeader = string.Empty;
            
            //Show Action Key Bindings
            ImGui.Text("Action Keys");

            ImGuiNative.igBeginGroup();

            ImGuiNative.igBeginGroup();

            ImGui.Text("A");

            ButtonHeader = CurrentSelectedDevice.DeviceType == DeviceType.Keyboard ?
                ((Key)KeyboardInputLayout.Right.ButtonA).ToString()
                : ControllerInputLayout.Right.ButtonA.ToString();

            if (ImGui.Button(ButtonHeader, new Vector2(ContentWidth, 50)))
            {
                Toggles[12] = true;
            }

            if (Toggles[12])
            {
                switch (CurrentSelectedDevice.DeviceType)
                {
                    case DeviceType.GamePad:
                        if (GetButton(ref PressedButton))
                        {
                            ControllerInputLayout.Right.ButtonA =
                                (GamePadButton)Enum.Parse(typeof(GamePadButton),PressedButton);

                            Toggles[12] = false;
                        }
                        break;
                    case DeviceType.Keyboard:
                        if (GetKey(ref PressedKey))
                        {
                            KeyboardInputLayout.Right.ButtonA = (int)PressedKey;

                            Toggles[12] = false;
                        }
                        break;
                }
            }

            ImGuiNative.igEndGroup();

            ImGui.SameLine();

            ImGuiNative.igBeginGroup();

            ImGui.Text("B");

            ButtonHeader = CurrentSelectedDevice.DeviceType == DeviceType.Keyboard ?
                ((Key)KeyboardInputLayout.Right.ButtonB).ToString()
                : ControllerInputLayout.Right.ButtonB.ToString();

            if (ImGui.Button(ButtonHeader, new Vector2(ContentWidth, 50)))
            {
                Toggles[13] = true;
            }

            if (Toggles[13])
            {
                switch (CurrentSelectedDevice.DeviceType)
                {
                    case DeviceType.GamePad:
                        if (GetButton(ref PressedButton))
                        {
                            ControllerInputLayout.Right.ButtonB =
                                (GamePadButton)Enum.Parse(typeof(GamePadButton),PressedButton);

                            Toggles[13] = false;
                        }
                        break;
                    case DeviceType.Keyboard:
                        if (GetKey(ref PressedKey))
                        {
                            KeyboardInputLayout.Right.ButtonB = (int)PressedKey;

                            Toggles[13] = false;
                        }
                        break;
                }
            }

            ImGuiNative.igEndGroup();

            ImGuiNative.igBeginGroup();

            ImGui.Text("X");

            ButtonHeader = CurrentSelectedDevice.DeviceType == DeviceType.Keyboard ?
                ((Key)KeyboardInputLayout.Right.ButtonX).ToString()
                : ControllerInputLayout.Right.ButtonX.ToString();

            if (ImGui.Button(ButtonHeader, new Vector2(ContentWidth, 50)))
            {
                Toggles[14] = true;
            }

            if (Toggles[14])
            {
                switch (CurrentSelectedDevice.DeviceType)
                {
                    case DeviceType.GamePad:
                        if (GetButton(ref PressedButton))
                        {
                            ControllerInputLayout.Right.ButtonX =
                                (GamePadButton)Enum.Parse(typeof(GamePadButton),PressedButton);

                            Toggles[14] = false;
                        }
                        break;
                    case DeviceType.Keyboard:
                        if (GetKey(ref PressedKey))
                        {
                            KeyboardInputLayout.Right.ButtonX = (int)PressedKey;

                            Toggles[14] = false;
                        }
                        break;
                }
            }

            ImGuiNative.igEndGroup();

            ImGui.SameLine();

            ImGuiNative.igBeginGroup();

            ImGui.Text("Y");

            ButtonHeader = CurrentSelectedDevice.DeviceType == DeviceType.Keyboard ?
                ((Key)KeyboardInputLayout.Right.ButtonY).ToString()
                : ControllerInputLayout.Right.ButtonY.ToString();

            if (ImGui.Button(ButtonHeader, new Vector2(ContentWidth, 50)))
            {
                Toggles[15] = true;
            }

            if (Toggles[15])
            {
                switch (CurrentSelectedDevice.DeviceType)
                {
                    case DeviceType.GamePad:
                        if (GetButton(ref PressedButton))
                        {
                            ControllerInputLayout.Right.ButtonY =
                                (GamePadButton)Enum.Parse(typeof(GamePadButton),PressedButton);

                            Toggles[15] = false;
                        }
                        break;
                    case DeviceType.Keyboard:
                        if (GetKey(ref PressedKey))
                        {
                            KeyboardInputLayout.Right.ButtonY = (int)PressedKey;

                            Toggles[15] = false;
                        }
                        break;
                }
            }

            ImGuiNative.igEndGroup();

            ImGuiNative.igEndGroup();
        }

        static void DrawTriggers()
        {
            string ButtonHeader = string.Empty;

            //Draw Triggers
            ImGuiNative.igBeginGroup();

            ImGui.Text("Triggers");

            ImGuiNative.igBeginGroup();

            ImGui.Text("L");

            ButtonHeader = CurrentSelectedDevice.DeviceType == DeviceType.Keyboard ?
                ((Key)KeyboardInputLayout.Left.ButtonL).ToString()
                : ControllerInputLayout.Left.ButtonL.ToString();

            if (ImGui.Button(ButtonHeader, new Vector2(ContentWidth, 50)))
            {
                Toggles[17] = true;
            }

            if (Toggles[17])
            {
                switch (CurrentSelectedDevice.DeviceType)
                {
                    case DeviceType.GamePad:
                        if (GetButton(ref PressedButton))
                        {
                            ControllerInputLayout.Left.ButtonL =
                                (GamePadButton)Enum.Parse(typeof(GamePadButton),PressedButton);

                            Toggles[17] = false;
                        }
                        break;
                    case DeviceType.Keyboard:
                        if (GetKey(ref PressedKey))
                        {
                            KeyboardInputLayout.Left.ButtonL = (int)PressedKey;

                            Toggles[17] = false;
                        }
                        break;
                }
            }

            ImGuiNative.igEndGroup();

            ImGui.SameLine();

            ImGuiNative.igBeginGroup();

            ImGui.Text("R");

            ButtonHeader = CurrentSelectedDevice.DeviceType == DeviceType.Keyboard ?
                ((Key)KeyboardInputLayout.Right.ButtonR).ToString()
                : ControllerInputLayout.Right.ButtonR.ToString();

            if (ImGui.Button(ButtonHeader, new Vector2(ContentWidth, 50)))
            {
                Toggles[16] = true;
            }

            if (Toggles[16])
            {
                switch (CurrentSelectedDevice.DeviceType)
                {
                    case DeviceType.GamePad:
                        if (GetButton(ref PressedButton))
                        {
                            ControllerInputLayout.Right.ButtonR =
                                (GamePadButton)Enum.Parse(typeof(GamePadButton),PressedButton);

                            Toggles[16] = false;
                        }
                        break;
                    case DeviceType.Keyboard:
                        if (GetKey(ref PressedKey))
                        {
                            KeyboardInputLayout.Right.ButtonR = (int)PressedKey;

                            Toggles[16] = false;
                        }
                        break;
                }
            }

            ImGuiNative.igEndGroup();

            ImGuiNative.igBeginGroup();

            ImGui.Text("ZL");

            ButtonHeader = CurrentSelectedDevice.DeviceType == DeviceType.Keyboard ?
                ((Key)KeyboardInputLayout.Left.ButtonZL).ToString()
                : ControllerInputLayout.Left.ButtonZL.ToString();

            if (ImGui.Button(ButtonHeader, new Vector2(ContentWidth, 50)))
            {
                Toggles[19] = true;
            }

            if (Toggles[19])
            {
                switch (CurrentSelectedDevice.DeviceType)
                {
                    case DeviceType.GamePad:
                        if (GetButton(ref PressedButton))
                        {
                            ControllerInputLayout.Left.ButtonZL =
                                (GamePadButton)Enum.Parse(typeof(GamePadButton),PressedButton);

                            Toggles[19] = false;
                        }
                        break;
                    case DeviceType.Keyboard:
                        if (GetKey(ref PressedKey))
                        {
                            KeyboardInputLayout.Left.ButtonZL = (int)PressedKey;

                            Toggles[19] = false;
                        }
                        break;
                }
            }

            ImGuiNative.igEndGroup();

            ImGui.SameLine();

            ImGuiNative.igBeginGroup();

            ImGui.Text("ZR");

            ButtonHeader = CurrentSelectedDevice.DeviceType == DeviceType.Keyboard ?
                ((Key)KeyboardInputLayout.Right.ButtonZR).ToString()
                : ControllerInputLayout.Right.ButtonZR.ToString();

            if (ImGui.Button(ButtonHeader, new Vector2(ContentWidth, 50)))
            {
                Toggles[18] = true;
            }

            if (Toggles[18])
            {
                switch (CurrentSelectedDevice.DeviceType)
                {
                    case DeviceType.GamePad:
                        if (GetButton(ref PressedButton))
                        {
                            ControllerInputLayout.Right.ButtonZR =
                                (GamePadButton)Enum.Parse(typeof(GamePadButton),PressedButton);

                            Toggles[18] = false;
                        }
                        break;
                    case DeviceType.Keyboard:
                        if (GetKey(ref PressedKey))
                        {
                            KeyboardInputLayout.Right.ButtonZR = (int)PressedKey;

                            Toggles[18] = false;
                        }
                        break;
                }
            }

            ImGuiNative.igEndGroup();

            ImGuiNative.igEndGroup();
        }

        static void DrawExtras()
        {
            string ButtonHeader = string.Empty;

            //Draw Extra
            ImGuiNative.igBeginGroup();

            ImGui.Text("Extra Keys");

            ImGuiNative.igBeginGroup();

            ImGui.Text("-");

            ButtonHeader = CurrentSelectedDevice.DeviceType == DeviceType.Keyboard ?
                ((Key)KeyboardInputLayout.Left.ButtonMinus).ToString()
                : ControllerInputLayout.Left.ButtonMinus.ToString();

            if (ImGui.Button(ButtonHeader, new Vector2(ContentWidth, 50)))
            {
                Toggles[20] = true;
            }

            if (Toggles[20])
            {
                switch (CurrentSelectedDevice.DeviceType)
                {
                    case DeviceType.GamePad:
                        if (GetButton(ref PressedButton))
                        {
                            ControllerInputLayout.Left.ButtonMinus =
                                (GamePadButton)Enum.Parse(typeof(GamePadButton),PressedButton);

                            Toggles[20] = false;
                        }
                        break;
                    case DeviceType.Keyboard:
                        if (GetKey(ref PressedKey))
                        {
                            KeyboardInputLayout.Left.ButtonMinus = (int)PressedKey;

                            Toggles[20] = false;
                        }
                        break;
                }
            }

            ImGuiNative.igEndGroup();

            ImGuiNative.igBeginGroup();

            ImGui.Text("+");

            ButtonHeader = CurrentSelectedDevice.DeviceType == DeviceType.Keyboard ?
                ((Key)KeyboardInputLayout.Right.ButtonPlus).ToString() 
                : ControllerInputLayout.Right.ButtonPlus.ToString();

            if (ImGui.Button(ButtonHeader, new Vector2(ContentWidth, 50)))
            {
                Toggles[21] = true;
            }

            if (Toggles[21])
            {
                switch (CurrentSelectedDevice.DeviceType)
                {
                    case DeviceType.GamePad:
                        if (GetButton(ref PressedButton))
                        {
                            ControllerInputLayout.Right.ButtonPlus = 
                                (GamePadButton)Enum.Parse(typeof(GamePadButton),PressedButton);

                            Toggles[21] = false;
                        }
                        break;
                    case DeviceType.Keyboard:
                        if (GetKey(ref PressedKey))
                        {
                            KeyboardInputLayout.Right.ButtonPlus = (int)PressedKey;

                            Toggles[21] = false;
                        }
                        break;
                }
            }

            ImGuiNative.igEndGroup();

            ImGuiNative.igEndGroup();
        }

        static bool GetKey(ref Key PressedKey)
        {
            IO IO = ImGui.GetIO();

            foreach (Key Key in Enum.GetValues(typeof(Key)))
            {
                if (IO.KeysDown[(int)Key])
                {
                    PressedKey = Key;

                    return true;
                }
            }
            return false;
        }

        static bool GetButton(ref string PressedButton)
        {
            IO IO = ImGui.GetIO();

            GamePadState GamePad = OpenTK.Input.GamePad.GetState(Config.GamePadIndex);

            foreach (GamePadButton Button in Enum.GetValues(typeof(GamePadButton)))
            {
                if (WindowHelper.IsGamePadButtonPressed(GamePad, Button))
                {
                    PressedButton = Button.ToString();

                    return true;
                }
            }
            return false;
        }

        static bool GetAxis(ref string Axis)
        {
            IO IO = ImGui.GetIO();

            GamePadState GamePad = OpenTK.Input.GamePad.GetState(Config.GamePadIndex);

            foreach (GamePadStick Stick in Enum.GetValues(typeof(GamePadStick)))
            {
                if (WindowHelper.GetJoystickAxis(GamePad,Stick).Length > 0)
                {
                    Axis = Stick.ToString();

                    return true;
                }
            }
            return false;
        }

        static void DrawKeyboardInputLayout()
        {
            if (ImGui.BeginChildFrame(11, GroupSize, WindowFlags.AlwaysAutoResize))
            {
                ContentWidth = (ImGui.GetContentRegionAvailableWidth() - 10) / 2;
                GroupSize    = ImGui.GetContentRegionMax();

                DrawLeftAnalog();

                ImGui.EndChildFrame();
            }

            ImGui.SameLine();

            if (ImGui.BeginChildFrame(12, GroupSize, WindowFlags.AlwaysAutoResize))
            {
                DrawRightAnalog();

                ImGui.EndChildFrame();
            }

            DrawMainLayout();
        }

        static void DrawControllerInputLayout()
        {
            if (ImGui.BeginChildFrame(11, GroupSize, WindowFlags.AlwaysAutoResize))
            {
                ContentWidth = (ImGui.GetContentRegionAvailableWidth() - 10) / 2;
                GroupSize    = ImGui.GetContentRegionMax();

                DrawControllerLeftAnalog();

                ImGui.EndChildFrame();
            }

            ImGui.SameLine();

            if (ImGui.BeginChildFrame(12, GroupSize, WindowFlags.AlwaysAutoResize))
            {
                DrawControllerRightAnalog();

                ImGui.EndChildFrame();
            }

            DrawMainLayout();
        }

        static void DrawMainLayout()
        {
            if (ImGui.BeginChildFrame(13, GroupSize, WindowFlags.AlwaysAutoResize))
            {
                DrawDpad();

                ImGui.EndChildFrame();
            }

            ImGui.SameLine();

            if (ImGui.BeginChildFrame(14, GroupSize, WindowFlags.AlwaysAutoResize))
            {
                DrawActionKeys();

                ImGui.EndChildFrame();
            }

            if (ImGui.BeginChildFrame(15, GroupSize, WindowFlags.AlwaysAutoResize))
            {
                DrawTriggers();

                ImGui.EndChildFrame();
            }

            ImGui.SameLine();

            if (ImGui.BeginChildFrame(16, GroupSize, WindowFlags.AlwaysAutoResize))
            {
                DrawExtras();

                ImGui.EndChildFrame();
            }
        }

        static void RefreshDevices()
        {
            ConnectedHIDs = new Dictionary<int, InputDevice>();

            int GamePadIndex = 0;

            InputDevice KeyboardInputDevice = new InputDevice()
                {
                    Index      = short.MaxValue,
                    DeviceType = DeviceType.Keyboard,
                    Name       = "Keyboard"
                };

            ConnectedHIDs.Add(short.MaxValue, KeyboardInputDevice);
            
            // Scans for connected joysticks
            while (true)
            {
                JoystickState GamePad = Joystick.GetState(GamePadIndex);

                if (GamePad.IsConnected)
                {
                    InputDevice GamePadDevice = new InputDevice()
                    {
                        Index      = GamePadIndex,
                        DeviceType = DeviceType.GamePad,
                        Name       = "GamePad " + GamePadIndex
                    };

                    ConnectedHIDs.Add(GamePadIndex, GamePadDevice);
                }
                else
                    break;

                GamePadIndex++;
            }
        }
    }
}