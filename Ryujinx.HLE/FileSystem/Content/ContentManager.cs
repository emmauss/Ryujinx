using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using LibHac;
using System.Linq;
using Ryujinx.HLE.HOS;
using Ryujinx.HLE.Loaders.Npdm;

namespace Ryujinx.HLE.FileSystem.Content
{
    public class ContentManager
    {
        public LocationEntry FirstLocationEntry { get; private set; }

        public SortedDictionary<ulong,string> ContentDictionary { get; private set; }

        public ContentStorageId DefaultInstallationStorage { get; private set; }

        private Switch Device;

        public ContentManager(Switch Device)
        {
            ContentDictionary = new SortedDictionary<ulong, string>();

            this.Device = Device;
        }

        public void LoadEntries()
        {
            string SystemContentPath = LocationHelper.GetRealPath(Device.FileSystem, ContentPath.SystemContent);

            Directory.CreateDirectory(SystemContentPath);

            FirstLocationEntry = null;

            LocationEntry PreviousEntry = null;

            ContentDictionary = new SortedDictionary<ulong, string>();

            foreach(string DirectoryPath in Directory.EnumerateDirectories(SystemContentPath))
            {
                if (Directory.GetFiles(DirectoryPath).Length > 0)
                {
                    string NcaName = new DirectoryInfo(DirectoryPath).Name;

                    using (FileStream NcaFile = new FileStream(Directory.GetFiles(DirectoryPath)[0], FileMode.Open))
                    {

                        Nca Nca = new Nca(Device.System.KeySet, NcaFile, false);

                        LocationEntry Entry = new LocationEntry()
                        {
                            ContentPath = ContentPath.SystemContent,
                            Flag = 0,
                            PreviousEntry = PreviousEntry,
                            TitleId = (long)Nca.Header.TitleId,
                        };

                        if (PreviousEntry == null)
                        {
                            FirstLocationEntry = Entry;
                        }
                        else
                        {
                            PreviousEntry.NextEntry = Entry;
                        }

                        PreviousEntry = Entry;

                        ContentDictionary.Add(Nca.Header.TitleId, NcaName);
                    }
                }
            }

            foreach(string FilePath in Directory.EnumerateFiles(SystemContentPath))
            {
                if (Path.GetExtension(FilePath) == ".nca")
                {
                    string NcaName = Path.GetFileName(FilePath);

                    using (FileStream NcaFile = new FileStream(FilePath, FileMode.Open))
                    {
                        Nca Nca = new Nca(Device.System.KeySet, NcaFile, false);

                        LocationEntry Entry = new LocationEntry()
                        {
                            ContentPath = ContentPath.SystemContent,
                            Flag = 0,
                            PreviousEntry = PreviousEntry,
                            TitleId = (long)Nca.Header.TitleId,
                        };

                        if (PreviousEntry == null)
                        {
                            FirstLocationEntry = Entry;
                        }
                        else
                        {
                            PreviousEntry.NextEntry = Entry;
                        }

                        PreviousEntry = Entry;

                        ContentDictionary.Add(Nca.Header.TitleId, NcaName);
                    }
                }
            }
        }

        public void RefreshEntries()
        {
            LocationEntry LocationEntry = FirstLocationEntry;

            while (LocationEntry != null)
            {
                LocationEntry NextLocationEntry = LocationEntry.NextEntry;

                if (LocationEntry.Flag == 0)
                {
                    if (LocationEntry.PreviousEntry == null)
                    {
                        FirstLocationEntry = NextLocationEntry;

                        NextLocationEntry.PreviousEntry = null;
                    }
                    else
                    {
                        LocationEntry.PreviousEntry.NextEntry = NextLocationEntry;

                        NextLocationEntry.PreviousEntry = LocationEntry.PreviousEntry;
                    }
                }

                LocationEntry = NextLocationEntry;
            }
        }

        /*public string GetProgramPath(long TitleId)
        {
            LocationEntry LocationEntry = GetLocation(TitleId);
        }*/

        public void InstallContent(string NcaPath)
        {
            if (File.Exists(NcaPath))
            {
                FileStream NcaStream = new FileStream(NcaPath, FileMode.Open);

                Nca Nca = new Nca(Device.System.KeySet, NcaStream, true);

                string Filename = Path.GetFileName(NcaPath);

                InstallContent(Nca, Filename);
            }
        }

        public void InstallContent(Nca Nca, string Filename)
        {
            if (Nca.Header.Distribution == DistributionType.Download)
            {
                if (Nca.Header.ContentType == ContentType.AocData || Nca.Header.ContentType == ContentType.Data)
                {
                    string ContentStoragePath = LocationHelper.GetContentPath(ContentStorageId.NandSystem);

                    string RealContentPath = LocationHelper.GetRealPath(Device.FileSystem, ContentStoragePath);

                    string NcaName = Filename;

                    if (!NcaName.EndsWith(".nca"))
                    {
                        NcaName += ".nca";
                    }

                    string InstallationPath = Path.Combine(RealContentPath, NcaName);

                    string FilePath = Path.Combine(InstallationPath, "00");

                    if (File.Exists(FilePath))
                    {
                        FileInfo FileInfo = new FileInfo(FilePath);

                        if (FileInfo.Length == (long)Nca.Header.NcaSize)
                        {
                            return;
                        }
                    }

                    if (ContentDictionary.ContainsKey(Nca.Header.TitleId))
                    {
                        string InstalledPath = GetInstalledPath((long)Nca.Header.TitleId);

                        if (File.Exists(InstalledPath))
                        {
                            File.Delete(InstalledPath);
                        }
                        if (Directory.Exists(InstalledPath))
                        {
                            Directory.Delete(InstalledPath, true);
                        }
                    }

                    if (!Directory.Exists(InstallationPath))
                    {
                        Directory.CreateDirectory(InstallationPath);
                    }

                    using (FileStream FileStream = File.Create(FilePath))
                    {
                        Stream NcaStream = Nca.GetStream();

                        NcaStream.CopyStream(FileStream, NcaStream.Length);
                    }
                }
            }
        }

        public NcaId GetInstalledNcaId(long TitleId)
        {            
            if (ContentDictionary.ContainsKey((ulong)TitleId))
            {
                return new NcaId(ContentDictionary[(ulong)TitleId]);
            }

            return null;
        }

        public string GetInstalledPath(long TitleId)
        {
            LocationEntry LocationEntry = GetLocation(TitleId);

            string ContentPath = LocationHelper.GetRealPath(Device.FileSystem, LocationEntry.ContentPath);

            return Path.Combine(ContentPath, ContentDictionary[(ulong)TitleId]);
        }

        private LocationEntry GetLocation(long TitleId)
        {
            LocationEntry CurrentLocationEntry = FirstLocationEntry;

            while (CurrentLocationEntry != null)
            {
                if(CurrentLocationEntry.TitleId == TitleId)
                {
                    return CurrentLocationEntry;
                }

                CurrentLocationEntry = CurrentLocationEntry.NextEntry;
            }

            return CurrentLocationEntry;
        }
    }
}
