using System.Collections.Generic;

namespace Codeshell.Abp
{
    public interface ICaptureSqlDbContext
    {
        void StartCapturing();
        void StopCapturing();
        List<string> GetCapturedCommands();
    }
}
