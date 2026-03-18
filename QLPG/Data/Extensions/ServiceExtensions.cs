using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QLPG_a.Data.Repositories;

namespace QLPG_a.Data.Extensions
{
    /// <summary>
    /// Extension methods để configure database và services
    /// </summary>
    public static class ServiceExtensions
    {
        /// <summary>
        /// Thêm ApplicationDbContext vào DI container
        /// </summary>
        public static IServiceCollection AddApplicationDbContext(
            this IServiceCollection services, 
            IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                if (IsSqliteConnection(connectionString))
                {
                    options.UseSqlite(connectionString);
                    return;
                }

                options.UseSqlServer(
                    connectionString,
                    sqlOptions =>
                    {
                        // Retry on failure
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(10),
                            errorNumbersToAdd: null);

                        // Command timeout
                        sqlOptions.CommandTimeout(30);
                    }
                );
            });

            return services;
        }

        private static bool IsSqliteConnection(string? connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                return false;
            }

            return connectionString.Contains("Data Source=", StringComparison.OrdinalIgnoreCase)
                || connectionString.EndsWith(".db", StringComparison.OrdinalIgnoreCase)
                || connectionString.EndsWith(".sqlite", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Thêm In-Memory Repositories vào DI container
        /// </summary>
        public static IServiceCollection AddInMemoryRepositories(this IServiceCollection services)
        {
            return services;
        }

        /// <summary>
        /// Khởi tạo database với seeds data (Data mẫu)
        /// </summary>
        public static IServiceProvider InitializeDatabase(this IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var isSqlite = string.Equals(
                    context.Database.ProviderName,
                    "Microsoft.EntityFrameworkCore.Sqlite",
                    StringComparison.OrdinalIgnoreCase);

                if (isSqlite)
                {
                    try
                    {
                        // Validate schema quickly; if stale/broken, recreate below.
                        _ = context.Members.Any();
                        DbInitializer.Initialize(context);
                    }
                    catch
                    {
                        context.Database.EnsureDeleted();
                        DbInitializer.Initialize(context);
                    }

                    return serviceProvider;
                }

                // SQL Server flow
                DbInitializer.ApplyMigrations(context);
                DbInitializer.Initialize(context);
            }

            return serviceProvider;
        }
    }
}
