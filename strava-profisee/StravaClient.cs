using Microsoft.AspNetCore.Authentication;

namespace StravaProfisee
{

internal static class StravaClient {
    internal static async Task<string> HandleAprilChallenge(HttpContext httpContext){
            var access_token = await httpContext.GetTokenAsync("Strava", "access_token"); 
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + access_token);
            var resp = await client.GetAsync("https://www.strava.com/api/v3/clubs/Profisee1/activities");
            var activities = await resp.Content.ReadFromJsonAsync<List<Activity>>();
            return activities.Count().ToString();    
    }
}
}