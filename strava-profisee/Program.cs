using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Authentication.Cookies;
using Azure.Core;
using Microsoft.AspNetCore.HttpOverrides;
using StravaProfisee;

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
var clientIdValue = clientId.Value;
var clientSecretValue = clientSecret.Value;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders =
                ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        });




builder.Services.AddAuthentication(options => {
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = "Strava";
    })
    .AddCookie()
.AddStrava(options =>
{
    options.ClientId = clientIdValue;
    options.ClientSecret = clientSecretValue;
    options.Scope.Add("read_all");
    options.SaveTokens = true;
});
builder.Services.AddAuthorization();

var app = builder.Build();

// app.UseCookiePolicy(new CookiePolicyOptions
// {
//     // HttpOnly =  HttpOnlyPolicy.Always,
//     MinimumSameSitePolicy = Microsoft.AspNetCore.Http.SameSiteMode.None,
//     Secure = CookieSecurePolicy.Always
//     // MinimumSameSitePolicy = SameSiteMode.Lax
// });

app.UseForwardedHeaders();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "Hello World");


app.MapGet("/test", async (HttpContext c) => {return await StravaClient.HandleAprilChallenge(c);}).RequireAuthorization();
app.Run();
