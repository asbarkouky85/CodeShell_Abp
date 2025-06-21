using Volo.Abp.DependencyInjection;
using Codeshell.Abp.DistributedEventBoxes;

namespace Codeshell.Abp.DistributedEventBoxes
{
    public interface IEventBoxLogger : ITransientDependency
    {
        bool Disable { get; set; }
        IEventBoxLogger SetId(string id);
        void Log(string log, byte[] data = null);
        void LogObject(string log, object data);
    }
}
