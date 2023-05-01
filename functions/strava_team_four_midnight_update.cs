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
        private static readonly HttpClient HttpClient = new HttpClient();

        [FunctionName("strava_team_four_midnight_update")]
        public async Task RunAsync([TimerTrigger("0 19 0 * * *")]TimerInfo myTimer, ILogger log)
        {
            try
            {
                string webhookUrl = "";

                var adaptiveCardJson = @"{
                ""type"": ""message"",
                ""attachments"": [
                    {
                    ""contentType"": ""application/vnd.microsoft.card.adaptive"",
                    ""content"": {
                        ""type"": ""AdaptiveCard"",
                        ""body"": [
                        {
                            ""type"": ""TextBlock"",
                            ""text"": ""Message Text""
                        }
                        ],
                        ""$schema"": ""http://adaptivecards.io/schemas/adaptive-card.json"",
                        ""version"": ""1.0""
                    }
                    }
                ]
                }";

                var client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var content = new StringContent(adaptiveCardJson, System.Text.Encoding.UTF8, "application/json");
                var response = await client.PostAsync(webhookUrl, content);

            }
            catch (Exception ex)
            {
                log.LogError($"Error sending message: {ex.Message}");
            }
        }
    }
}
