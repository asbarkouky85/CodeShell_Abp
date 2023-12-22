using Volo.Abp.Domain.Services;

namespace Codeshell.Abp.Helpers
{
    public interface IReplaceParameterService : IDomainService
    {
        string ReplaceParametersUsingFile(string filePath, object parameters);
        string ReplaceParameters(string contents, object parameters);
    }
}