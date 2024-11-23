using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;

namespace SeleafAPI.Controllers;

[ApiController]
[Route("[controller]")]  
public class ChatBotController : ControllerBase
{
    private readonly HttpClient _httpClient;
    
    public ChatBotController(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
        [HttpPost("ask")]
        public async Task<IActionResult> AskChatbot([FromBody] ChatRequest request)
        {
            // SALEAF-specific FAQs
            var faqs = new Dictionary<string, string>
            {
                { "what is saleaf", "SALEAF is a non-profit organization that supports students with bursaries and facilitates sponsor donations." },
                { "how do i apply for a bursary", "You can register as a student and submit your application, or upload your application directly without registration." },
                { "how do sponsors donate", "Sponsors can donate directly on the SALEAF platform and receive a Section 18A certificate via email." },
                { "how does event registration work", "Register for events on the SALEAF platform and get a QR code for event entry." }
            };

            // Check if the question matches any FAQ
            var lowerMessage = request.Message.ToLower();
            foreach (var faq in faqs)
            {
                if (lowerMessage.Contains(faq.Key))
                {
                    return Ok(new { Reply = faq.Value });
                }
            }

            // If not in FAQs, forward the request to OpenAI
            var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
            var model = Environment.GetEnvironmentVariable("OPENAI_MODEL");

            if (string.IsNullOrEmpty(apiKey))
            {
                return BadRequest("OpenAI API key is not configured.");
            }

            var client = new RestClient("https://api.openai.com/v1/chat/completions");
            var chatRequest = new
            {
                model = model,
                messages = new[]
                {
                    new { role = "system", content = "You are a helpful assistant specialized in SALEAF." },
                    new { role = "user", content = request.Message }
                },
                temperature = 0.7
            };

            var requestBody = new RestRequest()
                .AddHeader("Authorization", $"Bearer {apiKey}")
                .AddHeader("Content-Type", "application/json")
                .AddJsonBody(chatRequest);

            var response = await client.ExecutePostAsync(requestBody);

            if (response.IsSuccessful)
            {
                var jsonResponse = JsonConvert.DeserializeObject<dynamic>(response.Content);
                string reply = jsonResponse.choices[0].message.content.ToString();
                return Ok(new { Reply = reply });
            }

            return StatusCode((int)response.StatusCode, response.Content);
        }
    }

    public class ChatRequest
    {
        public string Message { get; set; }
    }

