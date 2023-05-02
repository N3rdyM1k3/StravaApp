using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Profisee.StravaApp
{
    public class strava_team_four_midnight_update
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private static string clubId = "1120789";

        [FunctionName("strava_team_four_midnight_update")]
        public async Task RunAsync([TimerTrigger("0 0 0 * * *")]TimerInfo myTimer, ILogger log)
        {
            try
            {
                string webhookUrl = "";

                
                string body = await getBodyAsync();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var content = createContent(body);
                var response = await httpClient.PostAsync(webhookUrl, content);

            }
            catch (Exception ex)
            {
                log.LogError($"Error sending message: {ex.Message}");
            }
        }

        private async Task<string> getBodyAsync(){
            var twentyFourHour = DateTime.UtcNow.AddHours(-24);
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var elapsedTime = twentyFourHour - epoch;
            var epochTimeStamp = (long)elapsedTime.TotalSeconds;
            var url = $"https://profisee-strava-app.azurewebsites.net/maybot?teamName={clubId}&after={epochTimeStamp}";
            // var url = $"http://localhost:5011/maybot?teamName={clubId}&after={epochTimeStamp}";
            var response = await httpClient.GetAsync(url);
            var body = await response.Content.ReadAsStringAsync();
            var dateString = DateTime.UtcNow.ToString("yyyy-MM-dd");
            return $"Strava Team Four Midnight Update for {dateString}\n\n{body}";	
        }   

        private StringContent createContent(string body){
            var adaptiveCardJson = @$"{{
                ""type"": ""message"",
                ""attachments"": [
                    {{
                    ""contentType"": ""application/vnd.microsoft.card.adaptive"",
                    ""content"": {{
                        ""type"": ""AdaptiveCard"",
                        ""body"": [
                        {{
                            ""type"": ""TextBlock"",
                            ""text"": ""{body}""
                        }}
                        ],
                        ""$schema"": ""http://adaptivecards.io/schemas/adaptive-card.json"",
                        ""version"": ""1.0""
                    }}
                    }}
                ]
                }}";
            return new StringContent(adaptiveCardJson, System.Text.Encoding.UTF8, "application/json");
        }
    }
}
