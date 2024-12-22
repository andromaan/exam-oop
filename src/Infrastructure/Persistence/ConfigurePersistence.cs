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
        services.AddSingletons();

        services.AddSingleton<ILogger>(provider => LoggerFactory.CreateLogger(configuration));
    }

    private static void AddObservers(this IServiceCollection services)
    {
        services.AddTransient<TransactionReportGenerator>();
        services.AddTransient<TransactionLogger>();
        services.AddTransient<TransactionUIUpdater>();
    }

    private static void AddSingletons(this IServiceCollection services)
    {
        services.AddSingleton<PayrollManager>(provider =>
        {
            var scopeFactory = provider.GetRequiredService<IServiceScopeFactory>();
            var notifier = provider.GetRequiredService<TransactionNotifier>();
            var logger = provider.GetRequiredService<ILogger>();

            var payrollManager = new PayrollManager(provider, notifier,logger);

            using var scope = scopeFactory.CreateScope();
            var scopedProvider = scope.ServiceProvider;

            notifier.Subscribe(scopedProvider.GetRequiredService<TransactionLogger>());
            notifier.Subscribe(scopedProvider.GetRequiredService<TransactionUIUpdater>());
            notifier.Subscribe(scopedProvider.GetRequiredService<TransactionReportGenerator>());

            return payrollManager;
        });


        services.AddSingleton<TransactionNotifier>();
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