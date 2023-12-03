using Shared.API;

namespace API.Extensions;

public static class MiddlewareExtensions
{
    public static void AddCore(this WebApplication app)
    {
        app.RegisterSwagger();
        app.RegisterMiddleware();
        // todo: seed database
    }

    private static void RegisterSwagger(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
        {
            return;
        }

        app.UseSwagger();
        app.UseSwaggerUI();
    }

    private static void RegisterMiddleware(this WebApplication app)
    {
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.UseMiddleware<ExceptionMiddleware>();
    }
}