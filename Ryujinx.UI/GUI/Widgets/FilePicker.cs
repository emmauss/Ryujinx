using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace ImGuiNET
{
    // Adapted from Mellinoe's file picker for imgui
    // https://github.com/mellinoe/synthapp/blob/master/src/synthapp/Widgets/FilePicker.cs
    public class FilePicker
    {
        private const  string   FilePickerID = "###FilePicker";
        private static readonly Dictionary<object, FilePicker> FilePickers = new Dictionary<object, FilePicker>();
        private static readonly Vector2 DefaultFilePickerSize = new Vector2(600, 400);

        public string CurrentFolder { get; set; }
        public string SelectedEntry { get; set; }
        public string CurrentDrive  { get; set; }

        public static FilePicker GetFilePicker(object Id, string StartingPath)
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

            if (!FilePickers.TryGetValue(Id, out FilePicker FilePicker))
            {
                FilePicker = new FilePicker
                {
                    CurrentFolder = StartingPath
                };

                FilePickers.Add(Id, FilePicker);
            }

            return FilePicker;
        }

        public DialogResult Draw(ref string SelectedPath, bool ReturnOnSelection, bool FoldersOnly = false)
        {
            return DrawFolder(ref SelectedPath, ReturnOnSelection, FoldersOnly);
        }

        private DialogResult DrawFolder(ref string SelectedPath, bool ReturnOnSelection = false, bool FoldersOnly = false)
        {
            ImGui.Text("Current Folder: " + CurrentFolder);

            if(ImGui.BeginChildFrame(0,new Vector2(ImGui.GetContentRegionAvailableWidth()/3,
                ImGui.GetContentRegionAvailable().Y - Values.ButtonHeight - 10), WindowFlags.Default))
            {
                DriveInfo[] DriveList = DriveInfo.GetDrives();
                
                foreach(DriveInfo Drive in DriveList)
                {
                    bool IsSelected = CurrentDrive == Drive.Name;

                    ImGui.PushStyleColor(ColorTarget.Text, Values.Color.Yellow);

                    if (ImGui.Selectable(Drive.Name + "/", IsSelected, SelectableFlags.DontClosePopups
                       , new Vector2(ImGui.GetContentRegionAvailableWidth(), Values.SelectibleHeight)))
                    {
                        CurrentDrive = Drive.Name;
                        CurrentFolder = Drive.Name;
                    }
                    ImGui.PopStyleColor();
                }

                ImGui.EndChildFrame();
            }

            ImGui.SameLine();
            if (ImGui.BeginChildFrame(1, ImGui.GetContentRegionAvailable() - new Vector2(20, Values.ButtonHeight + 10),
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

                    foreach (string Dir in Directory.EnumerateFileSystemEntries(CurrentDirectory.FullName))
                    {
                        if (Directory.Exists(Dir))
                        {
                            string Name       = Path.GetFileName(Dir);
                            bool   IsSelected = SelectedEntry == Dir;

                            ImGui.PushStyleColor(ColorTarget.Text, Values.Color.Yellow);

                            if (ImGui.Selectable(Name + "/", IsSelected, SelectableFlags.DontClosePopups
                               , new Vector2(ImGui.GetContentRegionAvailableWidth(), Values.SelectibleHeight)))
                            {
                                SelectedEntry = Dir;
                                SelectedPath  = SelectedEntry;
                            }

                            if (SelectedEntry != null)
                                if (ImGui.IsMouseDoubleClicked(0) && SelectedEntry.Equals(Dir))
                                {
                                    SelectedEntry = null;
                                    SelectedPath  = null;
                                    CurrentFolder = Dir;
                                }

                            ImGui.PopStyleColor();
                        }
                    }

                    if (!FoldersOnly)
                        foreach (string File in Directory.EnumerateFiles(CurrentDirectory.FullName))
                        {
                            string Name = Path.GetFileName(File);
                            bool IsSelected = SelectedEntry == File;

                            if (ImGui.Selectable(Name, IsSelected, SelectableFlags.DontClosePopups
                                , new Vector2(ImGui.GetContentRegionAvailableWidth(), Values.SelectibleHeight)))
                            {
                                SelectedEntry = File;
                                if (ReturnOnSelection)
                                {
                                    SelectedPath = SelectedEntry;
                                }
                            }

                            if (SelectedEntry != null)
                                if (ImGui.IsMouseDoubleClicked(0) && SelectedEntry.Equals(File))
                                {
                                    SelectedPath = File;
                                }
                        }
                }
            }
            ImGui.EndChildFrame();


            if (ImGui.Button("Cancel", new Vector2(Values.ButtonWidth, Values.ButtonHeight)))
            {
                return DialogResult.Cancel;
            }

            if (SelectedEntry != null)
            {
                ImGui.SameLine();
                if (ImGui.Button("Open", new Vector2(Values.ButtonWidth, Values.ButtonHeight)))
                {
                    SelectedPath = SelectedEntry;

                    return DialogResult.OK;
                }
                else if (ReturnOnSelection)
                {
                    SelectedPath = SelectedEntry;

                    return DialogResult.OK;
                }
            }

            return DialogResult.None;
        }

        public DialogResult GetFolder(ref string CurrentPath)
        {
            ImGui.SetNextWindowSize(new Vector2(600, 600), Condition.FirstUseEver);
            if (ImGui.BeginPopupModal("Open Folder", WindowFlags.NoResize))
            {
                try
                {
                    string Output = CurrentPath;
                    DialogResult DialogResult = Draw(ref Output, false, true);

                    if (DialogResult == DialogResult.OK)
                    {
                        if (string.IsNullOrWhiteSpace(Output))
                        {
                            return DialogResult.None;
                        }
                    }

                    if (DialogResult != DialogResult.None)
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
