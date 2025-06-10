namespace Codeshell.Abp.Lookups
{
    public interface IDescribedModel
    {
        long Id { get; set; }
        string Name { get; set; }
        string Description { get; set; }
    }
}
