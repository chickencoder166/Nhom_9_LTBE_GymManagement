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
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
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
                ));

            return services;
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
                
                // Áp dụng Migration hiện tại
                DbInitializer.ApplyMigrations(context);
                
                // Seed data
                DbInitializer.Initialize(context);
            }

            return serviceProvider;
        }
    }
}
