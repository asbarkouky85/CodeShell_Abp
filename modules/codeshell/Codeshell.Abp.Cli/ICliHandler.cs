using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Codeshell.Abp.Cli
{
    public interface ICliHandler : IApplicationService
    {
        Task HandleAsync(string[] args);
        string FunctionDescription { get; }
        void Document();
    }


}
