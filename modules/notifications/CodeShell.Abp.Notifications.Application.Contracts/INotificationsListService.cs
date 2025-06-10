using Codeshell.Abp.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Codeshell.Abp.Notifications
{
    public interface INotificationsListService
    {
        Task ChangeStatus(NotificationReadStatusDto dto);
        Task<int> CountByUser();
        Task<PagedResultDto<NotificationListDto>> GetByUser(CodeshellPagedRequestDto opts);
        Task Test();
    }
}