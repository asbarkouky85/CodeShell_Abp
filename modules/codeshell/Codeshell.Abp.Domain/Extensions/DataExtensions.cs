using Codeshell.Abp.Contracts;
using Codeshell.Abp.Data;
using Codeshell.Abp.Extensions;
using Codeshell.Abp.Extensions.Linq;
using Codeshell.Abp.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Auditing;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Threading;

namespace Codeshell.Abp.Extensions.Data
{
    public static class DataExtensions
    {
        

        public static List<T> GetChangedItems<T>(this IEnumerable<T> lst) where T : class, IDetailObject
        {
            return lst.Where(e => e.IsChanged()).ToList();
        }

        public static bool IsChanged(this IDetailObject ob)
        {
            return ob.ChangeType == ChangeType.Added || ob.ChangeType == ChangeType.Deleted || ob.ChangeType == ChangeType.Updated;
        }

        public static async Task MergeAsyncAndUpdate<TEntity, TPrime>(
            this IRepository<TEntity, TPrime> repo,
            IEnumerable<TEntity> list) where TEntity : class, IEntity<TPrime>, IUpdateFrom<TEntity, TPrime>
        {
            var existingList = await repo.GetListAsync();
            foreach (var item in list)
            {
                var existing = existingList.FirstOrDefault(e => e.Id.Equals(item.Id));
                if (existing == null)
                {
                    await repo.InsertAsync(item);
                }
                else
                {
                    existing.UpdateFrom(item);
                }
            }
        }

        public static async Task MergeAsyncAndUpdate<TEntity, TPrime>(
            this IRepository<TEntity, TPrime> repo,
            IEnumerable<TEntity> list,
            Func<TEntity, Func<TEntity, bool>> finder,
            bool skipUpdateIfModified = true
            ) where TEntity : class, IEntity<TPrime>, IModificationAuditedObject, IUpdateFrom<TEntity, TPrime>
        {
            var existingList = await repo.GetListAsync();
            foreach (var item in list)
            {
                var existing = existingList.FirstOrDefault(finder(item));
                if (existing == null)
                {
                    await repo.InsertAsync(item);
                }
                else
                {
                    if (!skipUpdateIfModified || item.LastModifierId == null)
                        existing.UpdateFrom(item);
                }
            }
        }

        public static async Task MergeAsync<TEntity, TPrime>(
            this IRepository<TEntity, TPrime> repo,
            IEnumerable<TEntity> list,
            Action<TEntity, TEntity> updateDelegate = null) where TEntity : class, IEntity<TPrime>
        {
            var existingList = await repo.GetListAsync();
            foreach (var item in list)
            {
                var existing = existingList.FirstOrDefault(e => e.Id.Equals(item.Id));
                if (existing == null)
                {
                    await repo.InsertAsync(item);
                }
                else
                {
                    updateDelegate?.Invoke(existing, item);
                }
            }
        }

        public static async Task MergeAsync<TEntity>(
            this IRepository<TEntity> repo,
            IEnumerable<TEntity> list,
            Func<TEntity, Func<TEntity, bool>> finder,
            Action<TEntity, TEntity> updateDelegate = null) where TEntity : class, IEntity
        {
            var existingList = await repo.GetListAsync();
            foreach (var item in list)
            {
                var existing = existingList.FirstOrDefault(finder.Invoke(item));
                if (existing == null)
                {
                    await repo.InsertAsync(item);
                }
                else
                {
                    updateDelegate?.Invoke(existing, item);
                }
            }
        }

        public static List<TEntity> ApplyChangesWithDelegates<TEntity, TDto>(this ICollection<TEntity> repo,
            IEnumerable<TDto> items,
            IObjectMapper mapper,
            Func<TDto, TEntity> addFunction = null,
            Action<TDto, TEntity> updateFunction = null,
            Action<TEntity> deleteFunction = null)
            where TEntity : Entity<Guid>
            where TDto : class, IDetailObject<Guid>
        {
            return ApplyChangesWithDelegatesGeneric<TEntity, TDto, Guid>(repo, items, mapper, addFunction, updateFunction, deleteFunction);
        }

        public static List<TEntity> ApplyChangesLongWithDelegates<TEntity, TDto>(
            this ICollection<TEntity> repo,
            IEnumerable<TDto> items,
            IObjectMapper mapper,
            Func<TDto, TEntity> addFunction = null,
            Action<TDto, TEntity> updateFunction = null,
            Action<TEntity> deleteFunction = null)
            where TEntity : Entity<long>
            where TDto : class, IDetailObject<long>
        {
            return ApplyChangesWithDelegatesGeneric<TEntity, TDto, long>(repo, items, mapper, addFunction, updateFunction, deleteFunction);
        }

        public static List<TEntity> ApplyChangesIntWithDelegates<TEntity, TDto>(
            this ICollection<TEntity> repo,
            IEnumerable<TDto> items,
            IObjectMapper mapper,
            Func<TDto, TEntity> addFunction = null,
            Action<TDto, TEntity> updateFunction = null,
            Action<TEntity> deleteFunction = null)
           where TEntity : Entity<int>
           where TDto : class, IDetailObject<int>
        {
            return ApplyChangesWithDelegatesGeneric<TEntity, TDto, int>(repo, items, mapper, addFunction, updateFunction, deleteFunction);
        }

        public static List<TEntity> ApplyChangesInt<TEntity, TDto>(this ICollection<TEntity> repo, IEnumerable<TDto> items, IObjectMapper mapper)
            where TEntity : class, IEntity<int>
            where TDto : class, IDetailObject<int>
        {
            return ApplyChangesGeneric<TEntity, TDto, int>(repo, items, mapper);
        }

        public static List<TEntity> ApplyChangesLong<TEntity, TDto>(this ICollection<TEntity> repo, IEnumerable<TDto> items, IObjectMapper mapper)
            where TEntity : class, IEntity<long>
            where TDto : class, IDetailObject<long>
        {
            return ApplyChangesGeneric<TEntity, TDto, long>(repo, items, mapper);
        }

        public static List<TEntity> ApplyChanges<TEntity, TDto>(this ICollection<TEntity> repo, IEnumerable<TDto> items, IObjectMapper mapper)
            where TEntity : class, IEntity<Guid>
            where TDto : class, IDetailObject<Guid>
        {
            return ApplyChangesGeneric<TEntity, TDto, Guid>(repo, items, mapper);
        }

        public static List<TEntity> ApplyChangesNoId<TEntity, TDto>(this ICollection<TEntity> repo, IEnumerable<TDto> items, Func<TEntity, TDto, bool> finder, IObjectMapper mapper)
            where TEntity : class
            where TDto : class, IDetailObject
        {
            List<TEntity> lst = new List<TEntity>();
            foreach (var item in items)
            {
                switch (item.ChangeType)
                {
                    case ChangeType.Added:
                        var added = mapper.Map<TDto, TEntity>(item);
                        repo.Add(added);
                        lst.Add(added);
                        break;
                    case ChangeType.Updated:
                        var updated = repo.FirstOrDefault(e => finder(e, item));
                        if (updated != null)
                        {
                            mapper.Map(item, updated);
                            lst.Add(updated);
                        }
                        break;
                    case ChangeType.Deleted:
                        var deleted = repo.FirstOrDefault(e => finder(e, item));
                        repo.Remove(deleted);
                        break;
                    case ChangeType.Attached:
                        var noAction = mapper.Map<TDto, TEntity>(item);
                        lst.Add(noAction);
                        break;
                }
            }
            return lst;
        }


        public static List<TEntity> ApplyChangesGeneric<TEntity, TDto, TPrime>(this ICollection<TEntity> repo, IEnumerable<TDto> items, IObjectMapper mapper)
             where TEntity : class, IEntity<TPrime>
             where TDto : class, IDetailObject<TPrime>
        {
            List<TEntity> lst = new List<TEntity>();
            foreach (var item in items)
            {
                switch (item.ChangeType)
                {
                    case ChangeType.Added:
                        var added = mapper.Map<TDto, TEntity>(item);
                        repo.Add(added);
                        lst.Add(added);
                        break;
                    case ChangeType.Updated:
                        var updated = repo.FirstOrDefault(e => e.Id.Equals(item.Id));
                        if (updated != null)
                        {
                            mapper.Map(item, updated);
                            lst.Add(updated);
                        }
                        break;
                    case ChangeType.Deleted:
                        var deleted = repo.FirstOrDefault(e => e.Id.Equals(item.Id));
                        repo.Remove(deleted);
                        break;
                    case ChangeType.Attached:
                        var noAction = repo.FirstOrDefault(e => e.Id.Equals(item.Id));
                        lst.Add(noAction);
                        break;
                }
            }
            return lst;
        }

        internal static List<TEntity> ApplyChangesWithDelegatesGeneric<TEntity, TDto, TPrime>(
            this ICollection<TEntity> repo,
            IEnumerable<TDto> items,
            IObjectMapper mapper,
            Func<TDto, TEntity> addFunction = null,
            Action<TDto, TEntity> updateFunction = null,
            Action<TEntity> deleteFunction = null)
            where TEntity : Entity<TPrime>
            where TDto : class, IDetailObject<TPrime>
        {
            List<TEntity> lst = new List<TEntity>();
            foreach (var item in items)
            {
                switch (item.ChangeType)
                {
                    case ChangeType.Added:

                        if (addFunction != null)
                        {
                            var added = addFunction(item);
                            lst.Add(added);
                        }
                        else
                        {
                            var added = mapper.Map<TDto, TEntity>(item);
                            repo.Add(added);
                            lst.Add(added);
                        }

                        break;
                    case ChangeType.Updated:
                        var updated = repo.FirstOrDefault(e => e.Id.Equals(item.Id));

                        if (updated != null)
                        {
                            if (updateFunction != null)
                            {
                                updateFunction(item, updated);
                            }
                            else
                            {
                                mapper.Map(item, updated);
                            }

                            lst.Add(updated);
                        }
                        break;
                    case ChangeType.Deleted:
                        var deleted = repo.FirstOrDefault(e => e.Id.Equals(item.Id));
                        if (deleteFunction != null)
                        {
                            deleteFunction(deleted);
                        }
                        else
                        {
                            repo.Remove(deleted);
                        }

                        break;
                    case ChangeType.Attached:
                        var noAction = repo.FirstOrDefault(e => e.Id.Equals(item.Id));
                        lst.Add(noAction);
                        break;
                }
            }
            return lst;
        }


    }
}
