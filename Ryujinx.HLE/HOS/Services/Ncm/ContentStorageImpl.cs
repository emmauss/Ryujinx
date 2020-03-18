using LibHac;
using LibHac.Fs;
using LibHac.FsService;
using LibHac.FsSystem;
using Ryujinx.HLE.Utilities;
using System;
using System.Collections.Generic;
using System.IO;

namespace Ryujinx.HLE.HOS.Services.Ncm
{
    internal class ContentStorageImpl : IContentStorage
    {
        const string contentDirectory = "/registered";
        public ContentStorageImpl(string rootPath, Types.StorageId storageId): base(rootPath, storageId) {}
        public override ResultCode CreatePlaceHolder(UInt128 placeHolderId, UInt128 contentId, long size)
        {
            // TODO ensure content directory

            return _placeholderAccessor.CreatePlaceHolderFile(placeHolderId, size);
        }

        public override bool HasPlaceHolder(UInt128 placeHolderId)
        {
            string placeHolderPath = _placeholderAccessor.MakePath(placeHolderId);

            return File.Exists(placeHolderPath);
        }

        public override ResultCode Register(UInt128 placeHolderId, UInt128 contentId)
        {
            // TODO cache

            string placeHolderDriectory = _placeholderAccessor.MakeBasePlaceHolderDirectoryPath(_rootPath);
            string contentPath = MakePath(contentId);

            var placeHolder = new LocalFileSystem(placeHolderDriectory);

            Result result = placeHolder.RenameFile(_placeholderAccessor.MakePath(placeHolderId), contentPath);

            if (result.IsFailure())
            {
                switch ((Fs.ResultCode)result.Value)
                {
                    case Fs.ResultCode.PathDoesNotExist:
                        return ResultCode.PlaceHolderNotFound;
                    case Fs.ResultCode.PathAlreadyExists:
                        return ResultCode.ContentAlreadyExists;
                }
            }
            
            return ResultCode.Success;
        }

        public override ResultCode WritePlaceHolder(UInt128 placeHolderId, long offset, Span<byte> buffer)
        {
            if (offset < 0)
            {
                return ResultCode.InvalidOffset;
            }

            return _placeholderAccessor.WritePlaceHolderFile(placeHolderId, buffer, offset);
        }

        public string MakeBasePlaceHolderDirectoryPath(string rootPath)
        {
            return Path.Combine(rootPath, contentDirectory);
        }

        private string MakePath(UInt128 contentId)
        {
            return FileHelpers.MakeContentPath(contentId, _rootPath, _storageId);
        }

        public override ResultCode Delete(UInt128 contentId)
        {
            string path = MakePath(contentId);

            FileInfo file = new FileInfo(path);

            var folder = new LocalFileSystem(file.DirectoryName);

            Result result = folder.DeleteFile(file.Name);

            if (result.IsFailure())
            {
                if ((Fs.ResultCode)result.Value == Fs.ResultCode.PathDoesNotExist)
                {
                    return ResultCode.ContentNotFound;
                }
            }

            return ResultCode.Success;
        }

        public override bool Has(UInt128 contentId)
        {
            string path = MakePath(contentId);

            return File.Exists(path);
        }

        public override string GetPath(UInt128 contentId)
        {
            string path = MakePath(contentId);

            var filesystem =  FileSystem.VirtualFileSystem.CreateInstance();

            return filesystem.SystemPathToSwitchPath(path);
        }
    }
}