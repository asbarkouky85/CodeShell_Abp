using AutoMapper;
using Codeshell.Abp.Extensions.Json;
using Codeshell.Abp.Extensions.Linq;
using Codeshell.Abp.Linq;
using Codeshell.Abp.Linq.Unique;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace Codeshell.Abp.Extensions
{
    public static class LinqApplicationExtensions
    {
        public static Task<PagedResultDto<BaseDto>> ToPagedResultDto<BaseEntity, BaseDto>(
           this IQueryable<BaseEntity> query, PagedResultRequestDto requestDto, Volo.Abp.ObjectMapping.IObjectMapper objectMapper)
           where BaseEntity : class
        {

            int totalCount = query.Count();
            var result = query.PageBy(requestDto.SkipCount, requestDto.MaxResultCount).ToList();

            var lstDtoItems = objectMapper.Map<List<BaseEntity>, List<BaseDto>>(result);

            PagedResultDto<BaseDto> pagedResultDto = new PagedResultDto<BaseDto>(totalCount, lstDtoItems);

            return Task.FromResult(pagedResultDto);

        }

        public static Task<PagedResultDto<TDto>> ToPageResultAsync<TEntity, TDto>(
           this IQueryable<TEntity> query, ICodeshellPagedRequest requestDto, IMapper objectMapper)
           where TEntity : class
            where TDto : class
        {

            int totalCount = query.Count();
            query = query.PageWith(requestDto);
            var lst = objectMapper.Map(query.ToList(), new List<TDto>());
            var res = new PagedResultDto<TDto>(totalCount, lst);
            return Task.FromResult(res);

        }

        public static Task<PagedResultDto<TDto>> ToProjectedPageResultAsync<TEntity, TDto>(
           this IQueryable<TEntity> query, ICodeshellPagedRequest requestDto, IMapper objectMapper)
           where TEntity : class
            where TDto : class
        {

            int totalCount = query.Count();
            query = query.PageWith(requestDto);

            var lst = objectMapper.ProjectTo<TDto>(query).ToList();
            var res = new PagedResultDto<TDto>(totalCount, lst);
            return Task.FromResult(res);
        }

        

    }
}
