using System.Text;
using API.Data;
using API.Models.Domain;
using API.Models.Enums;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Shared.Configuration;
using Shared.Filters;

namespace API.Extensions;

public static class StartupExtensions
{
    public static void AddCore(this IServiceCollection services)
    {
        services.SetupConfigFiles();
        services.SetupDatabase<CinemaDbContext>();
        services.SetupControllers();
        services.SetupSwagger();
        services.SetupFilters();
        services.SetupMsIdentity();
        services.SetupAuthentication();
        services.RegisterServices();
    }

    private static void ConfigureSettings<T>(IServiceCollection services, IConfiguration? configuration)
        where T : class, new()
    {
        services.Configure<T>(options => configuration?.GetSection(typeof(T).Name).Bind(options));
    }

    private static void SetupConfigFiles(this IServiceCollection services)
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .AddEnvironmentVariables()
            .Build();

        Console.WriteLine($"{configuration.AsEnumerable().Count()} secrets retrieved.");

        ConfigureSettings<DatabaseSettings>(services, configuration);
        ConfigureSettings<JwtSettings>(services, configuration);
        Console.WriteLine("Secrets have been bound to classes.");
    }

    private static void SetupControllers(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddRouting(options => options.LowercaseUrls = true);
        services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);
    }

    private static void SetupSwagger(this IServiceCollection services)
    {
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        // setup swagger to accept bearer tokens
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description =
                    "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });
        });
    }

    private static void SetupFilters(this IServiceCollection services)
    {
        services.AddScoped<CustomValidationFilter>();
    }

    private static void SetupMsIdentity(this IServiceCollection services)
    {
        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 6;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                options.User.RequireUniqueEmail = true;
                // options.SignIn.RequireConfirmedEmail = true;
            }).AddEntityFrameworkStores<CinemaDbContext>()
            .AddDefaultTokenProviders();

        // passwords only last 2 hours.
        services.Configure<DataProtectionTokenProviderOptions>(opt =>
            opt.TokenLifespan = TimeSpan.FromHours(2));
    }

    private static void SetupAuthentication(this IServiceCollection services)
    {
        using var scope = services.BuildServiceProvider().CreateScope();
        var jwtSettings = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<JwtSettings>>().Value;

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(
                x =>
                {
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidAudience = jwtSettings.Audience ??
                                        throw new InvalidOperationException("Audience is null!"),
                        ValidIssuer = jwtSettings.Issuer ??
                                      throw new InvalidOperationException("Security Key is null!"),
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey ??
                            throw new InvalidOperationException("Security Key is null!"))),
                        ValidateAudience = true,
                        ValidateIssuer = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        RoleClaimType = JwtClaims.Role
                    };
                });
        services.AddAuthorization();
    }

    private static void RegisterServices(this IServiceCollection services)
    {
    }
}