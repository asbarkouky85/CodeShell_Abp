using Codeshell.Abp.Devices;
using Codeshell.Abp.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

namespace Codeshell.Abp.Emitters
{
    public class SignalRHub<T> : Hub<T> where T : class
    {

        public SignalRHub()
        {

        }
        public virtual string GetConnectionId()
        {
            return Context.ConnectionId;
        }

        public virtual string UpdateConnectionData(SignalRDeviceDataDto data)
        {
            var t = Utils.RunWithUnitOfWork(async provider =>
            {
                data.ConnectionId = Context.ConnectionId;
                var service = provider.GetRequiredService<IDeviceService>();
                await service.UpdateSignalRConnectionData(data);
            });
            t.Wait();
            return Context.ConnectionId;
        }

        public virtual void ClearConnectionData(SignalRDeviceDataDto data)
        {
            var t = Utils.RunWithUnitOfWork(async provider =>
            {
                data.ConnectionId = Context.ConnectionId;
                var service = provider.GetRequiredService<IDeviceService>();
                await service.ClearSingalRConnectionData(data);
            });
            t.Wait();
        }
    }
}
