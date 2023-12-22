using Codeshell.Abp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace Codeshell.Abp.Attachments.Events
{
    public class TempFileConfirmedHandler : IDistributedEventHandler<TempFileConfirmed>,ITransientDependency
    {
        private readonly IAttachmentFileService svc;

        public TempFileConfirmedHandler(IAttachmentFileService svc)
        {
            this.svc = svc;
        }
        public Task HandleEventAsync(TempFileConfirmed eventData)
        {
            return Utils.HandleEvent(eventData, async () =>
            {
                await svc.SaveAttachment(eventData.SaveRequest);
            });
        }
    }
}
