using Codeshell.Abp.Notifications.Devices;
using Codeshell.Abp.Notifications.Providers;
using Codeshell.Abp.Notifications.Types;
using Codeshell.Abp.Notifications.Users;
using Microsoft.EntityFrameworkCore;

namespace Codeshell.Abp.Notifications
{
    public partial class NotificationsContext : CodeshellDbContext<NotificationsContext>, IDevicesDbContext
    {

        public NotificationsContext(DbContextOptions<NotificationsContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Notification> Notifications { get; set; }

        public virtual DbSet<User> Users { get; set; }
        public DbSet<UserDevice> UserDevices { get; set; }

        //public virtual DbSet<UserDevice> UserDevices { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ConfigureCodeshellDevices(null, "Note");
            modelBuilder.ConfigureCodeshellNotifications(null, "Note");
            modelBuilder.ConfigureCodeshellNotificationTypes(null, "Note");
            modelBuilder.ConfigureCodeshellNotificationProviders(null, "Note");
        }
    }
}
