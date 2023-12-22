using System;
using System.Collections.Generic;
using System.Text;

namespace Codeshell.Abp.Http
{
    public class SignalRDeviceDataDto
    {
        public Guid UserId { get; set; }
        public string DeviceId { get; set; }
        public string Culture { get; set; }
        public Guid? TenantId { get; set; }
        public string ConnectionId { get; set; }
    }
}
