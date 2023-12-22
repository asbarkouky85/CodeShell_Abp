using AutoMapper;
using Codeshell.Abp.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Volo.Abp.AutoMapper;

namespace Codeshell.Abp.Mapping
{
    public class AutoMapperQueryProjector : IQueryProjector
    {
        private IMapper _mapper;
        public AutoMapperQueryProjector(IMapperAccessor mapper)
        {
            _mapper = mapper.Mapper;
        }

        public virtual IQueryable<TDestination> Project<TSource, TDestination>(IQueryable<TSource> query)
        {
            return _mapper.ProjectTo<TDestination>(query);
        }
    }
}
