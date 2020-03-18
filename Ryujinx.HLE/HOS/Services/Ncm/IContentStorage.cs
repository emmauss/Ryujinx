using Ryujinx.HLE.HOS;
using Ryujinx.HLE.HOS.Services.Ncm.Types;
using Ryujinx.HLE.Utilities;
using System;

namespace Ryujinx.HLE.HOS.Services.Ncm
{
     abstract class IContentStorage : IpcService
    {
        protected string _rootPath;

        protected StorageId _storageId;
        protected PlaceHolderAccessor _placeholderAccessor;

        public IContentStorage(string rootPath, StorageId storageId)
        {
            _rootPath = rootPath;
            _storageId = storageId;

            _placeholderAccessor = new PlaceHolderAccessor();
            _placeholderAccessor.Initialize(_rootPath, storageId);
        }

        [Command(0)]
        // GeneratePlaceHolderId() -> Uuid
        public ResultCode GeneratePlaceHolderId(ServiceCtx context){
            UInt128 placeHolderId = UInt128.GenerateUuid();

            context.ResponseData.Write(placeHolderId.ToBytes());

            return ResultCode.Success;
        }

        [Command(1)]
        // GeneratePlaceHolderId() -> PlaceHolderId
        public  ResultCode CreatePlaceHolder(ServiceCtx context)
        {
            UInt128 contentId     = new UInt128(context.RequestData.ReadBytes(0x10));
            UInt128 placeHolderId = new UInt128(context.RequestData.ReadBytes(0x10));
            
            long size = context.RequestData.ReadInt64();

            return CreatePlaceHolder(placeHolderId, contentId, size);
        }

        [Command(2)]
        // GeneratePlaceHolderId() -> PlaceHolderId
        public ResultCode DeletePlaceHolder(ServiceCtx context)
        {
            UInt128 placeHolderId = new UInt128(context.RequestData.ReadBytes(0x10));

            return _placeholderAccessor.DeletePlaceHolderFile(placeHolderId);
        }

        [Command(3)]
        // GeneratePlaceHolderId() -> PlaceHolderId
        public ResultCode HasPlaceHolder(ServiceCtx context)
        {
            UInt128 placeHolderId = new UInt128(context.RequestData.ReadBytes(0x10));

            context.ResponseData.Write(HasPlaceHolder(placeHolderId));

            return ResultCode.Success;
        }

        [Command(4)]
        // GeneratePlaceHolderId() -> PlaceHolderId
        public ResultCode WritePlaceHolder(ServiceCtx context)
        {
            UInt128 placeHolderId = new UInt128(context.RequestData.ReadBytes(0x10));
            
            long offset = context.RequestData.ReadInt64();

            long position = context.Request.ReceiveBuff[0].Position;
            long size     = context.Request.ReceiveBuff[0].Size;

            Span<byte> data = context.Memory.ReadBytes(position, size);

            return WritePlaceHolder(placeHolderId, offset, data);
        }

        [Command(5)]
        // GeneratePlaceHolderId() -> PlaceHolderId
        public ResultCode Register(ServiceCtx context)
        {
            UInt128 contentId = new UInt128(context.RequestData.ReadBytes(0x10));
            UInt128 placeHolderId = new UInt128(context.RequestData.ReadBytes(0x10));

            return Register(placeHolderId, contentId);
        }

        [Command(6)]
        // GeneratePlaceHolderId() -> PlaceHolderId
        public ResultCode Delete(ServiceCtx context)
        {
            UInt128 contentId = new UInt128(context.RequestData.ReadBytes(0x10));

            return Delete(contentId);
        }

        [Command(7)]
        // GeneratePlaceHolderId() -> PlaceHolderId
        public ResultCode Has(ServiceCtx context)
        {
            UInt128 contentId = new UInt128(context.RequestData.ReadBytes(0x10));

            bool exists = Has(contentId);

            context.ResponseData.Write(exists);

            return ResultCode.Success;
        }

        [Command(8)]
        // GeneratePlaceHolderId() -> PlaceHolderId
        public ResultCode GetPath(ServiceCtx context)
        {
            UInt128 contentId = new UInt128(context.RequestData.ReadBytes(0x10));

            long bufferPosition = context.Request.ReceiveBuff[0].Position;
            long bufferLen      = context.Request.ReceiveBuff[0].Size;

            string path = GetPath(contentId);

            context.Memory.WriteBytes(bufferPosition, System.Text.Encoding.UTF8.GetBytes(path));

            return ResultCode.Success;
        }

        [Command(0)]
        // GeneratePlaceHolderId() -> PlaceHolderId
        public  ResultCode GeneratePlaceHolderId(ServiceCtx context);

        [Command(0)]
        // GeneratePlaceHolderId() -> PlaceHolderId
        public  ResultCode GeneratePlaceHolderId(ServiceCtx context);

        [Command(0)]
        // GeneratePlaceHolderId() -> PlaceHolderId
        public  ResultCode GeneratePlaceHolderId(ServiceCtx context);

        [Command(0)]
        // GeneratePlaceHolderId() -> PlaceHolderId
        public  ResultCode GeneratePlaceHolderId(ServiceCtx context);

        [Command(0)]
        // GeneratePlaceHolderId() -> PlaceHolderId
        public  ResultCode GeneratePlaceHolderId(ServiceCtx context);

        [Command(0)]
        // GeneratePlaceHolderId() -> PlaceHolderId
        public  ResultCode GeneratePlaceHolderId(ServiceCtx context);

        [Command(0)]
        // GeneratePlaceHolderId() -> PlaceHolderId
        public  ResultCode GeneratePlaceHolderId(ServiceCtx context);

        [Command(0)]
        // GeneratePlaceHolderId() -> PlaceHolderId
        public  ResultCode GeneratePlaceHolderId(ServiceCtx context);

        [Command(0)]
        // GeneratePlaceHolderId() -> PlaceHolderId
        public  ResultCode GeneratePlaceHolderId(ServiceCtx context);

        [Command(0)]
        // GeneratePlaceHolderId() -> PlaceHolderId
        public  ResultCode GeneratePlaceHolderId(ServiceCtx context);

        [Command(0)]
        // GeneratePlaceHolderId() -> PlaceHolderId
        public  ResultCode GeneratePlaceHolderId(ServiceCtx context);

        [Command(0)]
        // GeneratePlaceHolderId() -> PlaceHolderId
        public  ResultCode GeneratePlaceHolderId(ServiceCtx context);

        [Command(0)]
        // GeneratePlaceHolderId() -> PlaceHolderId
        public  ResultCode GeneratePlaceHolderId(ServiceCtx context);

        [Command(0)]
        // GeneratePlaceHolderId() -> PlaceHolderId
        public  ResultCode GeneratePlaceHolderId(ServiceCtx context);

        [Command(0)]
        // GeneratePlaceHolderId() -> PlaceHolderId
        public  ResultCode GeneratePlaceHolderId(ServiceCtx context);

        [Command(0)]
        // GeneratePlaceHolderId() -> PlaceHolderId
        public  ResultCode GeneratePlaceHolderId(ServiceCtx context);

        [Command(0)]
        // GeneratePlaceHolderId() -> PlaceHolderId
        public  ResultCode GeneratePlaceHolderId(ServiceCtx context);

        [Command(0)]
        // GeneratePlaceHolderId() -> PlaceHolderId
        public  ResultCode GeneratePlaceHolderId(ServiceCtx context);

        [Command(0)]
        // GeneratePlaceHolderId() -> PlaceHolderId
        public  ResultCode GeneratePlaceHolderId(ServiceCtx context);

        [Command(0)]
        // GeneratePlaceHolderId() -> PlaceHolderId
        public  ResultCode GeneratePlaceHolderId(ServiceCtx context);

        [Command(0)]
        // GeneratePlaceHolderId() -> PlaceHolderId
        public  ResultCode GeneratePlaceHolderId(ServiceCtx context);

        public abstract ResultCode CreatePlaceHolder(UInt128 placeHolderId, UInt128 contentId, long size);

        public abstract bool HasPlaceHolder(UInt128 placeHolderId);
        
        public abstract ResultCode WritePlaceHolder(UInt128 placeHolderId, long offset, Span<byte> buffer);

        public abstract ResultCode Register(UInt128 placeHolderId, UInt128 contentId);

        public abstract ResultCode Delete(UInt128 contentId);

        public abstract bool Has(UInt128 contentId);
        
        public abstract string GetPath(UInt128 contentId);
    }
}