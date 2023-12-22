using System;
using System.Collections;
using System.Collections.Generic;

namespace Codeshell.Abp.Devices
{
    public class DevicesRequest
    {
        public IEnumerable<Guid> UserIds { get; set; }
        public Guid? UserId { get; set; }
        public DeviceTypes? DeviceType { get; set; }
    }
}