using Appointment.Api;
using Appointment.Data;
using Appointment.Data.Snapshot;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Respawn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointment.Tests.Integration
{
    public class TestBase
    {
        protected readonly Respawner _respawner;
        protected readonly WebApplicationFactory<Program> _factory;
        protected readonly string _connectionString;
        protected readonly IDatabaseRestoreService _databaseRestoreService;
        public TestBase(string? connectionString = null)
        {
            var _builder = WebApplication.CreateBuilder();
            //_connectionString = connectionString ?? _builder.Configuration.GetConnectionString("azure-sql");
            _factory = new TestingWebAppFactory<Program>().WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.RemoveAll(typeof(DbContextOptions<CalendarContext>));
                    services.AddDbContext<CalendarContext>(options =>
                    {
                        options.UseSqlServer(_connectionString);
                    });
                });
            });
            _respawner = Respawner.CreateAsync(_connectionString, new RespawnerOptions
            {
                TablesToIgnore =
                [
                    "__EFMigrationsHistory",
                ],
            }).Result;
            _databaseRestoreService = new DatabaseRestoreService();
        }
    }
}
