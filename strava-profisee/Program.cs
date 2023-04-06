using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Authentication.Cookies;
using Azure.Core;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Authentication;

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
var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders =
                ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        });


KeyVaultSecret clientId = client.GetSecret("ClientId");
KeyVaultSecret clientSecret = client.GetSecret("ClientSecret");
var clientIdValue = clientId.Value;
var clientSecretValue = clientSecret.Value;

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
//     MinimumSameSitePolicy = Microsoft.AspNetCore.Http.SameSiteMode.None,
//     Secure = CookieSecurePolicy.Always
// });

app.UseForwardedHeaders();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "Hello World"); // .AllowAnonymous();

// app.MapGet("/signin-strava", async (HttpContext c) => {
//     var authFeatures = c.Features.Get<IAuthenticateResultFeature>();
//     var authProps = authFeatures.AuthenticateResult.Properties;
//     var t = authProps.GetTokens();
//     var a = c.Request.Headers[HeaderNames.Authorization];
//     var g = await c.GetTokenAsync("Strava", "access_token"); 

//  return a; 

//});
// app.MapGet("/login", async (HttpContext c) => {
//     var authFeatures = c.Features.Get<IAuthenticateResultFeature>();
//     var authProps = authFeatures.AuthenticateResult.Properties;
//     var t = authProps.GetTokens();
//     var a = c.Request.Headers[HeaderNames.Authorization];
//     var g = await c.GetTokenAsync("Strava", "access_token"); 

//  return a; 
//  var client = new HttpClient();
//  var resp = await client.GetAsync("https://www.strava.com/api/v3/clubs/Profisee1/activities");
//  return resp;
// }).RequireAuthorization();
app.MapGet("/test", async (HttpContext c) => {
    var g = await c.GetTokenAsync("Strava", "access_token"); 
    var client = new HttpClient();
    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + g);
    var resp = await client.GetAsync("https://www.strava.com/api/v3/clubs/Profisee1/activities");
    return await resp.Content.ReadAsStringAsync();
}).RequireAuthorization();
//app.MapPost("/signin-strava", async (AuthResponse r) => { return Results.Ok(r.access_token);}).AllowAnonymous();
app.Run();
