using Appointment.Api;
using Appointment.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using Microsoft.Extensions.DependencyInjection;

namespace Appointment.Tests.Integration
{
    public class TestingWebAppFactory<TEntryPoint> : WebApplicationFactory<Program> where TEntryPoint : Program
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                //var descriptor = services.SingleOrDefault(
                //    d => d.ServiceType ==
                //        typeof(DbContextOptions<CalendarContext>));
                //if(descriptor != null)
                //    services.Remove(descriptor);
                //services.AddDbContext<CalendarContext>(options =>
                //{
                //    options.UseInMemoryDatabase("InMemoryEmployeeTest");
                //});
                var sp = services.BuildServiceProvider();
                using(var scope = sp.CreateScope())
                using(var appContext = scope.ServiceProvider.GetRequiredService<CalendarContext>())
                {
                    try
                    {
                        appContext.Database.EnsureCreated();
                    }
                    catch(Exception ex)
                    {
                        //Log errors or do anything you think it's needed
                        throw;
                    }
                }
            });
        }
    }
}
