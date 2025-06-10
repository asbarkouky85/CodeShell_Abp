using Codeshell.Abp.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.EntityFrameworkCore;

namespace Codeshell.Abp
{
    public abstract class CodeshellDbContext<TDbContext> : AbpDbContext<TDbContext>, ICaptureSqlDbContext where TDbContext : DbContext
    {
        private Dictionary<string, object> _items = new Dictionary<string, object>();
        public CodeshellDbContext(DbContextOptions<TDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            var interceptor = new SqlCommandInterceptor();
            AddItem(interceptor);
            optionsBuilder.AddInterceptors(interceptor);
        }

        protected void AddItem(string key, object value)
        {
            _items[key] = value;
        }

        protected void AddItem<T>(T value)
        {
            _items[typeof(T).GUID.ToString()] = value;
        }

        protected T? GetItem<T>() where T : class
        {
            if (_items.TryGetValue(typeof(T).GUID.ToString(), out object? result))
                return (T)result;

            return null;
        }

        public void StartCapturing()
        {
            GetItem<SqlCommandInterceptor>()?.StartCapturing();
        }

        public List<string> GetCapturedCommands()
        {
            var interceptor = GetItem<SqlCommandInterceptor>();
            if (interceptor != null)
                return interceptor.GetCommands();
            return new List<string>();
        }

        public void StopCapturing()
        {
            GetItem<SqlCommandInterceptor>()?.StopCapturing();
        }
    }
}
