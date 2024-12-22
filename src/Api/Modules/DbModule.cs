using Infrastructure.Persistence;

namespace Api.Modules;

public static class DbModule
{
    public static async Task InitialiseDb(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var initializer = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();
        await initializer.InitializeAsync();
    }
}