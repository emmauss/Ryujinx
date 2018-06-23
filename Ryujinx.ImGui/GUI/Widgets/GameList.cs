using ImGuiNET;
using NanoJpeg;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace Ryujinx.UI.Widgets
{
    class GameList
    {
        static bool           OpenFolderPicker;
        static string         GameDirectory;
        static List<GameItem> GameItems;
        static GameItem       SelectedGame;
        static FolderPicker   FolderPicker;

        static GameList()
        {
            GameDirectory = Config.DefaultGameDirectory;
            FolderPicker  = FolderPicker.GetFolderPicker("FolderDialog", Config.DefaultGameDirectory);

            Refresh(GameDirectory);
        }

        public unsafe static void Refresh(string Path)
        {
            GameItems = new List<GameItem>();

            foreach (string entry in Directory.EnumerateFileSystemEntries(Path))
            {
                if (File.Exists(entry))
                {
                    string Extension = System.IO.Path.GetExtension(entry).ToLower();

                    if (Extension == ".nro" || Extension == ".nso")
                    {
                        GameItem GameItem = new GameItem(entry);

                        if (GameItem.IsNro && GameItem.HasIcon)
                        {
                            GameItem.TextureID = GL.GenTexture();

                            GL.BindTexture(TextureTarget.Texture2D, GameItem.TextureID);
                            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);
                            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);
                            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);

                            NanoJpeg.NJImage image = new NJImage();
                            image.Decode(GameItem.GetIconData());
                            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, image.Width, image.Height, 0, PixelFormat.Rgb,
                                PixelType.UnsignedByte, new IntPtr(image.Image));
                            image.Dispose();
                            GL.BindTexture(TextureTarget.Texture2D, 0);
                        }

                        GameItems.Add(GameItem);
                    }
                }
            }
        }

        public unsafe static Tuple<bool,string> DrawList()
        {
            uint id = 100;

            if (ImGui.Button("Refresh GameList"))
                Refresh(Config.DefaultGameDirectory);

            ImGui.SameLine();
            if(ImGui.Button("Select Game Directory"))
            {
                OpenFolderPicker = true;
            }

            if (OpenFolderPicker)
                ImGui.OpenPopup("OpenFolder");

            DialogResult DialogResult = FolderPicker.GetFolder(ref GameDirectory);
            if (DialogResult == DialogResult.OK)
            {
                Config.DefaultGameDirectory = GameDirectory;
                Refresh(GameDirectory);
            }

            if (DialogResult != DialogResult.None)
                OpenFolderPicker = false;

            if (ImGui.BeginChildFrame(20, ImGui.GetContentRegionAvailable(), WindowFlags.AlwaysAutoResize))
            {
                foreach (GameItem GameItem in GameItems)
                {
                    id++;

                    if (GameItem == SelectedGame)
                        ImGui.PushStyleColor(ColorTarget.FrameBg, Values.Color.Yellow);

                    if (ImGui.BeginChildFrame(id, new Vector2(ImGui.GetContentRegionAvailableWidth(), 60)
                        , WindowFlags.AlwaysAutoResize))
                    {
                        if (GameItem.IsNro && GameItem.HasIcon)
                        {
                            ImGui.Image(new IntPtr(GameItem.TextureID), new Vector2(50, 50), new Vector2(0, 0),
                                new Vector2(1, 1), new Vector4(255, 255, 255, 255), new Vector4(0, 0, 0, 255));
                        }
                        else
                        {
                            ImGui.BeginChildFrame(id + 500, new Vector2(50, 50), WindowFlags.NoResize);
                            ImGui.EndChildFrame();
                            ImGui.SameLine();
                            ImGui.Text(Path.GetFileName(GameItem.Path));

                        }

                        ImGui.SameLine();
                        ImGuiNative.igBeginGroup();
                        if (GameItem.IsNro)
                        {
                            if (GameItem.Nro.ControlArchive != null)
                            {
                                ImGui.Text(GameItem.Nro.ControlArchive.LanguageEntries[0].AplicationName);
                                ImGui.Text(GameItem.Nro.ControlArchive.LanguageEntries[0].DeveloperName);
                            }

                        }
                        ImGuiNative.igEndGroup();

                        if (GameItem == SelectedGame)
                            ImGui.PopStyleColor();

                        if (ImGui.IsMouseDoubleClicked(0) && ImGui.IsItemHovered(HoveredFlags.AllowWhenOverlapped) && GameItem == SelectedGame)
                        {
                            return new Tuple<bool, string>(true, GameItem.Path);
                        }
                        else if (ImGui.IsMouseClicked(0) && ImGui.IsItemHovered(HoveredFlags.AllowWhenOverlapped | HoveredFlags.RootAndChildWindows))
                        {
                            SelectedGame = GameItem;
                        }

                        ImGui.EndChildFrame();
                    }
                }

                ImGui.EndChildFrame();
            }

            return new Tuple<bool, string>(false,string.Empty);
        }
    }

    class GameItem
    {
        public AppletType AppletType { get; set; }
        public Nro        Nro        { get; set; }
        public string     Path       { get; set; }
        public int        TextureID  { get; set; }

        public bool IsNro => (System.IO.Path.GetExtension(Path) == ".nro");
        public bool HasIcon  => Nro?.IconData != null;

        public GameItem(string Path)
        {
            this.Path = Path;

            if (File.Exists(Path))
            {
                AppletType = AppletType.Homebrew;
                FileInfo Package = new FileInfo(Path);
                if (Package.Extension.ToLower() == ".nro")
                {
                    Nro = new Nro(File.Open(Path, FileMode.Open), new FileInfo(Path).Name);
                }
            }
            else
                AppletType = AppletType.Cartridge;
        }        

        public byte[] GetIconData()
        {
            if (IsNro)
            {
                return Nro.IconData;
            }
            else return null;
        }
    }

    enum AppletType
    {
        Homebrew,
        Cartridge
    }
}
