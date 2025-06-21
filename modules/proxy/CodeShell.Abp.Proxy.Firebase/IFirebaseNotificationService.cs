using System.Threading.Tasks;
using Codeshell.Abp.Integration.Firebase;
using Codeshell.Abp.Integration.Firebase.Flutter;
using Codeshell.Abp.Integration.Firebase.Results;

namespace Codeshell.Abp.Integration.Firebase
{
    public interface IFirebaseNotificationService
    {
        string Lang { get; set; }

        Task<FirebasePushResult> SendNotification(FirebaseMessage message, string to = null, string[] topics = null, object data = null);
        Task<FirebasePushResult> SendNotification(FirebaseRequest data);
        Task<FirebasePushResult> SendNotificationToFlutter(FirebaseFlutterRequest request);
    }
}
