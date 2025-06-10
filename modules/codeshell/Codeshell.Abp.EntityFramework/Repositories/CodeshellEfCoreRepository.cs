using Codeshell.Abp.Extensions.Linq;
using Codeshell.Abp.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.MultiTenancy;

namespace Codeshell.Abp.Repositories
{
    public class CodeshellEfCoreRepository<TDbContext, TEntity, TPrime> : EfCoreRepository<TDbContext, TEntity, TPrime>, ICodeshellRepository<TEntity, TPrime>
        where TDbContext : IEfCoreDbContext
        where TEntity : class, IEntity<TPrime>
    {
        private IQueryProjector? _projector;
        protected IQueryProjector Projector
        {
            get
            {
                if (_projector == null)
                    _projector = LazyServiceProvider.LazyGetRequiredService<IQueryProjector>();

                return _projector;
            }
        }

        public CodeshellEfCoreRepository(IDbContextProvider<TDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }

        public async Task<PagedResult<TDto>> GetProjected<TDto>(ICodeshellPagedRequest request, Expression<Func<TEntity, bool>>? filter = null) where TDto : class
        {
            var q = (await GetDbSetAsync()).AsQueryable();
            if (filter != null)
                q = q.Where(filter);

            return await GetProjectedPagedResultAsync<TDto>(q, request);
        }

        protected async Task<PagedResult<TDto>> GetProjectedPagedResultAsync<TDto>(IQueryable<TEntity> q, ICodeshellPagedRequest req) where TDto : class
        {
            var dtoQuery = Projector.Project<TEntity, TDto>(q).PageWith(req);

            return new PagedResult<TDto>
            {
                TotalCount = await q.CountAsync(),
                Items = await dtoQuery.ToListAsync()
            };
        }

        protected async Task<PagedResult<TDto>> GetProjectedPagedResultFromOtherEntityAsync<TOther, TDto>(IQueryable<TOther> q, ICodeshellPagedRequest req) where TOther : class where TDto : class
        {
            var dtoQuery = Projector.Project<TOther, TDto>(q);
            return await dtoQuery.ToPagedResult(req);
        }

        public async Task<TDto> GetSingleProjected<TDto>(TPrime id) where TDto : class
        {
            var q = (await GetDbSetAsync()).Where(e => e.Id.Equals(id));
            var dtoQuery = Projector.Project<TEntity, TDto>(q);
            return await dtoQuery.FirstOrDefaultAsync();
        }

        public async Task<List<TDto>> GetProjectedList<TDto>(Expression<Func<TEntity, bool>> filter = null) where TDto : class
        {
            var context = await GetDbContextAsync();
            //var ten = context.GetService<ICurrentTenant>();
            var q = (await GetDbSetAsync()).AsQueryable();
            if (filter != null)
                q = q.Where(filter);
            var dtoQuery = Projector.Project<TEntity, TDto>(q);

            return await dtoQuery.ToListAsync();
        }

        public async Task<List<TResult>> GetAs<TResult>(Expression<Func<TEntity, TResult>> expression, Expression<Func<TEntity, bool>> filter = null)
        {
            var q = (await GetDbSetAsync()).AsQueryable();
            if (filter != null)
                q = q.Where(filter);
            return await q.Select(expression).ToListAsync();
        }
    }
}
