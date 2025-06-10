using Codeshell.Abp.Notifications.Types;
using Microsoft.EntityFrameworkCore;

namespace Codeshell.Abp.Notifications.Providers
{
    public static class ProvidersEfConfiguration
    {
        public static void ConfigureCodeshellNotificationProviders(this ModelBuilder modelBuilder, string prefix = null, string schema = null)
        {
            modelBuilder.Entity<NotificationProvider>(entity =>
            {
                entity.ToTable(prefix + "NotificationProvider", schema);
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name).IsUnicode().HasMaxLength(255);

            });

        }
    }
}
