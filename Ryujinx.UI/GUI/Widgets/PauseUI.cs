using ImGuiNET;
namespace Ryujinx.UI
{
    partial class EmulationWindow
    {
        void RenderPauseUI()
        {
            ImGui.SetNextWindowPos(System.Numerics.Vector2.Zero, Condition.Always,
                    System.Numerics.Vector2.Zero);

            ImGui.SetNextWindowSize(new System.Numerics.Vector2(Width, Height), Condition.Always);

            if (ImGui.BeginWindow("PauseWindow", ref showMainUI, WindowFlags.NoTitleBar
                | WindowFlags.NoMove | WindowFlags.AlwaysAutoResize))
            {
                if (ImGui.BeginChildFrame(0, new System.Numerics.Vector2(-1, -1),
                    WindowFlags.AlwaysAutoResize))
                {
                    ImGuiNative.igBeginGroup();

                    if (ImGui.Button("Emulation", new System.Numerics.Vector2(Values.ButtonWidth,
                        Values.ButtonHeight)))
                    {
                        CurrentPage = Page.Emulation;
                    }

                    if (ImGui.Button("Settings", new System.Numerics.Vector2(Values.ButtonWidth,
                        Values.ButtonHeight)))
                    {
                        CurrentPage = Page.Configuration;
                    }

                    DrawQuitButton();

                    ImGuiNative.igEndGroup();

                    ImGui.SameLine();

                    if (ImGui.BeginChildFrame(1, ImGui.GetContentRegionAvailable(),
                        WindowFlags.AlwaysAutoResize))
                    {
                        switch (CurrentPage)
                        {
                            case Page.Emulation:
                                if (ImGui.Button("Resume", new System.Numerics.Vector2(Values.ButtonWidth,
                                    Values.ButtonHeight)))
                                {
                                    ShowPauseUI = false;

                                    EmulationController.Resume();
                                }

                                if (ImGui.Button("Stop", new System.Numerics.Vector2(Values.ButtonWidth,
                                    Values.ButtonHeight)))
                                {
                                    ShowPauseUI = false;

                                    EmulationController.ShutDown();

                                    ShowMainUI = true;
                                }

                                break;
                            case Page.Configuration:
                                Widgets.ConfigurationWidget.Draw();

                                break;
                        }

                        ImGui.EndChildFrame();
                    }

                    ImGui.EndChildFrame();
                }

                ImGui.EndWindow();
            }
        }
    }
}