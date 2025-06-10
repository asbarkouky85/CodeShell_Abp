using Volo.Abp.Settings;

namespace Codeshell.Abp.Attachments.Settings
{
    public class AttachmentsSettingDefinitionProvider : SettingDefinitionProvider
    {
        public override void Define(ISettingDefinitionContext context)
        {
            context.Add(new SettingDefinition(AttachmentsSettings.DriveRootPath, "C:/_attachments", isVisibleToClients: false));
            context.Add(new SettingDefinition(AttachmentsSettings.CleanupRequestsIntervalSeconds, "5", isVisibleToClients: false));
            context.Add(new SettingDefinition(AttachmentsSettings.MinimumChunkUploadSize, "3145728", isVisibleToClients: true));
        }
    }
}