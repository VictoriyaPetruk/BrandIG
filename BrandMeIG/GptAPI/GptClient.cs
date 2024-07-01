using BrandMeIG.Configuration;
using Newtonsoft.Json;


namespace BrandMeIG.GptAPI
{
    public interface IGptClient
    {
        Task<GptResponse> GetPageAnalysis(InstagramAPI.Business_Discovery data);
    }

    class GptClient : IGptClient
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerSettings _jsonSettings;

        public GptClient(HttpClient gptHttpClient, JsonSerializerSettings jsonSettings)
        {
            _httpClient = gptHttpClient;
            _jsonSettings = jsonSettings;
        }
        public async Task<GptResponse> GetPageAnalysis(InstagramAPI.Business_Discovery data)
        {
            var pageData = JsonConvert.SerializeObject(data, _jsonSettings);
            string prompt = $@"
            Analyze the following Instagram page content and provide insights in the following JSON format:
            {{
                ""description"": ""Short summary of the page content"",
                ""im"": [""List of hashtags used""],
                ""improveAdvice"": ""Advice instagram page could be improved to attract more new client"",
                ""topPosts"": [
                    {{
                        ""postId"": ""Post ID"",
                        ""likes"": ""Number of likes"",
                        ""comments"": ""Number of comments""
                    }}
                ]
            }}
            Here it is instagram page data: {pageData}";

            var requestContent = new
            {
                model = "gpt-3.5-turbo",
                prompt = prompt,
                // max_tokens = 100,  // todo get from config
                temperature = 0.7  // todo get from config
            };

            var content = new StringContent(JsonConvert.SerializeObject(requestContent), System.Text.Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync("completions", content);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to call GPT API: {response.StatusCode}");
            }

            string responseString = await response.Content.ReadAsStringAsync();
            var gptResponse = JsonConvert.DeserializeObject<GptResponse>(responseString, _jsonSettings);

            return gptResponse;
        }
    }


}