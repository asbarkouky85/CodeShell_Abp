using Codeshell.Abp.Notifications.Devices;
using System;

namespace Codeshell.Abp.Notifications.Pushing
{
    public class NotificationsHub : SignalRHub<INotificationsPushingContract>
    {

        public NotificationsHub()
        {

        }

        private SignalRDeviceDataDto _constructRequest(string tenantId, object userId)
        {
            var result = new SignalRDeviceDataDto();
            if (Guid.TryParse(tenantId, out Guid tout))
                result.TenantId = tout;

            if (Guid.TryParse(userId.ToString(), out Guid uout))
                result.UserId = uout.ToString();
            return result;
        }

        public string SetUserConnectionId(string tenantId, object userId)
        {
            return UpdateConnectionData(_constructRequest(tenantId, userId));
        }

        public void ClearUserConnectionId(string tenantId, object userId)
        {
            ClearConnectionData(_constructRequest(tenantId, userId));
        }
    }
}
