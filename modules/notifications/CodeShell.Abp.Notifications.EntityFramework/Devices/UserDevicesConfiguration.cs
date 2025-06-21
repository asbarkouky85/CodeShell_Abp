using Microsoft.EntityFrameworkCore;
using Codeshell.Abp.Notifications.Users;

namespace Codeshell.Abp.Notifications.Devices
{
    public static class UserDevicesConfiguration
    {
        public static void ConfigureCodeshellDevices(this ModelBuilder modelBuilder, string prefix = null, string schema = null)
        {
            modelBuilder.Entity<UserDevice>(entity =>
            {
                entity.ToTable(prefix + "UserDevices", schema);

                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.Property(e => e.DeviceId).IsUnicode().HasMaxLength(255);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable(prefix + "Users", schema);
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Mobile).IsUnicode(false);

                entity.Property(e => e.PreferredLanguage).IsUnicode(false);
            });

        }
    }
}
