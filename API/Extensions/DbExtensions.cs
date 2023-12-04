using System.Diagnostics;
using API.Data;
using API.Models.Domain;
using API.Models.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Options;
using Shared.Configuration;

namespace API.Extensions;

public static class DbExtensions
{
    private const string InMemoryProviderName = "Microsoft.EntityFrameworkCore.InMemory";

    public static void SetupDatabase<T>(this IServiceCollection services) where T : DbContext
    {
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        Console.WriteLine($"Current Environment: {env}");
        string connectionString;

        if (env == Environments.Development)
        {
            var dbSettings = services.BuildServiceProvider().GetService<IOptionsSnapshot<DatabaseSettings>>()?.Value;
            connectionString = dbSettings!.ConnectionString!;
        }
        else if (env == Environments.Production)
        {
            // Use connection string provided at runtime by Fly.
            connectionString = GetProdPostgresConnectionString();
            Console.WriteLine($"ConnectionString: {connectionString}");
        }
        else
        {
            // when running integration tests
            return;
        }

        services.AddDbContext<T>(options =>
        {
            options.UseNpgsql(connectionString, o => o.MigrationsHistoryTable(
                tableName: HistoryRepository.DefaultTableName, typeof(T).Name));
        });


        var dbContext = services.BuildServiceProvider().GetRequiredService<T>();
        if (env == Environments.Development)
        {
            dbContext.Database.EnsureCreated();
        }

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

    public static void SeedDatabase(this WebApplication app)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        Console.WriteLine("Database seeding starting.");
        SeedDatabaseInternal(app);

        stopwatch.Stop();
        var elapsedTime = stopwatch.Elapsed;
        Console.WriteLine($"Database seeding completed in {elapsedTime.TotalMilliseconds}ms.");
    }

    private static void SeedDatabaseInternal(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<CinemaDbContext>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        if (IsInMemoryDatabase(context))
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }
        else
        {
            context.Database.Migrate();
        }

        SeedRoles(roleManager);
        context.SaveChanges();
    }

    private static bool IsInMemoryDatabase(DbContext context)
    {
        return context.Database.ProviderName == InMemoryProviderName;
    }

    private static void SeedRoles(RoleManager<IdentityRole> roleManager)
    {
        var roles = new[] { UserRoles.User, UserRoles.Admin };

        foreach (var role in roles)
        {
            roleManager.CreateAsync(new IdentityRole(role));
        }

        Console.WriteLine("Role seeding complete.");
    }
}