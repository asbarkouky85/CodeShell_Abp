using Codeshell.Abp.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Codeshell.Abp.Emitters.Firebase
{
    public class FirebaseNotificationService : HttpService
    {
        private FirebaseConfig Configuration;

        public FirebaseNotificationService()
        {
            Configuration = new FirebaseConfig();
            BaseUrl = Configuration.ServerUrl ?? "https://fcm.googleapis.com/fcm";
            Headers["Authorization"] = "key=" + Configuration.ApiKey;
            if (Configuration.SenderId != null)
                Headers["Sender"] = "id=" + Configuration.SenderId;
        }

        public async Task<FirebasePushResult> SendNotification(FirebaseRequest data)
        {
            return await PostAsyncAs<FirebasePushResult>("send", data);
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

            return await PostAsyncAs<FirebasePushResult>("send", req);
        }



        public string Lang { get; set; }


    }
}
