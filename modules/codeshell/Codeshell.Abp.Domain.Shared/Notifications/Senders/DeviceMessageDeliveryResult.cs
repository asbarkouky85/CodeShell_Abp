using Codeshell.Abp.Notifications.Senders;

namespace Codeshell.Abp.Notifications.Senders
{

    public class DeviceMessageDeliveryResult : MessageDeliveryResultBase
    {
        public string DeviceId { get; set; }

    }
}
