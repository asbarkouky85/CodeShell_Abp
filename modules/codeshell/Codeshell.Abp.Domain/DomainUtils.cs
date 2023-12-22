using Codeshell.Abp.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Guids;
using Volo.Abp.Uow;

namespace Codeshell.Abp
{
    public static class DomainUtils
    {

        static IGuidGenerator _guidGenerator;
        public async static Task RunScoped(Func<IServiceProvider, Task> func)
        {
            var logger = NullLogger.Instance;
            logger.LogInformation("Run Scoped Requested");
            try
            {
                using (var scope = CodeshellRoot.RootProvider.CreateScope())
                {
                    await func(scope.ServiceProvider);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Run Scoped thrown an exception");
            }
        }
        public static void InitGuidGenerator(IServiceProvider provider)
        {
            _guidGenerator = provider.GetRequiredService<IGuidGenerator>();
        }

        private static IGuidGenerator Generator
        {
            get
            {
                if (_guidGenerator == null)
                    _guidGenerator = CodeshellRoot.RootProvider?.GetRequiredService<IGuidGenerator>();
                return _guidGenerator;
            }
        }

        public static Guid NewGuid()
        {
            if (Generator != null)
            {
                Thread.Sleep(2);
                return Generator.Create();
            }
            else
            {
                return Guid.NewGuid();
            }
        }


    }
}
