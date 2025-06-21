using Codeshell.Abp.Linq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace Codeshell.Abp.Repositories
{
    public interface ICodeshellNoKeyRepository<TEntity> : IRepository<TEntity>
        where TEntity : class, IEntity
    {
    }

    public interface ICodeshellRepository<TEntity, TPrime> : IRepository<TEntity, TPrime>, ICodeshellNoKeyRepository<TEntity>
        where TEntity : class, IEntity<TPrime>
    {
        Task<List<TResult>> GetAs<TResult>(Expression<Func<TEntity, TResult>> expression, Expression<Func<TEntity, bool>> filter = null);
        Task<List<TDto>> GetProjectedList<TDto>(Expression<Func<TEntity, bool>> filter = null) where TDto : class;
        Task<PagedResult<TDto>> GetProjected<TDto>(ICodeshellPagedRequest request, Expression<Func<TEntity, bool>> filter = null) where TDto : class;
        Task<TDto> GetSingleAs<TDto>(TPrime id) where TDto : class;
        Task<TDto> GetSingleAs<TDto>(TPrime id, Expression<Func<TEntity, TDto>> expression) where TDto : class;
        Task<TDto> GetSingleAs<TDto>(Expression<Func<TEntity, TDto>> expression, Expression<Func<TEntity, bool>> filter) where TDto : class;
    }
}
