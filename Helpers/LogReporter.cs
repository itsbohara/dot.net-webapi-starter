using System.Net.Http.Headers;
using System.Text.Json;
using Serilog;
using Serilog.Core;
using Serilog.Events;

public class LogReporter : ILogEventSink
{

    private IConfiguration _config;

    private readonly HttpClient _httpClient;

    public LogReporter(IConfiguration configuration)
    {
        _httpClient = new HttpClient();
        _config = configuration;
    }

    public void Emit(LogEvent logEvent)
    {
        // Check if the log event contains CustomErrorException
        if (logEvent.Exception is CustomErrorException)
        {
            // logEvent.Properties.TryGetValue("Endpoint", out var endpointValue);
            // Make webhook call when CustomErrorException is logged
            var errorException = logEvent.Exception as CustomErrorException;
            if (errorException != null && errorException.ReportToTeam)
            {
                // Send error report to microsoft teams
                SendReportToTeamsAsync(errorException).Wait();
            }
        }
    }

    private async Task SendReportToTeamsAsync(CustomErrorException error)
    {
        try
        {
            Console.WriteLine("\n -> Reporting Log to Teams \n");

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
                        ""size"": ""Medium"",
                        ""weight"": ""Bolder"",
                        ""text"": """ + error.Code + @"""
                        },
                        {
                            ""type"": ""TextBlock"",
                            ""text"": """ + error.Message + @"""
                        },
                        {
                            ""type"": ""FactSet"",
                             ""facts"": [";

            if (!string.IsNullOrEmpty(error.Payload))
            {
                adaptiveCardJson += @"
                            {
                                ""title"": ""Payload"",
                                ""value"": """ + error.Payload.Replace("\"", "\\\"") + @"""
                            },";
            }

            adaptiveCardJson += @" 
                            {
                                ""title"": ""Reported At"",
                                ""value"": """ + DateTime.Now + @"""
                            }
             ],
                        }
                        ],
                        ""$schema"": ""http://adaptivecards.io/schemas/adaptive-card.json"",
                        ""version"": ""1.0""
                    }
                    }
                ]
                }";


            Console.Write(adaptiveCardJson);


            var webhookUrl = _config["MSTeamsWebhook"];
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var content = new StringContent(adaptiveCardJson, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(webhookUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                // Handle API call failure
                Log.Error("Failed to report log with status code: {StatusCode}", response.StatusCode);
            }
            else
            {
                // API call successful
                Log.Information("Log report success.");
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error during Webhook call.");
        }
    }
}