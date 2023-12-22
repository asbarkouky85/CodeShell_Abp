using Codeshell.Abp.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Codeshell.Abp.Devices
{
    public interface IDeviceService : IApplicationService
    {
        Task UpdateSignalRConnectionData(SignalRDeviceDataDto data);
        Task ClearSingalRConnectionData(SignalRDeviceDataDto data);
    }
}
