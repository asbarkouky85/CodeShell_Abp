using AutoMapper;
using Codeshell.Abp;
using Codeshell.Abp.Attachments.Categories;
using Codeshell.Abp.Attachments.Attachments;

namespace Codeshell.Abp.Attachments
{
    public class AttachmentsApplicationAutoMapperProfile : Profile
    {
        public AttachmentsApplicationAutoMapperProfile()
        {
            /* You can configure your AutoMapper mapping configuration here.
             * Alternatively, you can split your mapping configurations
             * into multiple profile classes for a better organization. */
            CreateMap<AttachmentCategory, AttachmentCategoryDto>()
                .ForMember(e => e.Name, e => e.MapFrom(d => CurrentCulture.Lang == "en" ? d.NameEn : d.NameAr));
            CreateMap<Dimension, DimensionDto>();
            CreateMap<UploadedFileInfo, UploadedFileInfoDto>();
        }
    }
}