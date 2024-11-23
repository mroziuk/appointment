using Azure.Identity;
using Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Security.KeyVault.Secrets;

namespace Appointment.Data.SecretStore
{
    internal class AzureKeyVaultReader : IAzureKeyVaultReader
    {
        private readonly SecretClient secretClient;
        public AzureKeyVaultReader(Uri vaultUri, string tenantId)
        {
            var isDeployedToCloud = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME"));

            var options = new DefaultAzureCredentialOptions();

            if(!isDeployedToCloud)
            {
                //options.VisualStudioTenantId = tenantId;
                //options.VisualStudioCodeTenantId = tenantId;
                //options.SharedTokenCacheTenantId = tenantId;
                //options.ManagedIdentityClientId = tenantId;
            }

            this.secretClient = new SecretClient(vaultUri, new DefaultAzureCredential(options));
        }


        public async Task<(bool suceeded, string secret)> TryGetSecretAsync(string secretName)
        {
            try
            {
//#if DEBUG
//                return (true, "Server=tcp:appointment-server.database.windows.net,1433;Initial Catalog=appointment-sql;Persist Security Info=False;User ID=mroziuk;Password=Krokodyle1.;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
//#endif
                var kvSecret = await this.secretClient.GetSecretAsync(secretName);
                return (true, kvSecret.Value.Value);

            }
            catch(Exception e) when(e is RequestFailedException || e is ArgumentException)
            {
                return (false, null);
            }
        }
    }
}
