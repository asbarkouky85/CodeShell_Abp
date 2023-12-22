using System;

namespace Codeshell.Abp.Files
{
    public interface IDimension : IComparable<IDimension>
    {
        int Width { get; }
        int Height { get; }
    }
}
