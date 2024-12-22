using Application.Abstraction.Interfaces;
using Application.Abstraction.Interfaces.Queries;
using Application.Abstraction.Interfaces.Repositories;
using Application.Implementation;
using Application.Implementation.Observers;
using Infrastructure.Factiories;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Infrastructure.Persistence;

public static class ConfigurePersistence
{
    public static void AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var dataSourceBuild = new NpgsqlDataSourceBuilder(configuration.GetConnectionString("Default"));
        dataSourceBuild.EnableDynamicJson();
        var dataSource = dataSourceBuild.Build();

        services.AddDbContext<ApplicationDbContext>(
            options => options
                .UseNpgsql(
                    dataSource,
                    builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName))
                .UseSnakeCaseNamingConvention()
                .ConfigureWarnings(w => w.Ignore(CoreEventId.ManyServiceProvidersCreatedWarning)));

        services.AddScoped<ApplicationDbContextInitialiser>();
        services.AddObservers();
        services.AddRepositories();
        services.AddSingletons(configuration);

    }

    private static void AddObservers(this IServiceCollection services)
    {
        services.AddTransient<TransactionReportGenerator>();
        services.AddTransient<TransactionLogger>();
        services.AddTransient<TransactionUIUpdater>();
    }

    private static void AddSingletons(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<PayrollManager>();
        services.AddSingleton<TransactionNotifier>();
        services.AddSingleton<ILogger>(_ => LoggerFactory.CreateLogger(configuration));
    }

    private static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<EmployeeRepository>();
        services.AddScoped<IEmployeeRepository>(provider => provider.GetRequiredService<EmployeeRepository>());
        services.AddScoped<IEmployeeQueries>(provider => provider.GetRequiredService<EmployeeRepository>());

        services.AddScoped<TransactionRepository>();
        services.AddScoped<ITransactionRepository>(provider => provider.GetRequiredService<TransactionRepository>());
        services.AddScoped<ITransactionQueries>(provider => provider.GetRequiredService<TransactionRepository>());
    }
}