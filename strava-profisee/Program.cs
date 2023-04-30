using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Authentication.Cookies;
using Azure.Core;
using Microsoft.AspNetCore.HttpOverrides;
using StravaProfisee;


var builder = WebApplication.CreateBuilder(args);

(string? clientId, string? clientSecret) stravaCreds;
if (builder.Environment.IsDevelopment()){
    stravaCreds = SecretStore.ReadFromAppSettings(builder.Configuration);
}
else {
    stravaCreds = SecretStore.ReadFromAzureKeyVault();
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
    options.ClientId = stravaCreds.clientId ?? throw new Exception("Null Client Id");
    options.ClientSecret = stravaCreds.clientSecret ?? throw new Exception("Null Client Secret");
    options.Scope.Add("read_all");
    options.SaveTokens = true;
});
builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment()){
    app.UseCookiePolicy(new CookiePolicyOptions
    {
        // HttpOnly =  HttpOnlyPolicy.Always,
        MinimumSameSitePolicy = Microsoft.AspNetCore.Http.SameSiteMode.None,
        Secure = CookieSecurePolicy.Always
        // MinimumSameSitePolicy = SameSiteMode.Lax
    });
}


app.UseForwardedHeaders();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "Hello World");
app.MapGet("/may", async (HttpContext c) => {return await StravaClient.HandleMayChallenge(c);}).RequireAuthorization();


app.MapGet("forward/{*path}", async (HttpContext c, string path) => {return await StravaClient.ForwardRequest(c, path);}).RequireAuthorization();
app.Run();
