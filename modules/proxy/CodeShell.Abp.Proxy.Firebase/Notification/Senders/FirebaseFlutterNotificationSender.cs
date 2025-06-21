using Codeshell.Abp.Extensions;
using Codeshell.Abp.Integration.Firebase.Flutter;
using Codeshell.Abp.Notifications;
using Codeshell.Abp.Notifications.Senders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Codeshell.Abp.Integration.Firebase.Notification.Senders
{
    public class FirebaseFlutterNotificationSender : ApplicationService, INotificationSender
    {
        IFirebaseNotificationService Service => LazyServiceProvider.LazyGetService<IFirebaseNotificationService>();

        public NotificationProviders ProviderId => NotificationProviders.FireBaseFlutter;

        public async Task<MessageDeliveryResult> SendAsync(NotificationMessageDeliveryDto deliveryData)
        {
            var result = new MessageDeliveryResult();
            foreach (var device in deliveryData.Devices)
            {
                var deviceResult = new DeviceMessageDeliveryResult();
                deviceResult.DeviceId = device.DeviceId;
                try
                {
                    var msgBody = new FirebaseFlutterRequest
                    {
                        registration_ids = new List<string> { device.ConnectionId },
                        content_available = true,
                        apnspriority = 5,
                        data = new
                        {
                            body = deliveryData.Body,
                            title = deliveryData.Title,
                            entityType = deliveryData.EntityType,
                            entityId = deliveryData.EntityId,
                            notificationId = deliveryData.NotificationId
                        }
                    };
                    deviceResult.RequestContent = msgBody.ToString();
                    var sendResult = await Service.SendNotificationToFlutter(msgBody);
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
