using System.Net.Http.Headers;
using System.Net.Http.Json;
using API.Data;
using API.Features.Authentication.Register;
using API.Features.Authentication.Responses;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shared.Responses;

namespace CinemaManagement.API.Tests.IntegrationTests;

public class IntegrationTest
{
    protected HttpClient TestClient = null!;
    protected const string AuthEmailAddress = "test1@example.com";
    protected const string AuthPassword = "testPassword12@";

    [SetUp]
    public void Setup()
    {
        var webApplicationFactory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // remove dataContext 
                    var descriptorsToRemove = services.Where(
                        d => d.ServiceType == typeof(DbContextOptions<CinemaDbContext>)).ToList();

                    foreach (var descriptor in descriptorsToRemove)
                    {
                        services.Remove(descriptor);
                    }

                    // replace dataContext with in-memory version
                    services.AddDbContext<CinemaDbContext>(options => { options.UseInMemoryDatabase("TestCinemaDB"); });
                });
            });
        TestClient = webApplicationFactory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("http://localhost/api/")
        });
    }

    /// <summary>
    /// Authenticate the current request.
    /// </summary>
    /// <remarks>
    /// You can specify the role by passing user or admin. If none is passed, it defaults to user.
    /// </remarks>
    /// <param name="userRole"></param>
    protected async Task AuthenticateAsync(string userRole = "User")
    {
        TestClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("bearer", await GetJwtAsync(userRole));
    }

    private async Task<string> GetJwtAsync(string userRole = "User")
    {
        var result = await RegisterAsync(userRole);
        return result?.Data?.AccessToken ?? throw new InvalidOperationException("Registration failed.");
    }

    protected async Task<ApiResponse<UserAuthResponse>?> RegisterAsync(string userRole = "User")
    {
        var registerResponse = await TestClient.PostAsJsonAsync("Auth/register",
            new RegisterRequest("test", "user", AuthEmailAddress, AuthPassword, userRole));

        var result = await registerResponse.Content.ReadFromJsonAsync<ApiResponse<UserAuthResponse>>();
        return result;
    }
}