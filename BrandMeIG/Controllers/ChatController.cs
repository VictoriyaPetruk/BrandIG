using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace BrandMeIG.Controllers
{
    public class OpenAiResponse
    {
        public string Id { get; set; }
        public string Object { get; set; }
        public int Created { get; set; }
        public Choice[] Choices { get; set; }

        public class Choice
        {
            public Message Message { get; set; }
            public string Finish_reason { get; set; }
        }

        public class Message
        {
            public string Role { get; set; }
            public string Content { get; set; }
        }
    }

    public class ChatRequest
        {
            public string Message { get; set; }
        }

    public class ChatResponse
    {
        public string Id { get; set; }
        public string Object { get; set; }
        public int Created { get; set; }
        public Choice[] Choices { get; set; }
        public class Choice
        {
            public string Text { get; set; }
            public int Index { get; set; }
            public Logprobs Logprobs { get; set; }
            public string Finish_reason { get; set; }
        }

        public class Logprobs
        {
            public float[] Tokens { get; set; }
            public float[] Token_logprobs { get; set; }
            public float[] Top_logprobs { get; set; }
            public int[] Text_offset { get; set; }
        }
    }
}
