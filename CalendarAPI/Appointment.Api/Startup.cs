using Appointment.Api.Services.Auth.Interfaces;
using Appointment.Api.Services.Auth;
using Appointment.Data.Snapshot;
using Appointment.Data;
using Appointment.Domain.Services.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Appointment.Api.Extensions;
using Appointment.Api.Exceptions;
using Appointment.Domain;
using Appointment.Api.Services.Mail;
using Serilog;
using Appointment.Data.SecretStore.Extensions;

namespace Appointment.Api
{
    public class Startup
    {
        private IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public virtual void ConfigureServices(IServiceCollection services)
        {
            // Add services to the container.
            services.AddControllers()
                .AddNewtonsoftJson(opts => SerializerExtension.AddSerializerSettings(opts.SerializerSettings));

            services.AddDbContext<CalendarContext>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddSingleton<IMailSendingService, MailSendingService>();
            services.AddHttpContextAccessor();
            // auth
            services.AddSingleton<IPasswordHasher<IPasswordService>, PasswordHasher<IPasswordService>>();
            services.AddSingleton<IPasswordService, PasswordService>();
            services.AddSingleton<IJwtProvider, JwtProvider>();
            services.AddTransient<IRefreshTokenService, RefreshTokenService>();
            services.AddTransient<IIdentityService, IdentityService>();
            services.AddSingleton<IRng, Rng>();

            ConfigureAuth(services);

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddScoped<IDatabaseRestoreService, DatabaseRestoreService>();
            services.AddSwagger();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration).CreateLogger();
            //.MinimumLevel.Information()
            //.WriteTo.Console()
            //.WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
            //.CreateLogger();
            services.AddHealthChecks();
            services.AddKeyVault(Configuration);
        }
        public virtual void ConfigureAuth(IServiceCollection services)
        {
            services.AddJwtAuthorization(Configuration);
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider,ILoggerFactory loggerFactory)
        {
            app.UseMiddleware<ExceptionMiddleware>();
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                //app.ApplyMigrations();
            }

            app.UseCors(builder
                => builder
                    .WithOrigins("20.69.151.16", "http://localhost:5173", "https://localhost:5173", "https://192.168.1.13:5173/", "https://black-grass-01b480603.5.azurestaticapps.net/")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            app.UseRouting();
            app.UseHttpsRedirection();
            app.UseHealthChecks("/health");
            
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.CreateInitData(serviceProvider).Wait();
        }
    }
}
