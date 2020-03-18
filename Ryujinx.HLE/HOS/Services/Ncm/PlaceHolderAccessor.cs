using LibHac;
using LibHac.Fs;
using LibHac.FsSystem;
using Ryujinx.HLE.FileSystem;
using Ryujinx.HLE.HOS.Services.Ncm.Types;
using Ryujinx.HLE.Utilities;
using System;
using System.IO;
using System.Collections.Generic;

namespace Ryujinx.HLE.HOS.Services.Ncm
{
    public class PlaceHolderAccessor
    {
        const string BasePath = "/placehld";

        private string _rootPath;

        private Types.StorageId _storageId;

        private Dictionary<UInt128, CacheEntry> _cacheDictionary;

        public void Initialize(string rootPath, Types.StorageId storageId)
        {
            _rootPath = rootPath;
            _storageId = storageId;

            //TODO clear cache is exists;

            _cacheDictionary = new Dictionary<UInt128, CacheEntry>();
        }

        public string MakeBasePlaceHolderDirectoryPath(string rootPath)
        {
            return Path.Combine(rootPath, BasePath);
        }

        public string MakePath(UInt128 placeHolderId)
        {
            string rootPath = MakeBasePlaceHolderDirectoryPath(_rootPath);

            return FileHelpers.MakePlaceHolderPath(placeHolderId, rootPath, _storageId);
        }

        public ResultCode EnsurePlaceHolderDirectory(UInt128 placeHolderId)
        {
            string path = MakePath(placeHolderId);

            var fileSystem = VirtualFileSystem.CreateInstance();

            string realPath = fileSystem.SwitchPathToSystemPath(path);

            Directory.GetParent(realPath).Create();

            return ResultCode.Success;
        }

        public ResultCode GetplaceHolderIdFromFileName(out UInt128 placeHolderId, string name)
        {
            string fileName = Path.GetFileNameWithoutExtension(name);

            placeHolderId = new UInt128(fileName);

            return ResultCode.Success;
        }

        private LibHac.Result Open(UInt128 placeHolderId, out IFile file)
        {
            string basePath = MakeBasePlaceHolderDirectoryPath(_rootPath);

            var folder = new LocalFileSystem(basePath);

            return folder.OpenFile(out file, FileHelpers.MakePath(placeHolderId, _storageId), OpenMode.All);
        }

        private bool LoadFromCache(out IFile fileHandle, UInt128 placeHolderId)
        {
            lock (_cacheDictionary)
            {
                if (!_cacheDictionary.TryGetValue(placeHolderId, out CacheEntry entry))
                {

                    if (entry == null)
                    {
                        fileHandle = null;

                        return false;
                    }
                }

                fileHandle = entry.Handle;
            }

            return true;
        }

        private void StoreToCache(IFile fileHandle, UInt128 placeHolderId)
        {
            lock (_cacheDictionary)
            {
                _cacheDictionary.Add(placeHolderId, new CacheEntry()
                {
                    PlaceHolderId = placeHolderId,
                    Handle = fileHandle
                });
            }
        }

        public string GetPath(UInt128 placeHolderId)
        {
            lock (_cacheDictionary)
            {
                if (_cacheDictionary.TryGetValue(placeHolderId, out CacheEntry entry))
                {
                    entry.Close();

                    _cacheDictionary.Remove(placeHolderId);
                }
            }

            return MakePath(placeHolderId);
        }

        public ResultCode CreatePlaceHolderFile(UInt128 placeHolderId, long size)
        {
            EnsurePlaceHolderDirectory(placeHolderId);

            var fileSystem = VirtualFileSystem.CreateInstance();

            string basePath = fileSystem.SwitchPathToSystemPath(MakeBasePlaceHolderDirectoryPath(_rootPath));

            var folder = new LocalFileSystem(basePath);

            Result result = folder.CreateFile(FileHelpers.MakePath(placeHolderId, _storageId), size, CreateFileOptions.None);

            if (result.IsFailure())
            {
                if ((Fs.ResultCode)result.Value == Fs.ResultCode.PathDoesNotExist)
                {
                    return ResultCode.PlaceHolderNotFound;
                }
            }

            return ResultCode.Success;
        }
        
        public ResultCode DeletePlaceHolderFile(UInt128 placeHolderId)
        {
            EnsurePlaceHolderDirectory(placeHolderId);

            string basePath = MakeBasePlaceHolderDirectoryPath(_rootPath);

            var folder = new LocalFileSystem(basePath);

            Result result = folder.DeleteFile(FileHelpers.MakePath(placeHolderId, _storageId));

            if (result.IsFailure())
            {
                if ((Fs.ResultCode)result.Value == Fs.ResultCode.PathDoesNotExist)
                {
                    return ResultCode.PlaceHolderNotFound;
                }
            }

            return ResultCode.Success;
        }

        public ResultCode WritePlaceHolderFile(UInt128 placeHolderId, Span<byte> data, long offset)
        {
            Result result = Open(placeHolderId, out IFile file);

            if (result.IsFailure())
            {
                if ((Fs.ResultCode)result.Value == Fs.ResultCode.PathDoesNotExist)
                {
                    return ResultCode.PlaceHolderNotFound;
                }
            }

            file.Write(offset, data);

            StoreToCache(file, placeHolderId);

            return ResultCode.Success;
        }

        public ResultCode SetPlaceHolderFileSize(UInt128 placeHolderId, long size)
        {
            Result result = Open(placeHolderId, out IFile file);

            if (result.IsFailure())
            {
                if ((Fs.ResultCode)result.Value == Fs.ResultCode.PathDoesNotExist)
                {
                    return ResultCode.PlaceHolderNotFound;
                }
            }

            file.SetSize(size);

            file.Dispose();

            return ResultCode.Success;
        }

        public bool TryGetplaceHolderFileSize(UInt128 placeHolderId, out long size)
        {
            size = 0;

            if (LoadFromCache(out IFile file, placeHolderId))
            {
                file.GetSize(out size);

                return true;
            }

            return false;
        }

        public void CloseAll()
        {
            foreach (CacheEntry entry in _cacheDictionary.Values)
            {
                entry.Close();
            }

            _cacheDictionary.Clear();
        }
    }
}