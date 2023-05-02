using System.Text;
using Microsoft.AspNetCore.Authentication;
using StravaProfisee.StravaModels;

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

        internal static async Task<string> HandleMayBot(HttpContext context, string access_token){

            var teamName = context.Request.Query["teamName"].ToString();
            var after = context.Request.Query["after"].ToString();
            var targetUrl = $"https://www.strava.com/api/v3/clubs/{teamName}/activities?after={after}&per_page=200"; 
            
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + access_token);
            var resp = await client.GetAsync(targetUrl);

            var activities = await resp.Content.ReadFromJsonAsync<List<Activity>>();
            return formatMay(activities);  
        }

        private static string formatMay(List<Activity> activities){
            var resp = new StringBuilder();
            var sum = activities.Select(a => a.elapsed_time).Sum().ToString(); 
            resp.Append($"Total Seconds: {sum}\n\n");
            resp.Append("=================Details===================\n\n");
            activities.ForEach(a => resp.Append($"{a.athlete.firstname} {a.athlete.lastname} -- {a.name}: {a.elapsed_time} seconds\n\n"));
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

        internal static async Task<string> ReadTokens(HttpContext context){
            var access_token = await context.GetTokenAsync("Strava", "access_token"); 
            var refresh_token = await context.GetTokenAsync("Strava", "refresh_token");

            return $"Access: {access_token}\nRefresh: {refresh_token}";
        }

        internal static async Task RefreshTokenInline(StravaCredentials creds){
            var body = @$"{{
                ""client_id"": ""{creds.ClientId}"", 
                ""client_secret"": ""{creds.ClientSecret}"",
                ""refresh_token"": ""{creds.RefreshToken}"",
                ""grant_type"": ""refresh_token""
            }}";
            var client = new HttpClient();
            var httpContent = new StringContent(body, Encoding.UTF8, "application/json");
            var resp = await client.PostAsync("https://www.strava.com/oauth/token", httpContent);
            var tokenResp = await resp.Content.ReadFromJsonAsync<TokenResponse>();
            creds.RefreshToken = tokenResp.refresh_token;
            creds.AccessToken = tokenResp.access_token;
        }

        internal static async Task<string> RefreshTokenPrint(StravaCredentials creds){
            var body = @$"{{
                ""client_id"": ""{creds.ClientId}"", 
                ""client_secret"": ""{creds.ClientSecret}"",
                ""refresh_token"": ""{creds.RefreshToken}"",
                ""grant_type"": ""refresh_token""
            }}";
            var client = new HttpClient();
            var httpContent = new StringContent(body, Encoding.UTF8, "application/json");
            var resp = await client.PostAsync("https://www.strava.com/oauth/token", httpContent);

            var tokenResp = await resp.Content.ReadFromJsonAsync<TokenResponse>();
            return $"AccessToken: {tokenResp.access_token}\n RefreshToken: {tokenResp.refresh_token}";
        }

        internal static async Task<string> Test(HttpContext context, string accessToken, string path)
        {
            var queryParams = context.Request.Query.Select(q => $"{Uri.EscapeDataString(q.Key)}={Uri.EscapeDataString(q.Value)}")
                    .ToArray();


            var targetUrl = $"https://www.strava.com/api/v3/clubs/{path}?{string.Join("&", queryParams)}"; 
            
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
            var resp = await client.GetAsync(targetUrl);

            var activities = await resp.Content.ReadFromJsonAsync<List<Activity>>();
            return activities.Count().ToString();   
        }
    }
}