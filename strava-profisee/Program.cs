using StravaProfisee;

var app = WebApplication.Create(args);
// var stravaClient = new StravaClient();
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthentication().AddStrava(options =>
{
    options.ClientId = "";
    options.ClientSecret = "";
}
);
app.UseAuthentication();
app.UseAuthorization();
app.MapGet("/", () => "Hello World");

app.Run();
