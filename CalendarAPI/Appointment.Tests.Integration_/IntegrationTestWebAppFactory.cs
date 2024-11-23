using Appointment.Api;
using Appointment.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Testcontainers.MsSql;
using Xunit;

namespace Appointment.Tests.Integration_
{
    public class IntegrationTestWebAppFactory : WebApplicationFactory<Startup>, IAsyncLifetime
    {
        private readonly MsSqlContainer _dbContainer = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithPassword("Admin123.")
            .Build();

        protected override IHostBuilder CreateHostBuilder()
        {
            return Host.CreateDefaultBuilder().ConfigureWebHost((builder) =>
            {
                builder.UseStartup<TestStartup>();
            });
        }
        public Task InitializeAsync()
        {
            return _dbContainer.StartAsync();
        }
        public new Task DisposeAsync()
        {
            return _dbContainer.StopAsync();
        }
    }
}
