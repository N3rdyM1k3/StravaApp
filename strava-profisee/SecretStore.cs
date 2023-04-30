using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace StravaProfisee {
    
    internal static class SecretStore{

        internal static (string? clientId, string? clientSecret) ReadFromAppSettings(ConfigurationManager configuration)
        {
            return (configuration["StravaCredentials:ClientId"], configuration["StravaCredentials:ClientSecret"]);
        }

        internal static (string? clientId, string? clientSecret) ReadFromAzureKeyVault(){
            SecretClientOptions options = new SecretClientOptions()
                {
                    Retry =
                    {
                        Delay= TimeSpan.FromSeconds(2),
                        MaxDelay = TimeSpan.FromSeconds(16),
                        MaxRetries = 5,
                        Mode = RetryMode.Exponential
                    }
                };
            var client = new SecretClient(new Uri("https://profisee-strava-keyvault.vault.azure.net/"), new DefaultAzureCredential(),options);
            KeyVaultSecret clientId = client.GetSecret("ClientId");
            KeyVaultSecret clientSecret = client.GetSecret("ClientSecret");
            return (clientId.Value, clientSecret.Value);
        }
    }
}