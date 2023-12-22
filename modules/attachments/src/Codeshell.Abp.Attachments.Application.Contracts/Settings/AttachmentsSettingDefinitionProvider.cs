using Volo.Abp.Settings;

namespace Codeshell.Abp.Attachments.Settings
{
    public class AttachmentsSettingDefinitionProvider : SettingDefinitionProvider
    {
        public override void Define(ISettingDefinitionContext context)
        {
            context.Add(new SettingDefinition(AttachmentConst.DriveRootPath, "C:/_attachments", isVisibleToClients: false));
        }
    }
}