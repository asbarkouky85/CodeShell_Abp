using System;
using System.Collections;
using System.Collections.Generic;

namespace Codeshell.Abp.Notifications.Devices
{
    public class DevicesRequest
    {
        public IEnumerable<Guid> UserIds { get; set; }
        public Guid? UserId { get; set; }
        public NotificationProviders? DeviceType { get; set; }
        public bool LoggedInDevicesOnly { get; set; } = true;
    }
}