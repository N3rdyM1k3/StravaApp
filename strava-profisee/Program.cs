using StravaProfisee;

var app = WebApplication.Create(args);
// var stravaClient = new StravaClient();
app.MapGet("/", () => "Hello World");

app.Run();
