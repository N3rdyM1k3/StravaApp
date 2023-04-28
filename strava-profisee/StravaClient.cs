using System.Text;
using Microsoft.AspNetCore.Authentication;

namespace StravaProfisee
{

    internal static class StravaClient {
        internal static async Task<string> HandleMayChallenge(HttpContext context){

            var teamName = context.Request.Query["teamName"].ToString();
            var after = context.Request.Query["after"].ToString();
            var targetUrl = $"https://www.strava.com/api/v3/clubs/{teamName}/activities?after={after}&per_page=200"; 
            
            var access_token = await context.GetTokenAsync("Strava", "access_token"); 
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + access_token);
            var resp = await client.GetAsync(targetUrl);

            var activities = await resp.Content.ReadFromJsonAsync<List<Activity>>();
            return formatMay(activities);  
        }

        private static string formatMay(List<Activity> activities){
            var resp = new StringBuilder();
            var sum = activities.Select(a => a.elapsed_time).Sum().ToString(); 
            resp.AppendLine($"Total Seconds: {sum}");
            resp.AppendLine("=================Details===================");
            activities.ForEach(a => resp.AppendLine($"{a.name}: {a.elapsed_time}"));
            return resp.ToString();
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