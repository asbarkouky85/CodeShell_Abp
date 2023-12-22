using AutoMapper;
using Codeshell.Abp.Linq;
using Codeshell.Abp.Reporting;
using Volo.Abp.Application.Dtos;

namespace Codeshell.Abp
{
    public class CodeshellAutoMapperProfile : Profile
    {
        public CodeshellAutoMapperProfile()
        {
            CreateMap(typeof(PagedResult<>), typeof(PagedResultDto<>));
            CreateMap<PagedPeriodRequestDto, PagedPeriodRequest>();
        }
    }
}
