namespace Codeshell.Abp.Lookups
{
    public interface INamedModel
    {
        long Id { get; set; }
        string Name { get; set; }
    }
}
