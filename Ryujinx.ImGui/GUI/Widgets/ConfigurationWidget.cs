using ImGuiNET;
using Ryujinx.HLE.Input;
using System.Numerics;

namespace Ryujinx.UI.Widgets
{
    public partial class ConfigurationWidget
    { 
        static bool   ConfigIntialized = false;
        static bool   OpenFolderPicker;
        static string CurrentPath;

        static IniParser  IniParser;
        static FilePicker FolderPicker;
        static JoyCon     CurrentJoyConLayout;
        static Page       CurrentPage = Page.General;

        static ConfigurationWidget()
        {
            IniParser    = new IniParser(Config.IniPath);
            FolderPicker = FilePicker.GetFilePicker("FolderDialog",Config.DefaultGameDirectory);
            CurrentPath  = Config.DefaultGameDirectory.ToString();
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
                                ImGui.LabelText("", "Default Game Directory");
                                ImGui.SameLine();

                                if (ImGui.Selectable(Config.DefaultGameDirectory))
                                {
                                    OpenFolderPicker = true;
                                }
                                if (OpenFolderPicker)
                                    ImGui.OpenPopup("Open Folder");

                                DialogResult DialogResult = FolderPicker.GetFolder(ref CurrentPath);
                                if (DialogResult == DialogResult.OK)
                                {
                                    if (!string.IsNullOrWhiteSpace(CurrentPath))
                                    {
                                        Config.DefaultGameDirectory = CurrentPath;
                                    }
                                }
                                if (DialogResult != DialogResult.None)
                                {
                                    OpenFolderPicker = false;
                                }

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
                    Config.Save(EmulationWindow.Ns.Log);
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
