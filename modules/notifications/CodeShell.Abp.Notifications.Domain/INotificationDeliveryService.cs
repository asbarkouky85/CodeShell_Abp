﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace Codeshell.Abp.Notifications.Senders
{
    public interface INotificationDeliveryService
    {
        Task SendPendingMessages();
        Task RetryFailedMessages();

        Task SendMessages(IEnumerable<NotificationMessage> messages);
        Task<MessageDeliveryResult> SendMessage(NotificationMessage message);
    }
}