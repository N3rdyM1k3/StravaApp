using Microsoft.AspNetCore.Authentication;

namespace StravaProfisee
{

    internal static class StravaClient {
        internal static async Task<string> HandleAprilChallenge(HttpContext httpContext){
                var access_token = await httpContext.GetTokenAsync("Strava", "access_token"); 
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + access_token);
                var resp = await client.GetAsync("https://www.strava.com/api/v3/clubs/Profisee1/activities?after=1682035200&per_page=200");
                var activities = await resp.Content.ReadFromJsonAsync<List<Activity>>();
                return activities.Count().ToString();    
        }

        internal static async Task<string> ForwardRequest(HttpContext context, string path){

            var queryParams = context.Request.Query.Select(q => $"{Uri.EscapeDataString(q.Key)}={Uri.EscapeDataString(q.Value)}")
                    .ToArray();


            var targetUrl = $"https://www.strava.com/api/v3/clubs/{path}?{string.Join("&", queryParams)}"; 
            
            var access_token = await context.GetTokenAsync("Strava", "access_token"); 
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + access_token);
            var resp = await client.GetAsync(targetUrl);

            var activities = await resp.Content.ReadFromJsonAsync<List<Activity>>();
            return activities.Count().ToString();    

        }
    }
}