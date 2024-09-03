
using BrandMeIG.InstagramAPI;
using BrandMeIG.GptAPI;
using BrandMeIG.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Runtime.InteropServices;
using BrandMeIG.Configuration;
using static System.Net.WebRequestMethods;
using System.Net.Http.Headers;
using BrandMeIG.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace BrandMeIG
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var MyAllowSpecificOrigins = "AllowSpecificOrigin";

            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddAuthorization();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            string token = "_token_";
            string business_id = "_id_";
            string gptToken = "";

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                    policy =>
                    {
                        policy.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
                    });
            });

            builder.Services
            .AddHttpClient("Instagram API", (sp, client) =>
            {
                var configuration = sp.GetRequiredService<IOptionsMonitor<DeploymentSettings>>();
                client.BaseAddress = new Uri("https://graph.facebook.com/v17.0");
            })
            .AddTypedClient<IInstagramClient>((client, sp) =>
            {
                var contractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                };

                var settings = new JsonSerializerSettings
                {
                    ContractResolver = contractResolver,
                    Formatting = Formatting.None
                };

                var configuration = sp.GetRequiredService<IOptionsMonitor<DeploymentSettings>>();
                var botConfig = new DeploymentSettings()
                {
                    AccessToken = token,
                    PageID = configuration.CurrentValue.PageID,
                    InstagramBaseAPIUri = configuration.CurrentValue.InstagramBaseAPIUri,
                    BusinessAccount = business_id,
                    FaceBookAppId = configuration.CurrentValue.FaceBookAppId,
                    FaceBookAppSecret = configuration.CurrentValue.FaceBookAppSecret
                };
                return new InstagramClient(client, settings, botConfig);
            });

            builder.Services
            .AddHttpClient("GPTClient", (sp, client) =>
            {
                var configuration = sp.GetRequiredService<IOptionsMonitor<DeploymentSettings>>();
                client.BaseAddress = new Uri("https://api.openai.com/v1/");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", gptToken);
            })
            .AddTypedClient<IGptClient>((client, sp) =>
            {
                var contractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                };

                var settings = new JsonSerializerSettings
                {
                    ContractResolver = contractResolver,
                    Formatting = Formatting.None
                };
                return new GptClient(client, settings);
            });
            builder.Services.Configure<DeploymentSettings>(builder.Configuration);
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseCors(MyAllowSpecificOrigins);
            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapGet("/analitics", async (string userName, IInstagramClient _client) =>
            {
                var result = await _client.GetInfo(userName, default);

                return new ResultModel()
                {
                    AccountName = result.business_discovery.name,
                    BusinessDescription = result.business_discovery.biography,
                    BrandValue = 43,
                    BusinessArea = "",
                    Region = "Warsaw, Poland",
                    TargetNiches = "",
                    HashtagIdeas = "",
                    ImprovementsSuggestions = "",
                    Analitics = "Followers: 531\n" +
                    "Average Engagement Rate: 3.2%\n" +
                    "Top performing post: (31 likes)\n" +
                    "Video engagement higher than image posts.",
                    PotentialPartners = "",
                    TrendsContent = "",
                    TopicsContent = ""
                };
            }).WithOpenApi();

            app.MapGet("/analitics/gpt", async (IGptClient gptClient) =>
            {
                string filePath = "../mock data.json";

                string fileContent = System.IO.File.ReadAllText(filePath);

                var contractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                };

                var settings = new JsonSerializerSettings
                {
                    ContractResolver = contractResolver,
                    Formatting = Formatting.None
                };

                var data = JsonConvert.DeserializeObject<Business_Discovery>(fileContent, settings);
                var gptRes = await gptClient.GetPageAnalysis(data);

                return gptRes;
            }).WithOpenApi();

            app.MapPost("/api/chat/send", async (ChatRequest request) =>
            {
                if (string.IsNullOrWhiteSpace(request.Message))
                {
                    return Results.BadRequest("Message cannot be empty.");
                }

                var apiKey = "api_key";
                var apiUrl = "https://api.openai.com/v1/chat/completions";

                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                var chatRequest = new
                {
                    model = "gpt-4o-mini", // or another model like "text-curie-001"
                    messages = new[]
                    {
                        new { role = "user",  content = request.Message }
                    },
                    max_tokens = 150
                };

                var response = await httpClient.PostAsJsonAsync(apiUrl, chatRequest);
                if (response.IsSuccessStatusCode)
                {
                    var chatResponse = await response.Content.ReadFromJsonAsync<OpenAiResponse>();
                    return Results.Ok(chatResponse);
                }

                var errorMessage = await response.Content.ReadAsStringAsync();
                return Results.StatusCode((int)response.StatusCode);
            })
            .WithName("SendMessage")
            .Produces<ChatResponse>(StatusCodes.Status200OK)
            .Produces<string>(StatusCodes.Status400BadRequest)
            .Produces<string>(StatusCodes.Status500InternalServerError);
            app.Run();


        }
    }
}