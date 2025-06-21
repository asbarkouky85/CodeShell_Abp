using System.Threading.Tasks;

namespace Codeshell.Abp.DistributedEventBoxes.Outbox
{
    public interface IThiqahOutboxSender
    {
        Task SendEvents();
    }
}
