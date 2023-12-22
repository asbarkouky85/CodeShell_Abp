using Codeshell.Abp.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Codeshell.Abp.Devices
{
    public class NullDeviceService : IDeviceService,ITransientDependency
    {
        public Task ClearSingalRConnectionData(SignalRDeviceDataDto data)
        {
            return Task.CompletedTask;
        }

        public Task UpdateSignalRConnectionData(SignalRDeviceDataDto data)
        {
            return Task.CompletedTask;
        }
    }
}
