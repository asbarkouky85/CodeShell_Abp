using Codeshell.Abp.Attachments.Localization;
using Volo.Abp.Application.Services;

namespace Codeshell.Abp.Attachments
{
    public abstract class AttachmentsAppService : ApplicationService
    {
        protected AttachmentsAppService()
        {
            LocalizationResource = typeof(AttachmentsResource);
            ObjectMapperContext = typeof(AttachmentsApplicationModule);
        }
    }
}
