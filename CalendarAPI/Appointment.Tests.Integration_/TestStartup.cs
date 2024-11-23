using Appointment.Api;
using Appointment.Api.Exceptions;
using Appointment.Api.Extensions;
using Appointment.Api.Services.Auth.Interfaces;
using Appointment.Api.Services.Auth;
using Appointment.Data;
using Appointment.Domain;
using Appointment.Domain.Services.Auth;
using Appointment.Tests.Integration_.Handlers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Appointment.Api.Auth;
using CommandLine;
using Appointment.Api.Services.Mail;

namespace Appointment.Tests.Integration_
{
    public class TestStartup
    {
        private IConfiguration Configuration { get; }
        public TestStartup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public void ConfigureAuth(IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "Bearer";
                options.DefaultChallengeScheme = "Bearer";
            }).AddTestAuth(o => { });
            services.AddAuthorization(options =>
            {
                options.AddPolicy(Policies.User, policy => policy.RequireAuthenticatedUser());
                options.AddPolicy(Policies.Admin, policy => policy.RequireAuthenticatedUser());
                options.AddPolicy(Policies.SuperAdmin, policy => policy.RequireAuthenticatedUser());
            });
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                .AddNewtonsoftJson(opts => SerializerExtension.AddSerializerSettings(opts.SerializerSettings)).AddApplicationPart(typeof(Startup).Assembly);
            services.AddDbContext<CalendarContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("sqlserver-test")));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddSingleton<IMailSendingService , MailSendingService>();
            // auth
            services.AddSingleton<IPasswordHasher<IPasswordService>, PasswordHasher<IPasswordService>>();
            services.AddSingleton<IPasswordService, PasswordService>();
            services.AddSingleton<IJwtProvider, JwtProvider>();
            services.AddTransient<IRefreshTokenService, RefreshTokenService>();
            services.AddTransient<IIdentityService, IdentityService>();
            services.AddSingleton<IRng, Rng>();

            ConfigureAuth(services);
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            app.UseRouting();
            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
