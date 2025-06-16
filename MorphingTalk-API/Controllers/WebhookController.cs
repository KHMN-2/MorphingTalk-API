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

        [HttpPost("inference-result")]
        public async Task<IActionResult> HandleAICallback([FromBody] AIWebhookInferencePayloadDto payload)
        {
            if (payload == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, 
                    new ResponseViewModel<string>(null, "Invalid payload", false, StatusCodes.Status400BadRequest));
            }

            var response = await _webhookService.HandleVoiceTranslationWebhookAsync(payload);
          

            return StatusCode(response.StatusCode, response);
        }
        [HttpPost("training-result")]
        public async Task<IActionResult> HandleTrainingResult([FromBody] AIWebhookTrainingPayloadDto payload)
        {
            if (payload == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest,
                    new ResponseViewModel<string>(null, "Invalid payload", false, StatusCodes.Status400BadRequest));
            }

            //var response = await _webhookService.HandleTrainingResultWebhookAsync(payload);
            //return StatusCode(response.StatusCode, response);
            return StatusCode(StatusCodes.Status501NotImplemented,
                new ResponseViewModel<string>(null, "Training result webhook not implemented", false, StatusCodes.Status501NotImplemented));
        }

    }
} 