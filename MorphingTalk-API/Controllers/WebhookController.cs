using Application.DTOs;
using Application.DTOs.Chatting;
using Application.Interfaces.Services.Chatting;
using Microsoft.AspNetCore.Mvc;

namespace MorphingTalk_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WebhookController : ControllerBase
    {
        private readonly IAIWebhookService _webhookService;

        public WebhookController(IAIWebhookService webhookService)
        {
            _webhookService = webhookService;
        }

        [HttpPost("ai-callback")]
        public async Task<IActionResult> HandleAICallback([FromBody] AIWebhookPayloadDto payload)
        {
            if (payload == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, 
                    new ResponseViewModel<string>(null, "Invalid payload", false, StatusCodes.Status400BadRequest));
            }

            var response = payload.Type switch
            {
                WebhookType.TextTranslation => await _webhookService.HandleTextTranslationWebhookAsync(payload),
                WebhookType.VoiceTranslation => await _webhookService.HandleVoiceTranslationWebhookAsync(payload),
                _ => new ResponseViewModel<string>(null, "Unsupported webhook type", false, StatusCodes.Status400BadRequest)
            };

            return StatusCode(response.StatusCode, response);
        }
    }
} 