using System;
using System.Linq;
using System.Threading.Tasks;
using Backend.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.MsSql;

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
            _container = new MsSqlBuilder()
                .WithImage("mcr.microsoft.com/mssql/server:2019-latest")
                .WithPassword("Test_Password_123!")
                .Build();

            await _container.StartAsync();

            // Apply migrations and seed data
            using (var scope = Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                await dbContext.Database.MigrateAsync();
            }
        }
    }
}
