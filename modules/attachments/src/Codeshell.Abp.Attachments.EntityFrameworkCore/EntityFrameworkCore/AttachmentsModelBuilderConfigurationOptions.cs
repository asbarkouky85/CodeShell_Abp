using JetBrains.Annotations;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Codeshell.Abp.Attachments.EntityFrameworkCore
{
    public class AttachmentsModelBuilderConfigurationOptions : AbpModelBuilderConfigurationOptions
    {
        public AttachmentsModelBuilderConfigurationOptions(
            [NotNull] string tablePrefix = "",
            [CanBeNull] string schema = null)
            : base(
                tablePrefix,
                schema)
        {

        }
    }
}