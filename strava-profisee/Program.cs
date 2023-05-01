using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Authentication.Cookies;
using Azure.Core;
using Microsoft.AspNetCore.HttpOverrides;
using StravaProfisee;


var builder = WebApplication.CreateBuilder(args);

StravaCredentials? creds = null;
if (builder.Environment.IsDevelopment()){
    creds = CredentialStore.ReadFromAppSettings(builder.Configuration);
}
else {
    creds = CredentialStore.ReadFromAzureKeyVault();
}


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
    options.ClientId = creds.ClientId ?? throw new Exception("Null Client Id");
    options.ClientSecret = creds.ClientSecret ?? throw new Exception("Null Client Secret");
    options.Scope.Add("read_all");
    options.SaveTokens = true;
});
builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment()){
    app.UseCookiePolicy(new CookiePolicyOptions
    {
        MinimumSameSitePolicy = Microsoft.AspNetCore.Http.SameSiteMode.None,
        Secure = CookieSecurePolicy.Always
    });
}


app.UseForwardedHeaders();
app.UseAuthentication();
app.UseAuthorization();



if (app.Environment.IsDevelopment()){
    app.MapGet("/new_tokens", async (HttpContext c) => {return await StravaClient.ReadTokens(c);}).RequireAuthorization();
    app.MapGet("/refresh_token", async () => { return await StravaClient.RefreshTokenPrint(creds);} );
}
else {
    await StravaClient.RefreshTokenInline(creds);
    await CredentialStore.SaveToAzureKeyVault(creds);
}

app.MapGet("/", () => "Welcome to the Profisee Strava Bot API!");
app.MapGet("/may", async (HttpContext c) => {return await StravaClient.HandleMayChallenge(c);}).RequireAuthorization();
app.MapGet("forward/{*path}", async (HttpContext c, string path) => {return await StravaClient.ForwardRequest(c, path);}).RequireAuthorization();
app.MapGet("/test/{*path}", async (HttpContext c, string path) => {return await StravaClient.Test(c, creds.AccessToken, path);});

app.Run();
