using AutoMapper;
using Codeshell.Abp.Attachments;
using Codeshell.Abp.Contracts;
using Codeshell.Abp.Files.Uploads;
using System;
using System.Linq.Expressions;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace Codeshell.Abp.Extensions
{
    public static class MappingExtensions
    {

        public static IMappingExpression<TSrc, TDest> MapLocalizable<TSrc, TDest>(this IMappingExpression<TSrc, TDest> exp)
            where TSrc : class, ITwoLanguage
            where TDest : class, INamed
        {
            return exp.ForMember(e => e.Name, e =>
            {
                e.MapFrom(n => CurrentCulture.Lang == "en" ? n.NameEn : n.NameAr);
            });
        }

        public static IMappingExpression<TSrc, TDest> IgnoreMember<TSrc, TDest, TVal>(this IMappingExpression<TSrc, TDest> exp, Expression<Func<TDest, TVal>> memberExpression)
            where TSrc : class
            where TDest : class
        {
            return exp.ForMember(memberExpression, e => e.Ignore());
        }

        public static IMappingExpression<TEntity, TDto> MapChangeType<TEntity, TDto>(this IMappingExpression<TEntity, TDto> exp)
            where TDto : class, IDetailObject
        {
            return exp.ForMember(e => e.ChangeType, e => e.MapFrom(n => ChangeType.Attached));
        }

        public static IMappingExpression<TDto, TEntity> IgnoreId<TEntity, TDto>(this IMappingExpression<TDto, TEntity> exp)
            where TDto : class, IEntityDto<Guid>
            where TEntity : class, IEntity<Guid>
        {
            return exp.ForMember(e => e.Id, e => e.Ignore());
        }

        public static IMappingExpression<TDto, TEntity> IgnoreIdInt<TEntity, TDto>(this IMappingExpression<TDto, TEntity> exp)
            where TDto : class, IEntityDto<int>
            where TEntity : class, IEntity<int>
        {
            return exp.ForMember(e => e.Id, e => e.Ignore());
        }

        public static IMappingExpression<TDto, TEntity> IgnoreIdLong<TEntity, TDto>(this IMappingExpression<TDto, TEntity> exp)
            where TDto : class, IEntityDto<long>
            where TEntity : class, IEntity<long>
        {
            return exp.ForMember(e => e.Id, e => e.Ignore());
        }

        public static IMappingExpression<TDto, TEntity> MapFileDto<TDto, TEntity>(this IMappingExpression<TDto, TEntity> exp)
            where TDto : class, IHasFileDto
            where TEntity : class, IHasAttachmentEntity
        {
            return exp.ForMember(e => e.AttachmentId, e => e.MapFrom(d => d.File.Id));
        }

        public static IMappingExpression<TDto, TEntity> MapFileDto<TDto, TEntity>(this IMappingExpression<TDto, TEntity> exp, Expression<Func<TEntity, Guid>> attachmentExp)
            where TDto : class, IHasFileDto
            where TEntity : class
        {
            return exp.ForMember(attachmentExp, e => e.MapFrom(d => d.File.Id));
        }

        public static IMappingExpression<TDto, TEntity> MapOpFileDto<TDto, TEntity>(this IMappingExpression<TDto, TEntity> exp)
            where TDto : class, IHasFileDto
            where TEntity : class, IHasOptionalAttachmentEntity
        {
            return exp.ForMember(e => e.AttachmentId, e => e.MapFrom(d => d.File.Id));
        }

        public static IMappingExpression<TEntity, TDto> MapFile<TEntity, TDto>(this IMappingExpression<TEntity, TDto> exp, int type)
            where TDto : class, IHasFileDto
            where TEntity : class, IHasAttachmentEntity
        {
            return exp.ForMember(e => e.File, e => e.MapFrom(d => new TempFileDto
            {
                Id = d.AttachmentId == Guid.Empty ? null : (Guid?)d.AttachmentId,
                AttachmentTypeId = (int)type
            }));
        }

        public static IMappingExpression<TEntity, TDto> MapFile<TEntity, TDto>(this IMappingExpression<TEntity, TDto> exp, Expression<Func<TEntity, TempFileDto>> fileExp)
            where TDto : class, IHasFileDto
            where TEntity : class
        {
            return exp.ForMember(e => e.File, e => e.MapFrom(fileExp));
        }

        public static IMappingExpression<TEntity, TDto> MapOpFile<TEntity, TDto>(this IMappingExpression<TEntity, TDto> exp, int type)
            where TDto : class, IHasFileDto
            where TEntity : class, IHasOptionalAttachmentEntity
        {
            return exp.ForMember(e => e.File, e => e.MapFrom(d => new TempFileDto
            {
                Id = d.AttachmentId == Guid.Empty ? null : (Guid?)d.AttachmentId,
                AttachmentTypeId = (int)type
            }));
        }
    }
}
