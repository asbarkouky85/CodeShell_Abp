using Microsoft.EntityFrameworkCore;

namespace Codeshell.Abp.HealthCheck
{
    public class HealthCheckDbContext : CodeshellDbContext<HealthCheckDbContext>
    {
        public DbSet<CheckItem> CheckItems { get; set; }

        public HealthCheckDbContext(DbContextOptions<HealthCheckDbContext> opts) : base(opts)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CheckItem>(e =>
            {
                e.Property(e => e.ServiceName).HasMaxLength(100);
                e.Property(e => e.Host).HasMaxLength(100);
                e.Property(e => e.Response);
            });
        }
    }
}
