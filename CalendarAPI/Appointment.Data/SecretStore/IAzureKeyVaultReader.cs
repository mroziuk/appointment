namespace Appointment.Data.SecretStore;

public interface IAzureKeyVaultReader
{
    Task<(bool suceeded, string secret)> TryGetSecretAsync(string secretName);
}
