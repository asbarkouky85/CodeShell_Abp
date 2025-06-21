using Codeshell.Abp.Extensions;
using Codeshell.Abp.Notifications;
using Codeshell.Abp.Notifications.Senders;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Codeshell.Abp.Integration.Firebase;
using Codeshell.Abp.Integration.Firebase.Results;

namespace Codeshell.Abp.Integration.Firebase.Notification.Senders
{
    public class FirebaseNotificationSender : ApplicationService, INotificationSender
    {
        IFirebaseNotificationService Service => LazyServiceProvider.LazyGetService<FirebaseNotificationService>();

        public NotificationProviders ProviderId => NotificationProviders.FireBase;

        public async Task<MessageDeliveryResult> SendAsync(NotificationMessageDeliveryDto deliveryData)
        {
            var result = new MessageDeliveryResult();
            foreach (var device in deliveryData.Devices)
            {
                var deviceResult = new DeviceMessageDeliveryResult();
                try
                {
                    var msgBody = new FirebaseRequest
                    {
                        to = device.ConnectionId,
                        notification = new FirebaseMessage
                        {
                            body = deliveryData.Body,
                            title = deliveryData?.Title,
                        },
                        data = new
                        {
                            entityType = deliveryData.EntityType,
                            entityId = deliveryData.EntityId,
                            notificationId = deliveryData.NotificationId
                        }
                    };
                    deviceResult.RequestContent = msgBody.ToString();
                    var sendResult = await Service.SendNotification(msgBody);
                    deviceResult.ResponseContent = sendResult.ToString();
                    deviceResult.IsSuccess = sendResult.IsSuccess;

                }
                catch (Exception ex)
                {
                    deviceResult.Exception = ex.GetMessageAndStack();
                    deviceResult.SetException(ex);
                }
                result.DeviceResults.Add(deviceResult);

            }
            result.IsSuccess = result.DeviceResults.Any(e => e.IsSuccess);
            return result;
        }
    }
}
