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

            var cardFacts = new List<Fact> { };

            if (!string.IsNullOrEmpty(error.Payload))
            {
                cardFacts.Add(new Fact
                {
                    Title = "Payload",
                    Value = error.Payload?.Replace("\"", "\\\"") ?? string.Empty
                });
            }

            cardFacts.Add(new Fact
            {
                Title = "Reported At",
                Value = DateTime.Now.ToString()
            });

            var adaptiveCardData = new MSAdaptiveCard
            {
                Type = "message",
                Attachments = new List<Attachment> {
                new Attachment {
                    ContentType = "application/vnd.microsoft.card.adaptive",
                    Content = new AdaptiveCardContent
                    {
                        Type = "AdaptiveCard",
                        Body = new List<CardBody>
                        {
                            new CardBody
                            {
                                Type = "TextBlock",
                                Size = "Medium",
                                Weight = "Bolder",
                                Text = error.Code
                            },
                            new CardBody
                            {
                                Type = "TextBlock",
                                Text = error.Message
                            },
                            new CardBody
                            {
                                Type = "FactSet",
                                FactSet = new FactSet
                                { Facts = cardFacts }
                            }
                        },
                        Schema = "http://adaptivecards.io/schemas/adaptive-card.json",
                        Version = "1.0"
                        }
                }
                    }
            };

            var adaptiveCardJson = JsonSerializer.Serialize(adaptiveCardData);

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