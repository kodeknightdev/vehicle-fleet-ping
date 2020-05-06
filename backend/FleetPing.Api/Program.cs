using System;
using System.Threading;
using FleetPing.Api.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace FleetPing.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
                var db = scope.ServiceProvider.GetRequiredService<FleetContext>();

                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

                if (config.GetValue("MigrateOnStartup", true))
                {
                    MigrateWithRetry(db, logger);
                }

                if (config.GetValue<bool>("SeedDemoData"))
                {
                    DevSeeder.Seed(db);
                }
            }

            host.Run();
        }

        /// <summary>
        /// In docker-compose the database container can take a few seconds to accept connections.
        /// </summary>
        private static void MigrateWithRetry(FleetContext db, ILogger logger, int attempts = 10)
        {
            for (var i = 1; ; i++)
            {
                try
                {
                    db.Database.Migrate();
                    return;
                }
                catch (Exception ex) when (i < attempts && (ex is NpgsqlException || ex.InnerException is NpgsqlException))
                {
                    logger.LogWarning("Database not ready (attempt {Attempt}/{Max}), retrying...", i, attempts);
                    Thread.Sleep(TimeSpan.FromSeconds(3));
                }
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
