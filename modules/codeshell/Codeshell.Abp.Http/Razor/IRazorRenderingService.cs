using Microsoft.AspNetCore.Http;
using Volo.Abp.DependencyInjection;

namespace Codeshell.Abp.Razor
{
    public interface IRazorRenderingService : ITransientDependency
    {
        Task<string?> RenderPageAsync(HttpContext context, string pageName, string layout, object? model = null, Dictionary<string, object>? viewData = null);
        Task<string> RenderViewAsync(HttpContext context, string viewName, object? model = null, Dictionary<string, object>? viewData = null);
    }
}