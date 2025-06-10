using Codeshell.Abp.Data.EntityFramework;
using Codeshell.Abp.EntityFramework;
using Codeshell.Abp.Notifications.Devices;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Codeshell.Abp.Notifications
{
    public class CodeshellNotificationsUnit<TDbContext> : UnitOfWork<TDbContext>, ICodeshellNotificationsUnit
        where TDbContext : CodeshellDbContext<TDbContext>, IDevicesDbContext
    {
        public CodeshellNotificationsUnit(IServiceProvider provider) : base(provider)
        {
        }

        public IUserDeviceRepository UserDeviceRepository => GetRepository<IUserDeviceRepository>();
    }
}
