using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Appointment.Data.SecretStore.Extensions
{
    public static class KeyVault
    {
        public static IServiceCollection AddKeyVault(this IServiceCollection services, IConfiguration configuration)
        {
            var options = configuration.GetSection(nameof(KeyVaultOptions));
            services.Configure<KeyVaultOptions>(options);

            services.AddTransient<IAzureKeyVaultReader>(provider => new AzureKeyVaultReader(new Uri(options[nameof(KeyVaultOptions.KvUri)]),
                options[nameof(KeyVaultOptions.TenantId)]));

            return services;
        }
    }
}
