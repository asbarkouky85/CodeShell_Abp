using Codeshell.Abp.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Codeshell.Abp.Notifications
{

    public class NotificationsController : NotificationsBaseController, INotificationsListService
    {
        readonly INotificationsListService service;

        public NotificationsController(INotificationsListService service)
        {
            this.service = service;
        }

        public async Task<PagedResultDto<NotificationListDto>> GetByUser(CodeshellPagedRequestDto opts)
        {
            return await service.GetByUser(opts);

        }

        [HttpGet]
        public Task<int> CountByUser()
        {
            return service.CountByUser();
        }

        [HttpPut]
        public async Task ChangeStatus(NotificationReadStatusDto dto)
        {
            await service.ChangeStatus(dto);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task Test()
        {
            await service.Test();
        }
    }
}
