using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
    
    [HttpGet("query")]
    public async Task<IActionResult> QueryHuggingFaceApi(string userInput)
    {
        
        string apiUrl = "https://api-inference.huggingface.co/models/deepset/roberta-base-squad2"; // Replace with your desired model URL
        string apiKey = "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"; // Replace with your token

        string context = @"
                        The South African Lebanese Education Advancement Foundation (SALEAF) is a dedicated non-profit organization committed to providing financial support to young South Africans of Lebanese descent. Our mission is to empower, educate, and uplift the community by offering financial assistance to eligible individuals pursuing tertiary education. SALEAF operates as a civil NPO that is non-religious, non-racist, and non-sexist, emphasizing fairness and inclusivity in its operations.

                        ### How to Apply for a Bursary:
                        To apply for a SALEAF bursary, applicants must complete the application form available on the SALEAF website. The application requires submission of key documents, including:
                        - *Personal Information*: Full name, date of birth, South African ID number, contact details, home address, and proof of Lebanese descent.
                        - *Academic Records*: Grade 11 and Grade 12 results, as well as any relevant tertiary results if applicable.
                        - *Financial Disclosure*: Detailed information about family income, assets, liabilities, and financial dependents.
                        - *Additional Details*: Applicants are asked to provide information on leadership roles, sports and cultural activities, hobbies, and reasons for choosing their field of study. A short self-description and a statement explaining why they should be considered for a bursary are also required.

                        ### Important Guidelines for Applicants:
                        - All information submitted is subject to verification, and false or incomplete data may lead to disqualification.
                        - SALEAF’s funds are limited, so applicants are encouraged to explore alternative funding options before applying.
                        - The application process is designed to ensure transparency and fair allocation of bursaries to those most in need.

                        ### Eligibility Criteria:
                        SALEAF bursaries are available to young South Africans of Lebanese descent who demonstrate both financial need and academic potential. Applicants must meet the outlined criteria and submit all required documents for consideration.

                        SALEAF is dedicated to creating opportunities and fostering growth within the Lebanese community in South Africa through education. For more information about our mission, application process, or how to get involved as a donor or supporter, please visit our website or contact us directly.
                        ";

        // Prepare the request payload using Newtonsoft.Json
        var payload = new
        {
            inputs = new
            {
                question = userInput,
                context = context
            }
        };

        // Serialize the payload to JSON
        string jsonPayload = JsonConvert.SerializeObject(payload);
        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        // Add authorization header
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

        // Make the POST request to the Hugging Face API
        var response = await _httpClient.PostAsync(apiUrl, content);
        var responseContent = await response.Content.ReadAsStringAsync();

        // Return the response
        return Content(responseContent, "application/json");
    }
}