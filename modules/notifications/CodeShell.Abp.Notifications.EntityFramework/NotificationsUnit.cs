using Codeshell.Abp.Data;
using Codeshell.Abp.Data.EntityFramework;
using Codeshell.Abp.Notifications.Devices;
using Codeshell.Abp.Notifications.Users;
using System;

namespace Codeshell.Abp.Notifications
{
    public class NotificationsUnit : UnitOfWork<NotificationsContext>, INotificationsUnit
    {
        public NotificationsUnit(IServiceProvider userAccessor) : base(userAccessor)
        {
        }
        public INotificationRepository NotificationRepository => GetRepository<INotificationRepository>();

        public IRepository<User> UserRepository => GetRepositoryFor<User>();

        public IUserDeviceRepository UserDeviceRepository => GetRepository<IUserDeviceRepository>();

        public INotificationTypeRepository NotificationTypeRepository => GetRepository<INotificationTypeRepository>();
    }
}
