using Codeshell.Abp.Emails;
using Codeshell.Abp.Extensions;
using System;
using System.Threading.Tasks;

namespace Codeshell.Abp.Notifications.Senders
{
    public class EmailNotificationSender : INotificationSender
    {
        public NotificationProviders ProviderId => NotificationProviders.Email;
        IEmailService emailService;

        public async Task<MessageDeliveryResult> SendAsync(NotificationMessageDeliveryDto deliveryData)
        {
            var result = new MessageDeliveryResult();
            try
            {
                var res = await emailService.SendEmail(deliveryData.User.Email, deliveryData.Title, deliveryData.Body, true, deliveryData.FromDisplayName ?? "no-reply");
                result.ResponseContent = res.ToJson();
                return result;
            }
            catch (Exception ex)
            {
                result.SetException(ex);
            }
            return result;
        }
    }
}
