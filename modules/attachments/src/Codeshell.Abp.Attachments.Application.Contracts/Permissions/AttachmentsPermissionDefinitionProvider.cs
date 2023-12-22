using Codeshell.Abp.Attachments.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Codeshell.Abp.Attachments.Permissions
{
    public class AttachmentsPermissionDefinitionProvider : PermissionDefinitionProvider
    {
        public override void Define(IPermissionDefinitionContext context)
        {
             context.AddGroup(AttachmentsPermissions.GroupName, L("Permission:Attachments"));
        }

        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<AttachmentsResource>(name);
        }
    }
}