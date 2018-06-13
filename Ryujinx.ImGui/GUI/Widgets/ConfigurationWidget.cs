using System;
using System.Collections.Generic;
using System.Text;
using ImGuiNET;
using System.Numerics;
using Ryujinx.HLE.Input;
using OpenTK.Input;

namespace Ryujinx.UI.Widgets
{
    public partial class ConfigurationWidget
    {
        public static JoyCon CurrentJoyConLayout;
        static Page CurrentPage = Page.General;   
        static bool ConfigIntialized = false;
        static IniParser IniParser;

        static ConfigurationWidget()
        {
            IniParser = new IniParser(Config.IniPath);
        }

        public static void Draw()
        {
            if(!ConfigIntialized)
            {
                CurrentJoyConLayout = Config.FakeJoyCon;
                ConfigIntialized = true;
            }

            if (ImGui.BeginChildFrame(2, ImGui.GetContentRegionAvailable()  
                - new Vector2(0,Values.ButtonHeight), WindowFlags.AlwaysAutoResize))
            {
                ImGuiNative.igBeginGroup();

                if(ImGui.Button("General",new Vector2(Values.ButtonWidth,Values.ButtonHeight)))
                {
                    CurrentPage = Page.General;
                }

                ImGui.SameLine();
                if (ImGui.Button("Input", new Vector2(Values.ButtonWidth, Values.ButtonHeight)))
                {
                    CurrentPage = Page.Input;
                }

                ImGuiNative.igEndGroup();

                if (ImGui.BeginChildFrame(3, ImGui.GetContentRegionAvailable(), WindowFlags.AlwaysAutoResize))
                {
                    switch (CurrentPage)
                    {
                        case Page.General:
                            if (ImGui.BeginChild("generalFrame", true, WindowFlags.AlwaysAutoResize))
                            {
                                ImGui.Text("General Emulation Settings");
                                ImGui.Spacing();
                                ImGui.Checkbox("Disable Cpu Memory Checks", ref AOptimizations.DisableMemoryChecks);
                                ImGui.EndChild();
                            }
                            break;
                        case Page.Input:
                            if (ImGui.BeginChild("inputFrame", true, WindowFlags.AlwaysAutoResize))
                            {
                                DrawInputPage();
                                ImGui.EndChild();
                            }
                            break;
                    }

                    ImGui.EndChildFrame();
                }

                ImGui.EndChildFrame();

                if (CurrentPage == Page.Input)
                {
                    if (ImGui.Button("Apply", new Vector2(Values.ButtonWidth, Values.ButtonHeight)))
                    {
                        Config.FakeJoyCon = CurrentJoyConLayout;
                    }
                    ImGui.SameLine();
                }
                if (ImGui.Button("Save", new Vector2(Values.ButtonWidth, Values.ButtonHeight)))
                {
                    IniParser.Save();
                }
                ImGui.SameLine();
                if (ImGui.Button("Discard", new Vector2(Values.ButtonWidth, Values.ButtonHeight)))
                {
                    IniParser = new IniParser(Config.IniPath);
                }
            }  
        }      

        enum Page
        {
            General,
            Input
        }
    }
}
