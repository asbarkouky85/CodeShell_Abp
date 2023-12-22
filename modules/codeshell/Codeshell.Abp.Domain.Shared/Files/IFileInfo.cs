using System;
using System.Collections.Generic;
using System.Text;

namespace Codeshell.Abp.Files
{
    public interface IFileInfo
    {
        string FileName { get; }
        string Extension { get; }
        int? Size { get; }
        FileDimesion Dimesion { get; }

    }
}
