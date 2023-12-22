namespace Codeshell.Abp.Attachments
{
    public static class AttachmentsDbProperties
    {
        public static string DbTablePrefix { get; set; } = "Att_";

        public static string DbSchema { get; set; } = null;

        public const string ConnectionStringName = "Attachments";
    }
}
