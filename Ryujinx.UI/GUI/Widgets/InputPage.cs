using ImGuiNET;
using OpenTK.Input;
using System;
using System.Linq;
using System.Numerics;


namespace Ryujinx.UI.Widgets
{
    public partial class ConfigurationWidget
    {
        static bool[]  Toggles = new bool[50];

        static float   ContentWidth;
        static Vector2 GroupSize;
        static Key     pressedKey;
        static bool    RequestPopup;

        public static void DrawInputPage()
        {
            Vector2 AvailableSpace = ImGui.GetContentRegionAvailable();

            GroupSize = new Vector2(AvailableSpace.X / 2, AvailableSpace.Y / 3);

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

            if (Toggles.Contains(true))
            {
                ImGui.OpenPopup("Enter Key");
                RequestPopup = true;
            }
            else
                RequestPopup = false;

            if (ImGui.BeginPopupModal("Enter Key", WindowFlags.AlwaysAutoResize| WindowFlags.NoMove
                | WindowFlags.NoResize))
            {
                ImGui.Text("Please enter a key");

                if (!RequestPopup)
                    ImGui.CloseCurrentPopup();

                ImGui.EndPopup();
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
                if (GetKey(ref pressedKey))
                {
                    KeyboardInputLayout.Left.StickUp = (int)pressedKey;
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
                if (GetKey(ref pressedKey))
                {
                    KeyboardInputLayout.Left.StickDown = (int)pressedKey;
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
                if (GetKey(ref pressedKey))
                {
                    KeyboardInputLayout.Left.StickLeft = (int)pressedKey;
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
                if (GetKey(ref pressedKey))
                {
                    KeyboardInputLayout.Left.StickRight = (int)pressedKey;
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
                if (GetKey(ref pressedKey))
                {
                    KeyboardInputLayout.Right.StickUp = (int)pressedKey;

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
                if (GetKey(ref pressedKey))
                {
                    KeyboardInputLayout.Right.StickDown = (int)pressedKey;
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
                if (GetKey(ref pressedKey))
                {
                    KeyboardInputLayout.Right.StickLeft = (int)pressedKey;
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
                if (GetKey(ref pressedKey))
                {
                    KeyboardInputLayout.Right.StickRight = (int)pressedKey;
                    Toggles[7] = false;
                }
            }
            ImGuiNative.igEndGroup();

            ImGuiNative.igEndGroup();
        }

        static void DrawDpad()
        {
            //Show DPad Bindings
            ImGui.Text("D-Pad");
            ImGuiNative.igBeginGroup();

            ImGuiNative.igBeginGroup();
            ImGui.Text("Up");

            if (ImGui.Button(((Key)KeyboardInputLayout.Left.DPadUp).ToString(),
                new Vector2(ContentWidth, 50)))
            {
                Toggles[8] = true;
            }

            if (Toggles[8])
            {
                if (GetKey(ref pressedKey))
                {
                    KeyboardInputLayout.Left.DPadUp = (int)pressedKey;
                    Toggles[8] = false;
                }
            }
            ImGuiNative.igEndGroup();

            ImGui.SameLine();
            ImGuiNative.igBeginGroup();
            ImGui.Text("Down");

            if (ImGui.Button(((Key)KeyboardInputLayout.Left.DPadDown).ToString(),
                new Vector2(ContentWidth, 50)))
            {
                Toggles[9] = true;
            }

            if (Toggles[9])
            {
                if (GetKey(ref pressedKey))
                {
                    KeyboardInputLayout.Left.DPadDown = (int)pressedKey;
                    Toggles[9] = false;
                }
            }
            ImGuiNative.igEndGroup();

            ImGuiNative.igBeginGroup();
            ImGui.Text("Left");

            if (ImGui.Button(((Key)KeyboardInputLayout.Left.DPadLeft).ToString(),
                new Vector2(ContentWidth, 50)))
            {
                Toggles[10] = true;
            }

            if (Toggles[10])
            {
                if (GetKey(ref pressedKey))
                {
                    KeyboardInputLayout.Left.DPadLeft = (int)pressedKey;
                    Toggles[10] = false;
                }
            }
            ImGuiNative.igEndGroup();

            ImGui.SameLine();
            ImGuiNative.igBeginGroup();
            ImGui.Text("Right");

            if (ImGui.Button(((Key)KeyboardInputLayout.Left.DPadRight).ToString(),
                new Vector2(ContentWidth, 50)))
            {
                Toggles[11] = true;
            }

            if (Toggles[11])
            {
                if (GetKey(ref pressedKey))
                {
                    KeyboardInputLayout.Left.DPadRight = (int)pressedKey;
                    Toggles[11] = false;
                }
            }
            ImGuiNative.igEndGroup();

            ImGuiNative.igEndGroup();
        }

        static void DrawActionKeys()
        {
            //Show Action Key Bindings
            ImGui.Text("Action Keys");
            ImGuiNative.igBeginGroup();

            ImGuiNative.igBeginGroup();
            ImGui.Text("A");

            if (ImGui.Button(((Key)KeyboardInputLayout.Right.ButtonA).ToString(),
                new Vector2(ContentWidth, 50)))
            {
                Toggles[12] = true;
            }

            if (Toggles[12])
            {
                if (GetKey(ref pressedKey))
                {
                    KeyboardInputLayout.Right.ButtonA = (int)pressedKey;
                    Toggles[12] = false;
                }
            }
            ImGuiNative.igEndGroup();

            ImGui.SameLine();
            ImGuiNative.igBeginGroup();
            ImGui.Text("B");

            if (ImGui.Button(((Key)KeyboardInputLayout.Right.ButtonB).ToString(),
                new Vector2(ContentWidth, 50)))
            {
                Toggles[13] = true;
            }

            if (Toggles[13])
            {
                if (GetKey(ref pressedKey))
                {
                    KeyboardInputLayout.Right.ButtonB = (int)pressedKey;
                    Toggles[13] = false;
                }
            }
            ImGuiNative.igEndGroup();

            ImGuiNative.igBeginGroup();
            ImGui.Text("X");

            if (ImGui.Button(((Key)KeyboardInputLayout.Right.ButtonX).ToString(),
                new Vector2(ContentWidth, 50)))
            {
                Toggles[14] = true;
            }

            if (Toggles[14])
            {
                if (GetKey(ref pressedKey))
                {
                    KeyboardInputLayout.Right.ButtonX = (int)pressedKey;
                    Toggles[14] = false;
                }
            }
            ImGuiNative.igEndGroup();

            ImGui.SameLine();
            ImGuiNative.igBeginGroup();
            ImGui.Text("Y");

            if (ImGui.Button(((Key)KeyboardInputLayout.Right.ButtonY).ToString(),
                new Vector2(ContentWidth, 50)))
            {
                Toggles[15] = true;
            }

            if (Toggles[15])
            {
                if (GetKey(ref pressedKey))
                {
                    KeyboardInputLayout.Right.ButtonY = (int)pressedKey;
                    Toggles[15] = false;
                }
            }

            ImGuiNative.igEndGroup();

            ImGuiNative.igEndGroup();
        }

        static void DrawTriggers()
        {
            //Draw Triggers
            ImGuiNative.igBeginGroup();

            ImGui.Text("Triggers");

            ImGuiNative.igBeginGroup();
            ImGui.Text("L");

            if (ImGui.Button(((Key)KeyboardInputLayout.Left.ButtonL).ToString(),
                new Vector2(ContentWidth, 50)))
            {
                Toggles[17] = true;
            }

            if (Toggles[17])
            {
                if (GetKey(ref pressedKey))
                {
                    KeyboardInputLayout.Left.ButtonL = (int)pressedKey;
                    Toggles[17] = false;
                }
            }

            ImGuiNative.igEndGroup();

            ImGui.SameLine();
            ImGuiNative.igBeginGroup();
            ImGui.Text("R");

            if (ImGui.Button(((Key)KeyboardInputLayout.Right.ButtonR).ToString(),
                new Vector2(ContentWidth, 50)))
            {
                Toggles[16] = true;
            }

            if (Toggles[16])
            {
                if (GetKey(ref pressedKey))
                {
                    KeyboardInputLayout.Right.ButtonR = (int)pressedKey;
                    Toggles[16] = false;
                }
            }
            ImGuiNative.igEndGroup();            

            ImGuiNative.igBeginGroup();
            ImGui.Text("ZL");

            if (ImGui.Button(((Key)KeyboardInputLayout.Left.ButtonZL).ToString(),
                new Vector2(ContentWidth, 50)))
            {
                Toggles[19] = true;
            }

            if (Toggles[19])
            {
                if (GetKey(ref pressedKey))
                {
                    KeyboardInputLayout.Left.ButtonZL = (int)pressedKey;
                    Toggles[19] = false;
                }
            }
            ImGuiNative.igEndGroup();

            ImGui.SameLine();
            ImGuiNative.igBeginGroup();
            ImGui.Text("ZR");

            if (ImGui.Button(((Key)KeyboardInputLayout.Right.ButtonZR).ToString(),
                new Vector2(ContentWidth, 50)))
            {
                Toggles[18] = true;
            }

            if (Toggles[18])
            {
                if (GetKey(ref pressedKey))
                {
                    KeyboardInputLayout.Right.ButtonZR = (int)pressedKey;
                    Toggles[18] = false;
                }
            }
            ImGuiNative.igEndGroup();

            ImGuiNative.igEndGroup();
        }

        static void DrawExtras()
        {
            //Draw Extra
            ImGuiNative.igBeginGroup();
            ImGui.Text("Extra Keys");

            ImGuiNative.igBeginGroup();
            ImGui.Text("-");

            if (ImGui.Button(((Key)KeyboardInputLayout.Left.ButtonMinus).ToString(),
                new Vector2(ContentWidth, 50)))
            {
                Toggles[20] = true;
            }

            if (Toggles[20])
            {
                if (GetKey(ref pressedKey))
                {
                    KeyboardInputLayout.Left.ButtonMinus = (int)pressedKey;
                    Toggles[20] = false;
                }
            }
            ImGuiNative.igEndGroup();

            ImGuiNative.igBeginGroup();
            ImGui.Text("+");

            if (ImGui.Button(((Key)KeyboardInputLayout.Right.ButtonPlus).ToString(),
                new Vector2(ContentWidth, 50)))
            {
                Toggles[21] = true;
            }

            if (Toggles[21])
            {
                if (GetKey(ref pressedKey))
                {
                    KeyboardInputLayout.Right.ButtonPlus = (int)pressedKey;
                    Toggles[21] = false;
                }
            }
            ImGuiNative.igEndGroup();

            ImGuiNative.igEndGroup();
        }

        static bool GetKey(ref Key pressedKey)
        {
            IO IO = ImGui.GetIO();
            foreach (Key Key in Enum.GetValues(typeof(Key)))
            {
                if (IO.KeysDown[(int)Key])
                {
                    pressedKey = Key;
                    return true;
                }
            }
            return false;
        }
    }
}