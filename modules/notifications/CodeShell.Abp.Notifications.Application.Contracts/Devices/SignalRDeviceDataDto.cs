using System;
using System.Collections.Generic;
using System.Text;

namespace Codeshell.Abp.Notifications.Devices
{
    public class SignalRDeviceDataDto
    {
        public long UserId { get; set; }
        public string DeviceId { get; set; }
        public string Culture { get; set; }
        public Guid? TenantId { get; set; }
        public string ConnectionId { get; set; }
    }
}
