using System;
using System.Collections.Generic;
using System.Text;

namespace Ryujinx.Ui
{
    public enum UIActionResult
    {
        UnknownError = -1,
        Succcess = 0,
        DirectoryNotFound,
        FileNotFound,
        InvalidInput,
    }
}
