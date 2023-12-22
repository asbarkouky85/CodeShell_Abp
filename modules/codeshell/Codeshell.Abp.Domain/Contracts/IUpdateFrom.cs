using Volo.Abp.Domain.Entities;

namespace Codeshell.Abp.Contracts
{
    public interface IUpdateFrom<T, TPrime> : IEntity<TPrime>
    {
        void UpdateFrom(T model);
    }
}
