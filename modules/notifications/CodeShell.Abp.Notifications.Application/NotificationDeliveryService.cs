using Codeshell.Abp.Extensions;
using Codeshell.Abp.Linq;
using Codeshell.Abp.Notifications.Devices;
using Codeshell.Abp.Notifications.Types;
using Codeshell.Abp.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Codeshell.Abp.Notifications.Senders
{
    public class NotificationDeliveryService : ApplicationService, INotificationDeliveryService
    {
        INotificationRepository NotificationRepository => LazyServiceProvider.LazyGetRequiredService<INotificationRepository>();
        IUserDeviceRepository UserDeviceRepository => LazyServiceProvider.LazyGetRequiredService<IUserDeviceRepository>();
        INotificationTypeRepository NotificationTypeRepository => LazyServiceProvider.LazyGetRequiredService<INotificationTypeRepository>();
        INotificationSenderFactory factory => LazyServiceProvider.LazyGetRequiredService<INotificationSenderFactory>();
        ILocaleTextProvider textProvider => LazyServiceProvider.LazyGetRequiredService<ILocaleTextProvider>();
        public NotificationDeliveryService() : base()
        {
        }

        private NotificationMessageDeliveryDto _constructDeliveryData(
            NotificationMessage message, NotificationType type,
            IEnumerable<UserDevice> devices = null)
        {
            var template = type.GetTemplate(message.NotificationProviderId, message.Notification.User.PreferredLanguage ?? "en");
            var notificationData = new NotificationMessageDeliveryDto();

            notificationData.Parameters = message.Notification.Parameters;
            if (template.FieldsToUse != null && template.FieldsToUse.TryRead(out string[] fields))
            {
                notificationData.Parameters = notificationData.Parameters
                    .FromJson<Dictionary<string, object>>()
                    .Where(e => fields.Contains(e.Key))
                    .ToDictionary(e => e.Key, e => e.Value)
                    .ToJson();
            }
            notificationData.TemplateCode = template.Code;
            notificationData.Body = template.ReplaceParameters(textProvider, message.Notification.Parameters);
            notificationData.Title = template.ReplaceTitleParameters(textProvider, message.Notification.Parameters);
            notificationData.User = ObjectMapper.Map(message.Notification.User, new UserMessageDeliveryData());
            notificationData.NotificationId = message.Notification.Id;
            notificationData.EntityId = message.Notification.EntityId;
            notificationData.EntityType = message.Notification.EntityType;

            if (devices != null)
                notificationData.Devices = ObjectMapper.Map(devices, new List<UserDeviceData>());

            return notificationData;
        }

        public async Task<MessageDeliveryResult> SendMessage(NotificationMessage message)
        {
            var type = await NotificationTypeRepository.GetWithTemplates(message.Notification.NotificationTypeId);
            var devices = new List<UserDevice>();
            if (message.NotificationProvider.RequiresDevices)
            {
                var findDeviceType = message.NotificationProviderId == NotificationProviders.List ? NotificationProviders.Browser : message.NotificationProviderId;
                devices = await UserDeviceRepository.GetDevices(new DevicesRequest
                {
                    UserId = message.Notification.UserId,
                    DeviceType = findDeviceType
                });
            }
            var data = _constructDeliveryData(message, type, devices);
            var services = factory.GetSenders(message.NotificationProviderId);
            if (services.Any())
                return await _sendMessage(services.First(), message, data);
            return new MessageDeliveryResult
            {
                IsSuccess = false,
                Exception = $"No Senders found for provider {message.NotificationProvider.Name}"
            };
        }

        public async Task SendPendingMessages()
        {
            var pendingDeliveries = await NotificationRepository.GetPendingMessages(new CodeshellPagedRequest { MaxResultCount = 50 });
            if (pendingDeliveries.Items.Any())
            {
                await SendMessages(pendingDeliveries.Items);
            }
        }



        public async Task SendMessages(IEnumerable<NotificationMessage> messages)
        {
            var typeIds = messages.Select(e => e.Notification.NotificationTypeId).ToList();
            var types = await NotificationTypeRepository.GetWithTemplates(typeIds);

            var userIds = messages
                .Where(e => e.NotificationProvider.RequiresDevices)
                .Select(e => e.Notification.UserId)
                .ToList();
            var devices = await UserDeviceRepository.GetDevices(new DevicesRequest { UserIds = userIds });

            foreach (var message in messages)
            {
                try
                {
                    var notificationDevices = new List<UserDevice>();
                    var providerDeviceType = message.NotificationProviderId == NotificationProviders.List ? NotificationProviders.Browser : message.NotificationProviderId;
                    if (message.NotificationProvider.RequiresDevices)
                    {
                        notificationDevices = devices
                            .Where(e => e.UserId == message.Notification.UserId && e.DeviceTypeId == providerDeviceType)
                            .ToList();

                        if (!notificationDevices.Any())
                        {
                            message.NoDevicesFound();
                            continue;
                        }
                    }

                    var notificationType = types.FirstOrDefault(e => e.Id == message.Notification.NotificationTypeId);

                    var deliveryData = _constructDeliveryData(message, notificationType, notificationDevices);
                    var services = factory.GetSenders(message.NotificationProviderId);
                    foreach (var service in services)
                    {
                        await _sendMessage(service, message, deliveryData);
                    }
                }
                catch (Exception ex)
                {
                    message.SetFailed(new MessageDeliveryResult
                    {
                        Exception = ex.GetMessageAndStack()
                    });
                }
            }
        }

        async Task<MessageDeliveryResult> _sendMessage(INotificationSender service, NotificationMessage message, NotificationMessageDeliveryDto deliveryData)
        {
            try
            {
                var result = await service.SendAsync(deliveryData);
                if (result.IsSuccess)
                {
                    message.SetDelivered(result);
                }
                else
                {
                    message.SetFailed(result);
                }
                return result;
            }
            catch (Exception ex)
            {
                var result = new MessageDeliveryResult
                {
                    Exception = ex.GetMessageAndStack()
                };
                message.SetFailed(result);
                return result;
            }
        }

        public async Task RetryFailedMessages()
        {
            var pendingDeliveries = await NotificationRepository.GetPendingRetryMessages(new CodeshellPagedRequest { MaxResultCount = 50 });
            if (pendingDeliveries.Items.Any())
            {
                await SendMessages(pendingDeliveries.Items);
            }
        }
    }
}