using Hangfire;
using Hangfire.MemoryStorage;
using Hangfire.SqlServer;
using Microsoft.EntityFrameworkCore;

namespace Host;

public static class DatabaseExtentions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database");
        services.AddDbContext<DatabaseContext>(options =>
        {
            if (!string.IsNullOrEmpty(connectionString))
            {
                options.UseSqlServer(connectionString);

            }
            else
            {
                options.UseInMemoryDatabase("Database");
            }
        });

        services.AddHangfire(configuration =>
        {
            configuration.UseSerilogLogProvider();
            configuration.SetDataCompatibilityLevel(CompatibilityLevel.Version_170);
            configuration.UseSimpleAssemblyNameTypeSerializer();
            configuration.UseRecommendedSerializerSettings();

            if (!string.IsNullOrEmpty(connectionString))
            {
                configuration.UseSqlServerStorage(connectionString, new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.FromSeconds(10),
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true
                });
            }
            else
            {
                configuration.UseMemoryStorage();
            }
        });

        return services;
    }
}
