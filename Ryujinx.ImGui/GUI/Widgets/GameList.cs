using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Text;
using ImGuiNET;
using System.Numerics;
using NanoJpeg;
using OpenTK.Graphics.OpenGL;

namespace Ryujinx.UI.Widgets
{
    class GameList
    {
        static List<GameItem> GameItems = new List<GameItem>();
        static GameItem SelectedGame;
        static bool OpenFolderPicker;
        static FolderPicker FolderPicker;
        static string CurrentPath;

        static GameList()
        {
            Refresh(Config.DefaultGameDirectory);
            FolderPicker = FolderPicker.GetFolderPicker("FolderDialog", Config.DefaultGameDirectory);
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
                            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, image.Width, image.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Rgb,
                                PixelType.UnsignedByte, new IntPtr(image.Image));
                            image.Dispose();
                            GL.BindTexture(TextureTarget.Texture2D, 0);
                        }
                        GameItems.Add(GameItem);
                    }
                }
                else if (Directory.Exists(Path))
                {

                }

            }
        }

        public unsafe static void DrawList()
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

            ImGui.SetNextWindowSize(new Vector2(500, 500), Condition.FirstUseEver);
            if (ImGui.BeginPopupModal("OpenFolder", WindowFlags.NoResize))
            {
                string output = CurrentPath;
                if (FolderPicker.Draw(ref output, false))
                {
                    if (!string.IsNullOrWhiteSpace(output))
                    {
                        Config.DefaultGameDirectory = output;
                        Refresh(output);
                    }
                    ImGui.CloseCurrentPopup();
                    OpenFolderPicker = false;
                }
                ImGui.EndPopup();
            }
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
        }
    }

    class GameItem
    {
        public AppletType AppletType;
        public string Path;
        public Nro Nro;
        public bool IsNro;
        public bool HasIcon  => Nro?.IconData != null;
        public int TextureID;

        public GameItem(string Path)
        {
            this.Path = Path;

            if (File.Exists(Path))
            {
                AppletType = AppletType.Homebrew;
                FileInfo Package = new FileInfo(Path);
                if (Package.Extension.ToLower() == ".nro")
                {
                    IsNro = true;
                    Nro = new Nro(File.Open(Path, FileMode.Open), new FileInfo(Path).Name);
                }
            }
            else
                AppletType = AppletType.Cartridge;
        }

        public Bitmap GetBitmap()
        {
            if (IsNro)
            {
                return new Bitmap(new Bitmap(new MemoryStream(Nro.IconData)),new Size(50,50));
            }
            else return null;
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
