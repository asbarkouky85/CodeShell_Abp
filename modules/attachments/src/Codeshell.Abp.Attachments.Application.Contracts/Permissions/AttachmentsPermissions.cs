using Volo.Abp.Reflection;

namespace Codeshell.Abp.Attachments.Permissions
{
    public static class AttachmentsPermissions
    {
        public const string GroupName = "Attachments";

        public static string[] GetAll()
        {
            return ReflectionHelper.GetPublicConstantsRecursively(typeof(AttachmentsPermissions));
        }
    }
}