using System.Data.Common;
using HabitTracker.Api.Habits.Data;
using HabitTracker.Api.Payment;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Respawn;
using Testcontainers.SqlEdge;

namespace HabitTracker.Api.Tests.Integration.Infrastructure;

public class ApiFactory : WebApplicationFactory<IApiMarker>, IAsyncLifetime
{
    public PaymentApiServer ExternalPaymentApi { get; } = new();
    
    private readonly SqlEdgeContainer _sqlEdgeDb = new SqlEdgeBuilder()
        .WithPassword("HeabyHitPass1!")
        .Build();

    private DbConnection _dbConnection = default!;
    private Respawner _respawner = default!;

    public HttpClient HttpClient = default!;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                     typeof(DbContextOptions<ApplicationDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }
            
            services.AddDbContext<ApplicationDbContext>(opts => opts.UseSqlServer(_sqlEdgeDb.GetConnectionString()));

            services.AddHttpClient<PaymentServiceClient>(opts =>
            {
                opts.BaseAddress = new Uri(ExternalPaymentApi.Url);
            });
        });
    }

    public async Task InitializeAsync()
    {
        await ExternalPaymentApi.Start();
        await _sqlEdgeDb.StartAsync();
        HttpClient = CreateClient();
        await InitializeDbRespawner();
    }

    public new async Task DisposeAsync()
    {
        await ExternalPaymentApi.DisposeAsync();
        await _sqlEdgeDb.StopAsync();
    }

    public async Task ResetDatabaseAsync()
    {
        await _respawner.ResetAsync(_dbConnection);
    }

    private async Task InitializeDbRespawner()
    {
        _dbConnection = new SqlConnection(_sqlEdgeDb.GetConnectionString());
        await _dbConnection.OpenAsync();
        _respawner = await Respawner.CreateAsync(_dbConnection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.SqlServer,
            SchemasToInclude = new []{"dbo"}
        });
    }
}