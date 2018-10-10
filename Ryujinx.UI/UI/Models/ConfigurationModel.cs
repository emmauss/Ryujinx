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
                await Task.Delay(16);

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
            float TriggerThreshold = CurrentSettings.GetValue<float>("GamePad_Trigger_Threshold");

            while (true)
            {
                await Task.Delay(33);

                RefreshInputDevices();

                if (GamePadIndex >= GamePadStates.Length)
                {
                    // TODO :throw error here 

                    return string.Empty;
                }

                if(GamePadStates[GamePadIndex].Buttons.IsAnyButtonPressed)
                {
                    if (GamePadStates[GamePadIndex].Buttons.A == ButtonState.Pressed)
                    {
                        CurrentSettings.SetValue(SettingKey, "A");

                        return "A";
                    }
                    if (GamePadStates[GamePadIndex].Buttons.B == ButtonState.Pressed)
                    {
                        CurrentSettings.SetValue(SettingKey, "B");

                        return "B";
                    }
                    if (GamePadStates[GamePadIndex].Buttons.X == ButtonState.Pressed)
                    {
                        CurrentSettings.SetValue(SettingKey, "X");

                        return "X";
                    }
                    if (GamePadStates[GamePadIndex].Buttons.Y == ButtonState.Pressed)
                    {
                        CurrentSettings.SetValue(SettingKey, "Y");

                        return "Y";
                    }
                    if (GamePadStates[GamePadIndex].Buttons.LeftShoulder == ButtonState.Pressed)
                    {
                        CurrentSettings.SetValue(SettingKey, "LShoulder");

                        return "LShoulder";
                    }
                    if (GamePadStates[GamePadIndex].Buttons.RightShoulder == ButtonState.Pressed)
                    {
                        CurrentSettings.SetValue(SettingKey, "RShoulder");

                        return "RShoulder";
                    }
                    if (GamePadStates[GamePadIndex].Buttons.LeftStick == ButtonState.Pressed)
                    {
                        CurrentSettings.SetValue(SettingKey, "LStick");

                        return "LStick";
                    }
                    if (GamePadStates[GamePadIndex].Buttons.RightStick == ButtonState.Pressed)
                    {
                        CurrentSettings.SetValue(SettingKey, "RStick");

                        return "RStick";
                    }
                    if (GamePadStates[GamePadIndex].Buttons.Start == ButtonState.Pressed)
                    {
                        CurrentSettings.SetValue(SettingKey, "Start");

                        return "Start";
                    }
                    if (GamePadStates[GamePadIndex].Buttons.Back == ButtonState.Pressed)
                    {
                        CurrentSettings.SetValue(SettingKey, "Back");

                        return "Back";
                    }
                }
                else if (GamePadStates[GamePadIndex].DPad.IsUp)
                {
                    CurrentSettings.SetValue(SettingKey, "DPadUp");

                    return "A";
                }
                else if (GamePadStates[GamePadIndex].DPad.IsDown)
                {
                    CurrentSettings.SetValue(SettingKey, "DPadDown");

                    return "DPadDown";
                }
                else if (GamePadStates[GamePadIndex].DPad.IsLeft)
                {
                    CurrentSettings.SetValue(SettingKey, "DPadLeft");

                    return "DPadLeft";
                }
                else if (GamePadStates[GamePadIndex].DPad.IsRight)
                {
                    CurrentSettings.SetValue(SettingKey, "DPadRight");

                    return "DPadRight";
                }
            }

        }
    }
}
