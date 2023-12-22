using Codeshell.Abp.Devices;
using Codeshell.Abp.Extensions.EntityFramework;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Codeshell.Abp.EntityFrameworkCore.Devices
{
    public static class UserDevicesConfiguration
    {
        public static void ConfigureUserDevices(this ModelBuilder builder, string? prefix = null)
        {
            prefix = prefix ?? "Dev_";

            builder.Entity<UserDevice>(entity =>
            {
                entity.ToTable(prefix + "UserDevices");
                entity.ConfigureByConvention();
                entity.Property(e => e.DeviceId).IsUnicode().HasMaxLength(255);
            });
        }
    }
}
