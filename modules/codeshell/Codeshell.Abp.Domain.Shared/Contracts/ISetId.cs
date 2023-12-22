namespace Codeshell.Abp.Contracts
{
    public interface ISetId<in TPrime>
    {
        void SetId(TPrime id);
    }
}
