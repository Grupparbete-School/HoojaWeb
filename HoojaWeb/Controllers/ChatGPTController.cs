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
    //[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme, Roles = "Admin, Employee")]
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

            Message oMessage = new Message();
            oMessage.role = "user";
            oMessage.content = query;

            oRequest.messages = new Message[] { oMessage };

            string oReqJSON = JsonConvert.SerializeObject(oRequest);
            HttpContent oContent = new StringContent(oReqJSON, Encoding.UTF8, "application/json");

            HttpResponseMessage oResponseMessage = await oClient.PostAsync(chatURL, oContent);

            if (oResponseMessage.IsSuccessStatusCode)
            {
                string oResJSON = await oResponseMessage.Content.ReadAsStringAsync();

                ChatResponse oResponse = JsonConvert.DeserializeObject<ChatResponse>(oResJSON);

                foreach (Choice c in oResponse.choices)
                {
                    sb.Append(c.message.content);
                }

                HttpChatGPTResponse oHttpResponse = new HttpChatGPTResponse()
                {
                    Success = true,
                    Data = sb.ToString()
                };

                return Ok(oHttpResponse);
            }
            else
            {
                string oFailReason = await oResponseMessage.Content.ReadAsStringAsync();
                return BadRequest(oFailReason); ;
            }
        }
    }
}
