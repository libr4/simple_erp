using System;
using System.Linq;
using System.Threading.Tasks;
using Backend.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.MsSql;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers;
using Microsoft.Data.SqlClient;
using System.Diagnostics;

namespace Backend.Tests.IntegrationTests
{
    /// <summary>
    /// Custom WebApplicationFactory that uses Testcontainers for SQL Server.
    /// Automatically starts a container, applies migrations, and provides a clean DB for each test.
    /// </summary>
    public class TestWebApplicationFactory : WebApplicationFactory<Program>
    {
        private MsSqlContainer? _container;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove the existing ApplicationDbContext registration
                var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Register ApplicationDbContext with the Testcontainers connection string
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    // If a TEST_SQL_CONN environment variable is provided, reuse that (useful when running docker-compose)
                    var envConn = Environment.GetEnvironmentVariable("TEST_SQL_CONN");
                    if (!string.IsNullOrWhiteSpace(envConn))
                    {
                        options.UseSqlServer(envConn);
                        return;
                    }

                    var connString = _container?.GetConnectionString()
                        ?? throw new InvalidOperationException("Container connection string not available");
                    options.UseSqlServer(connString);
                });
            });

            base.ConfigureWebHost(builder);
        }

        public override async ValueTask DisposeAsync()
        {
            if (_container != null)
            {
                await _container.StopAsync();
                await _container.DisposeAsync();
            }

            await base.DisposeAsync();
        }

        /// <summary>
        /// Initializes and starts the SQL Server Testcontainer.
        /// Must be called before making HTTP requests.
        /// </summary>
        public async Task InitializeContainerAsync()
        {
            // If a TEST_SQL_CONN environment variable is present, assume an external DB is running
            // (for example started via `docker compose up`) and skip creating a Testcontainer.
            var envConn = Environment.GetEnvironmentVariable("TEST_SQL_CONN");
            if (!string.IsNullOrWhiteSpace(envConn))
            {
                // Do not start a container; ConfigureWebHost will pick up TEST_SQL_CONN
                _container = null;
                return;
            }

            _container = new MsSqlBuilder()
                .WithImage("mcr.microsoft.com/mssql/server:2019-latest")
                .WithPassword("Test_Password_123!")
                .Build();

            await _container.StartAsync();

            // Wait until we can open a SQL connection to the container.
            // This provides a configurable timeout and avoids brittle internal readiness checks.
            var connString = _container.GetConnectionString();
            var timeoutSeconds = 300; // default 5 minutes
            var envTimeout = Environment.GetEnvironmentVariable("TESTCONTAINERS_TIMEOUT_SECONDS");
            if (!string.IsNullOrWhiteSpace(envTimeout) && int.TryParse(envTimeout, out var parsed))
            {
                timeoutSeconds = parsed;
            }

            var sw = Stopwatch.StartNew();
            var connected = false;
            while (sw.Elapsed.TotalSeconds < timeoutSeconds)
            {
                try
                {
                    using var sqlConn = new SqlConnection(connString);
                    await sqlConn.OpenAsync();
                    connected = true;
                    break;
                }
                catch
                {
                    await Task.Delay(1000);
                }
            }

            if (!connected)
            {
                throw new TimeoutException($"Timed out waiting for SQL Server to become available after {timeoutSeconds} seconds.");
            }

            // Apply migrations and seed data
            using (var scope = Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                await dbContext.Database.MigrateAsync();
            }
        }
    }
}
