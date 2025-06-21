namespace Codeshell.Abp.DistributedEventBoxes.RabbitMq
{
    public interface ICodeshellRabbitMqMessageConsumer
    {
        void InitializeQueue(bool quorum = false);
    }
}
