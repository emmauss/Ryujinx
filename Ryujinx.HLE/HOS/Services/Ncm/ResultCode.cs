namespace Ryujinx.HLE.HOS.Services.Ncm
{
    public enum ResultCode
    {
        ModuleId       = 5,
        ErrorCodeShift = 9,

        Success = 0,

        InvalidContentStorageBase                 = (1    << ErrorCodeShift) | ModuleId,
        PlaceHolderAlreadyExists                  = (2    << ErrorCodeShift) | ModuleId,
        PlaceHolderNotFound                       = (3    << ErrorCodeShift) | ModuleId,
        ContentAlreadyExists                      = (4    << ErrorCodeShift) | ModuleId,
        ContentNotFound                           = (5    << ErrorCodeShift) | ModuleId,
        ContentMetaNotFound                       = (7    << ErrorCodeShift) | ModuleId,
        AllocationFailed                          = (8    << ErrorCodeShift) | ModuleId,
        UnknownStorage                            = (12   << ErrorCodeShift) | ModuleId,
        InvalidContentStorage                     = (100  << ErrorCodeShift) | ModuleId,
        InvalidContentMetaDatabase                = (110  << ErrorCodeShift) | ModuleId,
        InvalidPackageFormat                      = (130  << ErrorCodeShift) | ModuleId,
        InvalidPlaceHolderFile                    = (170  << ErrorCodeShift) | ModuleId,
        BufferInsufficient                        = (180  << ErrorCodeShift) | ModuleId,
        WriteToReadOnlyContentStorage             = (190  << ErrorCodeShift) | ModuleId,
        InvalidContentMetaKey                     = (240  << ErrorCodeShift) | ModuleId,
        GameCardContentStorageNotActive           = (251  << ErrorCodeShift) | ModuleId,
        BuiltInSystemContentStorageNotActive      = (252  << ErrorCodeShift) | ModuleId,
        BuiltInUserContentStorageNotActive        = (253  << ErrorCodeShift) | ModuleId,
        SdCardContentStorageNotActive             = (254  << ErrorCodeShift) | ModuleId,
        UnknownContentStorageNotActive            = (255  << ErrorCodeShift) | ModuleId,
        ContentStorageNotActive                   = (258  << ErrorCodeShift) | ModuleId,
        GameCardContentMetaDatabaseNotActive      = (261  << ErrorCodeShift) | ModuleId,
        BuiltInSystemContentMetaDatabaseNotActive = (262  << ErrorCodeShift) | ModuleId,
        BuiltInUserContentMetaDatabaseNotActive   = (263  << ErrorCodeShift) | ModuleId,
        SdCardContentMetaDatabaseNotActive        = (264  << ErrorCodeShift) | ModuleId,
        UnknownContentMetaDatabaseNotActive       = (268  << ErrorCodeShift) | ModuleId,
        ContentStorageBaseNotFound                = (310  << ErrorCodeShift) | ModuleId,
        InvalidOffset                             = (8182 << ErrorCodeShift) | ModuleId,
    }
}