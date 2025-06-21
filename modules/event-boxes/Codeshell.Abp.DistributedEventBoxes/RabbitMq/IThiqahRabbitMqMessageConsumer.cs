namespace Codeshell.Abp.DistributedEventBoxes.RabbitMq
{
    public interface IThiqahRabbitMqMessageConsumer
    {
        void InitializeQueue(bool quorum = false);
    }
}
