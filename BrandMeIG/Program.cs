
using BrandMeIG.InstagramAPI;
using BrandMeIG.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Runtime.InteropServices;
using BrandMeIG.Configuration;
using static System.Net.WebRequestMethods;

namespace BrandMeIG
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var MyAllowSpecificOrigins = "AllowSpecificOrigin";

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthorization();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            string token = "EAATyn3xurrEBO4vVwBJ3gZBYzdRqRRQYXWuPN1HuWrR9U86O5IWbDi2F3ntH4AuFGO9NKwO4SwC07Kz9EHeZBSQFg335AKHZCbA09o3lbWwqKmOe6gARajDCW6dWs59Lp0ld1SZBnSHWceD28TaRrUB86Lt8xEeTbBY3plbzdmAFztGBNTD7vbjJPXCss1z3v5JHo9QqdgucfEe3A58ZD";
            string business_id = "17841461918433846";
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                    policy =>
                    {
                        policy.WithOrigins("http://192.168.0.206:8080").AllowAnyMethod().AllowAnyHeader();
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
                //Instagram Client and get Data
                var result = await _client.GetInfo(userName, default);
                //OpenAI Client and process Data 
                //Return General Model with Data 
                //Use Cach for user Id 
                //Use Cookies 
                
                return new ResultModel () {
                    AccountName = "svitech_wawa",
                    BusinessDescription = "IT events Warszawa | Community | Networking",
                    BrandValue = 43,
                    BusinessArea = "IT community and networking events",
                    Region = "Warsaw, Poland",
                    TargetNiches = "Ukrainian IT professionals in Poland, IT startups, Tech enthusiasts",
                    HashtagIdeas = "#ITCommunity #NetworkingEvents #TechWarsaw #UkrainianIT #PolandTech",
                    ImprovementsSuggestions = "1. Increase engagement by collaborating with local tech companies for events.\n" +
                                  "2. Develop content focused on career growth in the IT sector in Poland.\n" +
                                  "3. Partner with popular tech influencers in Poland for joint events and content.\n" +
                                  "4. Enhance social media presence using trending hashtags and interactive posts.",
                    Analitics = "Followers: 531\n" +
                    "Average Engagement Rate: 3.2%\n" +
                    "Top performing post: Image post on startup registration (31 likes)\n" +
                    "Video engagement higher than image posts.",
                    PotentialPartners = "1. Google Poland\n" +
                            "2. Microsoft Poland\n" +
                            "3. Allegro Tech\n" +
                            "4. Techland\n" +
                            "5. CD Projekt Red",
                    TrendsContent = "1. Short-form educational videos on IT trends and career advice.\n" +
                        "2. Behind-the-scenes content of events and meetups.\n" +
                        "3. Interviews with successful IT professionals in Poland.\n" +
                        "4. Interactive Q&A sessions and live streams.",
                    TopicsContent = "1. Navigating the IT Job Market in Poland: Tips and Strategies\n" +
                        "2. The Role of Emotional Intelligence in IT Leadership\n" +
                        "3. Emerging Trends in Cybersecurity: What You Need to Know\n" +
                        "4. Success Stories: Ukrainian IT Professionals Thriving in Poland\n" +
                        "5. Maximizing Productivity with Remote Work Tools"
                };
            }).WithOpenApi();

            app.MapGet("/weatherforecast", (HttpContext httpContext) =>
            {
                var forecast = Enumerable.Range(1, 5).Select(index =>
                    new WeatherForecast
                    {
                        Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                        TemperatureC = Random.Shared.Next(-20, 55),
                        Summary = ""
                    })
                    .ToArray();
                return forecast;
            })
            .WithName("GetWeatherForecast")
            .WithOpenApi();


            app.Run();
           

        }
    }
}