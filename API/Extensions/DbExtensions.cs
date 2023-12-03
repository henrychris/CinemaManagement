using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Options;
using Shared.Configuration;

namespace API.Extensions;

public static class DbExtensions
{
    public static void SetupDatabase<T>(this IServiceCollection services) where T : DbContext
    {
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        string connectionString;

        if (env == Environments.Development)
        {
            var dbSettings = services.BuildServiceProvider().GetService<IOptionsSnapshot<DatabaseSettings>>()?.Value;
            connectionString = dbSettings!.ConnectionString!;
        }
        else
        {
            // Use connection string provided at runtime by Fly.
            connectionString = GetProdPostgresConnectionString();
            Console.WriteLine($"ConnectionString: {connectionString}");
        }

        services.AddDbContext<T>(options =>
        {
            options.UseNpgsql(connectionString, o => o.MigrationsHistoryTable(
                tableName: HistoryRepository.DefaultTableName, typeof(T).Name));
        });


        var dbContext = services.BuildServiceProvider().GetRequiredService<T>();
        dbContext.Database.Migrate();
    }

    private static string GetProdPostgresConnectionString()
    {
        var connUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
        ArgumentException.ThrowIfNullOrEmpty(connUrl);
        Console.WriteLine($"ConnUrl: {connUrl}");

        // Parse connection URL to connection string for Npgsql
        connUrl = connUrl.Replace("postgresql://", string.Empty);
        var pgUserPass = connUrl.Split("@")[0];
        var pgHostPortDb = connUrl.Split("@")[1];
        var pgHostPort = pgHostPortDb.Split("/")[0];
        var pgDb = pgHostPortDb.Split("/")[1];
        var pgUser = pgUserPass.Split(":")[0];
        var pgPass = pgUserPass.Split(":")[1];
        var pgHost = pgHostPort.Split(":")[0];
        var pgPort = pgHostPort.Split(":")[1];
        var updatedHost = pgHost.Replace("flycast", "internal");

        return $"Server={updatedHost};Port={pgPort};User Id={pgUser};Password={pgPass};Database={pgDb};";
    }
}