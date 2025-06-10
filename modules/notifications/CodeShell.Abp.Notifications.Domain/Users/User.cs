
using Codeshell.Abp.Notifications;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities.Auditing;

namespace CodeshellCore.Notifications.Users
{
    [Table("Users", Schema = "Note")]
    public partial class User : AuditedEntity<long>
    {
        public User()
        {
            Notifications = new HashSet<Notification>();
        }

        [StringLength(150)]
        public string Name { get; set; }
        [StringLength(150)]
        public string Email { get; set; }

        [StringLength(50)]
        public string Mobile { get; set; }
        public int UserType { get; set; }
        public long? PersonId { get; set; }
        [StringLength(3)]
        public string PreferredLanguage { get; set; }

        [InverseProperty("User")]
        public ICollection<Notification> Notifications { get; set; }

    }
}
