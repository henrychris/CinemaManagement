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
            BaseAddress = new Uri("http://localhost")
        });
    }

    protected async Task AuthenticateAsync()
    {
        TestClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("bearer", await GetJwtAsync());
    }

    private async Task<string> GetJwtAsync()
    {
        var result = await RegisterAsync();
        return result?.Data?.AccessToken ?? throw new InvalidOperationException("Registration failed.");
    }

    protected async Task<ApiResponse<UserAuthResponse>?> RegisterAsync()
    {
        var registerResponse = await TestClient.PostAsJsonAsync("api/Auth/register",
            new RegisterRequest("test", "user", AuthEmailAddress, AuthPassword, "User"));

        var result = await registerResponse.Content.ReadFromJsonAsync<ApiResponse<UserAuthResponse>>();
        return result;
    }
}