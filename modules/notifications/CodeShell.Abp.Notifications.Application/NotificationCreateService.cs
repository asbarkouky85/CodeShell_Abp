using Codeshell.Abp.Notifications.Senders;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Codeshell.Abp.Notifications;

namespace Codeshell.Abp.Notifications
{
    public class NotificationCreateService : ApplicationService, INotificationCreateService
    {
        INotificationRepository NotificationRepo => LazyServiceProvider.LazyGetRequiredService<INotificationRepository>();
        INotificationTypeRepository NotificationTypeRepo => LazyServiceProvider.LazyGetRequiredService<INotificationTypeRepository>();
        INotificationAttachmentStorage AttachmentStorage => LazyServiceProvider.LazyGetRequiredService<INotificationAttachmentStorage>();
        INotificationDeliveryService Delivery => LazyServiceProvider.LazyGetRequiredService<INotificationDeliveryService>();
        public NotificationCreateService(IServiceProvider provider) : base()
        {
        }

        public async Task CreateNotifications(NotificationCreateRequestData request)
        {
            if (request.Users != null && request.Users.Any())
            {
                if (request.HasAttachments())
                    await _createAttachments(request);

                var type = await NotificationTypeRepo.GetWithProviders(request.NotificationTypeId);
                Notification notification = _createNotification(request.NotificationTypeId, request.EntityType, request.EntityId, request.Parameters);
                foreach (var notificationUser in request.Users)
                {
                    var userNote = notification.Clone();
                    userNote.UserId = notificationUser.UserId;

                    userNote.SetProviders(type.Providers.Select(e => e.NotificationProviderId).ToList());

                    if (notificationUser.HasAttachments())
                    {
                        foreach (var attachmentDto in notificationUser.Attachments)
                        {
                            var attachment = ObjectMapper.Map(attachmentDto, new NotificationAttachment());
                            userNote.NotificationAttachments.Add(attachment);
                        }
                    }

                    if (request.HasAttachments())
                    {
                        foreach (var attachmentDto in request.Attachments)
                        {
                            var attachment = ObjectMapper.Map(attachmentDto, new NotificationAttachment());
                            userNote.NotificationAttachments.Add(attachment);
                        }
                    }

                    await NotificationRepo.InsertAsync(userNote);
                }
                await CurrentUnitOfWork.SaveChangesAsync();

                await Delivery.SendPendingMessages();

            }
        }

        private async Task _createAttachments(NotificationCreateRequestData request)
        {
            foreach (var attachmentDto in request.Attachments)
            {
                await _storeFileIfBase64(attachmentDto);
                foreach (var user in request.Users)
                {
                    if (user.HasAttachments())
                    {
                        foreach (var attachment in user.Attachments)
                            await _storeFileIfBase64(attachment);
                    }
                }
            }
        }

        private async Task _storeFileIfBase64(NotifyAttachmentData attachmentDto)
        {

            if (attachmentDto.Base64 != null)
            {
                attachmentDto.AttachmentId = await AttachmentStorage.CreateFileFromBase64(attachmentDto.FileName, attachmentDto.Base64);
            }
        }

        private Notification _createNotification(long typeId, string entityType, string entityId = null, object data = null)
        {
            Notification notification = new Notification(typeId, entityType, entityId, data);
            if (CurrentUser?.Id != null)
                notification.SetUser(CurrentUser.Id, CurrentUser.Name);
            return notification;
        }
    }
}
