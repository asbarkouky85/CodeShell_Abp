using System.Collections.Generic;

namespace Codeshell.Abp.Emitters.Firebase
{
    public class FirebasePushResult 
    {
        public string multicast_id { get; set; }
        public string message_id { get; set; }
        public int success { get; set; }
        public int failure { get; set; }
        public int canonical_ids { get; set; }

        public IEnumerable<FirebaseResultItem> results { get; set; }
    }
}
