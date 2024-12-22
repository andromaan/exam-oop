using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Xunit;

namespace Tests.Common;

public abstract class BaseIntegrationTest : IClassFixture<IntegrationTestWebFactory>
{
    protected readonly ApplicationDbContext Context;
    protected readonly HttpClient Client;

    protected BaseIntegrationTest(IntegrationTestWebFactory factory)
    {
        var scope = factory.Services.CreateScope();

        Context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        Client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
            });

        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(scheme: "TestScheme");
    }

    protected async Task<int> SaveChangesAsync()
    {
        var result = await Context.SaveChangesAsync();
        Context.ChangeTracker.Clear();

        return result;
    }
}