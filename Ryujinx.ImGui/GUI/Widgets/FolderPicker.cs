using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace ImGuiNET
{
    /// <summary>
    /// Adapted from Mellinoe's file picker for imgui
    /// https://github.com/mellinoe/synthapp/blob/master/src/synthapp/Widgets/FilePicker.cs
    /// </summary>
    public class FolderPicker
    {
        private const string FolderPickerID = "###FolderPicker";
        private static readonly Dictionary<object, FolderPicker> FolderPickers 
            = new Dictionary<object, FolderPicker>();
        private static readonly Vector2 DefaultFilePickerSize = new Vector2(600, 400);

        public string CurrentFolder { get; set; }
        public string SelectedDirectory { get; set; }

        public static FolderPicker GetFolderPicker(object Id, string StartingPath)
        {
            if (File.Exists(StartingPath))
            {
                StartingPath = new FileInfo(StartingPath).DirectoryName;
            }
            else if (string.IsNullOrEmpty(StartingPath) || !Directory.Exists(StartingPath))
            {
                StartingPath = Environment.CurrentDirectory;
                if (string.IsNullOrEmpty(StartingPath))
                {
                    StartingPath = AppContext.BaseDirectory;
                }
            }

            if (!FolderPickers.TryGetValue(Id, out FolderPicker FolderPicker))
            {
                FolderPicker = new FolderPicker
                {
                    CurrentFolder = StartingPath
                };

                FolderPickers.Add(Id, FolderPicker);
            }

            return FolderPicker;
        }

        public DialogResult Draw(ref string Selected, bool ReturnOnSelection)
        {
            return DrawFolder(ref Selected, ReturnOnSelection);
        }

        private DialogResult DrawFolder(ref string Selected, bool ReturnOnSelection = false)
        {
            ImGui.Text("Current Folder: " + CurrentFolder);

            if (ImGui.BeginChildFrame(1, ImGui.GetContentRegionAvailable() - new Vector2(20, Values.ButtonHeight),
                WindowFlags.Default))
            {
                DirectoryInfo CurrentDirectory = new DirectoryInfo(CurrentFolder);
                if (CurrentDirectory.Exists)
                {
                    if (CurrentDirectory.Parent != null)
                    {
                        ImGui.PushStyleColor(ColorTarget.Text, Values.Color.Yellow);

                        if (ImGui.Selectable("../", false, SelectableFlags.DontClosePopups
                            , new Vector2(ImGui.GetContentRegionAvailableWidth(), Values.SelectibleHeight)))
                        {
                            CurrentFolder = CurrentDirectory.Parent.FullName;
                        }

                        ImGui.PopStyleColor();
                    }
                    foreach (var Dir in Directory.EnumerateFileSystemEntries(CurrentDirectory.FullName))
                    {
                        if (Directory.Exists(Dir))
                        {
                            string Name       = Path.GetFileName(Dir);
                            bool   IsSelected = SelectedDirectory == Dir;

                            ImGui.PushStyleColor(ColorTarget.Text, Values.Color.Yellow);

                            if (ImGui.Selectable(Name + "/", IsSelected, SelectableFlags.DontClosePopups
                               , new Vector2(ImGui.GetContentRegionAvailableWidth(), Values.SelectibleHeight)))
                            {
                                SelectedDirectory = Dir;
                                Selected = SelectedDirectory;
                            }

                            if (SelectedDirectory != null)
                                if (ImGui.IsMouseDoubleClicked(0) && SelectedDirectory.Equals(Dir))
                                {
                                    SelectedDirectory = null;
                                    Selected = null;
                                    CurrentFolder = Dir;
                                }

                            ImGui.PopStyleColor();
                        }
                    }
                }
            }
            ImGui.EndChildFrame();


            if (ImGui.Button("Cancel", new Vector2(Values.ButtonWidth, Values.ButtonHeight)))
            {
                return DialogResult.Cancel;
            }

            if (SelectedDirectory != null)
            {
                ImGui.SameLine();
                if (ImGui.Button("Open", new Vector2(Values.ButtonWidth, Values.ButtonHeight)))
                {
                    Selected = SelectedDirectory;

                    return DialogResult.OK;
                }
                else if(ReturnOnSelection)
                {
                    Selected = SelectedDirectory;

                    return DialogResult.OK;
                }
            }

            return DialogResult.None;
        }

        public DialogResult GetFolder(ref string CurrentPath)
        {
            ImGui.SetNextWindowSize(new Vector2(500, 500), Condition.FirstUseEver);
            if (ImGui.BeginPopupModal("OpenFolder", WindowFlags.NoResize))
            {
                try
                {
                    string Output = CurrentPath;
                    DialogResult DialogResult = Draw(ref Output, false);

                    if (DialogResult == DialogResult.OK)
                    {
                        if (string.IsNullOrWhiteSpace(Output))
                        {
                            return DialogResult.None;
                        }                        
                    }

                    if(DialogResult!= DialogResult.None)
                        ImGui.CloseCurrentPopup();

                    return DialogResult;
                }
                finally
                {
                    ImGui.EndPopup();
                }
            }

            return DialogResult.None;
        }
    }
}
