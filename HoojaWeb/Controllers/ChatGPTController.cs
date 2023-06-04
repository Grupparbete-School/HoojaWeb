using HoojaWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OpenAI_API.Chat;
using OpenAI_API.Completions;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using ChatRequest = HoojaWeb.Models.ChatRequest;
using Choice = HoojaWeb.Models.Choice;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace HoojaWeb.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "Admin, Employee")]
    [ApiController]
    public class ChatGPTController : ControllerBase
    {
        [HttpPost]
        [Route("AskChatGPT")]
        public async Task<IActionResult> AskChatGPT([FromBody] string query)
        {
            // Definiera API-slutpunkten och åtkomsttoken
            string chatURL = "https://api.openai.com/v1/chat/completions";
            string apiKey = Environment.GetEnvironmentVariable("API_KEY_CHAT");

            StringBuilder sb = new StringBuilder();

            HttpClient oClient = new HttpClient();
            oClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            ChatRequest oRequest = new ChatRequest();
            oRequest.model = "gpt-3.5-turbo";

            // Ange en prompt för att kontrollera det aktuella året

            oRequest.messages = new Message[]
            {
                new Message { role = "system", content = "Behöver du hjälp med hårvårdsprodukter?" },
                new Message { role = "user", content = "Ja" },
                new Message { role = "system", content = "Vilken produkt handlar det om?" },
                new Message { role = "user", content = "Ja" },
                new Message { role = "system", content = "Ok, låt mig söka!" },
                new Message { role = "user", content = "Ja" },
            };


            // Serialisera förfrågan till JSON
            string oReqJSON = JsonConvert.SerializeObject(oRequest);
            HttpContent oContent = new StringContent(oReqJSON, Encoding.UTF8, "application/json");

            // Skicka HTTP POST-förfrågan till API-slutpunkten
            HttpResponseMessage oResponseMessage = await oClient.PostAsync(chatURL, oContent);

            if (oResponseMessage.IsSuccessStatusCode)
            {
                // Hantera det lyckade svaret
                string oResJSON = await oResponseMessage.Content.ReadAsStringAsync();

                ChatResponse oResponse = JsonConvert.DeserializeObject<ChatResponse>(oResJSON);

                string responseInSwedish = string.Empty;
                foreach (Choice c in oResponse.choices)
                {
                    if (c.message.role == "assistant")
                    {
                        responseInSwedish = c.message.content;
                        break;
                    }
                }

                HttpChatGPTResponse oHttpResponse = new HttpChatGPTResponse()
                {
                    Success = true,
                    Data = responseInSwedish
                };

                return Ok(oHttpResponse);
            }
            else
            {
                // Hantera felaktigt svar
                string oFailReason = await oResponseMessage.Content.ReadAsStringAsync();
                return BadRequest(oFailReason);
            }
        }
    }
}
