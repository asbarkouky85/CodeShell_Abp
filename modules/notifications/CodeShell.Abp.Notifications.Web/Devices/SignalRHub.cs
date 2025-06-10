﻿using Codeshell.Abp.Extensions;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

namespace Codeshell.Abp.Notifications.Devices
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
            var context = Context;
            
            using (var sc = Shell.GetScope())
            {
                if (data.TenantId != null)
                    sc.ServiceProvider.SetCurrentTenant(data.TenantId.Value);
                data.ConnectionId = Context.ConnectionId;
                var service = sc.ServiceProvider.GetRequiredService<IDeviceService>();
                var t = service.UpdateSignalRConnectionData(data);
                t.Wait();
            };
            return Context.ConnectionId;
        }

        public virtual void ClearConnectionData(SignalRDeviceDataDto data)
        {
            using (var sc = Shell.GetScope())
            {
                if (data.TenantId != null)
                    sc.ServiceProvider.SetCurrentTenant(data.TenantId.Value);
                data.ConnectionId = Context.ConnectionId;
                var service = sc.ServiceProvider.GetRequiredService<IDeviceService>();
                var t = service.ClearSingalRConnectionData(data);
                t.Wait();
            };
        }
    }
}
