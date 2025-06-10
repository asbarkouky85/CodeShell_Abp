using System;

namespace Codeshell.Abp.Notifications.Senders
{
    public class UserMessageDeliveryData
    {
        public Guid? UserId { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Name { get; set; }
    }
}
