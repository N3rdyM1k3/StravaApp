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

KeyVaultSecret clientId = client.GetSecret("ClientId");
KeyVaultSecret clientSecret = client.GetSecret("ClientSecret");

// var stravaClient = new StravaClient();
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication().AddStrava(options =>
{
    options.ClientId = clientId.Value;
    options.ClientSecret = clientSecret.Value;
});
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.MapGet("/", () => "Hello World").AllowAnonymous();
app.MapGet("/auth", () => "Auth").RequireAuthorization();
app.Run();
