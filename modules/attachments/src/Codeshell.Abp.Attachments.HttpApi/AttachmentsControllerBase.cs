using Microsoft.AspNetCore.Mvc;
using Codeshell.Abp.Attachments.Localization;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;

namespace Codeshell.Abp.Attachments
{
    [Area("attachments")]
    [RemoteService]
    public abstract class AttachmentsControllerBase : AbpController
    {
        protected AttachmentsControllerBase()
        {
            LocalizationResource = typeof(AttachmentsResource);
        }
    }
}
