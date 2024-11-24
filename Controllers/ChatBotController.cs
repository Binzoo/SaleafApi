using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RestSharp;
using SeleafAPI.Data;
using SeleafAPI.Models;

namespace SeleafAPI.Controllers;

[ApiController]
[Route("[controller]")]  
public class ChatBotController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly AppDbContext _context;
    
    public ChatBotController(HttpClient httpClient, AppDbContext context)
    {
        _httpClient = httpClient;
        _context = context;
    }
    
        [HttpPost("ask")]
        public async Task<IActionResult> AskChatbot([FromBody] ChatRequest request)
        {
            // SALEAF-specific FAQs
            var faqs = new Dictionary<string, string>
                {
                    {"What is SALEAF?", "SALEAF is a registered NPO dedicated to raising funds to educate young South Africans of Lebanese descent, empowering them to achieve their full potential."},
                    {"What is the mission of SALEAF?", "SALEAF’s mission is to raise funds to provide financial support for the tertiary education of young South Africans of Lebanese descent."},
                    {"Is SALEAF affiliated with any other organizations?", "No, SALEAF is a civil NPO that is non-religious, non-racist, non-sexist, and not affiliated with any other associations."},
                    {"When was SALEAF founded?", "SALEAF originated in the 1970s and was re-launched in 2006 by like-minded South Africans of Lebanese descent."},
                    {"What is the objective of SALEAF?", "SALEAF’s goal is to educate, empower, and uplift young South Africans of Lebanese descent by providing financial support for tertiary education."},
                    {"How can I make a donation?", "You can donate via EFT using the following details: Standard Bank, Sandton Branch, Branch Code 019205, Account Number 220739854."},
                    {"Why is financial assistance needed for South African Lebanese students?", "In South Africa, financial support is often prioritized for previously disadvantaged candidates, leaving many academically excelling South African Lebanese students overlooked."},
                    {"Who are the directors of SALEAF?", "The directors include Jerome Kourie, Brent Shahim, Garth Towell, Craig Wilkinson, Alan Ford, Alex Shahim, and Maha Deeb."},
                    {"Can anyone participate in SALEAF?", "Yes, you can participate as a donor or an applicant to support the cause."},
                    {"What are the origins of SALEAF?", "SALEAF was founded in the 1970s by South African Lebanese businessmen to raise funds for the education of their community members."},
                    {"What are some of the achievements of SALEAF?", "Over the past decades, SALEAF has supported many students who have gone on to become successful professionals and business leaders."},
                    {"How does SALEAF help students?", "SALEAF provides financial support to help students gain access to tertiary education and realize their full potential."},
                    {"What is the socio-economic context of SALEAF’s work?", "In South Africa, the socio-economic environment prioritizes financial support for previously disadvantaged groups, often leaving Lebanese students without assistance."},
                    {"How can I get involved?", "You can visit SALEAF’s website to learn more about participating as a donor or applicant."},
                    {"How do I apply for a bursary?", "You can register as a student and submit your application, or upload your application directly without registration."},
                    {"How do sponsors donate?", "Sponsors can donate directly on the SALEAF platform and receive a Section 18A certificate via email."},
                    {"How does event registration work?", "Register for events on the SALEAF platform and get a QR code for event entry."}
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
                    new { role = "system", content = "You are a helpful assistant specialized in SALEAF. Only answer questions related to SALEAF and respond with 'I can only assist with SALEAF-related queries' for unrelated topics. SALEAF is a registered NPO dedicated to raising funds to educate young South Africans of Lebanese descent, empowering them to achieve their full potential." },
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
        
        [Authorize]
        [HttpPost("authorize-ask")]
        public async Task<IActionResult> AskChatbotAuthorizeUser([FromBody] ChatRequest request)
        {
            var userId = User.FindFirst("userId")?.Value;
            
            // SALEAF-specific FAQs
            var faqs = new Dictionary<string, string>
                {
                    {"What is SALEAF?", "SALEAF is a registered NPO dedicated to raising funds to educate young South Africans of Lebanese descent, empowering them to achieve their full potential."},
                    {"What is the mission of SALEAF?", "SALEAF’s mission is to raise funds to provide financial support for the tertiary education of young South Africans of Lebanese descent."},
                    {"Is SALEAF affiliated with any other organizations?", "No, SALEAF is a civil NPO that is non-religious, non-racist, non-sexist, and not affiliated with any other associations."},
                    {"When was SALEAF founded?", "SALEAF originated in the 1970s and was re-launched in 2006 by like-minded South Africans of Lebanese descent."},
                    {"What is the objective of SALEAF?", "SALEAF’s goal is to educate, empower, and uplift young South Africans of Lebanese descent by providing financial support for tertiary education."},
                    {"How can I make a donation?", "You can donate via EFT using the following details: Standard Bank, Sandton Branch, Branch Code 019205, Account Number 220739854."},
                    {"Why is financial assistance needed for South African Lebanese students?", "In South Africa, financial support is often prioritized for previously disadvantaged candidates, leaving many academically excelling South African Lebanese students overlooked."},
                    {"Who are the directors of SALEAF?", "The directors include Jerome Kourie, Brent Shahim, Garth Towell, Craig Wilkinson, Alan Ford, Alex Shahim, and Maha Deeb."},
                    {"Can anyone participate in SALEAF?", "Yes, you can participate as a donor or an applicant to support the cause."},
                    {"What are the origins of SALEAF?", "SALEAF was founded in the 1970s by South African Lebanese businessmen to raise funds for the education of their community members."},
                    {"What are some of the achievements of SALEAF?", "Over the past decades, SALEAF has supported many students who have gone on to become successful professionals and business leaders."},
                    {"How does SALEAF help students?", "SALEAF provides financial support to help students gain access to tertiary education and realize their full potential."},
                    {"What is the socio-economic context of SALEAF’s work?", "In South Africa, the socio-economic environment prioritizes financial support for previously disadvantaged groups, often leaving Lebanese students without assistance."},
                    {"How can I get involved?", "You can visit SALEAF’s website to learn more about participating as a donor or applicant."},
                    {"How do I apply for a bursary?", "You can register as a student and submit your application, or upload your application directly without registration."},
                    {"How do sponsors donate?", "Sponsors can donate directly on the SALEAF platform and receive a Section 18A certificate via email."},
                    {"How does event registration work?", "Register for events on the SALEAF platform and get a QR code for event entry."}
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
                    new { role = "system", content = "You are a helpful assistant specialized in SALEAF. Only answer questions related to SALEAF and respond with 'I can only assist with SALEAF-related queries' for unrelated topics. SALEAF is a registered NPO dedicated to raising funds to educate young South Africans of Lebanese descent, empowering them to achieve their full potential. " },
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
                
                var conversation = new ChatBotConversation
                {
                    AppUserId = userId,
                    UserQuestion = request.Message,
                    BotResponse = response.Content,
                    DateCreated = DateTime.UtcNow
                };

                await _context.ChatBotConversations.AddAsync(conversation);
                await _context.SaveChangesAsync();
                
                
                return Ok(new { Reply = reply });
            }

            
            
            return StatusCode((int)response.StatusCode, response.Content);
        }


        [HttpGet("get-previous-conversation")]
        [Authorize]
        public async Task<IActionResult> GetPreviousConversations()
        {
            var userId = User.FindFirst("userId")?.Value;
            var conversation = await _context.ChatBotConversations
                .Where(e => e.AppUserId == userId)
                .OrderByDescending(e => e.DateCreated)
                .Select(e => new
                {
                    e.Id,
                    e.BotResponse,
                    e.UserQuestion,
                    e.DateCreated
                })
                .ToListAsync();

            return Ok(conversation);
        }
    }

    public class ChatRequest
    {
        public string Message { get; set; }
    }

