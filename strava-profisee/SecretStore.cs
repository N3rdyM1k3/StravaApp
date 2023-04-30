using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace StravaProfisee {
    
    internal static class CredentialStore{

        internal static StravaCredentials ReadFromAppSettings(ConfigurationManager configuration)
        {
            return new StravaCredentials(){
                ClientId = configuration["StravaCredentials:ClientId"], 
                ClientSecret = configuration["StravaCredentials:ClientSecret"], 
                RefreshToken = configuration["StravaCredentials:RefreshToken"],
                AccessToken = configuration["StravaCredentials:AccessToken"]
            };
        }

        internal static StravaCredentials ReadFromAzureKeyVault(){
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
            KeyVaultSecret accessToken = client.GetSecret("AccessToken");
            KeyVaultSecret refreshToken = client.GetSecret("RefreshToken");
            return new StravaCredentials(){
                ClientId = clientId.Value, 
                ClientSecret = clientSecret.Value, 
                RefreshToken = refreshToken.Value,
                AccessToken = accessToken.Value
            };
        }

        internal static async Task SaveToAzureKeyVault(StravaCredentials creds){
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
            await client.SetSecretAsync("AccessToken", creds.AccessToken);
            await client.SetSecretAsync("RefreshToken", creds.RefreshToken);
        }
    }
}