using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Codeshell.Abp.Data
{
    public interface ICodeshellDbSchemaMigrator
    {
        Task MigrateAsync();
    }
}
