﻿using ImGuiNET;
using System;

namespace Ryujinx.UI
{
    partial class EmulationWindow
    {
        void RenderMainUI()
        {
            ImGui.SetNextWindowPos(System.Numerics.Vector2.Zero, Condition.Always,
                    System.Numerics.Vector2.Zero);
            ImGui.SetNextWindowSize(new System.Numerics.Vector2(Width, Height), Condition.Always);
            if (ImGui.BeginWindow("MainWindow", ref showMainUI, WindowFlags.NoTitleBar
                | WindowFlags.NoMove | WindowFlags.AlwaysAutoResize))
            {
                if (ImGui.BeginChildFrame(0, new System.Numerics.Vector2(-1, -1),
                    WindowFlags.AlwaysAutoResize))
                {
                    ImGuiNative.igBeginGroup();

                    if (ImGui.Button("Load Package", new System.Numerics.Vector2(Values.ButtonWidth,
                        Values.ButtonHeight)))
                    {
                        CurrentPage = Page.PackageLoader;
                    }

                    if (ImGui.Button("Game List", new System.Numerics.Vector2(Values.ButtonWidth,
                        Values.ButtonHeight)))
                    {
                        CurrentPage = Page.GameList;
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
                            case Page.PackageLoader:
                                string output = CurrentPath;
                                if (FileDialog.Draw(ref output, false))
                                {
                                    if (!string.IsNullOrWhiteSpace(output))
                                    {
                                        PackagePath = output;
                                        LoadPackage(PackagePath);
                                    }
                                }
                                break;
                            case Page.Configuration:
                                Widgets.ConfigurationWidget.Draw();
                                break;
                            case Page.GameList:
                                Widgets.GameList.DrawList();
                                break;
                        }
                        ImGui.EndChildFrame();
                    }
                    ImGui.EndChildFrame();
                }
                ImGui.EndWindow();
            }
        }

        void DrawQuitButton()
        {
            if (ImGui.Button("Quit Ryujinx", new System.Numerics.Vector2(Values.ButtonWidth,
                        Values.ButtonHeight)))
            {
                ImGui.OpenPopup("Quit");
            }

            if (ImGui.BeginPopupModal("Quit"))
            {
                ImGui.Text("Do you want to quit Ryujinx and return to desktop?");

                if (ImGui.Button("Yes"))
                {
                    Environment.Exit(0);
                }

                if (ImGui.Button("No"))
                {
                    ImGui.CloseCurrentPopup();
                }

                ImGui.EndPopup();
            }
        }
    }
}