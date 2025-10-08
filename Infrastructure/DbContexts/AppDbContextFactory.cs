using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DbContexts
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var basePath = Directory.GetCurrentDirectory();

            var cfgBuilder = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddJsonFile(Path.Combine(basePath, "..", "API", "appsettings.json"), optional: true)
                .AddJsonFile(Path.Combine(basePath, "..", "API", "appsettings.Development.json"), optional: true)
                .AddEnvironmentVariables();

            var config = cfgBuilder.Build();
            var conn = config.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(conn)) throw new InvalidOperationException("Falta ConnectionStrings:DefaultConnection (design-time).");

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
                .UseNpgsql(conn, npg =>
                {
                    npg.MigrationsAssembly("Infrastructure");
                });

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
