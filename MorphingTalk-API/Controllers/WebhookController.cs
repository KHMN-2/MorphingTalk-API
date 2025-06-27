using Application.DTOs;
using Application.DTOs.Chatting;
using Application.Interfaces.Services.Chatting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MorphingTalk_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WebhookController : ControllerBase
    {
        private readonly IAIWebhookService _webhookService;
        private readonly ILogger<WebhookController> _logger;

        public WebhookController(IAIWebhookService webhookService, ILogger<WebhookController> logger)
        {
            _webhookService = webhookService;
            _logger = logger;
        }

        [HttpPost("inference-result")]
        public async Task<IActionResult> HandleAICallback([FromBody] AIWebhookInferencePayloadDto payload)
        {
            if (payload == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, 
                    new ResponseViewModel<string>("", "Invalid payload", false, StatusCodes.Status400BadRequest));
            }

            var response = await _webhookService.HandleVoiceTranslationWebhookAsync(payload);
          

            return StatusCode(response.StatusCode, response);
        }        
        [HttpPost("training-result")]
        public async Task<IActionResult> HandleTrainingResult([FromBody] AIWebhookTrainingPayloadDto payload)
        {
            _logger.LogInformation("Received training webhook:");
            _logger.LogInformation($"RequestId: {payload?.RequestId}");
            _logger.LogInformation($"ModelId: {payload?.modelId}");
            //_logger.LogInformation($"Success (string): {payload?.success}");
            _logger.LogInformation($"Success (boolean): {payload?.Success}");
            _logger.LogInformation($"ErrorMessage: {payload?.ErrorMessage}");

            if (payload == null)
            {
                _logger.LogWarning("Received null payload for training result");
                return StatusCode(StatusCodes.Status400BadRequest,
                    new ResponseViewModel<string>("", "Invalid payload", false, StatusCodes.Status400BadRequest));
            }

            var response = await _webhookService.HandleVoiceTrainingWebhookAsync(payload);
            _logger.LogInformation($"Webhook processing result: {response.Success}, Message: {response.Message}");

            return StatusCode(response.StatusCode, response);
        }

    }
}