using Codeshell.Abp.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Codeshell.Abp.Integration.Firebase;
using Codeshell.Abp.Integration.Firebase.Flutter;
using Codeshell.Abp.Integration.Firebase.Results;

namespace Codeshell.Abp.Integration.Firebase
{
    public class FirebaseNotificationService : HttpService, IFirebaseNotificationService
    {

        private FirebaseOptions Configuration;

        public FirebaseNotificationService(IOptions<FirebaseOptions> options)
        {
            BaseUrl = Configuration.ServerUrl ?? "https://fcm.googleapis.com/fcm";
            Configuration = options.Value;
            Headers["Authorization"] = "key=" + Configuration.ApiKey;
            if (Configuration.SenderId != null)
                Headers["Sender"] = "id=" + Configuration.SenderId;
        }

        public async Task<FirebasePushResult> SendNotificationToFlutter(FirebaseFlutterRequest request)
        {
            FirebasePushResult res = new FirebasePushResult();

            try
            {
                res = await PostAsyncAs<FirebasePushResult>("send", request);
            }
            catch (Exception ex)
            {
                res.SetException(ex);
            }
            return res;

        }

        public async Task<FirebasePushResult> SendNotification(FirebaseRequest data)
        {
            FirebasePushResult res = new FirebasePushResult();

            try
            {
                res = await PostAsyncAs<FirebasePushResult>("send", data);
            }
            catch (Exception ex)
            {
                res.SetException(ex);
            }
            return res;

        }

        public async Task<FirebasePushResult> SendNotification(FirebaseMessage message, string to = null, string[] topics = null, object data = null)
        {
            var req = new FirebaseRequest
            {
                notification = message,
                data = data,
                to = to
            };

            if (topics != null)
            {
                List<string> conds = new List<string>();
                foreach (var c in topics)
                {
                    conds.Add("'" + c + "' in topics");
                }
                req.condition = string.Join("||", conds);
            }

            FirebasePushResult res = new FirebasePushResult();

            try
            {
                res = await PostAsyncAs<FirebasePushResult>("send", req);
            }
            catch (Exception ex)
            {
                res.success = 0;
                res.failure = 1;
                res.SetException(ex);
            }
            return res;
        }



        public string Lang { get; set; }


    }
}
