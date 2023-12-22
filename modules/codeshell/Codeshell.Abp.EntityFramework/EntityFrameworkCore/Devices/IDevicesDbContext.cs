using Codeshell.Abp.Devices;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.EntityFrameworkCore;

namespace Codeshell.Abp.EntityFrameworkCore.Devices
{
    public interface IDevicesDbContext : IEfCoreDbContext
    {
        DbSet<UserDevice> UserDevices { get; set; }
    }
}
