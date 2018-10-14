using OpenTK.Graphics;
using OpenTK.Input;
using OpenTK;
using Qml.Net;
using Ryujinx.UI.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

using static OpenTK.Input.Keyboard;

namespace Ryujinx.UI.UI.Models
{
    [Signal("waitReleased")]
    [Signal("showWaitDialog")]
    [Signal("showError", NetVariantType.String, NetVariantType.String)]
    public class ConfigurationModel
    {
        private static Settings   CurrentSettings;

        private static GamePadState[] GamePadStates;

        private bool IsWaiting;

        public ConfigurationModel()
        {
            if(CurrentSettings == null)
            {
                string Path = "./Ryujinx.json";

                CurrentSettings = new Settings(Path);
            }
        }

        public void Save()
        {
            CurrentSettings.Apply(Emulation.EmulationController.Device);

            CurrentSettings.Commit();
        }

        public void Discard()
        {
            CurrentSettings = null;
        }

        public void ReleaseWait()
        {
            IsWaiting = false;

            this.ActivateSignal("waitReleased");
        }

        public void RefreshInputDevices()
        {
            int GamePadIndex = 0;

            List<GamePadState> GamePads = new List<GamePadState>();

            while (true)
            {
                GamePadState State = GamePad.GetState(GamePadIndex);

                if (State.IsConnected)
                {
                    GamePads.Add(State);
                }
                else
                {
                    break;
                }

                GamePadIndex++;
            }

            GamePadStates = GamePads.ToArray();
        }

        public void SetValue(string Key,object Value)
        {
            CurrentSettings.SetValue(Key, Value);
        }

        public object GetValue(string Key)
        {
            return CurrentSettings.GetValue<object>(Key);
        }

        public string GetKeyboardKey(string Key)
        {
            Key KeyCode = (Key)GetValue(Key);

            return Enum.GetName(typeof(Key), KeyCode);
        }

        public async Task<string> GetKeyboardInput(string SettingKey)
        {
            if (IsWaiting)
            {
                return string.Empty;
            }

            this.ActivateSignal("showWaitDialog");

            IsWaiting = true;

            Key Key = default(Key);

            bool GotKey = false;
            
            while (IsWaiting)
            {
                await Task.Delay(17);

                KeyboardState Keyboard = GetState();

                if (Keyboard.IsAnyKeyDown)
                {
                    foreach (Key KeyCode in Enum.GetValues(typeof(Key)))
                    {
                        if (Keyboard.IsKeyDown(KeyCode))
                        {
                            Key = KeyCode;

                            GotKey = true;

                            break;
                        }
                    }
                }

                if (GotKey)
                {
                    break;
                }
            }

            if (!GotKey)
            {
                return string.Empty;
            }

            CurrentSettings.SetValue(SettingKey, (short)Key);

            ReleaseWait();

            return Enum.GetName(typeof(Key), Key);
        }

        public async Task<string> GetGamePadInput(string SettingKey, int GamePadIndex)
        {
            double TriggerThreshold = CurrentSettings.GetValue<double>("GamePad_Trigger_Threshold");

            RefreshInputDevices();

            if (GamePadIndex >= GamePadStates.Length)
            {
                this.ActivateSignal("showError", "Failed to find GamePad", $"GamePad at Index {GamePadIndex} is not available");

                return string.Empty;
            }

            if (IsWaiting)
            {
                return string.Empty;
            }

            this.ActivateSignal("showWaitDialog");

            IsWaiting = true;

            try
            {
                while (IsWaiting)
                {
                    await Task.Delay(17);

                    RefreshInputDevices();

                    if (GamePadIndex >= GamePadStates.Length)
                    {
                        this.ActivateSignal("showError", "Failed to find GamePad", $"GamePad at Index {GamePadIndex} is not available");

                        return string.Empty;
                    }

                    GamePadState SelectedGamePad = GamePadStates[GamePadIndex];

                    if (SettingKey == "Controls_Left_JoyConController_Stick" || SettingKey == "Controls_Right_JoyConController_Stick")
                    {
                        // Check if Sticks have been moved since last update
                        if (SelectedGamePad.ThumbSticks.Left.Length > 0.1)
                        {
                            CurrentSettings.SetValue(SettingKey, "LJoystick");

                            return "LJoystick";
                        }
                        else if (SelectedGamePad.ThumbSticks.Right.Length  > 0.1)
                        {
                            CurrentSettings.SetValue(SettingKey, "RJoystick");

                            return "RJoystick";
                        }
                    }
                    else if (SelectedGamePad.Buttons.IsAnyButtonPressed)
                    {
                        if (SelectedGamePad.Buttons.A == ButtonState.Pressed)
                        {
                            CurrentSettings.SetValue(SettingKey, "A");

                            return "A";
                        }
                        if (SelectedGamePad.Buttons.B == ButtonState.Pressed)
                        {
                            CurrentSettings.SetValue(SettingKey, "B");

                            return "B";
                        }
                        if (SelectedGamePad.Buttons.X == ButtonState.Pressed)
                        {
                            CurrentSettings.SetValue(SettingKey, "X");

                            return "X";
                        }
                        if (SelectedGamePad.Buttons.Y == ButtonState.Pressed)
                        {
                            CurrentSettings.SetValue(SettingKey, "Y");

                            return "Y";
                        }
                        if (SelectedGamePad.Buttons.LeftShoulder == ButtonState.Pressed)
                        {
                            CurrentSettings.SetValue(SettingKey, "LShoulder");

                            return "LShoulder";
                        }
                        if (SelectedGamePad.Buttons.RightShoulder == ButtonState.Pressed)
                        {
                            CurrentSettings.SetValue(SettingKey, "RShoulder");

                            return "RShoulder";
                        }
                        if (SelectedGamePad.Buttons.LeftStick == ButtonState.Pressed)
                        {
                            CurrentSettings.SetValue(SettingKey, "LStick");

                            return "LStick";
                        }
                        if (SelectedGamePad.Buttons.RightStick == ButtonState.Pressed)
                        {
                            CurrentSettings.SetValue(SettingKey, "RStick");

                            return "RStick";
                        }
                        if (SelectedGamePad.Buttons.Start == ButtonState.Pressed)
                        {
                            CurrentSettings.SetValue(SettingKey, "Start");

                            return "Start";
                        }
                        if (SelectedGamePad.Buttons.Back == ButtonState.Pressed)
                        {
                            CurrentSettings.SetValue(SettingKey, "Back");

                            return "Back";
                        }
                        else if (SelectedGamePad.DPad.IsUp)
                        {
                            CurrentSettings.SetValue(SettingKey, "DPadUp");

                            return "A";
                        }
                        else if (SelectedGamePad.DPad.IsDown)
                        {
                            CurrentSettings.SetValue(SettingKey, "DPadDown");

                            return "DPadDown";
                        }
                        else if (SelectedGamePad.DPad.IsLeft)
                        {
                            CurrentSettings.SetValue(SettingKey, "DPadLeft");

                            return "DPadLeft";
                        }
                        else if (SelectedGamePad.DPad.IsRight)
                        {
                            CurrentSettings.SetValue(SettingKey, "DPadRight");

                            return "DPadRight";
                        }
                        else if (SelectedGamePad.Triggers.Left > TriggerThreshold)
                        {
                            CurrentSettings.SetValue(SettingKey, "LTrigger");

                            return "LTrigger";
                        }
                        else if (SelectedGamePad.Triggers.Right > TriggerThreshold)
                        {
                            CurrentSettings.SetValue(SettingKey, "RTrigger");

                            return "RTrigger";
                        }
                    }
                }
            }
            finally
            {
                ReleaseWait();
            }

            return string.Empty;
        }
    }
}
