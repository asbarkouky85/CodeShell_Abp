using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Codeshell.Abp.Devices
{
    public class UserDevice : AuditedEntity<Guid>, IMultiTenant
    {

        protected UserDevice()
        {

        }

        public UserDevice(string deviceId, Guid userId, Guid? tenantId, DeviceTypes deviceType) : this()
        {
            DeviceId = deviceId;
            UserId = userId;
            TenantId = tenantId;
            DeviceTypeId = deviceType;
        }

        public string DeviceId { get; protected set; }
        public string ConnectionId { get; protected set; }
        public DeviceTypes DeviceTypeId { get; protected set; }
        public Guid? TenantId { get; protected set; }
        public Guid UserId { get; protected set; }

        public void SetConnectionId(string connection)
        {
            ConnectionId = connection;
        }
    }
}
