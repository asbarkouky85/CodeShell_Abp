using Codeshell.Abp;
using Codeshell.Abp.Attachments.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Uow;

namespace Codeshell.Abp.Attachments.Events
{
    public class TempFileConfirmedHandler : IDistributedEventHandler<TempFileConfirmed>, ITransientDependency
    {
        private readonly IAttachmentFileService svc;

        public TempFileConfirmedHandler(IAttachmentFileService svc)
        {
            this.svc = svc;
        }

        [UnitOfWork]
        public virtual async Task HandleEventAsync(TempFileConfirmed eventData)
        {

            await svc.SaveAttachment(eventData.SaveRequest);

        }
    }
}
