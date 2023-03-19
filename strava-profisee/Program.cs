using StravaProfisee;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Core;

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

KeyVaultSecret secret = client.GetSecret("ClientId");

string secretValue = secret.Value;

var app = WebApplication.Create(args);
// var stravaClient = new StravaClient();
// var builder = WebApplication.CreateBuilder(args);
// builder.Services.AddAuthentication().AddStrava(options =>
// {
//     options.ClientId = "";
//     options.ClientSecret = "";
// }
// );
// app.UseAuthentication();
// app.UseAuthorization();
app.MapGet("/", () => "Hello World");
app.MapGet("/cid", () => secretValue);
app.Run();
