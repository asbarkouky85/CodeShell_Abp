using System.Threading.Tasks;

namespace Codeshell.Abp.DistributedEventBoxes.Outbox
{
    public interface ICodeshellOutboxSender
    {
        Task SendEvents();
    }
}
