using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace Codeshell.Abp.Attachments.EntityFrameworkCore
{
    /* This class is needed for EF Core console commands
     * (like Add-Migration and Update-Database commands) */
    public class AttachmentsDbContextFactory : IDesignTimeDbContextFactory<AttachmentsDbContext>
    {
        public AttachmentsDbContext CreateDbContext(string[] args)
        {
            var configuration = BuildConfiguration();
            string connectionString = configuration.GetConnectionString("Attachments");
            if (connectionString == null)
                connectionString = configuration.GetConnectionString("Default");
            var builder = new DbContextOptionsBuilder<AttachmentsDbContext>()
                .UseSqlServer(connectionString);

            return new AttachmentsDbContext(builder.Options);
        }

        private static IConfigurationRoot BuildConfiguration()
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile($"appsettings.{env}.json", true);

            return builder.Build();
        }
    }
}
