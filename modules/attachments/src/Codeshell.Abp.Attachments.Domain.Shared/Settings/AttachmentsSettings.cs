namespace Codeshell.Abp.Attachments.Settings
{
    public static class AttachmentsSettings
    {
        public const string GroupName = "Attachments";

        public const string DriveRootPath = "Attachments.DriveRootPath";
        public const string CleanupRequestsIntervalSeconds = "Attachments.CleanupRequestsIntervalSeconds";
        public const string MinimumChunkUploadSize = "Attachments.MinimumChunkUploadSize";

        /** Add constants for setting names. Example:
* public const string MySettingName = GroupName + ".MySettingName";
*/
    }
}