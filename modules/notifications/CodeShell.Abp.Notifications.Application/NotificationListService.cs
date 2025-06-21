using Codeshell.Abp.Linq;
using Codeshell.Abp.Notifications.Senders;
using Codeshell.Abp.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Codeshell.Abp.Notifications;

namespace Codeshell.Abp.Notifications
{
    public class NotificationListService : ApplicationService, INotificationsListService
    {
        CurrentCulture CurrentCult => LazyServiceProvider.LazyGetRequiredService<CurrentCulture>();
        ILocaleTextProvider TextProvider => LazyServiceProvider.LazyGetRequiredService<ILocaleTextProvider>();
        INotificationTypeRepository NotificationTypeRepository => LazyServiceProvider.LazyGetRequiredService<INotificationTypeRepository>();
        INotificationRepository NotificationRepository => LazyServiceProvider.LazyGetRequiredService<INotificationRepository>();
        INotificationDeliveryService Deliver => LazyServiceProvider.LazyGetRequiredService<INotificationDeliveryService>();
        IListNotificationSender listNotificationSender => LazyServiceProvider.LazyGetRequiredService<IListNotificationSender>();
        public NotificationListService(IServiceProvider provider) : base()
        {
        }

        public async Task ChangeStatus(NotificationReadStatusDto dto)
        {
            Notification note = await NotificationRepository.GetAsync(dto.Id);
            if (note != null)
            {
                note.IsRead = dto.Status;

                if (note.IsRead && note.EntityId != null)
                {
                    var nots = await NotificationRepository.GetListAsync(d => d.EntityId == note.EntityId && d.UserId == note.UserId);
                    foreach (var n in nots)
                    {
                        n.IsRead = true;
                    }
                }
            }
            await CurrentUnitOfWork.SaveChangesAsync();

            await listNotificationSender.SendCount(new NotificationCountSendDto { UserIds = new() { note.UserId } });

        }

        public async Task<int> CountByUser()
        {
            return await NotificationRepository.CountAsync(d => d.UserId == CurrentUser.Id && !d.IsRead);
        }

        public async Task<PagedResultDto<NotificationListDto>> GetByUser(CodeshellPagedRequestDto opts)
        {
            var notifications = await NotificationRepository.GetByUser(CurrentUser.Id ?? Guid.Empty, ObjectMapper.Map(opts, new CodeshellPagedRequest()));
            var types = notifications.Items.Select(e => e.NotificationTypeId).ToList();

            var templates = await NotificationTypeRepository.GetWithTemplates(types);
            var dtoList = new List<NotificationListDto>();
            var result = new PagedResultDto<NotificationListDto>
            {
                TotalCount = notifications.TotalCount,
            };

            foreach (var notification in notifications.Items)
            {
                var type = templates.FirstOrDefault(e => e.Id == notification.NotificationTypeId);
                var templateContent = type.GetTemplate(NotificationProviders.List, CurrentCult.Name);
                var dto = ObjectMapper.Map(notification, new NotificationListDto());
                dto.Body = templateContent.ReplaceParameters(TextProvider, notification.Parameters);
                dtoList.Add(dto);
            }

            result.Items = dtoList;
            return result;
        }


        public async Task Test()
        {
            await listNotificationSender.SendCount(new NotificationCountSendDto { UserIds = new() { Guid.Empty } });
        }
    }
}
