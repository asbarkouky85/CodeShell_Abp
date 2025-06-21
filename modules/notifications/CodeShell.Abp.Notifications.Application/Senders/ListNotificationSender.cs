using Codeshell.Abp.Data;
using Codeshell.Abp.Devices;
using Codeshell.Abp.Notifications.Devices;
using Codeshell.Abp.Notifications.Pushing;
using Codeshell.Abp.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Codeshell.Abp.Notifications.Users;

namespace Codeshell.Abp.Notifications.Senders
{
    public class ListNotificationSender : DomainService, INotificationSender, IListNotificationSender
    {
        IEmitter<INotificationsPushingContract> pusher => LazyServiceProvider.LazyGetRequiredService<IEmitter<INotificationsPushingContract>>();
        IUserDeviceRepository UserDeviceRepository => LazyServiceProvider.LazyGetRequiredService<IUserDeviceRepository>();
        ICodeshellRepository<User, Guid> UserRepository => LazyServiceProvider.LazyGetRequiredService<ICodeshellRepository<User, Guid>>();
        public NotificationProviders ProviderId => NotificationProviders.List;

        public ListNotificationSender() : base()
        {
        }

        public async Task<MessageDeliveryResult> SendAsync(NotificationMessageDeliveryDto deliveryData)
        {
            if (deliveryData.User.UserId != null)
            {
                var userNotificationCount = await UserRepository.GetSingleAs(d => new
                {
                    d.Id,
                    Count = d.Notifications.Count(e => !e.IsRead && e.NotificationMessages.Any(e => e.NotificationProviderId == NotificationProviders.List))
                }, d => d.Id == deliveryData.User.UserId);
                var count = userNotificationCount.Count;
                await pusher.EmitAsync(d => d.NotificationsChanged(count), deliveryData.Devices.Select(e => e.ConnectionId).ToArray());
            }
            return new MessageDeliveryResult(true);
        }

        public async Task<MessageDeliveryResult> SendCount(NotificationCountSendDto deliveryData)
        {
            var res = new MessageDeliveryResult();
            foreach (var user in deliveryData.UserIds)
            {
                var devices = await UserDeviceRepository.GetDevices(new DevicesRequest
                {
                    UserId = user,
                    DeviceType = NotificationProviders.Browser
                });

                if (devices.Any())
                {
                    var request = new NotificationMessageDeliveryDto
                    {
                        User = new UserMessageDeliveryData { UserId = user },
                        Devices = devices.Select(e => new UserDeviceData
                        {
                            DeviceId = e.DeviceId,
                            ConnectionId = e.ConnectionId,
                        }).ToList()
                    };
                    await SendAsync(request);
                }

            }
            return res;
        }
    }
}
