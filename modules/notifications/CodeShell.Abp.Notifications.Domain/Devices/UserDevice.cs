﻿using Codeshell.Abp.Notifications.Providers;
using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace Codeshell.Abp.Notifications.Devices
{
    public class UserDevice : AuditedEntity<long>, IMultiTenant
    {

        protected UserDevice() : base()
        {
            Id = Utils.GenerateID();
        }

        public UserDevice(string deviceId, long userId, Guid? tenantId, NotificationProviders deviceType, bool isLoggedIn = true) : this()
        {
            DeviceId = deviceId;
            UserId = userId;
            TenantId = tenantId;
            DeviceTypeId = deviceType;
            IsLoggedIn = isLoggedIn;
        }

        public string DeviceId { get; set; }
        public string ConnectionId { get; set; }
        public NotificationProviders DeviceTypeId { get; set; }
        public NotificationProvider NotificationProvider { get; set; }
        public Guid? TenantId { get; set; }
        public long UserId { get; set; }
        public bool IsLoggedIn { get; set; }

        public void SetConnectionId(string connection)
        {
            ConnectionId = connection;
        }
    }
}
